using Estuite.Domain;

namespace Estuite
{
    public interface ICreateStreamIdentities
    {
        StreamId Create<TId, TAggregate>(BucketId bucketId, TId id) where TAggregate : Aggregate<TId>;
    }
}