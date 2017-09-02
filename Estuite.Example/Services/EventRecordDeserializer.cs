using System;
using Estuite.StreamStore;
using Estuite.StreamStore.Azure;
using Newtonsoft.Json;

namespace Estuite.Example.Services
{
    public class EventRecordDeserializer : IRestoreEventRecords
    {
        public EventRecord RestoreFrom(EventRecordTableEntity entity)
        {
            var type = Type.GetType(entity.Type);
            var body = JsonConvert.DeserializeObject(entity.Payload, type);
            return new EventRecord(entity.Version, body);
        }
    }
}