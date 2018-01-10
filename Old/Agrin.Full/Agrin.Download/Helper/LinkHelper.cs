using Agrin.Data.Mapping;
using Agrin.Download.Data;
using Agrin.Download.Data.Serializition;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Download.Web.Link;
using Agrin.Helper.Collections;
using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Helper
{
    [Serializable]
    public class LinkInfoReportFile
    {
        public string FileName { get; set; }
        public long Lenght { get; set; }
    }

    [Serializable]
    public class LinkInfoReport
    {
        public LinkInfoSerialize LinkInfoData { get; set; }
        public List<LinkInfoReportFile> Files { get; set; }
    }

    public static class LinkHelper
    {
        public static void CreateLinkInfoReport(LinkInfo linkInfo, string savePath)
        {
            LinkInfoReport report = new LinkInfoReport();
            Func<string, object, object> convert = null;
            List<string> manualNames = new List<string>() { "Connections", "UserGroupInfo", "ApplicationGroupInfo" };

            convert = (mapName, mapType) =>
            {
                if (mapName == "Connections")
                {
                    List<ConnectionInfoSerialize> ret = new List<ConnectionInfoSerialize>();
                    foreach (var item in ((IList<LinkWebRequest>)mapType).ToList())
                    {
                        ConnectionInfoSerialize cloned = new ConnectionInfoSerialize();
                        Mapper.Map<LinkWebRequest, ConnectionInfoSerialize>(item, cloned, convert, manualNames);
                        ret.Add(cloned);
                    }
                    return ret;
                }
                else if (mapName == "UserGroupInfo" || mapName == "ApplicationGroupInfo")
                {
                    GroupInfo groupInfo = (GroupInfo)mapType;
                    if (groupInfo == null)
                        return null;
                    return groupInfo.Name;
                }
                return null;
            };
            LinkInfoSerialize serialed = new LinkInfoSerialize();
            Mapper.Map<LinkInfo, LinkInfoSerialize>(linkInfo, serialed, convert, manualNames);
            report.LinkInfoData = serialed;
            report.Files = new List<LinkInfoReportFile>();
            foreach (var item in linkInfo.Connections.ToArray())
            {
                using (var copystream = IOHelper.OpenFileStreamForRead(item.SaveFileName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                {
                    report.Files.Add(new LinkInfoReportFile() { FileName = item.SaveFileName, Lenght = copystream.Length });
                }
            }
            SerializeStream.SaveSerializeStream(savePath, report);
        }

        public static void ExtractLinkReport(string fileName)
        {
            var link = SerializeStream.OpenSerializeStream<LinkInfoSerialize>(fileName);
            if (link != null)
            {
                return;
            }
            var report = SerializeStream.OpenSerializeStream<LinkInfoReport>(fileName);
            var linkInfo = DeSerializeData.LinkInfoSerializeToLinkInfoData(report.LinkInfoData);
            string path = Path.Combine(MPath.SaveDataPath, linkInfo.PathInfo.Id.ToString());
            if (!System.IO.Directory.Exists(path))
            {
                IOHelper.CreateDirectory(path);
                foreach (var file in report.Files)
                {
                    string fileN = Path.Combine(path, Path.GetFileName(file.FileName));
                    using (var copystream = IOHelper.OpenFileStreamForWrite(fileN, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                    {
                        copystream.SetLength(file.Lenght);
                    }
                }
            }
            int id = linkInfo.PathInfo.Id;
            ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, null, false);
            linkInfo.PathInfo.Id = id;
            SerializeData.SaveLinkInfoesToFileNoThread();
            linkInfo.SaveThisLink(true);
        }
        //static WebHeaderCollection collection = new WebHeaderCollection();
        //static string AddRange(string rangeSpecifier, string from, string to)
        //{
        //    string str1 = collection["Range"];
        //    string str2;
        //    if (str1 == null || str1.Length == 0)
        //    {
        //        str2 = rangeSpecifier + "=";
        //    }
        //    else
        //    {
        //        if (string.Compare(str1.Substring(0, str1.IndexOf('=')), rangeSpecifier, StringComparison.OrdinalIgnoreCase) != 0)
        //            return "";
        //        str2 = string.Empty;
        //    }
        //    string str3 = str2 + ((object)from).ToString();
        //    if (to != null)
        //        str3 = str3 + "-" + to;
        //    return str3;
        //}
        //static string AddRange(string rangeSpecifier, long range)
        //{
        //    return AddRange(rangeSpecifier, range.ToString((IFormatProvider)System.Globalization.NumberFormatInfo.InvariantInfo), range >= 0L ? "" : (string)null);
        //}

        //public static string AddRange(long range)
        //{
        //    return AddRange("bytes", range);
        //}

        public static void AddRange(long range, WebRequest _request)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).AddRange(range);
            else if (_request is FtpWebRequest)
                ((FtpWebRequest)_request).ContentOffset = range;

            //#if(MobileApp || XamarinApp)
            //            int rng = (int)(range);
            //            _request.AddRange(rng);

            //#else
            //#endif
        }
        //public static void GetHeaders(HttpWebResponse response)
        //{
        //    try
        //    {
        //        string lower = "";
        //        string contentType = "";
        //        LinkInfoBindingToAdd bindingFileName = ((LinkInfoBindingToAdd)_threadInfo.LinkInfo.BindingProperty);
        //        foreach (string item in response.Headers)
        //        {
        //            lower = item.ToLower();
        //            string data = response.Headers[lower];
        //            if (IsDispose)
        //                return;
        //            switch (lower)
        //            {
        //                case "content-disposition":
        //                    {
        //                        Helper.ApplicationHelper.EnterDispatcherThreadAction(() =>
        //                        {
        //                            if (String.IsNullOrEmpty(bindingFileName._FileName))
        //                                _threadInfo.LinkInfo.BindingProperty.FileName = Agrin_Engine.Helper.AppString.FullDecodeString(Helper.Path.GetFileName(data));
        //                            _threadInfo.LinkInfo.LinksView.FileTypeIcon = null;
        //                        });
        //                        break;
        //                    }
        //                case "content-type":
        //                    {
        //                        contentType = data;
        //                        break;
        //                    }
        //                case "accept-ranges":
        //                    {
        //                        Helper.ApplicationHelper.EnterDispatcherThreadAction(() =>
        //                        {
        //                            if (_response.Headers[lower].Equals("bytes", StringComparison.CurrentCultureIgnoreCase))
        //                                _threadInfo.LinkInfo.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
        //                            else
        //                                _threadInfo.LinkInfo.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.No;
        //                        });
        //                        break;
        //                    }
        //            }
        //        }
        //        if (String.IsNullOrEmpty(bindingFileName._FileName))
        //        {
        //            string getfileName = _threadInfo.LinkInfo.BindingProperty.GetFileName(_response.ResponseUri);
        //            Helper.ApplicationHelper.EnterDispatcherThreadAction(() =>
        //            {
        //                _threadInfo.LinkInfo.BindingProperty.FileName = getfileName;
        //            });
        //        }
        //        if (!String.IsNullOrEmpty(contentType))
        //        {
        //            string ext = Agrin_Engine.Helper.Path.GetDefaultExtension(contentType);
        //            if (!bindingFileName._FileName.ToLower().Contains(ext.ToLower()))
        //            {
        //                Helper.ApplicationHelper.EnterDispatcherThreadAction(() =>
        //                {
        //                    _threadInfo.LinkInfo.BindingProperty.FileName += ext;
        //                });
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}
        public static List<LinkWebRequest> SortByPosition(FastCollection<LinkWebRequest> connections)
        {
            List<LinkWebRequest> col = new List<LinkWebRequest>(connections);
            int n = connections.Count - 1;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i; j++)
                {
                    if (col[j].StartPosition > col[j + 1].StartPosition)
                    {
                        LinkWebRequest temp = col[j];
                        col[j] = col[j + 1];
                        col[j + 1] = temp;
                    }
                }
            }
            return col;
        }
    }
}
