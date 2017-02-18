using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IConfirmEventsDispatched
    {
        Task Commit(CancellationToken token = new CancellationToken());
    }
}