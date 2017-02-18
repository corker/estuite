using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;

namespace Estuite.StreamStore
{
    public interface IReadStreams
    {
        Task Read(StreamId streamId, IHydrateEvents events, CancellationToken token = new CancellationToken());
        Task<bool> TryRead(StreamId streamId, IHydrateEvents events, CancellationToken token = new CancellationToken());
    }
}