using Agrin.Download.Data.Serializition;
using Agrin.Download.Data.Settings;
using Agrin.Download.Web;
using Agrin.Download.Web.Link;
using Agrin.IO.File;
using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Engine.Repairer
{
    public enum LinkRepairerState
    {
        FindConnectionProblems = 0,
        FindPositionOfProblems = 1,
        DownloadingProblems = 2,
        FixingProblems = 3
    }

    enum GetDownloadMode
    {
        Back,
        Go,
        None
    }

    public static class LinkRepairer
    {
        static string[] GetUserAuthentication(LinkInfoSerialize linkInfo, string address)
        {
            Func<string, string, string[]> GetCredentialCache = (user, pass) =>
            {
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass));
                return new string[] { "Authorization", "Basic " + encoded };
            };

            if (linkInfo.Management != null && linkInfo.Management.NetworkUserPass != null && !String.IsNullOrEmpty(linkInfo.Management.NetworkUserPass.UserName) && !String.IsNullOrEmpty(linkInfo.Management.NetworkUserPass.Password))
            {
                return GetCredentialCache(linkInfo.Management.NetworkUserPass.UserName, linkInfo.Management.NetworkUserPass.Password);
            }
            else
            {
                var findedItem = AppUserAccountsSetting.FindFromAddress(address);
                if (findedItem != null)
                    return GetCredentialCache(findedItem.UserName, findedItem.Password);
            }
            return null;
        }

        public static Action<long, long, LinkRepairerState> LinkRepairerProcessAction { get; set; }
        public static Action<Exception> ExceptionAction { get; set; }

        public static bool FileExist(LinkInfo info)
        {
            try
            {
                if (info == null || string.IsNullOrEmpty(info.PathInfo.BackUpCompleteAddress))
                    return false;
                return File.Exists(info.PathInfo.BackUpCompleteAddress);
            }
            catch
            {
                return false;
            }
        }

        public static string GetNewAddress(List<MultiLinkAddress> items)
        {
            foreach (var item in items)
            {
                if (item.IsSelected)
                    return item.Address;
            }
            return null;
        }

        public static bool Working = false;
        public static string RepairFile(string backUpCompleteAddress, string downloadedFileName)
        {
            string msg = "";
            try
            {
                if (Working)
                    return "نرم افزار در حال بازسازی می باشد.";
                if (!File.Exists(backUpCompleteAddress))
                    return "فایل پشتیبان برای لینک وجود ندارد.";
                Working = true;
                LinkInfoSerialize linkInfo = (Agrin.Download.Data.Serializition.LinkInfoSerialize)Agrin.IO.Helper.SerializeStream.OpenSerializeStream(backUpCompleteAddress);

                if (!File.Exists(downloadedFileName))
                {
                    Working = false;
                    return "فایل در مسیر مورد نظر وجود ندارد: " + downloadedFileName;
                }
                var address = GetNewAddress(linkInfo.Management.MultiLinks);
                if (string.IsNullOrEmpty(address))
                    address = linkInfo.PathInfo.Address;

                var authentication = GetUserAuthentication(linkInfo, address);

                long fileLength = 0;
                var checkValue = LinkChecker.CheckAddressContentSupportRange(new Uri(address), authentication, (len) => fileLength = len);
                if (checkValue == LinkaddressCheckMode.True)
                {
                    using (var fStream = new FileStream(downloadedFileName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        if (fStream.Length != fileLength)
                            fStream.SetLength(fileLength);
                    }
                    var cons = FindConnectionProblems(linkInfo, downloadedFileName, authentication);
                    if (cons.Count > 0)
                    {
                        var sums = FindPositionOfProblems(cons, linkInfo, downloadedFileName, authentication);
                        string sName = Path.GetFileNameWithoutExtension(backUpCompleteAddress);
                        sName = Path.Combine(MPath.RepairSaveDataPath, sName);
                        if (!Directory.Exists(sName))
                            Directory.CreateDirectory(sName);
                        var download = DownloadCheckSums(sName, sums, address, authentication);
                        var repaired = StartFileRepair(downloadedFileName, sums);
                        msg = "OK";
                    }
                    else
                    {
                        msg = "هیچ محل خرابی ای یافت نشد";
                    }

                }
                else
                {
                    if (checkValue == LinkaddressCheckMode.False)
                    {
                        msg = "لینک قابلیت توقف ندارد.";
                    }
                    else if (checkValue == LinkaddressCheckMode.UnknownFileSize)
                    {
                        msg = "حجم فایل توسط سرور نامشخص اعلام شد.";
                    }
                    else
                    {
                        msg = "بررسی قابلیت توقف ناموفق بود.";
                    }
                }
                Working = false;
            }
            catch (Exception e)
            {
                Working = false;
                if (ExceptionAction != null)
                    ExceptionAction(e);
                return e.Message;
            }
            return msg;
        }

        public static string RepairFileByCheckSum(string errorCheckSum, string trueCheckSum, string downloadedFileName, string linkInfoFile)
        {
            string msg = "";
            try
            {
                if (Working)
                    return "نرم افزار در حال بازسازی می باشد.";
                Working = true;
                LinkInfoSerialize linkInfo = (Agrin.Download.Data.Serializition.LinkInfoSerialize)Agrin.IO.Helper.SerializeStream.OpenSerializeStream(linkInfoFile);
                if (!File.Exists(errorCheckSum) || !File.Exists(trueCheckSum))
                {
                    Working = false;
                    return "فایل های باز سازی وجود ندارند";
                }
                var fixChecksums = FileCheckSum.GetErrorsFromTwoCheckSum(errorCheckSum, trueCheckSum);
                if (fixChecksums.Count == 0)
                {
                    Working = false;
                    return "هیچ محل خرابی ای یافت نشد";
                }
                var address = GetNewAddress(linkInfo.Management.MultiLinks);
                if (string.IsNullOrEmpty(address))
                    address = linkInfo.PathInfo.Address;

                var authentication = GetUserAuthentication(linkInfo, address);

                long fileLength = 0;
                var checkValue = LinkChecker.CheckAddressContentSupportRange(new Uri(address), authentication, (len) => fileLength = len);
                if (checkValue == LinkaddressCheckMode.True)
                {
                    using (var fStream = new FileStream(downloadedFileName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        if (fStream.Length != fileLength)
                            fStream.SetLength(fileLength);
                    }
                    string bkFileName = MPath.CreateOneFileByAddress(MPath.BackUpCompleteLinksPath);
                    var backUpCompleteAddress = System.IO.Path.Combine(MPath.BackUpCompleteLinksPath, bkFileName); 
                    string sName = Path.GetFileNameWithoutExtension(backUpCompleteAddress);
                    sName = Path.Combine(MPath.RepairSaveDataPath, sName);
                    if (!Directory.Exists(sName))
                        Directory.CreateDirectory(sName);
                    var download = DownloadCheckSums(sName, fixChecksums, address, authentication);
                    var repaired = StartFileRepair(downloadedFileName, fixChecksums);
                    msg = "OK";
                }
                else
                {
                    if (checkValue == LinkaddressCheckMode.False)
                    {
                        msg = "لینک قابلیت توقف ندارد.";
                    }
                    else if (checkValue == LinkaddressCheckMode.UnknownFileSize)
                    {
                        msg = "حجم فایل توسط سرور نامشخص اعلام شد.";
                    }
                    else
                    {
                        msg = "بررسی قابلیت توقف ناموفق بود.";
                    }
                }
                Working = false;
            }
            catch (Exception e)
            {
                Working = false;
                if (ExceptionAction != null)
                    ExceptionAction(e);
                return e.Message;
            }
            return msg;
        }
        public static List<long> FindConnectionProblems(LinkInfoSerialize linkInfo, string downloadedFileName, string[] authentication)
        {
            List<long> positions = new List<long>();
            using (var fStream = new FileStream(downloadedFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                int positionI = 1, count = linkInfo.DownloadingProperty.DownloadRangePositions.Count;
                foreach (var CPosition in linkInfo.DownloadingProperty.DownloadRangePositions)
                {
                    var newPosition = CPosition - 1024;
                    fStream.Seek(newPosition, SeekOrigin.Begin);
                    int countRead = 2048;
                    byte[] readBytes = FileCheckSum.GetBytesPerBuffer(fStream, countRead);

                    var _request = CreateHttpWebRequest(linkInfo.PathInfo.Address, authentication);
                    _request.AddRange(newPosition);
                    byte[] _read = new byte[countRead];
                    using (var readStream = _request.GetResponse().GetResponseStream())
                    {
                        byte[] readBytesFromServer = FileCheckSum.GetBytesPerBufferNet(readStream, countRead, (long)linkInfo.DownloadingProperty.Size, newPosition);
                        for (int j = 0; j < readBytes.Length; j++)
                        {
                            if (readBytes[j] != readBytesFromServer[j])
                            {
                                positions.Add(CPosition);
                                break;
                            }
                        }
                    }

                    if (LinkRepairerProcessAction != null)
                        LinkRepairerProcessAction(positionI, count, LinkRepairerState.FindConnectionProblems);
                    positionI++;
                }
            }
            return positions;
        }

        public static List<CheckSumItem> FindPositionOfProblems(List<long> positions, LinkInfoSerialize linkInfo, string downloadedFileName, string[] authentication)
        {
            List<CheckSumItem> checkSumPositions = new List<CheckSumItem>();
            using (var fStream = new FileStream(downloadedFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                int positionI = 1, count = positions.Count;
                Func<long, CheckSumItem> checkSumExistLast = (val) =>
                    {
                        foreach (var item in checkSumPositions)
                        {
                            if (item.StartPosition < val && item.EndPosition > val)
                            {
                                return item;
                            }
                        }
                        return null;
                    };
                foreach (var CPosition in positions)
                {
                    long start = CPosition;
                    GetDownloadMode mode = GetDownloadMode.None;
                GetNewDownloadMode:
                    if (mode == GetDownloadMode.None)
                        mode = GetDownloadMode.Back;
                    else if (mode == GetDownloadMode.Back)
                        mode = GetDownloadMode.Go;
                    else if (mode == GetDownloadMode.Go)
                        continue;
                    long oldPos = CPosition;
                    int getPosCount = 10;
                    Func<long> getNewPosition = () =>
                    {
                        if (mode == GetDownloadMode.Back)
                            oldPos -= 1024 * getPosCount;
                        else
                            oldPos += 1024 * getPosCount;

                        if (oldPos <= 0)
                            oldPos = 0;
                        if (getPosCount == 10)
                            getPosCount = 1024;
                        else
                            getPosCount = 1024 * 3;
                        return oldPos;
                    };
                MustGetOlderPositions:
                    var existCHK = checkSumExistLast(oldPos);
                    if (existCHK != null)
                    {
                        existCHK.EndPosition = CPosition;
                        goto GetNewDownloadMode;
                    }
                    var newPosition = getNewPosition();
                    fStream.Seek(newPosition, SeekOrigin.Begin);
                    int countRead = 1024 * 10;
                    byte[] readBytes = FileCheckSum.GetBytesPerBuffer(fStream, countRead);

                    var _request = CreateHttpWebRequest(linkInfo.PathInfo.Address, authentication);
                    _request.AddRange(newPosition);
                    byte[] _read = new byte[countRead];
                    using (var readStream = _request.GetResponse().GetResponseStream())
                    {
                        byte[] readBytesFromServer = FileCheckSum.GetBytesPerBufferNet(readStream, countRead, (long)linkInfo.DownloadingProperty.Size, newPosition);
                        for (int j = 0; j < readBytes.Length; j++)
                        {
                            if (readBytes[j] != readBytesFromServer[j])
                            {
                                goto MustGetOlderPositions;
                            }
                        }
                    }
                    if (mode == GetDownloadMode.Back)
                    {
                        start = newPosition;
                        goto GetNewDownloadMode;
                    }
                    else
                    {
                        checkSumPositions.Add(new CheckSumItem() { StartPosition = start, EndPosition = newPosition });
                    }

                    if (LinkRepairerProcessAction != null)
                        LinkRepairerProcessAction(positionI, count, LinkRepairerState.FindPositionOfProblems);
                    positionI++;
                }
            }
            return checkSumPositions;
        }

        public static bool DownloadCheckSums(string savePath, List<CheckSumItem> checkSums, string uri, string[] authentication)
        {
            int positionI = 1;
            foreach (var ParentLinkWebRequest in checkSums)
            {
                string fileName = Path.Combine(savePath, positionI + ".fix");
                ParentLinkWebRequest.FileName = fileName;
                using (FileStream _saveStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    if (_saveStream.Length == ParentLinkWebRequest.Size)
                    {
                        positionI++;
                        continue;
                    }

                    else if (_saveStream.Length > ParentLinkWebRequest.Size)
                    {
                        positionI++;
                        _saveStream.SetLength(ParentLinkWebRequest.Size);
                        continue;
                    }
                    _saveStream.Seek(0, SeekOrigin.End);
                    var _request = CreateHttpWebRequest(uri, authentication);
                    long range = (long)ParentLinkWebRequest.StartPosition + _saveStream.Length;
                    _request.AddRange(range);
                    var _BufferRead = 1024 * 100;
                    byte[] _read = new byte[_BufferRead];
                    using (var readStream = _request.GetResponse().GetResponseStream())
                    {
                        while (_saveStream.Length < ParentLinkWebRequest.Size)
                        {
                            int readCount = 0;

                            if (_BufferRead != 0)
                                readCount = readStream.Read(_read, 0, _BufferRead);
                            else
                                readCount = 0;

                            if (readCount == 0)
                            {
                                if (_saveStream.Length == ParentLinkWebRequest.Size)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                _saveStream.Write(_read, 0, readCount);
                            }
                            if (LinkRepairerProcessAction != null)
                                LinkRepairerProcessAction(_saveStream.Position, ParentLinkWebRequest.Size, LinkRepairerState.DownloadingProblems);
                        }
                    }
                    _saveStream.SetLength(ParentLinkWebRequest.Size);
                }
                positionI++;
            }
            return true;
        }

        static HttpWebRequest CreateHttpWebRequest(string addresss, string[] authentication)
        {
            var _request = (HttpWebRequest)WebRequest.Create(addresss);
            _request.Timeout = 60000;
            _request.AllowAutoRedirect = true;
            _request.ServicePoint.ConnectionLimit = int.MaxValue;

            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (authentication != null)
                _request.Headers.Add(authentication[0], authentication[1]);
            return _request;
        }

        public static bool StartFileRepair(string errorFileName, List<CheckSumItem> checkSums)
        {
            using (FileStream _saveStream = new FileStream(errorFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (var item in checkSums)
                {
                    using (FileStream _checkSumStream = new FileStream(item.FileName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        _saveStream.Seek(item.StartPosition, SeekOrigin.Begin);
                        while (_checkSumStream.Position != _checkSumStream.Length)
                        {
                            var bytes = FileCheckSum.GetBytesPerBuffer(_checkSumStream, 1024 * 1024);
                            _saveStream.Write(bytes, 0, bytes.Length);
                            if (LinkRepairerProcessAction != null)
                                LinkRepairerProcessAction(_checkSumStream.Position, _checkSumStream.Length, LinkRepairerState.DownloadingProblems);
                        }
                    }
                }
            }
            try
            {
                foreach (var item in checkSums)
                    File.Delete(item.FileName);
            }
            catch
            {

            }
            return true;
        }
    }
}
