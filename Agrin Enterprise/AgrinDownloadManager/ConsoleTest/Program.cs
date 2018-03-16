//using OctoTorrent.Client;
//using OctoTorrent.Client.Encryption;
//using OctoTorrent.Common;
using Agrin.Client.DataBase;
using Agrin.Download.CoreModels.Link;
using Agrin.Download.EntireModels.Link;
using Agrin.Download.ShortModels.Link;
using Agrin.Download.Web;
using Agrin.IO;
using Agrin.IO.Helpers;
using Agrin.Log;
using Agrin.UI.ViewModels.Helpers;
using Agrin.Web;
using CrazyMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTest
{

    public class MinProperties
    {
        public string Name { get; set; }
    }

    public class MaxProperties : MinProperties
    {
        public string Age { get; set; }
    }

    public class AndroidClass : CoreClass
    {
        MinProperties _Properties;
        public virtual MinProperties Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new MinProperties();
                return _Properties;
            }
            set
            {
                _Properties = value;
            }
        }
    }

    public class DesktopClass : AndroidClass
    {
        MaxProperties _Properties;
        public override MinProperties Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new MaxProperties();
                return _Properties;
            }
            set
            {
                _Properties = (MaxProperties)value;
            }
        }
    }

    public class CoreClass
    {
        public string CoreName { get; set; }
    }

    class Program
    {
        static void InitializeMapper()
        {
            //{ "Unable to cast object of type 'Agrin.Download.ShortModels.Link.LinkInfoPropertiesShort' to type 'Agrin.Download.EntireModels.Link.LinkInfoProperties'."}
            Mapper.Bind<LinkInfoRequestCore, LinkInfoRequestShort>();
            Mapper.AfterSetParameters<LinkInfoRequestShort>((target, parent) =>
            {
                try
                {
                    if (IOHelperBase.FileExists(target.SaveConnectionFileName))
                    {
                        using (var stream = IOHelperBase.OpenFileStreamForRead(target.SaveConnectionFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                            target.DownloadedSize = stream.Length;
                        if (target.DownloadedSize == target.Length)
                        {
                            target.Status = Agrin.Models.ConnectionStatus.Complete;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            });
            Mapper.AfterInstance<LinkInfoRequestShort>((target, parent) =>
            {
                target.LinkInfo = (LinkInfoShort)parent;
            });
            Mapper.AfterInstance<LinkInfoPathShort>((target, parent) =>
            {
                target.LinkInfo = (LinkInfoShort)parent;
            });
            Mapper.AfterInstance<LinkInfoPropertiesShort>((target, parent) =>
            {
                target.LinkInfo = (LinkInfoShort)parent;
            });
            Mapper.AfterInstance<LinkInfoManagementShort>((target, parent) =>
            {
                target.LinkInfo = (LinkInfoShort)parent;
            });
            Mapper.AfterInstance<LinkInfoDownloadShort>((target, parent) =>
            {
                target.LinkInfo = (LinkInfoShort)parent;
            });
        }

        static void LoadData()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            PathHelper.Initialize(path, path, path);
            AgrinClientContext.DataBasePath = path;
            AutoLogger.ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
            AgrinClientContext.LinkInfoTable.Initialize<LinkInfoShort>();
            AgrinClientContext.TaskSchedulerTable.Initialize();
        }

        //readonly static Uri SomeBaseUri = new Uri("http://canbeanything");
        //static string GetFileNameFromUrl(string url)
        //{
        //    Uri uri;
        //    if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
        //        uri = new Uri(SomeBaseUri, url);

        //    var fileName = Path.GetFileName(uri.LocalPath);
        //    if (string.IsNullOrEmpty(fileName))
        //    {
        //        Path.GetInvalidFileNameChars();
        //    }
        //}

        public static string GetFileNameValidChar(string fileName)
        {
            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
#if(MobileApp)
            foreach (var item in androidinvalidChars)
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
#endif
            return fileName;
        }

        public static string GetFileNameFromUrl(string url)
        {
            string fileName = "";
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                fileName = GetFileNameValidChar(Path.GetFileName(uri.AbsolutePath));
            }
            string ext = "";
            if (!string.IsNullOrEmpty(fileName))
            {
                ext = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(ext))
                    ext = ".html";
                else
                    ext = "";
                return GetFileNameValidChar(fileName + ext);

            }

            fileName = Path.GetFileName(url);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "noName";
            }
            ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext))
                ext = ".html";
            else
                ext = "";
            fileName = fileName + ext;
            if (!fileName.StartsWith("?"))
                fileName = fileName.Split('?').FirstOrDefault();
            fileName = fileName.Split('&').LastOrDefault().Split('=').LastOrDefault();
            return GetFileNameValidChar(fileName);
        }

        static void Main(string[] args)
        {
            try
            {
                //var fileName = GetFileNameFromUrl("http://cdn.p30download.com/?b=p30dl-software&f=Mozilla.Firefox.v58.0.x86_p30download.com.zip");
                //LinkChecker hostChecker = new LinkChecker() { };
                //hostChecker.Restart("http://cdn.p30download.com/?b=p30dl-software&f=Adobe.Photoshop.CC.2017.v18.1.1.252.x64_p30download.com.part2.rar");
                //Console.ReadKey();
                //hostChecker.Start();
                //Console.ReadKey();
                InitializeMapper();
                LoadData();

                var link = LinkInfoManager.Current.CreateInstance("https://se10.noyes.in/drive/1s0MNHjXvDK6BAgGeGgF7ZCpMXBNtRNaQ/482714_video_1519799697.mov?md5=pJwWnhe9ySJzlbJGe6sZyg&expires=1519987665");//"https://www.telerik.com/docs/default-source/fiddler/fiddlersetup.exe?sfvrsn=4"
                AgrinClientContext.LinkInfoTable.Add(link);

                //var link = AgrinClientContext.MainLoadedLinkInfoes.FirstOrDefault(x => x.Id == 16);


                LinkInfoManager.Current.Play(link);
                //link.LinkInfoDownloadCore.ConcurrentConnectionCount = 20;
                while (true)
                {
                    Logger.WriteLine(link.DownloadedSize + " , " + link.Size);
                    if (link.DownloadedSize == link.Size)
                        break;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {

            }
            Console.ReadKey();


            //AndroidClass android = new AndroidClass();
            //android.CoreName = "core";
            //((MinProperties)android.Properties).Name = "ali";
            //var androidjs = JsonConvert.SerializeObject(android);
            //var androiddejs = JsonConvert.DeserializeObject<AndroidClass>(androidjs);

            //DesktopClass desktop = new DesktopClass();
            //desktop.CoreName = "core";
            //((MaxProperties)desktop.Properties).Name = "ali";
            //((MaxProperties)desktop.Properties).Age = "aliage";
            //var desktopjs = JsonConvert.SerializeObject(desktop);
            //var desktopdejs = JsonConvert.DeserializeObject<DesktopClass>(desktopjs);

            //LinkInfo link = new LinkInfo();
            //link.Properties.Name = "ali";
            //link.Properties.Name = "ali";

            //MyData data = new MyData() { Age = 27, Name = "Ali" };
            //Patch(data, "{'Age': 14}");


            //EngineSettings settings = new EngineSettings();
            //settings.AllowedEncryption = EncryptionTypes.All;
            //settings.SavePath = Path.Combine("I:\\TestTorrent\\Test", "torrents");

            //if (!Directory.Exists(settings.SavePath))
            //    Directory.CreateDirectory(settings.SavePath);

            //var engine = new ClientEngine(settings);
            //engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, 6969));

            //var torrent = Torrent.Load(new Uri("http://filesbee.com/9VsQMI"), "I:\\TestTorrent\\Test.torrent");

            //TorrentManager manager = new TorrentManager(torrent, engine.Settings.SavePath, new TorrentSettings());
            //engine.Register(manager);

            //manager.Start();
            //Console.Write("Started...");
            //Console.ReadLine();
        }
    }
}
