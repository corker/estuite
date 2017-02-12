using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface ICanBeHydrated
    {
        Task HydrateTo(IHydrateStreams streams, CancellationToken token = new CancellationToken());
        Task<bool> TryHydrateTo(IHydrateStreams streams, CancellationToken token = new CancellationToken());
    }
}