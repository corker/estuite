namespace Estuite.Domain
{
    public interface IRegisterStreams
    {
        void Register<TId, TStream>(TId id, TStream stream) where TStream : IFlushEvents;
    }
}