using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface IHydrateAggregates
    {
        Task Hydrate(ICanReadStreams aggregate, CancellationToken token = new CancellationToken());
        Task<bool> TryHydrate(ICanReadStreams aggregate, CancellationToken token = new CancellationToken());
    }
}