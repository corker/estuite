using System;

namespace Estuite.StreamStore
{
    public interface ICreateStreamIdentities
    {
        StreamId Create<TId>(BucketId bucketId, TId id, Type type);
    }
}