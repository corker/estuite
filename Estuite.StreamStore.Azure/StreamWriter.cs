using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamStore.Azure
{
    public class StreamWriter : IWriteStreams
    {
        private readonly string _streamTableName;
        private readonly CloudTableClient _tableClient;

        public StreamWriter(CloudStorageAccount account, IStreamStoreConfiguration configuration)
        {
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Write(Session session, CancellationToken token = new CancellationToken())
        {
            var table = _tableClient.GetTableReference(_streamTableName);
            await table.CreateIfNotExistsAsync(token);
            await WriteDispatcherMarker(table, session, token);
            await WriteStream(table, session, token);
        }

        private static async Task WriteDispatcherMarker(CloudTable table, Session session, CancellationToken token)
        {
            var entity = new StreamMarkerTableEntity
            {
                PartitionKey = "StreamMarkers",
                RowKey = session.StreamId.Value,
                Updated = $"{DateTime.UtcNow:O}"
            };
            var operation = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(operation, token);
        }

        private static async Task WriteStream(CloudTable table, Session session, CancellationToken token)
        {
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