using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IConfirmDispatchedEvents
    {
        Task Confirm(CancellationToken token = new CancellationToken());
    }
}