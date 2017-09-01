using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface IReadStreams
    {
        Task ReadInto<TId, TStream>(TId id, TStream stream, CancellationToken token = new CancellationToken())
            where TStream : IHydrateEvents, IFlushEvents;

        Task<bool> TryReadInto<TId, TStream>(TId id, TStream stream, CancellationToken token = new CancellationToken())
            where TStream : IHydrateEvents, IFlushEvents;
    }
}