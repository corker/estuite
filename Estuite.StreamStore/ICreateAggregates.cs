using Estuite.Domain;

namespace Estuite.StreamStore
{
    public interface ICreateAggregates
    {
        T Create<T>(object id) where T: IReceiveEvents;
    }
}