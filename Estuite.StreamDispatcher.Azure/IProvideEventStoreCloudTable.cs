using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IProvideEventStoreCloudTable
    {
        Task<CloudTable> GetOrCreate();
    }
}