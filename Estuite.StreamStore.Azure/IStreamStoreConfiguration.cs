namespace Estuite.StreamStore.Azure
{
    public interface IStreamStoreConfiguration
    {
        string StreamTableName { get; }
    }
}