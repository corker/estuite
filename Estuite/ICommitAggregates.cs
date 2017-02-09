using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public interface ICommitAggregates
    {
        Task Commit(CancellationToken token = new CancellationToken());
    }
}