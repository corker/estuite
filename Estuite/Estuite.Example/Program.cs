using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Example.Events;
using Microsoft.WindowsAzure.Storage;

namespace Estuite.Example
{
    internal class Program
    {
        private static readonly Guid AccountId = Guid.NewGuid();

        private static void Main(string[] args)
        {
            CreateStream();
            AppendEvents();
            VersionCollision();
            SessionCollision();
        }

        private static void CreateStream()
        {
            var stream = CreateEventStream();
            var sessionId = new SessionId($"{Guid.NewGuid()}");

            stream.Add(1, new AccountRegistered {AccountId = AccountId, Name = "MyAccount1"});
            var save = stream.Save(sessionId, new CancellationToken());
            save.Wait();
        }

        private static void AppendEvents()
        {
            var stream = CreateEventStream();
            var sessionId = new SessionId($"{Guid.NewGuid()}");

            stream.Add(2, new AccountNameCorrected { AccountId = AccountId, Name = "MyAccount2" });
            var save = stream.Save(sessionId, new CancellationToken());
            save.Wait();
        }

        private static void VersionCollision()
        {
            var stream = CreateEventStream();
            var sessionId = new SessionId($"{Guid.NewGuid()}");

            stream.Add(3, new AccountNameCorrected {AccountId = AccountId, Name = "MyAccount3"});
            var save1 = stream.Save(sessionId, new CancellationToken());
            save1.Wait();

            sessionId = new SessionId($"{Guid.NewGuid()}");
            stream.Add(3, new AccountNameCorrected {AccountId = AccountId, Name = "MyAccount3"});
            var save2 = Task.Run(async () =>
            {
                try
                {
                    await stream.Save(sessionId, new CancellationToken());
                }
                catch (ConcurrencyException e)
                {
                    Debug.WriteLine($"{e}");
                }
            });
            save2.Wait();
        }

        private static void SessionCollision()
        {
            var stream = CreateEventStream();
            var sessionId = new SessionId($"{Guid.NewGuid()}");

            stream.Add(4, new AccountRegistered {AccountId = AccountId, Name = "MyAccount4"});
            var save1 = stream.Save(sessionId, new CancellationToken());
            save1.Wait();

            stream.Add(5, new AccountRegistered {AccountId = AccountId, Name = "MyAccount4"});
            var save2 = Task.Run(async () =>
            {
                try
                {
                    await stream.Save(sessionId, new CancellationToken());
                }
                catch (ConcurrencyException e)
                {
                    Debug.WriteLine($"{e}");
                }
            });
            save2.Wait();
        }

        private static EventStream CreateEventStream()
        {
            var configuration = new ProgramConfiguration();
            var bucketId = new BucketId("default");
            var aggregateType = new AggregateType("Account");
            var aggregateId = new AggregateId($"{AccountId}");
            var streamId = new StreamId(bucketId, aggregateType, aggregateId);
            ISerializeEvents serializeEvents = new EventSerializer();
            IProvideUtcDateTime dateTime = new UtcDateTimeProvider();
            ICreateSessions events = new SessionFactory(dateTime, serializeEvents);
            var account = CloudStorageAccount.Parse(configuration.ConnectionString);
            IWriteSessions sessions = new EventStore(account, configuration);
            return new EventStream(streamId, events, sessions);
        }
    }
}