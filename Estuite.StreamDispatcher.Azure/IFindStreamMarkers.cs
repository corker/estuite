using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IFindStreamMarkers
    {
        Task<StreamMarkerTableEntity> Find(StreamId streamId, CancellationToken token = new CancellationToken());
    }
}