namespace Estuite
{
    public interface ICreateStreamIdentities
    {
        StreamId Create<TId>(BucketId bucketId, TId id, object aggregate);
    }
}