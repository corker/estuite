using System.Collections.Generic;

namespace Estuite
{
    public interface IFlushEvents
    {
        IEnumerable<object> Flush();
    }
}