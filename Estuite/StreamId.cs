using System;

namespace Estuite
{
    public class StreamId
    {
        public StreamId(BucketId bucketId, AggregateType aggregateType, AggregateId aggregateId)
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            if (aggregateType == null) throw new ArgumentNullException(nameof(aggregateType));
            if (aggregateId == null) throw new ArgumentNullException(nameof(aggregateId));
            Value = $"{bucketId.Value}^{aggregateType.Value}^{aggregateId.Value}";
        }

        public string Value { get; }
    }
}