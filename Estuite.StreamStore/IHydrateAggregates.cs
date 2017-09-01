using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;

namespace Estuite.StreamStore
{
    public interface IHydrateAggregates
    {
        Task Hydrate(ICanReadStreams aggregate, CancellationToken token = new CancellationToken());
        Task<bool> TryHydrate(ICanReadStreams aggregate, CancellationToken token = new CancellationToken());
    }
}