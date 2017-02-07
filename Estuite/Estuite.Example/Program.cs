using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Estuite.AzureEventStore;
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
        private static readonly GuidCombGenerator Identities;

        static Program()
        {
            Configuration = new ProgramConfiguration();
            SerializeEvents = new EventSerializer();
            DateTime = new UtcDateTimeProvider();
            Sessions = new SessionFactory(DateTime, SerializeEvents);
            StorageAccount = CloudStorageAccount.Parse(Configuration.ConnectionString);
            WriteSessions = new EventStore(StorageAccount, Configuration);
            Identities = new GuidCombGenerator(DateTime);
        }

        private static void Main(string[] args)
        {
            RegisterAggregateAndAppendAStream();
            VersionCollision();
            //SessionCollision();
        }

        private static void RegisterAggregateAndAppendAStream()
        {
            var unitOfWork = CreateUnitOfWork();
            var aggregate = Account.Register(Guid.NewGuid(), "MyAccount1");
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

            var unitOfWork1 = CreateUnitOfWork();
            var aggregate1 = Account.Register(accountId, "MyAccount3");
            unitOfWork1.Register(aggregate1);
            var commit1 = unitOfWork1.Commit();
            commit1.Wait();

            var unitOfWork2 = CreateUnitOfWork();
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

        //private static void SessionCollision()
        //{
        //    var stream = CreateUnitOfWork();
        //    var sessionId = new SessionId($"{Guid.NewGuid()}");

        //    stream.Add(4, new AccountRegistered {AccountId = AccountId, Name = "MyAccount4"});
        //    var save1 = stream.Write(sessionId, new CancellationToken());
        //    save1.Wait();

        //    stream.Add(5, new AccountRegistered {AccountId = AccountId, Name = "MyAccount4"});
        //    var save2 = Task.Run(async () =>
        //    {
        //        try
        //        {
        //            await stream.Write(sessionId, new CancellationToken());
        //        }
        //        catch (ConcurrencyException e)
        //        {
        //            Debug.WriteLine($"{e}");
        //        }
        //    });
        //    save2.Wait();
        //}

        private static UnitOfWork CreateUnitOfWork()
        {
            var bucketId = new BucketId("default");
            return new UnitOfWork(bucketId, Sessions, WriteSessions, Identities);
        }
    }
}