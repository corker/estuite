using System;
using Estuite.Domain;

namespace Estuite
{
    public class DefaultStreamIdentityFactory : ICreateStreamIdentities
    {
        public StreamId Create<TId>(BucketId bucketId, TId id, Type type)
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            if (id.IsNullOrEmpty()) throw new ArgumentOutOfRangeException(nameof(id));
            if (type == null) throw new ArgumentNullException(nameof(type));
            var aggregateType = new AggregateType(type.Name);
            var aggregateId = new AggregateId($"{id}");
            return new StreamId(bucketId, aggregateType, aggregateId);
        }
    }
}