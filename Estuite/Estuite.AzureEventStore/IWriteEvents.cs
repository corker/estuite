using System.Threading;
using System.Threading.Tasks;

namespace Estuite.AzureEventStore
{
    public interface IWriteEvents
    {
        Task Write(SessionId sessionId, CancellationToken token = new CancellationToken());
    }
}