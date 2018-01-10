using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.DataBaseLogic;
using Agrin.Server.ServiceModels.Authentication;
using Agrin.Server.ServiceModels.StorageManager;
using Agrin.Server.ServiceModels.UserManager;
using SignalGo.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                //var items = PostExtension.FilterVirtualPostCategories(new Server.Models.Filters.FilterBaseInfo());
                //using (AgrinContext context = new AgrinContext())
                //{
                //    var users = context.UserInfoes.ToList();
                //}
                var CurrentProvider = new ClientProvider();
                CurrentProvider.Connect($"http://82.102.13.102:2222/AgringServices/SignalGo");
                var AuthenticationService = CurrentProvider.RegisterClientServiceInterfaceWrapper<IAuthentication>();
                var PostService = CurrentProvider.RegisterClientServiceInterfaceWrapper<IPostService>();
                var posts = PostService.GetListOfPost(0, 10);
                var ct = PostService.FilterPostCategories(new Server.Models.Filters.FilterBaseInfo() { });
                var v = PostService.FilterVirtualPostCategories(new Server.Models.Filters.FilterBaseInfo() { });
                DownloadFiles(CurrentProvider, v.Data[0].Posts.FirstOrDefault());
                //var postStorageManager = CurrentProvider.RegisterStreamServiceInterfaceWrapper<IPostStorageManager>();
                //var file = postStorageManager.DownloadPostImage(1, 1, "ali");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        static void DownloadFiles(ClientProvider CurrentProvider, PostInfo post)
        {
            foreach (var file in post.FileInfoes)
            {
                string[] ip = file.ServerAddress.Split(':');
                var postStorageManager = CurrentProvider.RegisterStreamServiceInterfaceWrapper<IPostStorageManager>(ip[0], int.Parse(ip[1]));
                var streamInfo = postStorageManager.DownloadPostImage(post.UserId, post.Id, file.FileName);
                using (var stream = new FileStream(Path.Combine("D:\\", file.FileName), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    while (stream.Length != streamInfo.Length)
                    {
                        var bytes = new byte[1024 * 10];
                        var readCount = streamInfo.Stream.Read(bytes, 0, bytes.Length);
                        stream.Write(bytes, 0, readCount);
                    }
                }
            }
        }
    }
}
