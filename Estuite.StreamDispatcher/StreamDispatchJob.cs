using System;

namespace Estuite.StreamStore
{
    public class StreamDispatchJob
    {
        public SessionId SessionId { get; }
        public StreamId StreamId { get; }


        public StreamDispatchJob(StreamId streamId, SessionId sessionId)
        {
            if (streamId == null) throw new ArgumentNullException(nameof(streamId));
            if (sessionId == null) throw new ArgumentNullException(nameof(sessionId));
            StreamId = streamId;
            SessionId = sessionId;
        }
    }
}