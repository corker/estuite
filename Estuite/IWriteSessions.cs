using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public interface IWriteSessions
    {
        Task Write(Session session, CancellationToken token = new CancellationToken());
    }
}