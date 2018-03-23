using Agrin.Server.Models;
using Agrin.Server.ServiceLogics.StorageManager;
using Agrin.Shared.Helpers;
using SignalGo.Server.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.DataStorageServer.WindowsConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FileManager.DataBaseFolder = "D:\\AgrinDataBaseFiles";
                ServerProvider provider = new ServerProvider();
                SignalGo.Server.Log.ServerMethodCallsLogger logger = new SignalGo.Server.Log.ServerMethodCallsLogger();
                logger.IsPersianDateLog = true;
                logger.Initialize();
                //provider.InitializeService<PostStorageManager>();
                provider.RegisterStreamService(typeof(PostStorageManager));
                provider.Start("http://localhost:4444/AgringServices/SignalGo");

                provider.ErrorHandlingFunction = (ex) =>
                {
                    return new MessageContract() { IsSuccess = false, Message = "server Exception", Error = ErrorMessage.ServerException };
                };

                provider.InternalSetting = new SignalGo.Server.Settings.InternalSetting() { IsEnabledDataExchanger = true, IsEnabledReferenceResolver = true, IsEnabledReferenceResolverForArray = true };
                Console.WriteLine("server started");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }
}
