using System.Collections.Generic;

namespace Estuite.Domain
{
    public interface IFlushEvents
    {
        IEnumerable<object> Flush();
    }
}