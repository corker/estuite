using Estuite.Example.Configuration;
using Estuite.StreamStore.Azure;

namespace Estuite.Example
{
    public class ProgramConfiguration : IEventStoreConfiguration, ICloudStorageAccountConfiguration
    {
        public string ConnectionString => "UseDevelopmentStorage=true";
        public string StreamTableName => "esStreams";
        public string EventTableName => "esEvents";
    }
}