using System;

namespace Estuite
{
    public class BucketId
    {
        public BucketId(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
        }

        public string Value { get; }
    }
}