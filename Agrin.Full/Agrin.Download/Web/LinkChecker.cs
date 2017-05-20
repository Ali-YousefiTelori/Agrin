using Agrin.Download.Helper;
using Agrin.Download.Web.Link;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web
{
    public enum LinkaddressCheckMode
    {
        UnknownFileSize,
        False,
        True,
        Exception,
        ReadCountError,
        MinimumLenght,
        NotCheckedNow
    }

    public class LinkCheckerItem
    {
        public LinkaddressCheckMode Mode { get; set; }
        public object LockOBJ = new object();
    }

    public static class LinkChecker
    {
        static int readLength = 1024 * 20;
        static List<byte> ReadBytesFromArray(Stream stream)
        {
            List<byte> buffer_1 = new List<byte>();
            using (stream)
            {
                int readingLength = readLength;
                while (true)
                {
                    byte[] buffer = new byte[readingLength];
                    var readCount = stream.Read(buffer, 0, buffer.Length);
                    buffer_1.AddRange(buffer.ToList().GetRange(0, readCount));
                    if (buffer_1.Count == readLength || readCount == 0)
                        break;
                    else
                        readingLength -= readCount;
                }
            }
            return buffer_1;
        }

        public static object lockAdd = new object();
        public static Dictionary<string, LinkCheckerItem> LockObjectOfUris = new Dictionary<string, LinkCheckerItem>();
        public static LinkaddressCheckMode CheckAddressContentSupportRange(Uri address, string[] authentication, Action<long> getedLenght = null)
        {
            if (address == null || string.IsNullOrEmpty(address.OriginalString))
                return LinkaddressCheckMode.Exception;
            if (address.OriginalString.ToLower().Contains("ftp://"))
                return LinkaddressCheckMode.True;
            lock (lockAdd)
            {
                if (!LockObjectOfUris.ContainsKey(address.OriginalString))
                    LockObjectOfUris.Add(address.OriginalString, new LinkCheckerItem() { Mode = LinkaddressCheckMode.NotCheckedNow });
            }
            var item = LockObjectOfUris[address.OriginalString];
            lock (item.LockOBJ)
            {
                try
                {
                    if (item.Mode != LinkaddressCheckMode.NotCheckedNow && item.Mode != LinkaddressCheckMode.Exception)
                        return item.Mode;
                    Func<HttpWebRequest> createRequest = () =>
                    {
                        HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(address);
                        _request.AllowAutoRedirect = true;
                        _request.KeepAlive = true;
                        _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                        _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                        if (authentication != null)
                            _request.Headers.Add(authentication[0], authentication[1]);
                        return _request;
                    };

                    HttpWebRequest request = createRequest();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    long length = response.ContentLength;
                    if (length < 0)
                    {
                        item.Mode = LinkaddressCheckMode.False;
                        return item.Mode;
                    }
                    if (length < 1024 * 100)
                    {
                        item.Mode = LinkaddressCheckMode.MinimumLenght;
                        return item.Mode;
                    }
                    if (getedLenght != null)
                        getedLenght(length);
                    List<byte> buffer_1 = ReadBytesFromArray(response.GetResponseStream());
                    response.Close();


                    request = createRequest();
                    LinkHelper.AddRange(response.ContentLength / 2, request);
                    response = (HttpWebResponse)request.GetResponse();
                    List<byte> buffer_2 = ReadBytesFromArray(response.GetResponseStream());
                    response.Close();
                    if (!buffer_1.SequenceEqual(buffer_2) && buffer_1.Count == buffer_2.Count)
                    {
                        item.Mode = LinkaddressCheckMode.True;
                        return item.Mode;
                    }
                    request = createRequest();
                    LinkHelper.AddRange(response.ContentLength - readLength - 1, request);
                    response = (HttpWebResponse)request.GetResponse();
                    List<byte> buffer_3 = ReadBytesFromArray(response.GetResponseStream());
                    response.Close();

                    if (!buffer_2.SequenceEqual(buffer_3) && buffer_2.Count == buffer_3.Count)
                    {
                        item.Mode = LinkaddressCheckMode.True;
                        return item.Mode;
                    }
                }
                catch (Exception e)
                {
                    Agrin.Log.AutoLogger.LogError(e, "CheckAddressContentSupportRange");
                    item.Mode = LinkaddressCheckMode.Exception;
                    return item.Mode;
                }
                item.Mode = LinkaddressCheckMode.False;
            }
            
            return item.Mode;
        }

        public static LinkaddressCheckMode CheckMultiLinkInfo(List<MultiLinkAddress> items, List<ProxyInfo> proxies)
        {
            long size = long.MinValue;
            foreach (var item in items)
            {
                if (!item.IsSelected)
                    continue;
                try
                {
                    long getedSize = GetSize(item, proxies);
                    if (getedSize < 0)
                    {
                        return LinkaddressCheckMode.UnknownFileSize;
                    }
                    else if (size != long.MinValue && size != getedSize)
                    {
                        return LinkaddressCheckMode.False;
                    }
                    size = getedSize;
                }
                catch
                {
                    return LinkaddressCheckMode.Exception;
                }
            }
            return LinkaddressCheckMode.True;
        }

        static long GetSize(MultiLinkAddress address, List<ProxyInfo> items)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address.Address);
            request.KeepAlive = true;
            request.Proxy = ProxyInfo.GetProxyByID(items, address.ProxyID);
            request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            return response.ContentLength;
        }
    }
}
