using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamDispatcher.Azure
{
    public class CurrentPageIndexRepository : IProvideCurrentPageIndexes, IUpdateCurrentPageIndexes
    {
        private readonly IProvideEventStoreCloudTable _table;

        public CurrentPageIndexRepository(IProvideEventStoreCloudTable table)
        {
            _table = table;
        }

        public async Task<CurrentPageIndexTableEntity> Get(CancellationToken token)
        {
            var table = await _table.GetOrCreate();
            var query = table.CreateQuery<CurrentPageIndexTableEntity>()
                .Where(x => x.PartitionKey == "Indexes")
                .Where(x => x.RowKey == "CurrentPageIndex")
                .Take(1)
                .AsTableQuery();

            CurrentPageIndexTableEntity entity;
            do
            {
                token.ThrowIfCancellationRequested();
                var segment = await table.ExecuteQuerySegmentedAsync(query, null, token);
                entity = segment.SingleOrDefault() ?? await Create(table, token);
            } while (entity == null);
            return entity;
        }

        public async Task TryUpdate(CurrentPageIndexTableEntity entity, CancellationToken token)
        {
            var table = await _table.GetOrCreate();
            var operation = TableOperation.Replace(entity);
            try
            {
                await table.ExecuteAsync(operation, token);
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode != (int) HttpStatusCode.PreconditionFailed) throw;
            }
        }

        private static async Task<CurrentPageIndexTableEntity> Create(CloudTable table, CancellationToken token)
        {
            var entity = new CurrentPageIndexTableEntity
            {
                PartitionKey = "Indexes",
                RowKey = "CurrentPageIndex",
                Index = 0
            };
            var operation = TableOperation.Insert(entity);
            try
            {
                var result = await table.ExecuteAsync(operation, token);
                return (CurrentPageIndexTableEntity) result.Result;
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode != (int) HttpStatusCode.Conflict) throw;
                return null;
            }
        }
    }
}