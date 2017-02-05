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
        private static void Main(string[] args)
        {
            var accountId = Guid.NewGuid();
            var eventStream = CreateEventStream(accountId);
            SuccessfulSession1(eventStream, accountId);
            ConcurrentSession1(eventStream, accountId);
            eventStream = CreateEventStream(accountId);
            SuccessfulSession2(eventStream, accountId);
        }

        private static void SuccessfulSession1(EventStream eventStream, Guid accountId)
        {
            eventStream.Add(1, new AccountRegistered {AccountId = accountId, Name = "MyAccount1"});
            eventStream.Add(2, new AccountNameCorrected {AccountId = accountId, Name = "MyAccount2"});

            var sessionId = new SessionId($"{Guid.NewGuid()}");
            var save = eventStream.Save(sessionId, new CancellationToken());
            save.Wait();
        }

        private static void ConcurrentSession1(EventStream eventStream, Guid accountId)
        {
            eventStream.Add(1, new AccountRegistered {AccountId = accountId, Name = "MyAccount1"});
            eventStream.Add(3, new AccountNameCorrected {AccountId = accountId, Name = "MyAccount4"});

            var sessionId = new SessionId($"{Guid.NewGuid()}");
            var save = Task.Run(async () =>
            {
                try
                {
                    await eventStream.Save(sessionId, new CancellationToken());
                }
                catch (ConcurrencyException e)
                {
                    Debug.WriteLine($"{e}");
                }
            });
            save.Wait();
        }

        private static void SuccessfulSession2(EventStream eventStream, Guid accountId)
        {
            eventStream.Add(3, new AccountRegistered {AccountId = accountId, Name = "MyAccount3"});
            eventStream.Add(4, new AccountNameCorrected {AccountId = accountId, Name = "MyAccount4"});

            var sessionId = new SessionId($"{Guid.NewGuid()}");
            var save = eventStream.Save(sessionId, new CancellationToken());
            save.Wait();
        }

        private static EventStream CreateEventStream(Guid accountId)
        {
            var configuration = new ProgramConfiguration();
            var bucketId = new BucketId("default");
            var aggregateType = new AggregateType("Account");
            var aggregateId = new AggregateId($"{accountId}");
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