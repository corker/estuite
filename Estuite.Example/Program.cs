using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Estuite.AzureEventStore;
using Estuite.Domain;
using Estuite.Example.Aggregates;
using Estuite.Example.Events;
using Microsoft.WindowsAzure.Storage;

namespace Estuite.Example
{
    internal class Program
    {
        private static readonly Guid AccountId = Guid.NewGuid();
        private static readonly ProgramConfiguration Configuration;
        private static readonly ISerializeEvents SerializeEvents;
        private static readonly IProvideUtcDateTime DateTime;
        private static readonly ICreateSessions Sessions;
        private static readonly CloudStorageAccount StorageAccount;
        private static readonly IWriteSessions WriteSessions;
        private static readonly BucketId BucketId = new BucketId("default");

        static Program()
        {
            Configuration = new ProgramConfiguration();
            SerializeEvents = new EventSerializer();
            DateTime = new UtcDateTimeProvider();
            Sessions = new SessionFactory(DateTime, SerializeEvents);
            StorageAccount = CloudStorageAccount.Parse(Configuration.ConnectionString);
            WriteSessions = new EventStore(StorageAccount, Configuration);
        }

        private static void Main(string[] args)
        {
            RegisterAggregateAndAppendAStream();
            VersionCollision();
            SessionCollision();
        }

        private static void RegisterAggregateAndAppendAStream()
        {
            var accountId = Guid.NewGuid();

            var unitOfWork = new UnitOfWork(BucketId, Sessions, WriteSessions);
            var aggregate = Account.Register(accountId, "MyAccount1");
            unitOfWork.Register(aggregate);
            var commit1 = unitOfWork.Commit();
            commit1.Wait();

            aggregate.ChangeName("MyAccount2");
            var commit2 = unitOfWork.Commit();
            commit2.Wait();
        }

        private static void VersionCollision()
        {
            var accountId = Guid.NewGuid();

            var unitOfWork1 = new UnitOfWork(BucketId, Sessions, WriteSessions);
            var aggregate1 = Account.Register(accountId, "MyAccount3");
            unitOfWork1.Register(aggregate1);
            var commit1 = unitOfWork1.Commit();
            commit1.Wait();

            var unitOfWork2 = new UnitOfWork(BucketId, Sessions, WriteSessions);
            var aggregate2 = Account.Register(accountId, "MyAccount3");
            unitOfWork2.Register(aggregate2);
            var commit2 = Task.Run(async () =>
            {
                try
                {
                    await unitOfWork2.Commit();
                }
                catch (ConcurrencyException e)
                {
                    Debug.WriteLine($"{e}");
                }
            });
            commit2.Wait();
        }

        private static void SessionCollision()
        {
            var sessionId = new SessionId($"{Guid.NewGuid()}");
            var aggregateType = new AggregateType("Account");
            var aggregateId = new AggregateId($"{Guid.NewGuid()}");

            var streamId = new StreamId(BucketId, aggregateType, aggregateId);
            var session1 = Sessions.Create(
                streamId,
                sessionId,
                new[]
                {
                    new Event(1, new AccountRegistered {AccountId = AccountId, Name = "MyAccount4"})
                });
            var write1 = WriteSessions.Write(session1);
            write1.Wait();

            var session2 = Sessions.Create(
                streamId,
                sessionId,
                new[]
                {
                    new Event(2, new AccountNameCorrected {AccountId = AccountId, Name = "MyAccount4"})
                });
            var write2 = Task.Run(async () =>
            {
                try
                {
                    await WriteSessions.Write(session2);
                }
                catch (ConcurrencyException e)
                {
                    Debug.WriteLine($"{e}");
                }
            });
            write2.Wait();
        }
    }
}