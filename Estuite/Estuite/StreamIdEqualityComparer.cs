using System;
using System.Collections.Generic;

namespace Estuite
{
    public sealed class StreamIdEqualityComparer : IEqualityComparer<StreamId>
    {
        public static readonly StreamIdEqualityComparer Instance = new StreamIdEqualityComparer();

        private StreamIdEqualityComparer()
        {
        }

        public bool Equals(StreamId x, StreamId y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.Value, y.Value, StringComparison.InvariantCulture);
        }

        public int GetHashCode(StreamId obj)
        {
            return obj.Value != null ? StringComparer.InvariantCulture.GetHashCode(obj.Value) : 0;
        }
    }
}