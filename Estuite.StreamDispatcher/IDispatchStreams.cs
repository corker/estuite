using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamStore
{
    public interface IDispatchStreams
    {
        Task Dispatch(StreamDispatchJob job, CancellationToken token = new CancellationToken());
    }
}