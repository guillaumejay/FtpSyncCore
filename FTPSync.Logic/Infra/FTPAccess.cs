using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using FluentFTP;

namespace FTPSync.Logic.Infra
{
    public class FTPAccess : ServerAccess,IServerAccess, IDisposable
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
            var addressPort = settings.address.Split(':');
            int port = 21;
            NetworkCredential nwc = null;
            if (addressPort.Length > 1)
            {
                port = Convert.ToInt32(addressPort[1]);
            }
            
            if (!string.IsNullOrEmpty(settings.userName) && settings.userName != "anonymous")
            {
                nwc = new NetworkCredential(settings.userName, settings.password);
            }

            _client = new FtpClient($"{addressPort[0]}", port, nwc)
            {
                DataConnectionType = (settings.mode.ToLower() == "active")
                    ? FtpDataConnectionType.AutoActive
                    : FtpDataConnectionType.AutoPassive,
               
            };

            _client.Connect();
            _client.SetWorkingDirectory(settings.directory);
            return true;
        }

        public List<string> GetFileList(string prefix)
        {
            List<string> files = new List<string>();
            foreach (FtpListItem item in _client.GetListing())
            {

                // if this is a file
                if (item.Type == FtpFileSystemObjectType.File)
                {

                    files.Add(item.Name);
                }

            }

            CleanFilesList(files, prefix);
                return  files;
        }

        public void Disconnect()
        {
           _client?.Disconnect();
            _client = null;
        }

        public void DownloadFile(string nameOnFTP, string localName)
        {
            _client.DownloadFile(localName, nameOnFTP, true, FtpVerify.None, null);
        }

        public override void RenameFile(string @from, string to)
        {
            _client.MoveFile(from, to, FtpExists.Overwrite);
        }

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="settings">see FTPDestination check for ifExists and removePrepend</param>
        /// <param name="prepend">Prefix for file (or null)</param>
        /// <returns>True if uploaded, false if not uploaded because already existing</returns>
        public bool UploadFile(string from, string to, DestinationFTP settings,string prepend)
        {
            if (!File.Exists(from))
            {
                throw new FileNotFoundException("Local file not found",from);
            }
            if (settings.actionIfFileExists == DestinationFTP.IfExistsDontTransfer)
            {
                if (_client.FileExists(to))
                    return false;
            }
            _client.UploadFile(from, to,FtpExists.Overwrite);
            RenameIfPrepend(to, prepend);
            return true;
        }


        public void DeleteFile(string file)
        {
           _client.DeleteFile(file);
        }
    }
}
