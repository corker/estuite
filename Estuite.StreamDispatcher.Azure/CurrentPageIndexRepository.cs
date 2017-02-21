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
        private readonly CloudTableClient _tableClient;
        private readonly string _tableName;

        public CurrentPageIndexRepository(CloudStorageAccount account, IStreamDispatcherConfiguration configuration)
        {
            _tableName = configuration.EventTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task<CurrentPageIndexTableEntity> Get(CancellationToken token)
        {
            var table = _tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync(token);

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
            var table = _tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync(token);
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