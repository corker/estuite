using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamStore
{
    public interface IWriteStreams
    {
        Task Write(
            StreamId streamId, 
            IReadOnlyCollection<EventRecord> records,
            CancellationToken token = new CancellationToken()
            );
    }
}