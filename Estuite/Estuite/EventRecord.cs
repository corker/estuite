using System;

namespace Estuite
{
    public class EventRecord
    {
        public DateTime Created { get; set; }
        public string SessionId { get; set; }
        public int Version { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}