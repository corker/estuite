using Autofac;
using Estuite.Example.Services;
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

            // Event store
            builder.RegisterInstance(new BucketId("default"));
            builder.RegisterType<UtcDateTimeProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SessionFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StreamWriter>().AsImplementedInterfaces();
            builder.RegisterType<StreamReader>().AsImplementedInterfaces();
            builder.RegisterType<UnitOfWork>();
            builder.Register(CreateCloudStorageAccount).InstancePerLifetimeScope();

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