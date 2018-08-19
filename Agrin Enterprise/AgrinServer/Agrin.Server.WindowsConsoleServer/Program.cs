using Agrin.Server.DataBase.Contexts;
using Agrin.Server.Models;
using Agrin.Server.Models.Filters;
using Agrin.Server.ServiceLogics;
using Agrin.Server.ServiceLogics.Authentication;
using Agrin.Server.ServiceLogics.StorageManager;
using Framesoft.Helpers.Helpers;
using Microsoft.EntityFrameworkCore;
using SignalGo.Client;
using SignalGo.Server.ServiceManager;
using SMSService.OneWayServices;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.WindowsConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SignalGo.Shared.Log.AutoLogger.IsEnabled = false;
                //var client = new System.Net.Sockets.TcpClient("94.130.144.179", 9878);
                using (var context = new AgrinContext())
                {
                    //var user = context.UserInfoes.FirstOrDefault();

                    context.Database.EnsureCreated();
                    context.Database.Migrate();
                    context.UserInfoes.FirstOrDefault();
                    Console.WriteLine("databse OK");
                }
                //StreamIdentifier.DefaultFolderPath = "E:\\AgrinDataBaseFiles\\Files";
                SMSSenderController.Current = new SMSSenderController("94.130.144.181", 9157);
                ServerProvider provider = new ServerProvider();
                //SignalGo.Server.Log.ServerMethodCallsLogger logger = new SignalGo.Server.Log.ServerMethodCallsLogger();
                //logger.IsPersianDateLog = true;
                provider.ProviderSetting.ServerServiceSetting.IsEnabledToUseTimeout = true;
                provider.ProviderSetting.ServerServiceSetting.ReceiveDataTimeout = new TimeSpan(0, 30, 0);
                provider.ProviderSetting.ServerServiceSetting.SendDataTimeout = new TimeSpan(0, 30, 0);
                //logger.Initialize();
                provider.Start("http://localhost:80/AgringServices/SignalGo", new List<System.Reflection.Assembly>() { typeof(PostService).Assembly });

                provider.ErrorHandlingFunction = (ex, type, method) =>
                {
                    return new MessageContract() { IsSuccess = false, Message = "server Exception", Error = MessageType.ServerException };
                };

                provider.ProviderSetting.IsEnabledDataExchanger = true;
                provider.ProviderSetting.IsEnabledReferenceResolver = true;
                provider.ProviderSetting.IsEnabledReferenceResolverForArray = true;
                Console.WriteLine("server started");
                //TestOneWayClient();
                //var daya = new SignalGo.Server.Helpers.ServiceReferenceHelper().GetServiceReferenceCSharpCode("ali", provider);
                //TestClient();
            }
            catch (SqlException ex)
            {
                foreach (SqlError item in ex.Errors)
                {
                    Console.WriteLine(item);
                }
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException);

                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        public static void TestClient()
        {
            Console.WriteLine("test client runed");
            ClientProvider client = new ClientProvider();
            client.Connect("http://localhost:80/AgringServices/SignalGo");

            //client.Connect("http://localhost:80/AgringServices/SignalGo");
            //var postService = client.RegisterServerService<IPostService>();
            //var virtualPosts = postService.FilterVirtualPostCategories(new FilterBaseInfo() { });

        }

        //public static void TestOneWayClient()
        //{
        //    ClientProvider client = new ClientProvider();
        //    //client.Connect("http://localhost:80/AgringServices/SignalGo");
        //    var result = SignalGo.Client.ClientProvider.SendOneWayMethod<MessageContract<int>>("localhost", 80, "StorageAuthentication", "HelloWorld", "alli", Guid.NewGuid(), new MessageContract<string>()
        //    {
        //        IsSuccess = true,
        //        Data = "hello ali",
        //        Error = MessageType.FileNotFound,
        //        Message = "my message"
        //    });
        //}
    }
}
