using Estuite.AzureEventStore;
using Estuite.Example.Configuration;

namespace Estuite.Example
{
    public class ProgramConfiguration : IEventStoreConfiguration, ICloudStorageAccountConfiguration
    {
        public string ConnectionString => "UseDevelopmentStorage=true";
        public string StreamTableName => "esStreams";
        public string EventTableName => "esEvents";
    }
}