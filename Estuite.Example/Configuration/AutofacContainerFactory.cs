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
            builder.RegisterType<SessionFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<UnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<UtcDateTimeProvider>().AsImplementedInterfaces().SingleInstance();

            // StreamStore.Azure
            builder.RegisterType<StreamWriter>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<StreamReader>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<StreamStoreCloudTableProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();

            // StreamDispatcher.Azure
            builder.RegisterType<AzureStreamDispatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CurrentPageIndexRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DispatchStreamRecoveryJobRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<EventToDispatchRecordQueue>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<EventToDispatchRecordRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<EventDispatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<EventStoreCloudTableProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();

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