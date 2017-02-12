using System.Collections.Generic;
using Estuite.Domain;

namespace Estuite
{
    public interface ICreateSessions
    {
        Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> events);
    }
}