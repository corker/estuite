namespace Estuite
{
    public class DefaultStreamIdentityFactory : ICreateStreamIdentities
    {
        public StreamId Create<TId>(BucketId bucketId, TId id, object aggregate)
        {
            var aggregateType = new AggregateType(aggregate.GetType().Name);
            var aggregateId = new AggregateId($"{id}");
            return new StreamId(bucketId, aggregateType, aggregateId);
        }
    }
}