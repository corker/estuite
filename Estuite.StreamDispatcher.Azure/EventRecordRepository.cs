using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamDispatcher.Azure
{
    public class EventRecordRepository : IReadEventRecords, IDeleteEventRecords
    {
        private readonly string _streamTableName;
        private readonly CloudTableClient _tableClient;

        public EventRecordRepository(CloudStorageAccount account, IStreamDispatcherConfiguration configuration)
        {
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Delete(
            IEnumerable<EventRecord> records,
            CancellationToken token = new CancellationToken())
        {
            var operation = new TableBatchOperation();
            foreach (var record in records) operation.Delete(record);
            var table = _tableClient.GetTableReference(_streamTableName);
            await table.ExecuteBatchAsync(operation, token);
        }

        public async Task<IEnumerable<EventRecord>> Read(
            string partitionKey,
            CancellationToken token = new CancellationToken())
        {
            var table = _tableClient.GetTableReference(_streamTableName);
            if (await table.ExistsAsync(token))
            {
                var query = table.CreateQuery<EventRecord>()
                    .Where(x => x.PartitionKey == partitionKey)
                    .Where(x => string.Compare(x.RowKey, "D^", StringComparison.Ordinal) > 0)
                    .Where(x => string.Compare(x.RowKey, "E^", StringComparison.Ordinal) < 0)
                    .AsTableQuery();
                return await table.ExecuteQuerySegmentedAsync(query, null, token);
            }
            return new EventRecord[0];
        }
    }
}