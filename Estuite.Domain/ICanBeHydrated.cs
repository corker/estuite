using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface ICanBeHydrated
    {
        Task HydrateTo(IHydrateStreams streams, CancellationToken token = new CancellationToken());
    }
}