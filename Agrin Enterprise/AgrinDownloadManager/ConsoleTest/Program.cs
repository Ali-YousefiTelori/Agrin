//using OctoTorrent.Client;
//using OctoTorrent.Client.Encryption;
//using OctoTorrent.Common;
using Agrin.Client.DataBase;
using Agrin.Client.DataBase.Tables;
using Agrin.Download.CoreModels.Link;
using Agrin.Download.DataBaseModels;
using Agrin.Download.EntireModels.Link;
using Agrin.Download.EntireModels.Managers;
using Agrin.Download.ShortModels.Link;
using Agrin.Download.Web;
using Agrin.IO;
using Agrin.IO.Helpers;
using Agrin.IO.Streams;
using Agrin.Log;
using Agrin.UI.ViewModels.Helpers;
using Agrin.Web;
using CrazyMapper;
using Framesoft.Helpers.Helpers;
using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
                //StreamCross.OpenFile = (filename, filemode, fileaccess) =>
                //{
                //    return new AndroidStreamCross(filename, filemode, fileaccess);
                //};
                LinkInfoManagerBase.Current = new LinkInfoManager();
                TaskScheduleManagerBase.Current = new TaskScheduleManager();
                //var fileName = GetFileNameFromUrl("http://cdn.p30download.com/?b=p30dl-software&f=Mozilla.Firefox.v58.0.x86_p30download.com.zip");
                //LinkChecker hostChecker = new LinkChecker() { };
                //hostChecker.Restart("http://cdn.p30download.com/?b=p30dl-software&f=Adobe.Photoshop.CC.2017.v18.1.1.252.x64_p30download.com.part2.rar");
                //Console.ReadKey();
                //hostChecker.Start();
                //Console.ReadKey();
                InitializeMapper();
                LoadData();

                //var linklast = LinkInfoManager.Current.LinkInfoes.Where(x => x.AsShort().PathInfo.FileName.ToLower().Contains("ep06")).FirstOrDefault();
                var linklast = LinkInfoManager.Current.LinkInfoes.Where(x => !x.IsComplete).OrderByDescending(x => x.LastDownloadedDateTime).FirstOrDefault();
                var ex = AgrinClientContext.ExceptionInfoTable.GetExceptionsByLinkId(linklast.Id);
                var exs = TextHelper.Base64Decode(ex.FirstOrDefault().FullMessage);
                var exsm = TextHelper.Base64Decode(ex.FirstOrDefault().Message);
                linklast.IsComplete = false;
                foreach (var item in linklast.Connections)
                {
                    var stream = File.Create(item.SaveConnectionFileName);
                    stream.SetLength(item.DownloadedSize);
                    stream.Dispose();
                }
                var iseror = linklast.IsError;
                //ExceptionInfoTable etable = new ExceptionInfoTable();
                //var ex1 = etable.GetExceptionByLinkId(linklast.Id);
                //var ex2 = etable.GetExceptionsByLinkId(linklast.Id);
                //var ex3 = etable.GetList();
                linklast.Play();
                //GetErrorByLinkId(linklast.Id);
                //var link = LinkInfoManager.Current.CreateInstance("http://clicksite.org/dl_file.php?key=2800634339&idG=818");
                //AgrinClientContext.LinkInfoTable.Add(link);

                //var link = AgrinClientContext.MainLoadedLinkInfoes.FirstOrDefault(x => x.Id == 16);


                //LinkInfoManager.Current.Play(link);
                ////link.LinkInfoDownloadCore.ConcurrentConnectionCount = 20;
                //while (true)
                //{
                //    System.Logger.WriteLine(link.DownloadedSize + " , " + link.Size);
                //    if (link.DownloadedSize == link.Size)
                //        break;
                //    Thread.Sleep(1000);
                //}
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

        static void GetErrorByLinkId(int linkId)
        {
            using (var db = new LiteDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "AgrinClientDatabase.db")))
            {
                var erros = db.GetCollection<AddRangePositionInfo>("AddRangePositionInfoes");
                var all = erros.FindAll().Where(x => x.LinkId == linkId).ToList();
                var addRangePositionInfo = new AddRangePositionInfo();
                addRangePositionInfo.LinkId = linkId;
                addRangePositionInfo.Position = 4524;
                AgrinClientContext.AddRangePositionInfoTable.Add(addRangePositionInfo);
            }
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "AgrinClientDatabase.db")))
            {
                var erros = db.GetCollection<ExceptionInfo>("ExceptionInfoes");
                var all = erros.FindAll().Where(x => x.LinkId == linkId).OrderByDescending(x => x.LastDateTimeErrorDetected).ToList();
                foreach (var item in all)
                {
                    var message = TextHelper.Base64Decode(item.Message);
                    var full = TextHelper.Base64Decode(item.FullMessage);
                }
            }
        }
    }

    public class AndroidStreamCross : IStreamWriter
    {
        FileStream Stream { get; set; }

        public Guid Guid { get; set; }
        public AndroidStreamCross(string fileName, System.IO.FileMode mode, FileAccess access)
        {
            Guid = Guid.NewGuid();
            Stream = new FileStream(fileName, mode, access);
        }

        public long Length
        {
            get
            {
                return Stream.Length;
            }
        }

        public long Position
        {
            get
            {
                return Stream.Position;
            }
            set
            {
                Stream.Seek(value, SeekOrigin.Begin);
            }
        }

        bool isDispose = false;
        public void Dispose()
        {
            if (isDispose)
                return;
            isDispose = true;
            AutoLogger.LogText($"AndroidStreamCross {Guid} dispose");
            try
            {
                Stream.Flush();
            }
            catch (Exception ex)
            {

            }
            Stream.Dispose();
        }

        public void Flush()
        {
            Stream.Flush();
        }

        public int Read(byte[] bytes, int offest, int count)
        {
            return Stream.Read(bytes, offest, count);
        }

        public long Seek(long position, SeekOrigin seek)
        {
            if (seek == SeekOrigin.Begin)
                Stream.Seek(0, SeekOrigin.Begin);
            else if (seek == SeekOrigin.End)
                Stream.Seek(position, SeekOrigin.End);
            else
                Stream.Seek(position, SeekOrigin.Current);
            return Position;
        }

        public void SetLength(long lenght)
        {
            Stream.SetLength(lenght);
        }

        public void Write(byte[] bytes, int offest, int count)
        {
            Stream.Write(bytes, offest, count);
        }
    }
}
