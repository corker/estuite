using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public interface IWriteEvents
    {
        Task Write(SessionId sessionId, CancellationToken token = new CancellationToken());
    }
}