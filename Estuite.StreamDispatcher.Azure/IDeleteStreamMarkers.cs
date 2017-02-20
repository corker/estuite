using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IDeleteStreamMarkers
    {
        Task TryDelete(StreamMarkerTableEntity streamMarker, CancellationToken token = new CancellationToken());
    }
}