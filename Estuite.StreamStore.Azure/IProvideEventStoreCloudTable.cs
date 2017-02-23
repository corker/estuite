using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.StreamStore.Azure
{
    public interface IProvideStreamStoreCloudTable
    {
        Task<CloudTable> GetOrCreate();
    }
}