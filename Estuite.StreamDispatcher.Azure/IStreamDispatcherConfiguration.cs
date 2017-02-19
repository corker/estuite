namespace Estuite.StreamDispatcher.Azure
{
    public interface IStreamDispatcherConfiguration
    {
        string StreamTableName { get; }
        string EventTableName { get; }
    }
}