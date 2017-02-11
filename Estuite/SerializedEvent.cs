using System;

namespace Estuite
{
    public class SerializedEvent
    {
        public SerializedEvent(string type, string payload)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentOutOfRangeException(nameof(type));
            if (string.IsNullOrWhiteSpace(payload)) throw new ArgumentOutOfRangeException(nameof(payload));
            Type = type;
            Payload = payload;
        }

        public string Type { get; }
        public string Payload { get; }
    }
}