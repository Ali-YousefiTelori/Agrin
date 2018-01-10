using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Agrin.LinkExtractor
{
    internal static class HttpHelper
    {
        public static string DownloadString(string url)
        {
            url = url.Replace("http://","https://");
            var _request = (HttpWebRequest)HttpWebRequest.Create(url);
            _request.AllowAutoRedirect = true;
            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            var response = _request.GetResponse();
            //System.Threading.Tasks.Task<WebResponse> task = System.Threading.Tasks.Task.Factory.FromAsync(
            //    request.BeginGetResponse,
            //    asyncResult => request.EndGetResponse(asyncResult),
            //    null);

            //return task.ContinueWith(t => ReadStreamFromResponse(t.Result)).Result;
            return ReadStreamFromResponse(response);

        }

        public static string HtmlDecode(string value)
        {
            return Agrin.IO.Strings.Decodings.HtmlDecoding(value);
        }

        public static IDictionary<string, string> ParseQueryString(string s)
        {
            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }

            var dictionary = new Dictionary<string, string>();

            foreach (string vp in Regex.Split(s, "&"))
            {
                string[] strings = Regex.Split(vp, "=");
                dictionary.Add(strings[0], strings.Length == 2 ? UrlDecode(strings[1]) : string.Empty);
            }

            return dictionary;
        }

        public static string ReplaceQueryStringParameter(string currentPageUrl, string paramToReplace, string newValue)
        {
            var query = ParseQueryString(currentPageUrl);

            query[paramToReplace] = newValue;

            var resultQuery = new StringBuilder();
            bool isFirst = true;

            foreach (KeyValuePair<string, string> pair in query)
            {
                if (!isFirst)
                {
                    resultQuery.Append("&");
                }

                resultQuery.Append(pair.Key);
                resultQuery.Append("=");
                resultQuery.Append(pair.Value);

                isFirst = false;
            }

            var uriBuilder = new UriBuilder(currentPageUrl)
            {
                Query = resultQuery.ToString()
            };

            return uriBuilder.ToString();
        }

        public static string UrlDecode(string url)
        {
            return Agrin.IO.Strings.Decodings.UrlDecode(url);
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(responseStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}