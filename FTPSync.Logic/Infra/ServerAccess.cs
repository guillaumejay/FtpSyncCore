using System;
using System.Collections.Generic;

namespace FTPSync.Logic.Infra
{
    public abstract class ServerAccess
    {
        public static IServerAccess CreateAccessTo(IFTPSettings settings)
        {
            switch (settings.protocol.ToLower())
            {
                case "ftp":
                   return new FTPAccess(settings);
                case "sftp":
                    return new SFTPAccess(settings);
                default:
                    throw new ArgumentException($"Unknown protocol {settings.protocol}");
            }

        }
        protected void RenameIfPrepend(string to,string prepend)
        {
            if (!string.IsNullOrEmpty(prepend))
            {
                if (to.StartsWith(prepend))
                {
                    string finalName = to.Remove(0, prepend.Length);
                    RenameFile(to, finalName);
                }
            }
        }

        public abstract void RenameFile(string from, string to);
        
        public void CleanFilesList(List<string> files, string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
                files.RemoveAll(x => x.StartsWith(prefix));
        }
    }
}
