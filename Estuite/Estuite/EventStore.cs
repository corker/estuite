using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite
{
    public class EventStore : ISaveSessions
    {
        private readonly CloudTableClient _tableClient;
        private readonly string _tableName;

        public EventStore(CloudStorageAccount account, IEventStoreConfiguration configuration)
        {
            _tableName = configuration.TableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Save(StreamId streamId, Session session, CancellationToken token = new CancellationToken())
        {
            var operation = new TableBatchOperation();
            var sessionTableEntity = new SessionTableEntity
            {
                PartitionKey = streamId.Value,
                RowKey = $"s_{session.SessionId.Value}",
                Created = $"{session.Created:O}",
                RecordCount = session.Records.Length
            };
            operation.Add(TableOperation.Insert(sessionTableEntity));

            foreach (var record in session.Records)
            {
                var eventTableEntity = new EventTableEntity
                {
                    PartitionKey = streamId.Value,
                    RowKey = $"e_{record.Version:D10}",
                    Created = $"{record.Created:O}",
                    SessionId = record.SessionId,
                    Type = record.Type,
                    Payload = record.Payload
                };
                operation.Add(TableOperation.Insert(eventTableEntity));

                var dispatchTableEntity = new DispatchTableEntity
                {
                    PartitionKey = streamId.Value,
                    RowKey = $"d_{record.Version:D10}",
                    Created = $"{record.Created:O}",
                    SessionId = record.SessionId,
                    Type = record.Type,
                    Payload = record.Payload
                };
                operation.Add(TableOperation.Insert(dispatchTableEntity));
            }

            var table = _tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync(token);
            await table.ExecuteBatchAsync(operation, token);
        }


        public class EventTableEntity : TableEntity
        {
            public string Created { get; set; }
            public string SessionId { get; set; }
            public string Type { get; set; }
            public string Payload { get; set; }
        }

        public class DispatchTableEntity : TableEntity
        {
            public string Created { get; set; }
            public string SessionId { get; set; }
            public string Type { get; set; }
            public string Payload { get; set; }
        }

        public class SessionTableEntity : TableEntity
        {
            public string Created { get; set; }
            public int RecordCount { get; set; }
        }
    }
}