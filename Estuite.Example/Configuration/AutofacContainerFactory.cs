using Autofac;
using Estuite.Example.Services;
using Estuite.StreamDispatcher.Azure;
using Estuite.StreamStore;
using Estuite.StreamStore.Azure;
using Microsoft.WindowsAzure.Storage;

namespace Estuite.Example.Configuration
{
    public static class AutofacContainerFactory
    {
        public static IContainer Create()
        {
            var builder = new ContainerBuilder();

            // Program
            builder.RegisterType<ProgramConfiguration>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProgramRunner>();

            // AzureStorageAccount
            builder.Register(CreateCloudStorageAccount).InstancePerLifetimeScope();

            // StreamStore
            builder.RegisterInstance(new BucketId("default"));
            builder.RegisterType<UtcDateTimeProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SessionFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StreamWriter>().AsImplementedInterfaces();
            builder.RegisterType<StreamReader>().AsImplementedInterfaces();
            builder.RegisterType<UnitOfWork>();

            // StreamDispatcher
            builder.RegisterType<AzureEventDispatcher>().AsImplementedInterfaces();
            builder.RegisterType<AzureStreamDispatcher>().AsImplementedInterfaces();
            builder.RegisterType<DispatchEventRecordQueue>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DispatchEventRecordRepository>().AsImplementedInterfaces();
            builder.RegisterType<DispatchStreamRecoveryJobRepository>().AsImplementedInterfaces();
            builder.RegisterType<CurrentPageIndexRepository>().AsImplementedInterfaces();

            // Services
            builder.RegisterType<EventSerializer>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EventDeserializer>().AsImplementedInterfaces().SingleInstance();

            // Examples
            builder.RegisterAssemblyTypes(typeof(IRunExamples).Assembly)
                .InNamespaceOf<IRunExamples>()
                .AsImplementedInterfaces();

            return builder.Build();
        }

        private static CloudStorageAccount CreateCloudStorageAccount(IComponentContext context)
        {
            var configuration = context.Resolve<ICloudStorageAccountConfiguration>();
            return CloudStorageAccount.Parse(configuration.ConnectionString);
        }
    }
}