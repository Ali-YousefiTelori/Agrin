//using OctoTorrent.Client;
//using OctoTorrent.Client.Encryption;
//using OctoTorrent.Common;
using Agrin.Client.DataBase;
using Agrin.Download.CoreModels.Link;
using Agrin.Download.EntireModels.Link;
using Agrin.Download.ShortModels.Link;
using Agrin.IO;
using Agrin.Log;
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
            Mapper.Bind<LinkInfoPropertiesShort, LinkInfoProperties>();
            Mapper.Bind<LinkInfoRequestCore, LinkInfoRequest>();
            Mapper.AfterInstance<LinkInfoRequest>((target, parent) =>
            {
                target.LinkInfo = (LinkInfo)parent;
                try
                {
                    if (File.Exists(target.SaveConnectionFileName))
                    {
                        FileInfo file = new FileInfo(target.SaveConnectionFileName);
                        target.DownloadedSize = file.Length;
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
            Mapper.AfterInstance<LinkInfoPathShort>((target, parent) =>
            {
                target.LinkInfo = (LinkInfo)parent;
            });
            Mapper.AfterInstance<LinkInfoProperties>((target, parent) =>
            {
                target.LinkInfo = (LinkInfo)parent;
            });
            Mapper.AfterInstance<LinkInfoManagementShort>((target, parent) =>
            {
                target.LinkInfo = (LinkInfo)parent;
            });
            Mapper.AfterInstance<LinkInfoDownloadCore>((target, parent) =>
            {
                target.LinkInfo = (LinkInfo)parent;
            });
        }

        static void LoadData()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            PathHelper.Initialize(path, path, path);
            AgrinClientContext.DataBasePath = path;
            AutoLogger.ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
            AgrinClientContext.LinkInfoTable.Initialize<LinkInfo>();
        }

        static void Main(string[] args)
        {
            try
            {
                InitializeMapper();
                LoadData();

                //var link = LinkInfoCore.CreateInstance("http://up.bia4music.org/music/96/Khordad/Hoorosh%20Band%20-%20Be%20Ki%20Poz%20Midi.mp3");//"https://www.telerik.com/docs/default-source/fiddler/fiddlersetup.exe?sfvrsn=4"
                
                var link = AgrinClientContext.LinkInfoes.Last();
                //item.CreatedDateTime = DateTime.Now;
                //AgrinClientContext.LinkInfoTable.Update(item);

                //AgrinClientContext.LinkInfoTable.Add(link);

                link.Play();
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
