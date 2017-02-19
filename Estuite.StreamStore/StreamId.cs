using System;

namespace Estuite.StreamStore
{
    public class StreamId
    {
        public StreamId(BucketId bucketId, AggregateType aggregateType, AggregateId aggregateId)
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            if (aggregateType == null) throw new ArgumentNullException(nameof(aggregateType));
            if (aggregateId == null) throw new ArgumentNullException(nameof(aggregateId));
            BucketId = bucketId;
            AggregateType = aggregateType;
            AggregateId = aggregateId;
            Value = $"{bucketId.Value}^{aggregateType.Value}^{aggregateId.Value}";
        }

        public BucketId BucketId { get; }
        public AggregateType AggregateType { get; }
        public AggregateId AggregateId { get; }
        public string Value { get; }
    }
}