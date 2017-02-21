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
    public class AzureEventDispatcher : IDispatchEvents<DispatchEventRecordTableEntity>
    {
        private readonly IProvideCurrentPageIndexes _provideCurrentPageIndexes;
        private readonly CloudTableClient _tableClient;
        private readonly string _tableName;
        private readonly IUpdateCurrentPageIndexes _updateCurrentPageIndexes;

        public AzureEventDispatcher(
            CloudStorageAccount account,
            IStreamDispatcherConfiguration configuration,
            IProvideCurrentPageIndexes provideCurrentPageIndexes,
            IUpdateCurrentPageIndexes updateCurrentPageIndexes)
        {
            _provideCurrentPageIndexes = provideCurrentPageIndexes;
            _updateCurrentPageIndexes = updateCurrentPageIndexes;
            _tableName = configuration.EventTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Dispatch(List<DispatchEventRecordTableEntity> events, CancellationToken token)
        {
            var table = _tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync(token);

            CurrentPageIndexTableEntity pageIndex = null;
            EventStorePageInfoTableEntity pageInfo = null;

            var eventsCount = events.Count;
            var eventIndex = 0L;

            do
            {
                token.ThrowIfCancellationRequested();

                pageIndex = await _provideCurrentPageIndexes.Get(token);

                var partitionKey = $"P^{pageIndex.Index:x16}";

                var queryCurrentPageInfo = table.CreateQuery<EventStorePageInfoTableEntity>()
                    .Where(x => x.PartitionKey == partitionKey)
                    .Where(x => x.RowKey == "PageInfo")
                    .Take(1)
                    .AsTableQuery();

                var result = await table.ExecuteQuerySegmentedAsync(queryCurrentPageInfo, null, token);
                pageInfo = result.SingleOrDefault();

                if (pageInfo == null)
                {
                    eventIndex = 0;
                    pageInfo = new EventStorePageInfoTableEntity
                    {
                        PartitionKey = partitionKey,
                        RowKey = "PageInfo",
                        NextIndex = eventsCount,
                        NextPageIndex = 0
                    };
                    if (pageInfo.NextIndex > 500) pageInfo.NextPageIndex = pageIndex.Index + 1;
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
                    if (pageInfo.NextPageIndex == 0)
                    {
                        eventIndex = pageInfo.NextIndex;
                        pageInfo.NextIndex += eventsCount;
                        if (pageInfo.NextIndex > 500) pageInfo.NextPageIndex = pageIndex.Index + 1;
                        var operation = TableOperation.Replace(pageInfo);
                        try
                        {
                            await table.ExecuteAsync(operation, token);
                        }
                        catch (StorageException e)
                        {
                            if (e.RequestInformation.HttpStatusCode != (int) HttpStatusCode.PreconditionFailed) throw;
                            pageInfo = null;
                        }
                    }
                    else
                    {
                        pageIndex.Index = pageInfo.NextPageIndex;
                        await _updateCurrentPageIndexes.TryUpdate(pageIndex, token);
                        pageInfo = null;
                    }
                }
            } while (pageInfo == null);

            var batchOperation = new TableBatchOperation();

            foreach (var @event in events)
            {
                var record = new EventStoreRecordTableEntity
                {
                    PartitionKey = $"P^{pageIndex.Index:x16}",
                    RowKey = $"E^{eventIndex:x16}",
                    PageIndex = pageIndex.Index,
                    RowIndex = eventIndex,
                    AggregateType = @event.AggregateType,
                    BucketId = @event.BucketId,
                    AggregateId = @event.AggregateId,
                    SessionId = @event.SessionId,
                    Version = @event.Version,
                    Created = @event.Created,
                    Type = @event.Type,
                    Payload = @event.Payload
                };
                eventIndex++;
                batchOperation.InsertOrReplace(record);
            }

            await table.ExecuteBatchAsync(batchOperation, token);
        }
    }
}