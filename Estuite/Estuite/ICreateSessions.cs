using System.Collections.Generic;

namespace Estuite
{
    public interface ICreateSessions
    {
        Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> @event);
    }
}