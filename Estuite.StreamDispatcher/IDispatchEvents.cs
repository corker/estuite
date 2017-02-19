using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IDispatchEvents<TEvent>
    {
        Task Dispatch(List<TEvent> events, CancellationToken token = new CancellationToken());
    }
}