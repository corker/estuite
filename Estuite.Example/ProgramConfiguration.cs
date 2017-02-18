using Estuite.Example.Configuration;
using Estuite.StreamStore.Azure;

namespace Estuite.Example
{
    public class ProgramConfiguration : IStreamStoreConfiguration, ICloudStorageAccountConfiguration
    {
        public string ConnectionString => "UseDevelopmentStorage=true";
        public string TableName => "esStreams";
        public string EventTableName => "esEvents";
    }
}