using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace Estuite
{
    public interface ICreateSessions
    {
        Session Create(IEnumerable<Event> @event, SessionId sessionId);
    }
}