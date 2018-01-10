using Agrin.Log;
using Framesoft.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Framesoft.WindowsService
{
    partial class FramesoftWindowsService : ServiceBase
    {
        // public ServiceHost taskInfoServiceHost = null;
        //public ServiceHost projectInfoServiceHost = null;
        public ServiceHost CPMServiceServiceHost = null;
        public ServiceHost UserManagerServiceHost = null;

        public FramesoftWindowsService()
        {
            //InitializeComponent();
            ServiceName = "FramesoftindowsService";
        }

        void CloseService(ServiceHost serviceHost)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += new UnhandledExceptionEventHandler(currentDomain_UnhandledException);

                System.Diagnostics.Debugger.Launch();
                CloseService(CPMServiceServiceHost);
                CloseService(UserManagerServiceHost);
                //CloseService(projectInfoServiceHost);
                // Create a ServiceHost for the CalculatorService type and 
                // provide the base address.
                //CPMServiceServiceHost = new ServiceHost(typeof(CPMService));
                //CPMServiceServiceHost.Open();

                UserManagerServiceHost = new ServiceHost(typeof(UserManagerService));
                UserManagerServiceHost.Open();
                //projectInfoServiceHost = new ServiceHost(typeof(ProjectInfoService));
                //projectInfoServiceHost.Open();
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "FramesoftWindowsService OnStart", true);
            }
        }

        private void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex == null)
            {
                AutoLogger.LogError(ex, "FramesoftWindowsService UnhandledException ex = null && IsTerminating :" + e.IsTerminating, true);
            }
            else
                AutoLogger.LogError(ex, "FramesoftWindowsService UnhandledException && IsTerminating :" + e.IsTerminating, true);
        }

        protected override void OnStop()
        {
            AutoLogger.LogText("FramesoftWindowsService OnStop");
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
