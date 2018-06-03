using System.Collections.Generic;
using System.IO;
using System.Linq;
using Renci.SshNet;

namespace FTPSync.Logic.Infra
{
    public class SFTPAccess : ServerAccess, IServerAccess
    {
        private SftpClient _client;
        private string _directory;

        public SFTPAccess() { }

        public SFTPAccess(IFTPSettings settings)
        {
            Connect(settings);
        }
        public void Dispose()
        {

            Disconnect();
        }

        public bool Connect(IFTPSettings settings)
        {
            var connectionInfo = new ConnectionInfo(settings.address,
                settings.userName,
                new PasswordAuthenticationMethod(settings.userName, settings.password)
            );
            _client = new SftpClient(connectionInfo);
            _client.Connect();
            _directory = string.IsNullOrWhiteSpace(settings.directory) ? "." : settings.directory;
            return true;
        }

        public void Disconnect()
        {
            _client?.Disconnect();
            _client = null;
        }

        public void DownloadFile(string nameOnFTP, string localName)
        {
            using (Stream fileStream = File.Create(localName))
            {
                _client.DownloadFile(nameOnFTP, fileStream);
            }
        }


        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="settings">see FTPDestination check for ifExists and removePrepend</param>
        ///    /// <param name="prepend">File Prefix or null</param>
        /// <returns>True if uploaded, false if not uploaded because already existing</returns>
        public bool UploadFile(string from, string to, DestinationFTP settings, string prepend)
        {
            if (!File.Exists(from))
            {
                throw new FileNotFoundException("Local file not found", from);
            }
            if (settings.actionIfFileExists == DestinationFTP.IfExistsDontTransfer)
            {
                if (_client.Exists(to))
                    return false;
            }
            using (Stream fileStream = File.OpenRead(from))
            {

                _client.UploadFile(fileStream, to, true, null);
            }
            RenameIfPrepend(to, prepend);
            return true;
        }

        public void DeleteFile(string file)
        {
            _client.DeleteFile(file);
        }

        public override void RenameFile(string from, string to)
        {
            _client.RenameFile(from, to);
        }

        public List<string> GetFileList(string prefix)
        {
            List<string> files = new List<string>();
            var result = _client.ListDirectory(_directory).ToList();
            foreach (var entry in result)
            {
                if (entry.IsRegularFile)
                    files.Add(entry.Name);
            }

            CleanFilesList(files, prefix);
            return files;
        }
    }
}
