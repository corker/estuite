namespace Estuite
{
    public interface ISerializeEvents
    {
        SerializedEvent Serialize(object @event);
    }
}