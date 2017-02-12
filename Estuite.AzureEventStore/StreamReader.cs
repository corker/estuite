using System;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Estuite.AzureEventStore
{
    public class StreamReader : IReadStreams
    {
        private readonly string _streamTableName;
        private readonly CloudTableClient _tableClient;

        public StreamReader(CloudStorageAccount account, IEventStoreConfiguration configuration)
        {
            _streamTableName = configuration.StreamTableName;
            _tableClient = account.CreateCloudTableClient();
        }

        public Task Read(
            StreamId streamId,
            IHydrateEvents events,
            CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryRead(
            StreamId streamId,
            IHydrateEvents events,
            CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}