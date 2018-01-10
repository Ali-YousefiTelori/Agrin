using Agrin.IO.Helper;
using Agrin.IO.Strings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using YoutubeExtractor;

namespace FrameSoft.Agrin.DataBase.Helper
{
    public enum DownloadManagerStatus
    {
        Unknown = 0,
        Creating = 1,
        Starting = 2,
        Downloading = 3,
        Error = 4,
        Complete = 5,
    }

    public class DownloadManagerEngine : IDisposable
    {
        public long DownloadedSize { get; set; }

        public UserFileInfo FileInfo { get; set; }

        public int FileID { get; set; }
        bool _isYoutube = false;
        public DownloadManagerEngine(int userID, int fileID, bool isYoutube)
        {
            _isYoutube = isYoutube;
            FileID = fileID;
            FileInfo = AgrinDataBaseHelper.GetLeechFileInfo(userID, fileID);
            Status = DownloadManagerStatus.Creating;
        }

        DownloadManagerStatus _Status = DownloadManagerStatus.Unknown;

        public DownloadManagerStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                FileInfo.Status = (byte)value;
            }
        }

        FileStream _saveStream = null;
        void Download()
        {
            Status = DownloadManagerStatus.Downloading;
            FileInfo.IsError = false;
            AgrinDataBaseHelper.UpdateLeechFile(FileInfo);
            var downloadCount = SmallDownloadManager.DownloadingCount();
            var sleep = downloadCount;
            if (downloadCount < 20)
                sleep = 20;
            else if (downloadCount < 50)
                sleep = 40;
            else if (downloadCount < 150)
                sleep = 60;
            else if (downloadCount < 300)
                sleep = 80;
            else
                sleep = 100;
            using (FileStream saveStream = new FileStream(FileInfo.DiskPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                _saveStream = saveStream;
                if (_saveStream.Length == FileInfo.Size)
                {
                    Status = DownloadManagerStatus.Complete;
                    return;
                }

                else if (_saveStream.Length > FileInfo.Size)
                {
                    _saveStream.SetLength((long)FileInfo.Size);
                    Status = DownloadManagerStatus.Complete;
                    return;
                }
                _saveStream.Seek(0, SeekOrigin.End);
                string address = FileInfo.Link;
                if (_isYoutube)
                {
                    string normalURL = address;
                    List<VideoInfo> videos = null;
                    if (DownloadUrlResolver.TryNormalizeYoutubeUrl(address, out normalURL))
                        videos = DownloadUrlResolver.GetDownloadUrls(normalURL).ToList();
                    else
                        videos = DownloadUrlResolver.GetDownloadUrls(address).ToList();

                    var video = DownloadUrlResolver.GetVideoInfoByFormatCode(videos, FileInfo.FormatCode);
                    address = video.DownloadUrl;
                }


                var _request = (HttpWebRequest)WebRequest.Create(address);
                _request.Timeout = 60000;
                _request.AllowAutoRedirect = true;
                _request.ServicePoint.ConnectionLimit = int.MaxValue;

                _request.KeepAlive = true;
                _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                long range = _saveStream.Length;
                _request.AddRange(range);
                var _BufferRead = 1024 * 100;
                byte[] _read = new byte[_BufferRead];
                using (var readStream = _request.GetResponse().GetResponseStream())
                {
                    while (_saveStream.Length < (long)FileInfo.Size && !_isDispose)
                    {
                        int readCount = 0;

                        if (_BufferRead != 0)
                            readCount = readStream.Read(_read, 0, _BufferRead);
                        else
                            readCount = 0;

                        if (readCount == 0)
                        {
                            if (_saveStream.Length == (long)FileInfo.Size)
                            {
                                break;
                            }
                        }
                        else
                        {
                            _saveStream.Write(_read, 0, readCount);
                        }
                        DownloadedSize = _saveStream.Position;
                        Thread.Sleep(sleep);
                    }
                }
                DownloadedSize = (long)FileInfo.Size;
                _saveStream.SetLength((long)FileInfo.Size);
                Status = DownloadManagerStatus.Complete;
            }
            if (Status == DownloadManagerStatus.Complete)
            {
                FileInfo.IsComplete = true;
                AgrinDataBaseHelper.UpdateLeechFile(FileInfo);
            }
        }

        bool isStart = false;
        public void Start()
        {
            if (isStart)
                return;
            Status = DownloadManagerStatus.Starting;
            isStart = true;
            Thread thread = new Thread(() =>
            {
                try
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (_isDispose)
                            break;
                        try
                        {
                            Download();
                            break;
                        }
                        catch
                        {
                            if (_isDispose)
                                break;
                            FileInfo.IsError = true;
                            Status = DownloadManagerStatus.Error;
                            AgrinDataBaseHelper.UpdateLeechFile(FileInfo);
                        }
                    }
                    if (!_isDispose)
                        Dispose();
                }
                catch
                {
                    if (!_isDispose)
                        Dispose();
                }
            });
            thread.Start();
        }

        bool _isDispose = false;
        public void Dispose()
        {
            try
            {
                _isDispose = true;
                _saveStream.Dispose();
            }
            catch
            {

            }
            SmallDownloadManager.DisposedItem(this);
        }
    }

    public static class SmallDownloadManager
    {
        static object lockOBJ = new object();
        public static Dictionary<int, DownloadManagerEngine> DownloadManagers = new Dictionary<int, DownloadManagerEngine>();
        public static UserFileInfo StartDownloadingFile(int userID, int fileID, bool isYoutube)
        {
            lock (lockOBJ)
            {
                if (DownloadManagers.ContainsKey(fileID))
                    return null;
                var downloader = new DownloadManagerEngine(userID, fileID, isYoutube);
                DownloadManagers.Add(fileID, downloader);
                downloader.Start();
                return downloader.FileInfo;
            }
        }

        public static int RefreshDownloadLinks()
        {
            int count = 0;
            foreach (var item in AgrinDataBaseHelper.GetCanDownloadLeechFileInfoes())
            {
                var info = StartDownloadingFile(item.UserID, item.ID, IsYoutubeLink(item.Link));
                if (info != null)
                    count++;
            }
            return count;
        }

        public static bool IsYoutubeLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if (url.ToLower().Contains("youtu.be") || url.ToLower().Contains("youtube.com"))
                return true;
            return false;
        }

        public static int DownloadingCount()
        {
            lock (lockOBJ)
            {
                return DownloadManagers.Count;
            }
        }

        public static void DisposeAndStopDownloadManager(int fileID)
        {
            DownloadManagerEngine manager = null;
            lock (lockOBJ)
            {
                if (DownloadManagers.ContainsKey(fileID))
                    manager = DownloadManagers[fileID];
                else
                    return;
            }
            manager.Dispose();
        }

        public static void DisposedItem(DownloadManagerEngine item)
        {
            lock (lockOBJ)
            {
                DownloadManagers.Remove(item.FileID);
            }
        }

        public static string GetNewFileNameAddress(string path, string ext)
        {
            int i = 0;
            string fileName = Path.Combine(path, i + ext);
            while (File.Exists(fileName))
            {
                fileName = Path.Combine(path, i + ext);
                i++;
            }
            return fileName;
        }

        public static string GetFileNameValidChar(string fileName)
        {
            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            return fileName;
        }

        public static string AddressFiles = "D:\\UserLeechFiles";
        public static UserFileInfo CreateNewFileStorageForDownload(int userID, string link, long fileSize, string fileName, bool isYoutube = false, int formatCode = 0)
        {
            fileName = GetFileNameValidChar(fileName);
            var dt = DateTime.Now;
            string path = Path.Combine(new string[] { AddressFiles, dt.Year.ToString(), dt.Month.ToString(), dt.Day.ToString(), userID.ToString() });
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = GetNewFileNameAddress(path, Path.GetExtension(fileName));
            File.Create(path).Dispose();
            var fileID = AgrinDataBaseHelper.CreateLeechFile(userID, fileSize, path, link, formatCode, fileName);
            return StartDownloadingFile(userID, fileID, isYoutube);
        }

        public static UserFileInfo GetFileStatus(int userID, int fileID, out long downloadedSize, bool returnNullifNoDownloading)
        {
            lock (lockOBJ)
            {
                if (DownloadManagers.ContainsKey(fileID))
                {
                    downloadedSize = DownloadManagers[fileID].DownloadedSize;
                    return DownloadManagers[fileID].FileInfo;
                }
            }
            if (returnNullifNoDownloading)
            {
                downloadedSize = 0;
                return null;
            }

            var file = AgrinDataBaseHelper.GetLeechFileInfo(userID, fileID);
            downloadedSize = new FileInfo(file.DiskPath).Length;
            return file;
        }

        public static long GetFileSize(string address)
        {
            try
            {
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(address);
                _request.AllowAutoRedirect = true;
                _request.KeepAlive = true;
                _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
                {
                    long length = response.ContentLength;
                    if (length <= 0)
                    {
                        return -1;
                    }
                    return length;
                }
            }
            catch
            {
                return -2;
            }
        }

        public static long GetYoutubeFileSize(string address, int formatCode, out string outFileName)
        {
            try
            {
                string normalURL = address;
                List<VideoInfo> videos = null;
                if (DownloadUrlResolver.TryNormalizeYoutubeUrl(address, out normalURL))
                    videos = DownloadUrlResolver.GetDownloadUrls(normalURL).ToList();
                else
                    videos = DownloadUrlResolver.GetDownloadUrls(address).ToList();

                var video = DownloadUrlResolver.GetVideoInfoByFormatCode(videos, formatCode);
                address = video.DownloadUrl;
                outFileName = video.Title + video.VideoExtension;

                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(address);
                _request.AllowAutoRedirect = true;
                _request.KeepAlive = true;
                _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
                {
                    long length = response.ContentLength;
                    if (length <= 0)
                    {
                        return -1;
                    }
                    return length;
                }
            }
            catch(Exception ex)
            {
                outFileName = null;
                return -2;
            }
        }

        public static string GetLinksFileName(string newFileName)
        {
            if (string.IsNullOrEmpty(newFileName))
                return newFileName;
            string decode = Decodings.FullDecodeString(newFileName.Trim().Trim(new char[] { '/', '\\' }));
            Uri uri = null;
            string fileName = null;
            if (Uri.TryCreate(decode, UriKind.Absolute, out uri))
            {
                fileName = Decodings.FullDecodeString(System.IO.Path.GetFileName(uri.AbsolutePath));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Decodings.FullDecodeString(System.IO.Path.GetFileName(decode));
                    if (fileName.Contains("="))
                    {
                        int l = fileName.LastIndexOf('=');
                        if (l < fileName.Length - 1)
                        {
                            string name = fileName.Substring(l + 1, fileName.Length - l - 1);
                            if (!string.IsNullOrEmpty(name))
                                fileName = name;
                        }

                    }
                }
            }
            else
                fileName = System.IO.Path.GetFileName(decode);

            foreach (var item in System.IO.Path.GetInvalidPathChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            if (string.IsNullOrEmpty(fileName))
                return "notName.html";
            else if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fileName)))
                return fileName + ".html";
            return fileName;
        }

        public static string GetFileName(string address)
        {
            try
            {
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(address);
                _request.AllowAutoRedirect = true;
                _request.KeepAlive = true;
                _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                string addressFileName = Path.GetFileName(_request.RequestUri.OriginalString);
                string fileName = null;
                using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
                {
                    string lower = "";
                    string contentType = "";
                    foreach (string item in response.Headers)
                    {
                        lower = item.ToLower();
                        string estrin = response.Headers[lower];
                        switch (lower)
                        {
                            case "content-disposition":
                                {
                                    fileName = Decodings.FullDecodeString(MPath.GetFileName(estrin)).Trim(new char[] { '"' });

                                    break;
                                }
                            case "content-type":
                                {
                                    contentType = estrin;
                                    break;
                                }
                        }
                    }
                    if (String.IsNullOrEmpty(fileName))
                    {
                        string getfileName = GetLinksFileName(response.ResponseUri.OriginalString);
                        fileName = getfileName.Trim(new char[] { '"' });
                    }
                    if (!String.IsNullOrEmpty(contentType))
                    {
                        string ext = MPath.GetDefaultExtension(contentType);
                        if (!addressFileName.ToLower().Contains(ext.ToLower()))
                        {
                            var extF = Path.GetExtension(addressFileName);
                            if (extF != null)
                                extF = extF.ToLower();
                            if (extF == ".htm" || extF == ".html")
                            {
                                var fn = Path.GetFileNameWithoutExtension(addressFileName);
                                addressFileName = fn + ext;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(Path.GetExtension(fileName)))
                    {
                        fileName = addressFileName;
                    }
                    return fileName;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
