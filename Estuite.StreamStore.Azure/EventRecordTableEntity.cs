using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamStore.Azure
{
    public class EventRecordTableEntity : TableEntity
    {
        public string Created { get; set; }
        public string SessionId { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}