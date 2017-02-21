using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IDeleteDispatchStreamRecoveryJobs
    {
        Task Delete(StreamDispatchJob job, CancellationToken token = new CancellationToken());
    }
}