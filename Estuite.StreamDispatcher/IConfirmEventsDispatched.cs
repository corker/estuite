using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IConfirmEventsDispatched
    {
        Task Confirm(CancellationToken token = new CancellationToken());
    }
}