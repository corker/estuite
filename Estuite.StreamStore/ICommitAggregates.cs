using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamStore
{
    public interface ICommitAggregates
    {
        Task Commit(CancellationToken token = new CancellationToken());
    }
}