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
            _streamTableName = configuration.TableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Write(Session session, CancellationToken token = new CancellationToken())
        {
            var operation = new TableBatchOperation();
            var sessionTableEntity = new SessionTableEntity
            {
                PartitionKey = session.StreamId.Value,
                RowKey = $"S^{session.SessionId.Value}",
                Created = $"{session.Created:O}",
                RecordCount = session.Records.Length
            };
            operation.Add(TableOperation.Insert(sessionTableEntity));

            foreach (var record in session.Records)
            {
                var eventTableEntity = new EventTableEntity
                {
                    PartitionKey = session.StreamId.Value,
                    RowKey = $"E^{record.Version:D10}",
                    Created = $"{record.Created:O}",
                    SessionId = record.SessionId.Value,
                    Type = record.Type,
                    Payload = record.Payload
                };
                operation.Add(TableOperation.Insert(eventTableEntity));

                var dispatchTableEntity = new DispatchTableEntity
                {
                    PartitionKey = session.StreamId.Value,
                    RowKey = $"D^{record.Version:D10}",
                    Created = $"{record.Created:O}",
                    SessionId = record.SessionId.Value,
                    Type = record.Type,
                    Payload = record.Payload
                };
                operation.Add(TableOperation.Insert(dispatchTableEntity));
            }

            var table = _tableClient.GetTableReference(_streamTableName);
            await table.CreateIfNotExistsAsync(token);
            try
            {
                await table.ExecuteBatchAsync(operation, token);
            }
            catch (StorageException e)
            {
                switch (e.RequestInformation.HttpStatusCode)
                {
                    case HttpStatusCodes.EntityAlreadyExists:
                        throw new StreamConcurrentWriteException(
                            $"The stream {session.StreamId.Value} was modified between read and write or the session {session.SessionId.Value} was already registered.",
                            e
                        );
                    default:
                        throw;
                }
            }
        }

        private static class HttpStatusCodes
        {
            public const int EntityAlreadyExists = 409;
        }
        
        private class EventTableEntity : TableEntity
        {
            public string Created { get; set; }
            public string SessionId { get; set; }
            public string Type { get; set; }
            public string Payload { get; set; }
        }

        private class DispatchTableEntity : TableEntity
        {
            public string Created { get; set; }
            public string SessionId { get; set; }
            public string Type { get; set; }
            public string Payload { get; set; }
        }

        private class SessionTableEntity : TableEntity
        {
            public string Created { get; set; }
            public int RecordCount { get; set; }
        }
    }
}