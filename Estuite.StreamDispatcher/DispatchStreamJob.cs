using System;
using Estuite.StreamStore;

namespace Estuite.StreamDispatcher
{
    public class DispatchStreamJob
    {
        public SessionId SessionId { get; }
        public StreamId StreamId { get; }

        public DispatchStreamJob(StreamId streamId, SessionId sessionId)
        {
            if (streamId == null) throw new ArgumentNullException(nameof(streamId));
            if (sessionId == null) throw new ArgumentNullException(nameof(sessionId));
            StreamId = streamId;
            SessionId = sessionId;
        }
    }
}