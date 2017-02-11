namespace Estuite.Domain
{
    public interface IRegisterAggregates
    {
        void Register(ICanBeRegistered aggregate);
    }
}