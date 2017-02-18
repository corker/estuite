using Estuite.StreamStore;
using Newtonsoft.Json;

namespace Estuite.Example.Services
{
    public class EventSerializer : ISerializeEvents
    {
        public SerializedEvent Serialize(object @event)
        {
            var type = @event.GetType().FullName;
            var payload = JsonConvert.SerializeObject(@event);
            return new SerializedEvent(type, payload);
        }
    }
}