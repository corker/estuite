using System;

namespace Estuite.AzureEventStore
{
    public class SessionId
    {
        public SessionId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            Value = value;
        }

        public string Value { get; private set; }
    }
}