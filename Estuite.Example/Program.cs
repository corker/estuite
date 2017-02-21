using System;
using System.Reflection;
using Autofac;
using Estuite.Domain;
using Estuite.Example.Configuration;
using Estuite.Example.Services;
using Estuite.StreamStore;
using Estuite.StreamStore.Azure;
using log4net;
using log4net.Config;
using Microsoft.WindowsAzure.Storage;

namespace Estuite.Example
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static Program()
        {
            XmlConfigurator.Configure();
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