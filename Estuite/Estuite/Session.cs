using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace Estuite
{
    public class Session
    {
        private readonly SessionId _sessionId;
        private readonly StreamId _streamId;
        private readonly IEnumerable<EventRecord> _records;

        public Session(StreamId streamId, SessionId sessionId, IEnumerable<EventRecord> records)
        {
            if (streamId == null) throw new ArgumentNullException(nameof(streamId));
            if (sessionId == null) throw new ArgumentNullException(nameof(sessionId));
            if (records == null) throw new ArgumentNullException(nameof(records));
            _streamId = streamId;
            _sessionId = sessionId;
            _records = records;
        }
    }
}