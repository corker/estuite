using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamStore.Azure
{
    public class StreamReader : IReadStreams
    {
        private readonly IDeserializeEvents _events;
        private readonly IProvideStreamStoreCloudTable _table;

        public StreamReader(IProvideStreamStoreCloudTable table, IDeserializeEvents events)
        {
            _table = table;
            _events = events;
        }

        public async Task Read(StreamId streamId, IHydrateEvents events, CancellationToken token)
        {
            var result = await TryRead(streamId, events, token);
            if (result) return;
            string message = $"Stream {streamId.Value} not found.";
            throw new StreamNotFoundException(message);
        }

        public async Task<bool> TryRead(StreamId streamId, IHydrateEvents events, CancellationToken token)
        {
            var table = await _table.GetOrCreate();
            var hydratedAny = false;
            var query = table.CreateQuery<EventRecordTableEntity>()
                .Where(x => x.PartitionKey == streamId.Value)
                .Where(x => string.Compare(x.RowKey, "E^", StringComparison.Ordinal) > 0)
                .Where(x => string.Compare(x.RowKey, "F^", StringComparison.Ordinal) < 0)
                .AsTableQuery();
            TableContinuationToken queryToken = null;
            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, queryToken, token);
                queryToken = segment.ContinuationToken;
                var eventsFromStore = segment
                    .Select(x =>
                    {
                        hydratedAny = true;
                        var serializedEvent = new SerializedEvent(x.Type, x.Payload);
                        return _events.Deserialize(serializedEvent);
                    });
                events.Hydrate(eventsFromStore);
                if (token.IsCancellationRequested) throw new OperationCanceledException(token);
            } while (queryToken != null);
            return hydratedAny;
        }
    }
}