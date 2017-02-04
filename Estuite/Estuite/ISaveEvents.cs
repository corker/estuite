using System;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite
{
    public interface ISaveEvents
    {
        Task Save(SessionId sessionId, CancellationToken token = new CancellationToken());
    }
}