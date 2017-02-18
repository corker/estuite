namespace Estuite.StreamDispatcher.Azure
{
    public interface IStreamDispatcherConfiguration
    {
        string TableName { get; }
    }
}