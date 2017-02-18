using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IPullEventsForDispatching<TEvent>
    {
        Task<List<TEvent>> Pull(StreamId id, CancellationToken token = new CancellationToken());
    }
}