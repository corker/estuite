using System;

namespace Estuite.StreamStore
{
    public class EventRecord
    {
        public DateTime Created { get; set; }
        public SessionId SessionId { get; set; }
        public long Version { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}