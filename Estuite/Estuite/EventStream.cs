using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public class EventStream : ISaveEvents, IAddEvents
    {
        private readonly ICreateSessions _events;
        private readonly List<Event> _eventsToSave;
        private readonly IWriteSessions _sessions;
        private readonly StreamId _streamId;

        public EventStream(StreamId streamId, ICreateSessions events, IWriteSessions sessions)
        {
            if (streamId == null) throw new ArgumentNullException(nameof(streamId));
            if (events == null) throw new ArgumentNullException(nameof(events));
            _streamId = streamId;
            _events = events;
            _sessions = sessions;
            _eventsToSave = new List<Event>();
        }

        public void Add(int version, object body)
        {
            var @event = new Event(version, body);
            _eventsToSave.Add(@event);
        }

        public async Task Save(SessionId sessionId, CancellationToken token)
        {
            var session = _events.Create(_streamId, sessionId, _eventsToSave);
            await _sessions.Write(session, token);
            _eventsToSave.Clear();
        }
    }
}