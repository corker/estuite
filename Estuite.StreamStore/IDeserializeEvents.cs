namespace Estuite.StreamStore
{
    public interface IDeserializeEvents
    {
        object Deserialize(SerializedEvent @event);
    }
}