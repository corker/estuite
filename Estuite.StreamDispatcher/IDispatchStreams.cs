using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IDispatchStreams
    {
        Task Dispatch(DispatchStreamJob job, CancellationToken token = new CancellationToken());
    }
}