using System;

namespace Estuite
{
    public class SerializedEvent
    {
        public SerializedEvent(string type, string payload)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(type));
            if (string.IsNullOrWhiteSpace(payload))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(payload));
            Type = type;
            Payload = payload;
        }

        public string Type { get; }
        public string Payload { get; }
    }
}