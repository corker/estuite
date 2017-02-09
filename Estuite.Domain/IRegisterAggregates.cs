namespace Estuite.Domain
{
    public interface IRegisterAggregates
    {
        void Register(ICanBeRegistered aggregate);
        void Register<TId>(TId id, IFlushEvents aggregate);
    }
}