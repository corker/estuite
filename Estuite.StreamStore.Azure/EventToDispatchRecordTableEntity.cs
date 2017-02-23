using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamStore.Azure
{
    public class EventToDispatchRecordTableEntity : TableEntity
    {
        public string AggregateType { get; set; }
        public string BucketId { get; set; }
        public string AggregateId { get; set; }
        public string SessionId { get; set; }
        public long Version { get; set; }
        public string Created { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}