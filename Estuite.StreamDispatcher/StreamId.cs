using System;

namespace Estuite.StreamDispatcher
{
    public class StreamId
    {
        public StreamId(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
        }

        public string Value { get; }
    }
}