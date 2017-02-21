using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public class DispatchStreamRecoveryJobTableEntity : TableEntity
    {
        public string StreamId { get; set; }
    }
}