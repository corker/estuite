using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamStore.Azure
{
    public interface IDeleteEventToDispatchRecords
    {
        Task Delete(IEnumerable<EventToDispatchRecordTableEntity> records, CancellationToken token = new CancellationToken());
    }
}