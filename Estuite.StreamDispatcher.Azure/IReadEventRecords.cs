using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IReadEventRecords
    {
        Task<IEnumerable<EventRecord>> Read(string partitionKey, CancellationToken token);
    }
}