using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface ICanBeHydrated
    {
        Task HydrateFrom(IHydrateStreams streams, CancellationToken token = new CancellationToken());
        Task<bool> TryHydrateFrom(IHydrateStreams streams, CancellationToken token = new CancellationToken());
    }
}