using Agrin.Download.CoreModels.Link;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web
{
    /// <summary>
    /// link helper extensions
    /// </summary>
    public static class LinkHelper
    {
        /// <summary>
        /// create a request by default datas
        /// </summary>
        /// <param name="address"></param>
        /// <param name="authentication"></param>
        /// <param name="timeOut"></param>
        /// <param name="proxy"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="connectionLimit"></param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>
        public static WebRequest CreateRequest(string address, string authentication, int timeOut, IWebProxy proxy, CookieContainer cookieContainer,
            int connectionLimit, WebHeaderCollection customHeaders)
        {
            var _request = WebRequest.Create(address);
            bool isFTP = false;
            if (address.ToLower().StartsWith("ftp://"))
                isFTP = true;
            if (!isFTP)
            {
                ((HttpWebRequest)_request).CookieContainer = new CookieContainer();
                (((HttpWebRequest)_request).CookieContainer).Add(new Cookie("allow", "yes", "", _request.RequestUri.Host));
            }

            _request.Timeout = timeOut;// 60000;
            _request.SetAllowAutoRedirect(true);
            _request.Proxy = proxy;

            _request.SetConnectionLimit(connectionLimit);
            if (cookieContainer != null)
                _request.SetCookieContainer(cookieContainer);

            if (!string.IsNullOrEmpty(authentication))
                _request.Headers.Add(authentication);
            _request.InitializeCustomHeaders(customHeaders);
            return _request;
        }

        /// <summary>
        /// set connection limit of we request
        /// </summary>
        /// <param name="_request"></param>
        /// <param name="value"></param>
        public static void SetConnectionLimit(this WebRequest _request, int value)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).ServicePoint.ConnectionLimit = value;
            else if (_request is FtpWebRequest)
                ((FtpWebRequest)_request).ServicePoint.ConnectionLimit = value;
        }

        /// <summary>
        /// set cookie for request
        /// </summary>
        /// <param name="_request"></param>
        /// <param name="value"></param>
        public static void SetCookieContainer(this WebRequest _request, CookieContainer value)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).CookieContainer = value;
        }

        public static void SetExpect100Continue(this WebRequest _request, bool value)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).ServicePoint.Expect100Continue = value;
            else if (_request is FtpWebRequest)
                ((FtpWebRequest)_request).ServicePoint.Expect100Continue = value;
        }

        public static Uri GetRequestAddress(this WebRequest _request)
        {
            if (_request is HttpWebRequest)
                return ((HttpWebRequest)_request).Address;
            else if (_request is FtpWebRequest)
                return ((FtpWebRequest)_request).RequestUri;
            return null;
        }


        public static void AddRange(this WebRequest _request, long range)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).AddRange(range);
            else if (_request is FtpWebRequest)
                ((FtpWebRequest)_request).ContentOffset = range;
        }

        public static void SetAllowAutoRedirect(this WebRequest _request, bool value)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).AllowAutoRedirect = value;
        }

        public static List<LinkInfoRequestCore> SortByPosition(IEnumerable<LinkInfoRequestCore> connections)
        {
            List<LinkInfoRequestCore> col = new List<LinkInfoRequestCore>(connections);
            int n = connections.Count() - 1;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i; j++)
                {
                    if (col[j].StartPosition > col[j + 1].StartPosition)
                    {
                        LinkInfoRequestCore temp = col[j];
                        col[j] = col[j + 1];
                        col[j + 1] = temp;
                    }
                }
            }
            return col;
        }
    }
}
