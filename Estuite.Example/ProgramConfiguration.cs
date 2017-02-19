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
        public string EventTableName => "esEvents";
        public string StreamTableName => "esStreams";
    }
}