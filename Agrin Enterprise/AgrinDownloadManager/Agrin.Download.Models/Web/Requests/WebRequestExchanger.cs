using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web.Requests
{
    public class WebRequestExchanger : WebRequestExchangerBase
    {
        public WebRequestExchanger()
        {

        }

        private WebRequest _request;
        private WebResponse _response;

        public override Stream ResponseStream { get; set; }

        public override void SetExpect100Continue(bool value)
        {
            _request.SetExpect100Continue(false);
        }

        public override void CreateRequest(string address, string authentication, int timeOut, IWebProxy proxy, CookieContainer cookieContainer, int connectionLimit, WebHeaderCollection customHeaders)
        {
            _request = LinkHelper.CreateRequest(address, authentication, timeOut, proxy, cookieContainer, int.MaxValue, customHeaders);
        }

        public override void AddRange(long range)
        {
            _request.AddRange(range);
        }

        public override void GetResponse()
        {
            _response = _request.GetResponse();
            ResponseUri = _response.ResponseUri;
            ContentLength = _response.ContentLength;
            ResponseHeaders = _response.Headers;
        }

        public override void GetResponseStream()
        {
            ResponseStream = _response.GetResponseStream();
        }

        public override void Dispose()
        {
            _response?.Close();
            ResponseStream?.Dispose();
        }
    }
}
