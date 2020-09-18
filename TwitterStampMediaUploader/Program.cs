using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TwitterStampMediaUploader
{
    class Program
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            logger.Info("start");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var appSettings = configuration.Get<AppSettings>();

            var uploader = new Uploader(appSettings);

            await uploader.Run();

            logger.Info("end");

            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error(e.ExceptionObject.ToString());
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
