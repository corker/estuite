using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamStore.Azure
{
    public class StreamMarkerTableEntity : TableEntity
    {
        public string Updated { get; set; }
    }
}