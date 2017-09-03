using System;

namespace Estuite.StreamStore
{
    public class StreamId
    {
        public StreamId(BucketId bucketId, AggregateType aggregateType, AggregateId aggregateId)
        {
            BucketId = bucketId ?? throw new ArgumentNullException(nameof(bucketId));
            AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));
            AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
            Value = $"{bucketId.Value}^{aggregateType.Value}^{aggregateId.Value}";
        }

        public BucketId BucketId { get; }
        public AggregateType AggregateType { get; }
        public AggregateId AggregateId { get; }
        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}