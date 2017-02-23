using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public class DispatchStreamRecoveryJobRepository : IAddDispatchStreamRecoveryJobs, IDeleteDispatchStreamRecoveryJobs
    {
        private readonly string _eventTableName;
        private readonly CloudTableClient _tableClient;

        public DispatchStreamRecoveryJobRepository(
            CloudStorageAccount account,
            IStreamDispatcherConfiguration configuration)
        {
            _eventTableName = configuration.EventTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Add(DispatchStreamJob job, CancellationToken token)
        {
            var table = _tableClient.GetTableReference(_eventTableName);
            await table.CreateIfNotExistsAsync(token);
            var entity = new DispatchStreamRecoveryJobTableEntity
            {
                PartitionKey = "StreamsToDispatchOnRecovery",
                RowKey = $"R^{job.StreamId.Value}^{job.SessionId.Value}",
                StreamId = job.StreamId.Value
            };
            var operation = TableOperation.Insert(entity);
            await table.ExecuteAsync(operation, token);
        }

        public async Task Delete(DispatchStreamJob job, CancellationToken token)
        {
            var table = _tableClient.GetTableReference(_eventTableName);
            var entity = new TableEntity
            {
                PartitionKey = "StreamsToDispatchOnRecovery",
                RowKey = $"R^{job.StreamId.Value}^{job.SessionId.Value}",
                ETag = "*"
            };
            var operation = TableOperation.Delete(entity);
            await table.ExecuteAsync(operation, token);
        }
    }
}