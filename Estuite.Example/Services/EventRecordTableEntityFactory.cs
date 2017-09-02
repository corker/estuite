using Estuite.StreamStore;
using Estuite.StreamStore.Azure;
using Newtonsoft.Json;

namespace Estuite.Example.Services
{
    public class EventRecordTableEntityFactory : ICreateEventRecordTableEntities
    {
        public EventRecordTableEntity CreateFrom(EventRecord record)
        {
            var type = record.Body.GetType().FullName;
            var payload = JsonConvert.SerializeObject(record.Body);
            return new EventRecordTableEntity
            {
                Type = type,
                Payload = payload
            };
        }
    }
}