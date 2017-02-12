using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface IHydrateEventStreams
    {
        Task Hydrate<TId, TEventStream>(
            TId id,
            TEventStream stream,
            CancellationToken token = new CancellationToken())
            where TEventStream : IHydrateEvents, IFlushEvents;

        Task<bool> TryHydrate<TId, TEventStream>(
            TId id,
            TEventStream stream,
            CancellationToken token = new CancellationToken())
            where TEventStream : IHydrateEvents, IFlushEvents;
    }
}