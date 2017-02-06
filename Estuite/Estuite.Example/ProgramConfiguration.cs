using Estuite.Example.Configuration;

namespace Estuite.Example
{
    public class ProgramConfiguration : IEventStoreConfiguration, ICloudStorageAccountConfiguration
    {
        //public string ConnectionString => "UseDevelopmentStorage=true";
        public string ConnectionString => "DefaultEndpointsProtocol=https;AccountName=estuite;AccountKey=WqombaM2u/Tx41EJnT1LDuholL7Y2JbB2DoYzrdlUVYJSaX1ilLt+9r2I06YOtw+GdLqNuBsNdB+873kdKFItg==;TableEndpoint=https://estuite.table.core.windows.net/;";
        public string StreamTableName => "esStreams";
        public string EventTableName => "esEvents";
    }
}