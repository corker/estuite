using System;
using System.Threading;
using Estuite.Example.Events;
using Microsoft.WindowsAzure.Storage;

namespace Estuite.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var eventStream = CreateEventStream();
            eventStream.Add(1, new AccountRegistered {AccountId = Guid.NewGuid(), Name = "MyAccount"});
            eventStream.Add(2, new AccountNameCorrected {AccountId = Guid.NewGuid(), Name = "MyAccount"});

            var sessionId = new SessionId($"{Guid.NewGuid()}");
            var save = eventStream.Save(sessionId, new CancellationToken());
            save.ContinueWith(t => Console.WriteLine("Finished"));
            save.Wait();
        }

        private static EventStream CreateEventStream()
        {
            var configuration = new ProgramConfiguration();
            var bucketId = new BucketId("default");
            var aggregateType = new AggregateType("Account");
            var aggregateId = new AggregateId($"{Guid.NewGuid()}");
            var streamId = new StreamId(bucketId, aggregateType, aggregateId);
            ISerializeEvents serializeEvents = new EventSerializer();
            IProvideUtcDateTime dateTime = new UtcDateTimeProvider();
            ICreateSessions events = new SessionFactory(dateTime, serializeEvents);
            var account = CloudStorageAccount.Parse(configuration.ConnectionString);
            ISaveSessions sessions = new EventStore(account, configuration);
            return new EventStream(streamId, events, sessions);
        }
    }
}