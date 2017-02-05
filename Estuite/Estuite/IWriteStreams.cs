using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public interface IWriteStreams
    {
        Task Save(SessionId sessionId, CancellationToken token = new CancellationToken());
    }
}