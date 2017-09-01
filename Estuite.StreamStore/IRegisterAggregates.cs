using Estuite.Domain;

namespace Estuite.StreamStore
{
    public interface IRegisterAggregates
    {
        void Register(ICanBeRegistered aggregate);
    }
}