namespace Estuite.AzureEventStore
{
    public interface ISerializeEvents
    {
        SerializedEvent Serialize(object @event);
    }
}