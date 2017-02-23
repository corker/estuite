namespace Estuite.StreamDispatcher.Azure
{
    public interface IStreamDispatcherConfiguration
    {
        string EventTableName { get; }
    }
}