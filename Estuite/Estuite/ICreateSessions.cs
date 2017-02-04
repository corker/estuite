using System.Collections.Generic;

namespace Estuite
{
    public interface ICreateSessions
    {
        Session Create(IEnumerable<Event> @event, SessionId sessionId);
    }
}