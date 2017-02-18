using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IDispatchEvents<in TEvent>
    {
        Task Dispatch(IEnumerable<TEvent> events, CancellationToken token = new CancellationToken());
    }
}