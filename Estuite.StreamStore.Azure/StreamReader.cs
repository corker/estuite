using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamStore.Azure
{
    public class StreamReader : IReadStreams
    {
        private readonly IRestoreEventRecords _records;
        private readonly IProvideStreamStoreCloudTable _table;

        public StreamReader(IProvideStreamStoreCloudTable table, IRestoreEventRecords records)
        {
            _table = table;
            _records = records;
        }

        public async Task Read(StreamId streamId, IReceiveEventRecords records, CancellationToken token)
        {
            var result = await TryRead(streamId, records, token);
            if (result) return;
            var message = $"Stream {streamId.Value} not found.";
            throw new StreamNotFoundException(message);
        }

        public async Task<bool> TryRead(StreamId streamId, IReceiveEventRecords records, CancellationToken token)
        {
            var table = await _table.GetOrCreate();
            var handledAny = false;
            var query = table.CreateQuery<EventRecordTableEntity>()
                .Where(x => x.PartitionKey == streamId.Value)
                .Where(x => string.Compare(x.RowKey, "E^", StringComparison.Ordinal) > 0)
                .Where(x => string.Compare(x.RowKey, "F^", StringComparison.Ordinal) < 0)
                .AsTableQuery();
            TableContinuationToken queryToken = null;
            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, queryToken, token);
                if (segment.Any()) handledAny = true;
                queryToken = segment.ContinuationToken;
                var segmentRecords = segment.Select(x => _records.RestoreFrom(x));
                records.Receive(segmentRecords);
                if (token.IsCancellationRequested) throw new OperationCanceledException(token);
            } while (queryToken != null);
            return handledAny;
        }
    }
}