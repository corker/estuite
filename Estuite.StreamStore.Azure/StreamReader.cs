using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamStore.Azure
{
    public class StreamReader : IReadStreams
    {
        private readonly IDeserializeEvents _events;
        private readonly string _streamTableName;
        private readonly CloudTableClient _tableClient;

        public StreamReader(
            CloudStorageAccount account, 
            IStreamStoreConfiguration configuration,
            IDeserializeEvents events)
        {
            _events = events;
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task Read(
            StreamId streamId,
            IHydrateEvents events,
            CancellationToken token = new CancellationToken())
        {
            var result = await TryRead(streamId, events, token);
            if (result) return;
            string message = $"Stream {streamId.Value} not found.";
            throw new StreamNotFoundException(message);
        }

        public async Task<bool> TryRead(
            StreamId streamId,
            IHydrateEvents events,
            CancellationToken token = new CancellationToken())
        {
            var table = _tableClient.GetTableReference(_streamTableName);
            if (await table.CreateIfNotExistsAsync(token)) return false;
            var hydratedAny = false;
            var query = table.CreateQuery<EventTableEntity>()
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

        private class EventTableEntity : TableEntity
        {
            public string Created { get; set; }
            public string SessionId { get; set; }
            public string Type { get; set; }
            public string Payload { get; set; }
        }
    }
}