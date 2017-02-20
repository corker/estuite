using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Estuite.StreamDispatcher.Azure
{
    public class StreamMarkerRepository : IFindStreamMarkers, IDeleteStreamMarkers
    {
        private readonly string _streamTableName;
        private readonly CloudTableClient _tableClient;

        public StreamMarkerRepository(CloudStorageAccount account, IStreamDispatcherConfiguration configuration)
        {
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task TryDelete(
            StreamMarkerTableEntity streamMarker,
            CancellationToken token = new CancellationToken())
        {
            var table = _tableClient.GetTableReference(_streamTableName);
            var operation = TableOperation.Delete(streamMarker);
            try
            {
                await table.ExecuteAsync(operation, token);
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode != (int) HttpStatusCode.Conflict) throw;
            }
        }

        public async Task<StreamMarkerTableEntity> Find(
            StreamId streamId,
            CancellationToken token = new CancellationToken())
        {
            var table = _tableClient.GetTableReference(_streamTableName);
            var query = table.CreateQuery<StreamMarkerTableEntity>()
                .Where(x => x.PartitionKey == "StreamMarkers")
                .Where(x => x.RowKey == streamId.Value)
                .AsTableQuery();
            var segment = await table.ExecuteQuerySegmentedAsync(query, null, token);
            return segment.SingleOrDefault();
        }
    }
}