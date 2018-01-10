using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Framesoft.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new FramesoftWindowsService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "FramesoftWindowsService Main", true);
            }
        }
    }
}
