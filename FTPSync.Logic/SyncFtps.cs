using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FTPSync.Logic.Infra;

namespace FTPSync.Logic
{
    public class SyncFtps
    {
        private NLog.Logger _logger;

        public SyncFtps(NLog.Logger logger)
        {
            _logger = logger;
        }
        public int Process(SyncSettings settings)
        {
            int nbUploaded = 0;

            try
            {
                var result = settings.Validate();
                if (result.Any())
                {
                    foreach (var error in result)
                    {
                        _logger.Error(error.ErrorMessage);
                    }
                    return 0;
                }
                _logger.Info("Connecting to Source " + settings.sourceFTP.address);
                var source = ServerAccess.CreateAccessTo(settings.sourceFTP);
                List<string> sourceList = source.GetFileList(settings.changeFileNamePrepend);
                _logger.Info($"{sourceList.Count} files found to transfer");
                if (sourceList.Count == 0)
                {
                    return 0;
                }
              
                if (Directory.Exists("tmp"))
                    Directory.CreateDirectory("tmp");

                string prepend = settings.changeFileNamePrepend;
                foreach (string sourceFile in sourceList)
                {
                    string currentFile = sourceFile;
                    if (!string.IsNullOrWhiteSpace(prepend))
                    {
                        currentFile = prepend + currentFile;
                        source.RenameFile(sourceFile, currentFile);
                    }

                    string localFile = Path.Combine("tmp", currentFile);
                    _logger.Info("Downloading " + currentFile);
                    source.DownloadFile(currentFile, localFile);
                    source.Disconnect();
                    _logger.Info("Uploading " + localFile);
                    var destinationClient = ServerAccess.CreateAccessTo(settings.destinationFTP);
                    if (destinationClient.UploadFile(localFile, currentFile, settings.destinationFTP,settings.changeFileNamePrepend))
                    {
                        nbUploaded++;
                    }

                    // Reconnect to source do delete and do next download
                    source = ServerAccess.CreateAccessTo(settings.sourceFTP);
                    if (settings.sourceFTP.deleteFileAfterTransfer)
                    {
                        _logger.Info($"Delete {currentFile} on source {settings.sourceFTP.address} ");
                        source.DeleteFile(currentFile);
                    }
                    else
                    {
                        source.RenameFile(currentFile,sourceFile);
                    }
                }
                _logger.Info($"Done ! {sourceList.Count} file{(sourceList.Count > 1 ? "s" : "")} on source, {nbUploaded} file{(nbUploaded > 1 ? "s" : "")} uploaded");
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            return nbUploaded;
        }
    }
}
