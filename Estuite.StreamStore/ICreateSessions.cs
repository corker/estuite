using System.Collections.Generic;
using Estuite.Domain;

namespace Estuite.StreamStore
{
    public interface ICreateSessions
    {
        Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> events);
    }
}