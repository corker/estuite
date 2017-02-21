using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public class CurrentPageIndexTableEntity : TableEntity
    {
        public long Index { get; set; }
    }
}