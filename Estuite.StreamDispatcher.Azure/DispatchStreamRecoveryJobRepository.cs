using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public class DispatchStreamRecoveryJobRepository : IAddDispatchStreamRecoveryJobs, IDeleteDispatchStreamRecoveryJobs
    {
        private readonly IProvideEventStoreCloudTable _table;

        public DispatchStreamRecoveryJobRepository(IProvideEventStoreCloudTable table)
        {
            _table = table;
        }

        public async Task Add(DispatchStreamJob job, CancellationToken token)
        {
            var table = await _table.GetOrCreate();
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
            var table = await _table.GetOrCreate();
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