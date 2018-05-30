using Agrin.Server.Models;
using Agrin.StorageServer.ServiceLogics.StorageManager;
using Agrin.TelegramBot;
using Agrin.TelegramBot.UserBot;
using AgrinMainServer.OneWayServices;
using SignalGo.Server.ServiceManager;
using SignalGo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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


                AsyncActions.Run(() =>
                {
                    AgrinUserBotEngine.Run();
                });

                StorageAuthenticationService.Current = new StorageAuthenticationService("agrin.info", 80);
                FileManager.Current = new FileManager("agrin.info", 80);

                UltraStreamGo.StreamIdentifier.DefaultFolderPath = "F:\\AgrinUserFiles";

                BotsManager.StartBot<AgrinBotEngine>("519772219:AAH-l3Uxbz0QByJ4uapsK1PlwaU2OLG_WO4");

                ServerProvider provider = new ServerProvider();
                SignalGo.Server.Log.ServerMethodCallsLogger logger = new SignalGo.Server.Log.ServerMethodCallsLogger();
                logger.IsPersianDateLog = true;
                logger.Initialize();
                provider.Start("http://localhost:1397/AgringServices/SignalGo");
                provider.RegisterServerService<LinkUploadManager>();
                provider.RegisterServerService<LinkDownloadManager>();
                provider.ErrorHandlingFunction = (ex) =>
                {
                    return new MessageContract() { IsSuccess = false, Message = "server Exception", Error = MessageType.ServerException };
                };

                provider.InternalSetting = new SignalGo.Server.Settings.InternalSetting() { IsEnabledDataExchanger = true, IsEnabledReferenceResolver = true, IsEnabledReferenceResolverForArray = true };
                Console.WriteLine("server started");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            while (true)
            {
                Console.ReadLine();
                try
                {
                    Console.WriteLine(LinkUploadManager.UserFileUploading.Count);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
