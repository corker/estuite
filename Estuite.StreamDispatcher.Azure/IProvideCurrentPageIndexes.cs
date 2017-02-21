using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IProvideCurrentPageIndexes
    {
        Task<CurrentPageIndexTableEntity> Get(CancellationToken token);
    }
}