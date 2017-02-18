using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IDispatchStreams
    {
        Task Dispatch(StreamId id, CancellationToken token = new CancellationToken());
    }
}