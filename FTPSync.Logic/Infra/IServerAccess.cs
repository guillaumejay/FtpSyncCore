using System;
using System.Collections.Generic;
using System.Text;

namespace FTPSync.Logic.Infra
{
    public interface IServerAccess
    {
        bool Connect(IFTPSettings settings);

        List<string> GetFileList();

        void Disconnect();
    }
}
