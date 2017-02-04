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
        private readonly StreamId _streamId;
        private readonly ISaveSessions _sessions;

        public EventStream(StreamId streamId, ICreateSessions events, ISaveSessions sessions)
        {
            if (streamId == null) throw new ArgumentNullException(nameof(streamId));
            if (events == null) throw new ArgumentNullException(nameof(events));
            _streamId = streamId;
            _events = events;
            _sessions = sessions;
            _eventsToSave = new List<Event>();
        }

        public void Add(Event @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            _eventsToSave.Add(@event);
        }

        public async Task Save(SessionId sessionId, CancellationToken token)
        {
            var session = _events.Create(_eventsToSave, sessionId);
            await _sessions.Save(_streamId, session, token);
            _eventsToSave.Clear();
        }
    }
}