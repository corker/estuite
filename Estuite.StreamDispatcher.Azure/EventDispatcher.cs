using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamDispatcher.Azure
{
    public class EventDispatcher : IDispatchEvents
    {
        private const int PageSize = 10;
        private const int UndefinedNextPageIndex = 0;
        private const string PageInfoRowKey = "PageInfo";

        private readonly IProvideCurrentPageIndexes _provideCurrentPageIndexes;
        private readonly IProvideEventStoreCloudTable _table;
        private readonly IUpdateCurrentPageIndexes _updateCurrentPageIndexes;

        public EventDispatcher(
            IProvideEventStoreCloudTable table,
            IProvideCurrentPageIndexes provideCurrentPageIndexes,
            IUpdateCurrentPageIndexes updateCurrentPageIndexes)
        {
            _table = table;
            _provideCurrentPageIndexes = provideCurrentPageIndexes;
            _updateCurrentPageIndexes = updateCurrentPageIndexes;
        }

        public async Task Dispatch(List<EventToDispatchRecordTableEntity> events, CancellationToken token)
        {
            var table = await _table.GetOrCreate();

            CurrentPageIndexTableEntity pageIndex;
            EventStorePageInfoTableEntity pageInfo;

            var eventsCount = events.Count;
            var eventIndex = 0L;

            do
            {
                token.ThrowIfCancellationRequested();

                pageIndex = await _provideCurrentPageIndexes.Get(token);

                var partitionKey = $"P^{pageIndex.Index:x16}";

                var queryCurrentPageInfo = table.CreateQuery<EventStorePageInfoTableEntity>()
                    .Where(x => x.PartitionKey == partitionKey)
                    .Where(x => x.RowKey == PageInfoRowKey)
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
                        RowKey = PageInfoRowKey,
                        NextIndex = eventsCount,
                        NextPageIndex = 0
                    };
                    if (pageInfo.NextIndex > PageSize) pageInfo.NextPageIndex = pageIndex.Index + 1;
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
                    if (pageInfo.NextPageIndex == UndefinedNextPageIndex)
                    {
                        eventIndex = pageInfo.NextIndex;
                        pageInfo.NextIndex += eventsCount;
                        if (pageInfo.NextIndex > PageSize) pageInfo.NextPageIndex = pageIndex.Index + 1;
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