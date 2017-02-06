namespace Estuite.Domain
{
    public interface IRegisterAggregates
    {
        void Register<T>(T aggregate) where T : ICanBeRegistered;
        void Register<T, TId>(TId id, T aggregate) where T : Aggregate<TId>;
    }
}