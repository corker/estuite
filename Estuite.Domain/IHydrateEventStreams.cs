namespace Estuite.Domain
{
    public interface IHydrateEventStreams
    {
        void Hydrate<TId, TEventStream>(TId id, TEventStream stream) where TEventStream : IHydrateEvents, IFlushEvents;
    }
}