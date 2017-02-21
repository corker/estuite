using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public class EventStorePageInfoTableEntity : TableEntity
    {
        public long NextIndex { get; set; }
        public long NextPageIndex { get; set; }
    }
}