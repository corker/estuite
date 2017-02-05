using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite
{
    public class EventStore : ISaveSessions
    {
        private readonly CloudTableClient _tableClient;
        private readonly string _streamTableName;

        public EventStore(CloudStorageAccount account, IEventStoreConfiguration configuration)
        {
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Save(Session session, CancellationToken token = new CancellationToken())
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
            await table.ExecuteBatchAsync(operation, token);
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