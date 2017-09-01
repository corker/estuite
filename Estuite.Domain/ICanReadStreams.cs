using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public interface ICanReadStreams
    {
        Task ReadFrom(IReadStreams streams, CancellationToken token = new CancellationToken());
        Task<bool> TryReadFrom(IReadStreams streams, CancellationToken token = new CancellationToken());
    }
}