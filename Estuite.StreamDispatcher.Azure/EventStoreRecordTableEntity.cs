using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public class EventStoreRecordTableEntity : TableEntity
    {
        public long PageIndex { get; set; }
        public long RowIndex { get; set; }
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