namespace Estuite
{
    public interface IEventStoreConfiguration
    {
        string StreamTableName { get; }
        string EventTableName { get; }
    }
}