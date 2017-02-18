using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IDeleteEventRecords
    {
        Task Delete(IEnumerable<EventRecord> records, CancellationToken token = new CancellationToken());
    }
}