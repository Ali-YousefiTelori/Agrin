using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web.Requests
{
    public enum RequestExchangerType
    {
        NetFrameworkWebRequest
    }

    public abstract class WebRequestExchangerBase : IDisposable
    {
        public abstract Stream ResponseStream { get; set; }
        
        public static WebRequestExchangerBase Create(RequestExchangerType type)
        {
            if (type == RequestExchangerType.NetFrameworkWebRequest)
                return new WebRequestExchanger();
            throw new Exception("WebRequestExchangerBase type not support");
        }

        public long ContentLength { get; set; } = -2;
        public Uri ResponseUri { get; set; }
        public WebHeaderCollection RequestHeaders { get; set; }
        public WebHeaderCollection ResponseHeaders { get; set; }

        public abstract void CreateRequest(string address, string authentication, int timeOut, IWebProxy proxy, CookieContainer cookieContainer,
            int connectionLimit, WebHeaderCollection customHeaders);

        public abstract void SetExpect100Continue(bool value);
        public abstract void AddRange(long range);
        public abstract void GetResponse();
        public abstract void GetResponseStream();

        public abstract void Dispose();

    }
}
