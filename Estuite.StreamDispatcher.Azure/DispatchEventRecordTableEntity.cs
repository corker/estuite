using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public class DispatchEventRecordTableEntity : TableEntity
    {
        public string AggregateType { get; set; }
        public string BucketId { get; set; }
        public string AggregateId { get; set; }
        public string SessionId { get; set; }
        public string Version { get; set; }
        public string Created { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}