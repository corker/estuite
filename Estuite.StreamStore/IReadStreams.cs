using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamStore
{
    public interface IReadStreams
    {
        Task Read(StreamId streamId, IReceiveEventRecords records, CancellationToken token = new CancellationToken());
        Task<bool> TryRead(StreamId streamId, IReceiveEventRecords records, CancellationToken token = new CancellationToken());
    }
}