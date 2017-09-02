using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;

namespace Estuite.StreamStore
{
    public interface IProvideAggregates
    {
        Task<T> Get<T>(object id, CancellationToken token = new CancellationToken()) where T : IReceiveEvents;
    }
}