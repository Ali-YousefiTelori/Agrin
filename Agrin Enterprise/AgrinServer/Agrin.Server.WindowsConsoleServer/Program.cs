using Agrin.Server.DataBase.Contexts;
using Agrin.Server.Models;
using Agrin.Server.ServiceLogics;
using Microsoft.EntityFrameworkCore;
using SignalGo.Client;
using SignalGo.Server.ServiceManager;
using SMSService.OneWayServices;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agrin.Server.WindowsConsoleServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //ConcurrentQueueCollection<int> BlockBuffers = new ConcurrentQueueCollection<int>();
            //int i = 0;
            //while (true)
            //{
            //    System.Threading.Tasks.Task.Run(async () =>
            //    {
            //        Console.WriteLine(System.Threading.Tasks.Task.CurrentId);
            //        int ok = await BlockBuffers.TakeAsync();
            //        BlockBuffers.Add(4);
            //        int ok1 = await BlockBuffers.TakeAsync();
            //        int ok2 = await BlockBuffers.TakeAsync();
            //        int ok3 = await BlockBuffers.TakeAsync();
            //        await System.Threading.Tasks.Task.Run(async () =>
            //         {
            //             await System.Threading.Tasks.Task.Delay(5000);
            //             BlockBuffers.Add(5);
            //         });
            //        int ok4 = await BlockBuffers.TakeAsync();
            //        int ok5 = await BlockBuffers.TakeAsync();
            //        int nok = ok;
            //    });
            //    Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
            //    //i++;
            //    //if (i % 1000 == 0)
            //    BlockBuffers.Add(1);
            //    BlockBuffers.Add(2);
            //    BlockBuffers.Add(3);
            //}

            ServerProvider provider = new ServerProvider();
            try
            {

                //int minWorker, minIOC;
                // Get the current settings.
                //System.Threading.ThreadPool.GetMaxThreads(out minWorker, out minIOC);
                //System.Threading.ThreadPool.SetMaxThreads(out minWorker, out minIOC);

                SignalGo.Shared.Log.AutoLogger.IsEnabled = false;
                using (AgrinContext context = new AgrinContext())
                {
                    //var user = context.UserInfoes.FirstOrDefault();

                    context.Database.EnsureCreated();
                    context.Database.Migrate();
                    context.UserInfoes.FirstOrDefault();
                    Console.WriteLine("databse OK");
                }
                //StreamIdentifier.DefaultFolderPath = "E:\\AgrinDataBaseFiles\\Files";
                SMSSenderController.Current = new SMSSenderController("main.atitec.ir", 9157);
                //SignalGo.Server.Log.ServerMethodCallsLogger logger = new SignalGo.Server.Log.ServerMethodCallsLogger();
                //logger.IsPersianDateLog = true;
                provider.ProviderSetting.ServerServiceSetting.IsEnabledToUseTimeout = true;
                provider.ProviderSetting.ServerServiceSetting.ReceiveDataTimeout = new TimeSpan(0, 30, 0);
                provider.ProviderSetting.ServerServiceSetting.SendDataTimeout = new TimeSpan(0, 30, 0);

                provider.ProviderSetting.HttpSetting.IsEnabledToUseTimeout = true;
                provider.ProviderSetting.HttpSetting.ReceiveDataTimeout = new TimeSpan(0, 0, 5);
                provider.ProviderSetting.HttpSetting.SendDataTimeout = new TimeSpan(0, 0, 5);

                provider.ProviderSetting.IsEnabledToUseTimeout = true;
                provider.ProviderSetting.ReceiveDataTimeout = new TimeSpan(0, 0, 5);
                provider.ProviderSetting.SendDataTimeout = new TimeSpan(0, 0, 5);
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
                //TestTCPHTTpClient();
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
            while (true)
            {
                try
                {
                    Console.WriteLine("write command");
                    string cmd = Console.ReadLine();
                    if (cmd == "c")
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        Console.WriteLine("GC.Collect");
                    }
                    else if (cmd == "i")
                    {
                        Console.WriteLine($"{provider.ServerDataProvider.GetInformation()}");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("unhandle exception " + e.IsTerminating);
            Console.WriteLine((Exception)e.ExceptionObject);

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

        public static void TestTCPHTTpClient()
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1000);

                        //Console.WriteLine("test http client runed");
                        TcpClient tcpClient = new TcpClient();
                        tcpClient.Connect("localhost", 80);
                        string data = "HTTP/1.1 localhost:80\r\n";
                        byte[] bytes = Encoding.ASCII.GetBytes(data);
                        tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                        int what = tcpClient.GetStream().ReadByte();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
            thread.Start();
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
