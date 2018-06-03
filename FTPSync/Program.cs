using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentFTP;
using FTPSync.Logic;
using FTPSync.Logic.Infra;
using Microsoft.Extensions.Configuration;
using NLog;
using Renci.SshNet;

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
            logger.Info($"SourceFTP/address {settings.sourceFTP.address}");
            logger.Info($"DestinationFTP/address {settings.destinationFTP.address}");
            SyncFtps sf=new SyncFtps(logger);
            sf.Process(settings);
            //    Console.WriteLine($"Connecting to FTP/{settings.sourceFTP.address}");
            //var source = ServerAccess.CreateAccessTo(settings.sourceFTP);
            //var sourceClient = ConnectToFtp(settings.sourceFTP);
            //foreach (FtpListItem item in sourceClient.GetListing(settings.sourceFTP.directory))
            //{

            //    // if this is a file
            //    if (item.Type == FtpFileSystemObjectType.File)
            //    {
            //        Console.WriteLine(item.FullName);
            //    }

            //}
            //foreach (string file in source.GetFileList())
            //{
            //    Console.WriteLine(file);
            //}

            //IFTPSettings destSettings = settings.destinationFTP as IFTPSettings;
            //Console.WriteLine($"Connecting to SFTP/{destSettings.address}");
            //var destination = ServerAccess.CreateAccessTo(destSettings);
            //foreach (string file in destination.GetFileList())
            //{
            //    Console.WriteLine(file);
            //}
            //var connectionInfo = new ConnectionInfo(destination.address,
            //    settings.destinationFTP.userName,
            //    new PasswordAuthenticationMethod(settings.destinationFTP.userName,settings.destinationFTP.password)
            // );
            //string directory= string.IsNullOrWhiteSpace(destination.directory) ? "." : destination.directory;
            //using (var client = new SftpClient(connectionInfo))
            //{
            //    client.Connect();

            // List<string> files=  client.ListDirectory(directory).Select(x => x.FullName).ToList();
            //    foreach (string file in files)
            //    {
            //        Console.WriteLine(file);
            //    }
            //}
        }


    }
}
