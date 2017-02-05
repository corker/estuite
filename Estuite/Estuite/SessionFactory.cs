using System;
using System.Collections.Generic;
using System.Linq;

namespace Estuite
{
    public class SessionFactory : ICreateSessions
    {
        private readonly IProvideUtcDateTime _dateTime;
        private readonly ISerializeEvents _events;

        public SessionFactory(IProvideUtcDateTime dateTime, ISerializeEvents events)
        {
            _dateTime = dateTime;
            _events = events;
        }

        public Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> events)
        {
            var created = _dateTime.Now;
            var records = events.Select(x => CreateEventRecord(x, sessionId, created)).ToArray();
            return new Session(streamId, sessionId, created, records);
        }

        private EventRecord CreateEventRecord(Event @event, SessionId sessionId, DateTime created)
        {
            var eventSerialized = _events.Serialize(@event.Body);
            return new EventRecord
            {
                SessionId = sessionId,
                Version = @event.Version,
                Created = created,
                Type = eventSerialized.Type,
                Payload = eventSerialized.Payload
            };
        }
    }
}