namespace Estuite.Domain
{
    public interface IRegisterAggregates
    {
        void Register(ICanBeRegistered aggregate);
        void Register<TId, TAggregate>(TId id, TAggregate aggregate) where TAggregate : Aggregate<TId>;
    }
}