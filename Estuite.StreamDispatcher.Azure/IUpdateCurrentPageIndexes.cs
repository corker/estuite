using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IUpdateCurrentPageIndexes
    {
        Task TryUpdate(CurrentPageIndexTableEntity entity, CancellationToken token = new CancellationToken());
    }
}