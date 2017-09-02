using System;
using Estuite.StreamStore;

namespace Estuite.StreamDispatcher
{
    public class DispatchStreamJob
    {
        public DispatchStreamJob(StreamId streamId, SessionId sessionId)
        {
            StreamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            SessionId = sessionId ?? throw new ArgumentNullException(nameof(sessionId));
        }

        public SessionId SessionId { get; }
        public StreamId StreamId { get; }
    }
}