using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace FTPSync.Logic.Infra
{
    public static class ServerAccessFactory
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
    }
}
