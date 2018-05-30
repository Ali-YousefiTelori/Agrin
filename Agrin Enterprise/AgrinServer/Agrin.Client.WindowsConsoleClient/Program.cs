using Agrin.Server.DataBaseLogic;
using Agrin.TelegramBot;
using SignalGo.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Client.WindowsConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //WebProxy webProxy = new WebProxy(new Uri("https://dev.atitec.ir:808"));
                //webProxy.BypassProxyOnLocal = false;
                //webProxy.Credentials = new NetworkCredential("atitec", "atitec123", "dev.atitec.ir");
                //webProxy.UseDefaultCredentials = false;
                
                //WebRequest webRequest = WebRequest.Create("https://core.telegram.org/bots/api");
                //webRequest.Proxy = webProxy;
                //webRequest.Proxy.Credentials = new NetworkCredential("atitec", "atitec123", "dev.atitec.ir");

                //var response = webRequest.GetResponse();

                BotsManager.StartBot<AgrinBotEngine>("519772219:AAHA_Yn4K2wyi-Io_BD9Z5D80y2zkNKPA6A");
                //var items = PostExtension.FilterVirtualPostCategories(new Server.Models.Filters.FilterBaseInfo());
                //using (AgrinContext context = new AgrinContext())
                //{
                //    var users = context.UserInfoes.ToList();
                //}
                //var CurrentProvider = new ClientProvider();
                //CurrentProvider.Connect($"http://82.102.13.102:2222/AgringServices/SignalGo");
                //var AuthenticationService = CurrentProvider.RegisterClientServiceInterfaceWrapper<IAuthentication>();
                //var PostService = CurrentProvider.RegisterClientServiceInterfaceWrapper<IPostService>();
                //var posts = PostService.GetListOfPost(0, 10);
                //var ct = PostService.FilterPostCategories(new Server.Models.Filters.FilterBaseInfo() { });
                //var v = PostService.FilterVirtualPostCategories(new Server.Models.Filters.FilterBaseInfo() { });
                //DownloadFiles(CurrentProvider, v.Data[0].Posts.FirstOrDefault());
                //var postStorageManager = CurrentProvider.RegisterStreamServiceInterfaceWrapper<IPostStorageManager>();
                //var file = postStorageManager.DownloadPostImage(1, 1, "ali");
                Console.WriteLine("bot runned!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        //static void DownloadFiles(ClientProvider CurrentProvider, PostInfo post)
        //{
        //    foreach (var file in post.FileInfoes)
        //    {
        //        string[] ip = file.ServerAddress.Split(':');
        //        var postStorageManager = CurrentProvider.RegisterStreamServiceInterfaceWrapper<IPostStorageManager>(ip[0], int.Parse(ip[1]));
        //        var streamInfo = postStorageManager.DownloadPostImage(post.UserId, post.Id, file.FileName);
        //        using (var stream = new FileStream(Path.Combine("D:\\", file.FileName), FileMode.OpenOrCreate, FileAccess.ReadWrite))
        //        {
        //            while (stream.Length != streamInfo.Length)
        //            {
        //                var bytes = new byte[1024 * 10];
        //                var readCount = streamInfo.Stream.Read(bytes, 0, bytes.Length);
        //                stream.Write(bytes, 0, readCount);
        //            }
        //        }
        //    }
        //}
    }
}
