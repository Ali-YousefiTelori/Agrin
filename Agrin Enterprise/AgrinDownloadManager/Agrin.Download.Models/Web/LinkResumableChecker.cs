using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web
{
    /// <summary>
    /// check link support resumable
    /// </summary>
    public class LinkResumableChecker : IDisposable
    {
        int readLength = 1024 * 20;
        List<byte> ReadBytesFromArray(Stream stream)
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

        /// <summary>
        /// Check address content for support resumable
        /// </summary>
        /// <param name="address"></param>
        /// <param name="authentication"></param>
        /// <param name="proxy"></param>
        /// <param name="cookie"></param>
        /// <param name="customHeaders"></param>
        /// <param name="getedLenght"></param>
        /// <returns></returns>
        public bool CheckAddressContentForSupportResumable(string address, string authentication, IWebProxy proxy, CookieContainer cookie, WebHeaderCollection customHeaders, Action<long> getedLenght = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new Exception("CheckAddressContentSupportRange address is null or empty!");

            WebRequest request = LinkHelper.CreateRequest(address, authentication, 60000, proxy, cookie, int.MaxValue, customHeaders);
            List<byte> buffer_1 = null;
            long length = 0;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                length = response.ContentLength;
                if (length < 0)
                {
                    //item.Status = LinkaddressCheckMode.False;
                    //throw new Exception("CheckAddressContentSupportRange unknown file size detected!");
                    return false;
                }
                getedLenght?.Invoke(length);
                buffer_1 = ReadBytesFromArray(response.GetResponseStream());
            }



            request = LinkHelper.CreateRequest(address, authentication, 60000, proxy, cookie, int.MaxValue, customHeaders);
            request.AddRange(length / 2);
            List<byte> buffer_2 = null, buffer_3 = null;
            using (var response = request.GetResponse())
            {
                buffer_2 = ReadBytesFromArray(response.GetResponseStream());
                response.Close();
                if (!buffer_1.SequenceEqual(buffer_2) && buffer_1.Count == buffer_2.Count)
                {
                    //item.Status = LinkaddressCheckMode.True;
                    return true;
                }
                request = LinkHelper.CreateRequest(address, authentication, 60000, proxy, cookie, int.MaxValue, customHeaders);
                request.AddRange(length - readLength - 1);
                using (var response2 = request.GetResponse())
                {
                    buffer_3 = ReadBytesFromArray(response2.GetResponseStream());
                }

            }

            if (!buffer_2.SequenceEqual(buffer_3) && buffer_2.Count == buffer_3.Count)
            {
                //item.Status = LinkaddressCheckMode.True;
                return true;
            }
            return false;
        }

        public bool CheckAddressContentForSupportResumableJustHeader(string address, string authentication, IWebProxy proxy, CookieContainer cookie, WebHeaderCollection customHeaders, Action<long> getedLenght = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new Exception("CheckAddressContentSupportRange address is null or empty!");

            WebRequest request = LinkHelper.CreateRequest(address, authentication, 60000, proxy, cookie, int.MaxValue, customHeaders);
            LinkHelper.AddRange(request, 1);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.Headers["Accept-Ranges"] != null)
                {
                    var acceptRanges = response.Headers["Accept-Ranges"].ToLower().Contains("bytes");
                    if (acceptRanges)
                        return true;
                }
                if (response.Headers["Content-Range"] != null)
                {
                    var acceptRanges = response.Headers["Content-Range"].ToLower().Contains("bytes");
                    if (acceptRanges)
                        return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        //public LinkResumableStatus CheckMultiLinkInfo(List<MultiLinkAddress> items, List<ProxyInfo> proxies)
        //{
        //    long size = long.MinValue;
        //    foreach (var item in items)
        //    {
        //        if (!item.IsSelected)
        //            continue;
        //        try
        //        {
        //            long getedSize = GetSize(item, proxies);
        //            if (getedSize < 0)
        //            {
        //                return LinkaddressCheckMode.UnknownFileSize;
        //            }
        //            else if (size != long.MinValue && size != getedSize)
        //            {
        //                return LinkaddressCheckMode.False;
        //            }
        //            size = getedSize;
        //        }
        //        catch
        //        {
        //            return LinkaddressCheckMode.Exception;
        //        }
        //    }
        //    return LinkaddressCheckMode.True;
        //}

        //long GetSize(MultiLinkAddress address, List<ProxyInfo> items)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address.Address);
        //    request.KeepAlive = true;
        //    request.Proxy = ProxyInfo.GetProxyByID(items, address.ProxyID);
        //    request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
        //    request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    response.Close();
        //    return response.ContentLength;
        //}
    }
}
