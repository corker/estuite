using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public interface IWriteEventStreams
    {
        Task Write(Session session, CancellationToken token = new CancellationToken());
    }
}