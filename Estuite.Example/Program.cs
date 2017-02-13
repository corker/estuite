using System;
using System.Reflection;
using Autofac;
using Estuite.AzureEventStore;
using Estuite.Domain;
using Estuite.Example.Configuration;
using Estuite.Example.Services;
using log4net;
using log4net.Config;
using Microsoft.WindowsAzure.Storage;

namespace Estuite.Example
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Guid AccountId = Guid.NewGuid();
        private static readonly ProgramConfiguration Configuration;
        private static readonly ISerializeEvents SerializeEvents;
        private static readonly IDeserializeEvents DeserializeEvents;
        private static readonly IProvideUtcDateTime DateTime;
        private static readonly ICreateSessions Sessions;
        private static readonly CloudStorageAccount StorageAccount;
        private static readonly IReadStreams ReadStreams;
        private static readonly IWriteStreams WriteStreams;
        private static readonly BucketId BucketId = new BucketId("default");

        static Program()
        {
            XmlConfigurator.Configure();

            Configuration = new ProgramConfiguration();
            SerializeEvents = new EventSerializer();
            DeserializeEvents = new EventDeserializer();
            DateTime = new UtcDateTimeProvider();
            Sessions = new SessionFactory(DateTime, SerializeEvents);
            StorageAccount = CloudStorageAccount.Parse(Configuration.ConnectionString);
            WriteStreams = new StreamWriter(StorageAccount, Configuration);
            ReadStreams = new StreamReader(StorageAccount, Configuration, DeserializeEvents);
        }

        private static void Main(string[] args)
        {
            Log.Info("Program started.");

            try
            {
                using (var container = AutofacContainerFactory.Create())
                {
                    var runner = container.Resolve<ProgramRunner>();
                    runner.Run();
                }
                Log.Info("Program finished.");
            }
            catch (Exception e)
            {
                Log.Fatal("Exception thrown. Program will be stopped.", e);
            }

            Console.ReadKey();
        }
    }
}