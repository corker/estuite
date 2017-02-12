using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public interface IWriteStreams
    {
        Task Write(Session session, CancellationToken token = new CancellationToken());
    }
}