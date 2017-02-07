using System.Collections.Generic;

namespace Estuite.Domain
{
    public interface IFlushEvents
    {
        List<Event> Flush();
    }
}