using System;
using Newtonsoft.Json;

namespace Estuite.Example.Services
{
    public class EventDeserializer : IDeserializeEvents
    {
        public object Deserialize(SerializedEvent @event)
        {
            var type = Type.GetType(@event.Type);
            return JsonConvert.DeserializeObject(@event.Payload, type);
        }
    }
}