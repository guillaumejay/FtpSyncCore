using System.ServiceProcess;
using System.Timers;
using FTPSync.Logic;
using FTPSync.Logic.Infra;

namespace FTPSync.Service
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private NLog.Logger _logger;
  
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var settings = SyncSettings.Load();
            NLog.Logger logger = NLog.LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();
            logger.Info($"serviceIntervalInMinutes {settings.serviceIntervalInMinutes}");
            logger.Info($"changeFileNamePrepend {settings.changeFileNamePrepend}");
            logger.Info($"SourceFTP/address {settings.sourceFTP.address}");
            logger.Info($"DestinationFTP/address {settings.destinationFTP.address}");
            timer =new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Interval = settings.serviceIntervalInMinutes * 60 * 1000;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SyncFtps sync=new SyncFtps(_logger);
            var settings = SyncSettings.Load();
            sync.Process(settings);
        }

        private void LoadSettings()
        {

        }
        protected override void OnStop()
        {
        
        }
    }
}
