using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using FTPSync.Logic.Infra;
using NLog;
using NLog.LayoutRenderers;

namespace FTPSync.Logic
{
    public class SyncFtps
    {
        private NLog.Logger _logger;
        private string _pathTemp = "tmp";
        private string _currentAction;
        public SyncFtps(NLog.Logger logger)
        {
            _logger = logger;
        }
        public int Process(SyncSettings settings)
        {
            int nbUploaded = 0;
            _currentAction = "Starting...";
            IServerAccess source = null;
            IServerAccess destinationClient = null;
            string currentFile = null;
            string originalFile = null;
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
                SetCurrentAction("Connecting to Source " + settings.sourceFTP.address);
 
             source  = ServerAccess.CreateAccessTo(settings.sourceFTP);
                List<string> sourceList = source.GetFileList(settings.changeFileNamePrepend);
                _logger.Info($"{sourceList.Count} files found to transfer");
                if (sourceList.Count == 0)
                {
                    return 0;
                }

                if (Directory.Exists(_pathTemp))
                    Directory.CreateDirectory(_pathTemp);

                string prepend = settings.changeFileNamePrepend;
                foreach (string sourceFile in sourceList)
                {
                    originalFile = sourceFile;
                    currentFile = sourceFile;
                    if (!string.IsNullOrWhiteSpace(prepend))
                    {
                        currentFile = prepend + currentFile;
                        SetCurrentAction($"Renaming {originalFile} => {currentFile}");
                        source.RenameFile(originalFile, currentFile);
                    }

                    string localFile = Path.Combine(_pathTemp, currentFile);
                    SetCurrentAction("Downloading " + currentFile);
                    source.DownloadFile(currentFile, localFile);
                    source.Disconnect();
                    source = null;
                    SetCurrentAction("Uploading " + localFile);
                   destinationClient = ServerAccess.CreateAccessTo(settings.destinationFTP);
                    if (destinationClient.UploadFile(localFile, currentFile, settings.destinationFTP,
                        settings.changeFileNamePrepend))
                    {
                        nbUploaded++;
                    }
                    SetCurrentAction("Connecting to Source " + settings.sourceFTP.address);
                    // Reconnect to source do delete and do next download
                    source = ServerAccess.CreateAccessTo(settings.sourceFTP);
                    if (settings.sourceFTP.deleteFileAfterTransfer)
                    {
                       SetCurrentAction($"Delete {currentFile} on source {settings.sourceFTP.address} ");
                        source.DeleteFile(currentFile);
                    }
                    else
                    {
                        if (currentFile != originalFile)
                        {
                            SetCurrentAction($"Renaming {currentFile} => {originalFile}");
                            source.RenameFile(currentFile, originalFile);
                        }
                    }

                    currentFile = null;
                }

                _logger.Info(
                    $"Done ! {sourceList.Count} file{(sourceList.Count > 1 ? "s" : "")} on source, {nbUploaded} file{(nbUploaded > 1 ? "s" : "")} uploaded");
            }
            catch (Exception e)
            {
                LogExceptionCleanly(e);
        
                if (currentFile != null && originalFile != null && currentFile != originalFile)
                { //Need to rename the original file
                    try
                    {
                        if (source == null)
                        {
                            SetCurrentAction("AFTER EXCEPTION Connecting to Source " + settings.sourceFTP.address);
                            source = ServerAccess.CreateAccessTo(settings.sourceFTP);
                        }

                        SetCurrentAction($"AFTER EXCEPTION Renaming {currentFile} => {originalFile}");
                        source.RenameFile(currentFile, originalFile);
                    }
                    catch (Exception exception)
                    {
                        LogExceptionCleanly(exception);
                    }
                    finally
                    {
                        source?.Disconnect();
                    }
           
                }
             
                return nbUploaded;
            }
            finally
            {
                EmptyTmpFolder();
                source?.Disconnect();
                destinationClient?.Disconnect();
                
            }
            return nbUploaded;
        }

        private void SetCurrentAction(string s)
        {
            _currentAction = s;
            _logger.Info(s);
        }

        private void LogExceptionCleanly(Exception exception)
        {
            if (exception.InnerException != null)
            {
                LogExceptionCleanly(exception.InnerException);
                return;
            }

            string onAction = $"On {_currentAction} - ";
            if (exception is SocketException ce)
            {
                _logger.Error($"{onAction}{ce.Message}");
                return;
            }

            _logger.Fatal( exception, "On { _currentAction}");
        }

        private void EmptyTmpFolder()
        {
            if (Directory.Exists(_pathTemp))
            {
                foreach (var s in Directory.GetFiles(_pathTemp))
                {
                    try
                    {
                        File.Delete(s);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e,"Can't delete " + s);
                        
                    }
                }
            }
        }
    }
}
