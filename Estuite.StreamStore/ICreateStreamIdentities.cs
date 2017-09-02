namespace Estuite.StreamStore
{
    public interface ICreateStreamIdentities
    {
        StreamId Create<T>(BucketId bucketId, object id);
    }
}