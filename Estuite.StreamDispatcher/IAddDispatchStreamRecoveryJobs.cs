using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamStore
{
    public interface IAddDispatchStreamRecoveryJobs
    {
        Task Add(StreamDispatchJob id, CancellationToken token = new CancellationToken());
    }
}