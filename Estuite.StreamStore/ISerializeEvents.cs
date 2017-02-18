namespace Estuite.StreamStore
{
    public interface ISerializeEvents
    {
        SerializedEvent Serialize(object @event);
    }
}