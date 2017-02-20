using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamDispatcher.Azure
{
    public class AzureEventDispatcher : IDispatchEvents<EventRecordTableEntity>
    {
        private readonly CloudTableClient _tableClient;
        private readonly string _tableName;

        public AzureEventDispatcher(CloudStorageAccount account, IStreamDispatcherConfiguration configuration)
        {
            _tableName = configuration.EventTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Dispatch(List<EventRecordTableEntity> events, CancellationToken token = new CancellationToken())
        {
            var table = _tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync(token);

            EventStoreCurrentPage page = null;
            EventStorePageInfo pageInfo = null;

            while (pageInfo == null)
            {
                token.ThrowIfCancellationRequested();

                page = await GetCurrentPage(table, token);

                var partitionKey = $"P^{page.Index:x16}";

                var queryCurrentPageInfo = table.CreateQuery<EventStorePageInfo>()
                    .Where(x => x.PartitionKey == partitionKey)
                    .Where(x => x.RowKey == "PageInfo")
                    .Take(1)
                    .AsTableQuery();

                var result = await table.ExecuteQuerySegmentedAsync(queryCurrentPageInfo, null, token);
                pageInfo = result.SingleOrDefault();

                if (pageInfo == null)
                {
                    pageInfo = new EventStorePageInfo
                    {
                        PartitionKey = partitionKey,
                        RowKey = "PageInfo",
                        NextIndex = 1,
                        NextPageIndex = 0
                    };
                    var operation = TableOperation.Insert(pageInfo);
                    try
                    {
                        await table.ExecuteAsync(operation, token);
                    }
                    catch (StorageException e)
                    {
                        if (e.RequestInformation.HttpStatusCode != (int) HttpStatusCode.Conflict) throw;
                        pageInfo = null;
                    }
                }
                else
                {
                    if (pageInfo.NextPageIndex == 0) continue;
                    page.Index = pageInfo.NextPageIndex;
                    var operation = TableOperation.Replace(page);
                    await table.ExecuteAsync(operation, token);
                    pageInfo = null;
                }
            }

            var batchOperation = new TableBatchOperation();

            foreach (var @event in events)
            {
                var record = new EventStoreRecord
                {
                    PartitionKey = $"P^{page.Index:x16}",
                    RowKey = $"E^{pageInfo.NextIndex:x16}",
                    PageIndex = page.Index,
                    RowIndex = pageInfo.NextIndex,
                    AggregateType = @event.AggregateType,
                    BucketId = @event.BucketId,
                    AggregateId = @event.AggregateId,
                    SessionId = @event.SessionId,
                    Version = @event.Version,
                    Created = @event.Created,
                    Type = @event.Type,
                    Payload = @event.Payload
                };
                pageInfo.NextIndex++;
                batchOperation.InsertOrReplace(record);
            }

            if (pageInfo.NextIndex > 500) pageInfo.NextPageIndex = page.Index + 1;

            batchOperation.Replace(pageInfo);

            await table.ExecuteBatchAsync(batchOperation, token);
        }

        private static async Task<EventStoreCurrentPage> GetCurrentPage(CloudTable table, CancellationToken token)
        {
            var queryCurrentPartition = table.CreateQuery<EventStoreCurrentPage>()
                .Where(x => x.PartitionKey == "Indexes")
                .Where(x => x.RowKey == "CurrentPageIndex")
                .Take(1)
                .AsTableQuery();

            EventStoreCurrentPage currentPage = null;

            while (currentPage == null)
            {
                token.ThrowIfCancellationRequested();
                var result = await table.ExecuteQuerySegmentedAsync(queryCurrentPartition, null, token);
                currentPage = result.SingleOrDefault();
                if (currentPage != null) continue;
                currentPage = new EventStoreCurrentPage
                {
                    PartitionKey = "Indexes",
                    RowKey = "CurrentPageIndex",
                    Index = 0
                };
                var operation = TableOperation.Insert(currentPage);
                try
                {
                    await table.ExecuteAsync(operation, token);
                }
                catch (StorageException e)
                {
                    if (e.RequestInformation.HttpStatusCode != (int) HttpStatusCode.Conflict) throw;
                    currentPage = null;
                }
            }

            return currentPage;
        }

        private class EventStorePageInfo : TableEntity
        {
            public long NextIndex { get; set; }
            public long NextPageIndex { get; set; }
        }

        private class EventStoreCurrentPage : TableEntity
        {
            public long Index { get; set; }
        }

        private class EventStoreRecord : TableEntity
        {
            public long PageIndex { get; set; }
            public long RowIndex { get; set; }
            public string AggregateType { get; set; }
            public string BucketId { get; set; }
            public string AggregateId { get; set; }
            public string SessionId { get; set; }
            public string Version { get; set; }
            public string Created { get; set; }
            public string Type { get; set; }
            public string Payload { get; set; }
        }
    }
}