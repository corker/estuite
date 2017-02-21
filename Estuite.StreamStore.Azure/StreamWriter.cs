using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamDispatcher.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamStore.Azure
{
    public class StreamWriter : IWriteStreams
    {
        private readonly IAddDispatchStreamRecoveryJobs _addDispatchStreamRecoveryJobs;
        private readonly IDeleteDispatchStreamRecoveryJobs _deleteDispatchStreamRecoveryJobs;
        private readonly IDispatchStreams _dispatchStreams;
        private readonly string _streamTableName;
        private readonly CloudTableClient _tableClient;

        public StreamWriter(
            CloudStorageAccount account,
            IStreamStoreConfiguration configuration,
            IAddDispatchStreamRecoveryJobs addDispatchStreamRecoveryJobs,
            IDeleteDispatchStreamRecoveryJobs deleteDispatchStreamRecoveryJobs,
            IDispatchStreams dispatchStreams)
        {
            _addDispatchStreamRecoveryJobs = addDispatchStreamRecoveryJobs;
            _deleteDispatchStreamRecoveryJobs = deleteDispatchStreamRecoveryJobs;
            _dispatchStreams = dispatchStreams;
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Write(Session session, CancellationToken token)
        {
            var job = new StreamDispatchJob(session.StreamId, session.SessionId);
            await _addDispatchStreamRecoveryJobs.Add(job, token);
            try
            {
                await WriteStream(session, token);
            }
            catch
            {
                await _deleteDispatchStreamRecoveryJobs.Delete(job, token);
                throw;
            }
            await _dispatchStreams.Dispatch(job, token);
            await _deleteDispatchStreamRecoveryJobs.Delete(job, token);
        }

        private async Task WriteStream(Session session, CancellationToken token)
        {
            var table = _tableClient.GetTableReference(_streamTableName);
            await table.CreateIfNotExistsAsync(token);

            var operation = new TableBatchOperation();
            var sessionTableEntity = new StreamSessionTableEntity
            {
                PartitionKey = session.StreamId.Value,
                RowKey = $"S^{session.SessionId.Value}",
                Created = $"{session.Created:O}",
                RecordCount = session.Records.Length
            };
            operation.Add(TableOperation.Insert(sessionTableEntity));

            foreach (var record in session.Records)
            {
                var eventTableEntity = new StreamEventTableEntity
                {
                    PartitionKey = session.StreamId.Value,
                    RowKey = $"E^{record.Version:x16}",
                    Created = $"{record.Created:O}",
                    SessionId = record.SessionId.Value,
                    Type = record.Type,
                    Payload = record.Payload
                };
                operation.Add(TableOperation.Insert(eventTableEntity));

                var dispatchTableEntity = new StreamDispatchTableEntity
                {
                    PartitionKey = session.StreamId.Value,
                    RowKey = $"D^{record.Version:x16}",
                    AggregateType = session.StreamId.AggregateType.Value,
                    BucketId = session.StreamId.BucketId.Value,
                    AggregateId = session.StreamId.AggregateId.Value,
                    SessionId = session.SessionId.Value,
                    Version = record.Version,
                    Created = $"{record.Created:O}",
                    Type = record.Type,
                    Payload = record.Payload
                };
                operation.Add(TableOperation.Insert(dispatchTableEntity));
            }

            try
            {
                await table.ExecuteBatchAsync(operation, token);
            }
            catch (StorageException e)
            {
                switch (e.RequestInformation.HttpStatusCode)
                {
                    case (int) HttpStatusCode.Conflict:
                        throw new StreamConcurrentWriteException(
                            $"The stream {session.StreamId.Value} was modified between read and write or the session {session.SessionId.Value} was already registered.",
                            e
                        );
                    default:
                        throw;
                }
            }
        }
    }
}