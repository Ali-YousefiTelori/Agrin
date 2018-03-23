using Agrin.Server.Models;
using Agrin.Server.Models.Filters;
using Agrin.Server.ServiceLogics;
using Agrin.Server.ServiceLogics.Authentication;
using Agrin.Server.ServiceLogics.StorageManager;
using Agrin.Shared.Helpers;
using Framesoft.Helpers.Helpers;
using SignalGo.Client;
using SignalGo.Server.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
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
                FileManager.DataBaseFolder = "D:\\AgrinDataBaseFiles";
                ServerProvider provider = new ServerProvider();
                SignalGo.Server.Log.ServerMethodCallsLogger logger = new SignalGo.Server.Log.ServerMethodCallsLogger();
                logger.IsPersianDateLog = true;
                logger.Initialize();
                provider.RegisterServerService<PostService>();
                provider.RegisterServerService<AuthenticationService>();
                provider.RegisterStreamService(typeof(PostStorageManager));
                provider.Start("http://localhost:2222/AgringServices/SignalGo");

                provider.ErrorHandlingFunction = (ex) =>
                {
                    return new MessageContract() { IsSuccess = false, Message = "server Exception", Error = ErrorMessage.ServerException };
                };

                provider.InternalSetting = new SignalGo.Server.Settings.InternalSetting() { IsEnabledDataExchanger = true, IsEnabledReferenceResolver = true, IsEnabledReferenceResolverForArray = true };
                Console.WriteLine("server started");
                var daya = new SignalGo.Server.Helpers.ServiceReferenceHelper().GetServiceReferenceCSharpCode("ali", provider);
                //TestClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        //public static void TestClient()
        //{
        //    Console.WriteLine("test client runed");
        //    ClientProvider client = new ClientProvider();
        //    client.Connect("http://localhost:2222/AgringServices/SignalGo");
        //    var postService = client.RegisterServerService<IPostService>();
        //    var virtualPosts = postService.FilterVirtualPostCategories(new FilterBaseInfo() { });

        //}
    }
}
