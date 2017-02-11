namespace Estuite.Domain
{
    public interface IRegisterEventStreams
    {
        void Register<TId, TEventStream>(TId id, TEventStream stream) where TEventStream : IFlushEvents;
    }
}