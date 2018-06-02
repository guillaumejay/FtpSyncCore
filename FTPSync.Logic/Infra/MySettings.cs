

// ReSharper disable InconsistentNaming

namespace FTPSync.Logic.Infra
{
    public interface IFTPSettings
    {
         string protocol { get; set; }
        string address { get; set; }
        string userName { get; set; }
        string password { get; set; }
        string directory { get; set; }
        
    } 
    public class SourceFTP:IFTPSettings
    {
        public string protocol { get; set; }
        public string address { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string directory { get; set; }
        public string encryption { get; set; }
        public string mode { get; set; }
        public bool changeFileNameBeforeTransfer { get; set; }
        public string changeFileNamePrepend { get; set; }
        public bool deleteFileAfterTransfer { get; set; }
    }

    public class DestinationFTP: IFTPSettings
    {
        public string protocol { get; set; }
        public string address { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string directory { get; set; }
        public string encryption { get; set; }
        public string mode { get; set; }
        public bool changeFileNameAfterTransfer { get; set; }
        public string changeFileNamerRemovePrepend { get; set; }
        public string actionIfFileExists { get; set; }
    }

    public class MySettings
    {
        public int serviceIntervalInMinutes { get; set; }
        public SourceFTP sourceFTP { get; set; }
        public DestinationFTP destinationFTP { get; set; }
    }
}
