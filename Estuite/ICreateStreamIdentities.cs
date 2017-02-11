using System;

namespace Estuite
{
    public interface ICreateStreamIdentities
    {
        StreamId Create<TId, TAggregate>(BucketId bucketId, TId id);
    }
}