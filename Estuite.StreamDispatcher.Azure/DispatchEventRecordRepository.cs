using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamDispatcher.Azure
{
    public class DispatchEventRecordRepository : IReadEventRecords, IDeleteEventRecords
    {
        private static readonly DispatchEventRecordTableEntity[] Empty = new DispatchEventRecordTableEntity[0];

        private readonly string _streamTableName;
        private readonly CloudTableClient _tableClient;

        public DispatchEventRecordRepository(CloudStorageAccount account, IStreamDispatcherConfiguration configuration)
        {
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Delete(IEnumerable<DispatchEventRecordTableEntity> records, CancellationToken token)
        {
            var operation = new TableBatchOperation();
            foreach (var record in records) operation.Delete(record);
            var table = _tableClient.GetTableReference(_streamTableName);
            await table.ExecuteBatchAsync(operation, token);
        }

        public async Task<IEnumerable<DispatchEventRecordTableEntity>> Read(StreamId streamId, CancellationToken token)
        {
            var table = _tableClient.GetTableReference(_streamTableName);
            if (!await table.ExistsAsync(token)) return Empty;
            var query = table.CreateQuery<DispatchEventRecordTableEntity>()
                .Where(x => x.PartitionKey == streamId.Value)
                .Where(x => string.Compare(x.RowKey, "D^", StringComparison.Ordinal) > 0)
                .Where(x => string.Compare(x.RowKey, "E^", StringComparison.Ordinal) < 0)
                .AsTableQuery();
            return await table.ExecuteQuerySegmentedAsync(query, null, token);
        }
    }
}