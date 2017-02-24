using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public class EventStoreCloudTableProvider : IProvideEventStoreCloudTable
    {
        private readonly CloudTableClient _tableClient;
        private readonly string _tableName;
        private CloudTable _table;

        public EventStoreCloudTableProvider(CloudStorageAccount account, IStreamDispatcherConfiguration configuration)
        {
            _tableName = configuration.TableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public async Task<CloudTable> GetOrCreate()
        {
            if (_table != null) return _table;
            var table = _tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync();
            _table = table;
            return _table;
        }
    }
}