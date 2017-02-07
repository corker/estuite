namespace Estuite.AzureEventStore
{
    public interface IAddEvents
    {
        void Add(int version, object body);
    }
}