using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamStore.Azure
{
    public class EventToDispatchRecordRepository : IReadEventToDispatchRecords, IDeleteEventToDispatchRecords
    {
        private static readonly EventToDispatchRecordTableEntity[] Empty = new EventToDispatchRecordTableEntity[0];

        private readonly string _streamTableName;
        private readonly CloudTableClient _tableClient;

        public EventToDispatchRecordRepository(CloudStorageAccount account, IStreamStoreConfiguration configuration)
        {
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Delete(IEnumerable<EventToDispatchRecordTableEntity> records, CancellationToken token)
        {
            var operation = new TableBatchOperation();
            foreach (var record in records) operation.Delete(record);
            var table = _tableClient.GetTableReference(_streamTableName);
            await table.ExecuteBatchAsync(operation, token);
        }

        public async Task<IEnumerable<EventToDispatchRecordTableEntity>> Read(StreamId streamId, CancellationToken token)
        {
            var table = _tableClient.GetTableReference(_streamTableName);
            if (!await table.ExistsAsync(token)) return Empty;
            var query = table.CreateQuery<EventToDispatchRecordTableEntity>()
                .Where(x => x.PartitionKey == streamId.Value)
                .Where(x => string.Compare(x.RowKey, "D^", StringComparison.Ordinal) > 0)
                .Where(x => string.Compare(x.RowKey, "E^", StringComparison.Ordinal) < 0)
                .AsTableQuery();
            return await table.ExecuteQuerySegmentedAsync(query, null, token);
        }
    }
}