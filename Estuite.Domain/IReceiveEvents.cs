using System.Collections.Generic;

namespace Estuite.Domain
{
    public interface IReceiveEvents
    {
        void Receive(IEnumerable<Event> events);
    }
}