using System;
using System.Collections.Generic;
using System.Net;
using FluentFTP;

namespace FTPSync.Logic.Infra
{
    public class FTPAccess : IServerAccess, IDisposable
    {
        private FtpClient _client;

        public FTPAccess() { }

        public FTPAccess(IFTPSettings settings)
        {
            Connect(settings);
        }

        public void Dispose()
        {
            Disconnect();
        }

        public bool Connect(IFTPSettings settings)
        {
            _client?.Disconnect();
            _client = new FtpClient($"{settings.address}");
            if (!string.IsNullOrEmpty(settings.userName) && settings.userName != "anonymous")
            {
                _client.Credentials = new NetworkCredential(settings.userName, settings.password);
            }
            _client.Connect();
            _client.SetWorkingDirectory(settings.directory);
            return true;
        }

        public List<string> GetFileList()
        {
            List<string> files = new List<string>();
            foreach (FtpListItem item in _client.GetListing())
            {

                // if this is a file
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    files.Add(item.FullName);
                }

            }

            return files;
        }

        public void Disconnect()
        {
           _client?.Disconnect();
            _client = null;
        }
    }
}
