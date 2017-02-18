namespace Estuite.StreamStore.Azure
{
    public interface IEventStoreConfiguration
    {
        string StreamTableName { get; }
        string EventTableName { get; }
    }
}