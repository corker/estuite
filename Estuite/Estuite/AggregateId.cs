using System;

namespace Estuite
{
    public class AggregateId
    {
        public AggregateId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            Value = value;
        }

        public string Value { get; }
    }
}