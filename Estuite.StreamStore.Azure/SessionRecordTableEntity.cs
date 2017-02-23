using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamStore.Azure
{
    public class SessionRecordTableEntity : TableEntity
    {
        public string Created { get; set; }
        public int RecordCount { get; set; }
    }
}