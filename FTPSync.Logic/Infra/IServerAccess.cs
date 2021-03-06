﻿using System.Collections.Generic;

namespace FTPSync.Logic.Infra
{
    public interface IServerAccess
    {
        bool Connect(IFTPSettings settings);

        List<string> GetFileList(string prefix);

        void Disconnect();

        void DownloadFile(string nameOnFTP, string localName);

        void RenameFile(string from, string to);

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="settings">see FTPDestination check for ifExists and removePrepend</param>
        /// <param name="prepend">File Prefix or null</param>
        /// <returns>True if uploaded, false if not uploaded because already existing</returns>
        bool UploadFile(string from, string to,DestinationFTP settings,string prepend);

        void DeleteFile(string file);
    }
}
