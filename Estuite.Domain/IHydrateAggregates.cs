using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface IHydrateAggregates
    {
        Task Hydrate(ICanBeHydrated aggregate, CancellationToken token = new CancellationToken());
        Task<bool> TryHydrate(ICanBeHydrated aggregate, CancellationToken token = new CancellationToken());
    }
}