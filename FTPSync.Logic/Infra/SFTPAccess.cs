using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.SshNet;

namespace FTPSync.Logic.Infra
{
   public class SFTPAccess:IServerAccess
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

        public List<string> GetFileList()
        {
            List<string> files=new List<string>();
            var result=_client.ListDirectory(_directory).ToList();
            foreach (var entry in result)
            {
                if (entry.IsRegularFile)
                    files.Add(entry.FullName);
            }
            return files;
        }
    }
}
