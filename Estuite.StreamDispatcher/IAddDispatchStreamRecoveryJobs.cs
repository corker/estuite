using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public interface IAddDispatchStreamRecoveryJobs
    {
        Task Add(DispatchStreamJob id, CancellationToken token = new CancellationToken());
    }
}