using System;

namespace Estuite.StreamStore
{
    public class AggregateType
    {
        public AggregateType(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
        }

        public string Value { get; }
    }
}