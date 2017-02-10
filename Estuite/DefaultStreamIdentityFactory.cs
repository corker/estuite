using System;
using Estuite.Domain;

namespace Estuite
{
    public class DefaultStreamIdentityFactory : ICreateStreamIdentities
    {
        public StreamId Create<TId, TAggregate>(BucketId bucketId, TId id) where TAggregate : Aggregate<TId>
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            if (id.IsNullOrEmpty()) throw new ArgumentOutOfRangeException(nameof(id));
            var aggregateType = new AggregateType(typeof(TAggregate).Name);
            var aggregateId = new AggregateId($"{id}");
            return new StreamId(bucketId, aggregateType, aggregateId);
        }
    }
}