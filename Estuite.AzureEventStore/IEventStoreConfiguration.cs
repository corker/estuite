namespace Estuite.AzureEventStore
{
    public interface IEventStoreConfiguration
    {
        string StreamTableName { get; }
        string EventTableName { get; }
    }
}