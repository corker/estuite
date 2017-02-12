using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface ICanBeHydrated
    {
        Task HydrateTo(IHydrateEventStreams streams, CancellationToken token = new CancellationToken());
    }
}