using System;

namespace Estuite
{
    public class DefaultStreamIdentityFactory : ICreateStreamIdentities
    {
        public StreamId Create<TId>(BucketId bucketId, TId id, object aggregate)
        {
            throw new NotImplementedException();
        }
    }
}