using System;

namespace Estuite
{
    public interface ICreateStreamIdentities
    {
        StreamId Create<TId>(BucketId bucketId, TId id, Type type);
    }
}