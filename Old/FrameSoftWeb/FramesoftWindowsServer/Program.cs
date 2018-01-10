using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FramesoftWindowsServer
{
    class Program
    {
        static SignalGo.Server.ServiceManager.ServerProvider server = new SignalGo.Server.ServiceManager.ServerProvider();
        static void Main(string[] args)
        {
            try
            {
                System.Diagnostics.Debugger.Launch();
                string url = @"http://localhost:9191/FramesoftService";
                server.Start(url);
                server.InitializeService(typeof(FramwsoftSignalGoServices.FramesoftService));
                FrameSoft.AmarGiri.DataBase.Extensions.GetSqlExceptionLogFunc = (ex) =>
                {
                    return FrameSoft.AmarGiri.DataBase.DataBaseExtensions.GetSqlExceptionLog(ex);
                };

                server.ErrorHandlingFunction = (ex) =>
                {
                    var sqlLog = FrameSoft.AmarGiri.DataBase.Extensions.GetSqlExceptionLogFunc?.Invoke(ex);
                    if (sqlLog != null)
                        SignalGo.Shared.Log.AutoLogger.LogText("SQL Log Text: " + sqlLog);
                    SignalGo.Shared.Log.AutoLogger.LogError(ex, "ErrorHandlingFunction");
                    return new FramwsoftSignalGoServices.MessageContract() { Message = "Server Exception: " + ex.Message, IsSuccess = false };
                };
                //server.RegisterClientCallbackInterfaceService<ITransportServiceCallback>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            while (true)
                Console.ReadLine();
        }
    }
}
