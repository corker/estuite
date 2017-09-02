namespace Estuite.StreamStore
{
    public class DefaultStreamIdentityFactory : ICreateStreamIdentities
    {
        public StreamId Create<T>(BucketId bucketId, object id)
        {
            var type = new AggregateType(typeof(T).Name);
            var aggregateId = new AggregateId($"{id}");
            return new StreamId(bucketId, type, aggregateId);
        }
    }
}