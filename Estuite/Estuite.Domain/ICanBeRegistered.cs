namespace Estuite.Domain
{
    public interface ICanBeRegistered
    {
        void RegisterWith(IRegisterAggregates aggregates);
    }
}