using Estuite.Example.Configuration;
using Estuite.StreamDispatcher.Azure;
using Estuite.StreamStore.Azure;

namespace Estuite.Example
{
    public class ProgramConfiguration :
        IStreamStoreConfiguration,
        ICloudStorageAccountConfiguration,
        IStreamDispatcherConfiguration
    {
        public string ConnectionString => "UseDevelopmentStorage=true";
        string IStreamDispatcherConfiguration.TableName => "esEvents";
        string IStreamStoreConfiguration.TableName => "esStreams";
        public long PageSize => 500;
    }
}