using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IDeleteDispatchStreamRecoveryJobs
    {
        Task Delete(DispatchStreamJob job, CancellationToken token = new CancellationToken());
    }
}