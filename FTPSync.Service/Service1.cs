using System.ServiceProcess;

namespace FTPSync.Service
{
    public partial class Service1 : ServiceBase
    {
  
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
        }

        private void LoadSettings()
        {

        }
        protected override void OnStop()
        {
        
        }
    }
}
