namespace Estuite
{
    public interface IDeserializeEvents
    {
        object Deserialize(SerializedEvent @event);
    }
}