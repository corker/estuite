namespace Estuite.StreamStore.Azure
{
    public interface IStreamStoreConfiguration
    {
        string TableName { get; }
    }
}