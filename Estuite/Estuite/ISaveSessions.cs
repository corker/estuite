using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public interface ISaveSessions
    {
        Task Save(StreamId streamId, Session session, CancellationToken token = new CancellationToken());
    }
}