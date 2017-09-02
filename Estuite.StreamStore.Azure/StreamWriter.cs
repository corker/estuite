using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamDispatcher;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamStore.Azure
{
    public class StreamWriter : IWriteStreams
    {
        private readonly IAddDispatchStreamRecoveryJobs _addDispatchStreamRecoveryJobs;
        private readonly IProvideUtcDateTime _dateTime;
        private readonly IDeleteDispatchStreamRecoveryJobs _deleteDispatchStreamRecoveryJobs;
        private readonly ICreateEventRecordTableEntities _entities;
        private readonly IProvideSessions _sessions;
        private readonly IDispatchStreams _streams;
        private readonly IProvideStreamStoreCloudTable _table;

        public StreamWriter(
            IProvideStreamStoreCloudTable table,
            IAddDispatchStreamRecoveryJobs addDispatchStreamRecoveryJobs,
            IDeleteDispatchStreamRecoveryJobs deleteDispatchStreamRecoveryJobs,
            IDispatchStreams streams,
            IProvideUtcDateTime dateTime,
            ICreateEventRecordTableEntities entities,
            IProvideSessions sessions
        )
        {
            _table = table;
            _addDispatchStreamRecoveryJobs = addDispatchStreamRecoveryJobs;
            _deleteDispatchStreamRecoveryJobs = deleteDispatchStreamRecoveryJobs;
            _streams = streams;
            _dateTime = dateTime;
            _entities = entities;
            _sessions = sessions;
        }

        public async Task Write(StreamId streamId, IReadOnlyCollection<EventRecord> records, CancellationToken token)
        {
            var sessionId = _sessions.Current();
            var job = new DispatchStreamJob(streamId, sessionId);
            await _addDispatchStreamRecoveryJobs.Add(job, token);
            try
            {
                await WriteStream(sessionId, streamId, records, token);
            }
            catch
            {
                await _deleteDispatchStreamRecoveryJobs.Delete(job, token);
                throw;
            }
            await _streams.Dispatch(job, token);
            await _deleteDispatchStreamRecoveryJobs.Delete(job, token);
        }

        private async Task WriteStream(
            SessionId sessionId,
            StreamId streamId,
            IReadOnlyCollection<EventRecord> records,
            CancellationToken token
        )
        {
            var table = await _table.GetOrCreate();
            var operation = new TableBatchOperation();
            var created = _dateTime.Now;
            var sessionTableEntity = new SessionRecordTableEntity
            {
                PartitionKey = streamId.Value,
                RowKey = $"S^{sessionId.Value}",
                Created = $"{created:O}",
                RecordCount = records.Count
            };
            operation.Add(TableOperation.Insert(sessionTableEntity));

            foreach (var record in records)
            {
                var entity = _entities.CreateFrom(record);
                entity.PartitionKey = streamId.Value;
                entity.RowKey = $"E^{record.Version:x16}";
                entity.Created = $"{created:O}";
                entity.SessionId = sessionId.Value;
                entity.Version = record.Version;
                operation.Add(TableOperation.Insert(entity));

                var dispatchTableEntity = new EventToDispatchRecordTableEntity
                {
                    PartitionKey = streamId.Value,
                    RowKey = $"D^{record.Version:x16}",
                    AggregateType = streamId.AggregateType.Value,
                    BucketId = streamId.BucketId.Value,
                    AggregateId = streamId.AggregateId.Value,
                    SessionId = sessionId.Value,
                    Version = record.Version,
                    Created = $"{created:O}",
                    Type = entity.Type,
                    Payload = entity.Payload
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
                            $"The stream {streamId.Value} was modified between read and write or the session {sessionId.Value} was already registered.",
                            e);
                    default:
                        throw;
                }
            }
        }
    }
}