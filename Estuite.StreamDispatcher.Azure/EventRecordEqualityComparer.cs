using System.Collections.Generic;

namespace Estuite.StreamDispatcher.Azure
{
    public sealed class EventRecordEqualityComparer : IEqualityComparer<EventRecord>
    {
        public bool Equals(EventRecord x, EventRecord y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.PartitionKey, y.PartitionKey) && string.Equals(x.RowKey, y.RowKey);
        }

        public int GetHashCode(EventRecord obj)
        {
            unchecked
            {
                return (obj.PartitionKey.GetHashCode()*397) ^ obj.RowKey.GetHashCode();
            }
        }
    }
}