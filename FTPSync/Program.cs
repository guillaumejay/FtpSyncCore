using System.IO;
using System.Threading;
using FTPSync.Logic;
using FTPSync.Logic.Infra;
using Microsoft.Extensions.Configuration;

namespace FTPSync
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");
            var configuration = builder.Build();
            var settings = new SyncSettings();
            configuration.Bind(settings);

            NLog.Logger logger = NLog.LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();
            logger.Info($"serviceIntervalInMinutes {settings.serviceIntervalInMinutes}");
            logger.Info($"changeFileNamePrepend {settings.changeFileNamePrepend}");
            logger.Info($"SourceFTP/address {settings.sourceFTP.address}");
            logger.Info($"DestinationFTP/address {settings.destinationFTP.address}");
            SyncFtps sf = new SyncFtps(logger);
            while (true)
            {
                sf.Process(settings);
                if (settings.serviceIntervalInMinutes == 0)
                    break;
                Thread.Sleep(1000 * settings.serviceIntervalInMinutes);
            }
        }
    }
}