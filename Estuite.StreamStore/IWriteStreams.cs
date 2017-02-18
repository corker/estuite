using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamStore
{
    public interface IWriteStreams
    {
        Task Write(Session session, CancellationToken token = new CancellationToken());
    }
}