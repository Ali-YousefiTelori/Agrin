using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace System
{
    public static class HttpWebRequestExtensions
    {
        static string[] RestrictedHeaders = new string[] {
            "Accept",
            "Connection",
            "Content-Length",
            "Content-Type",
            "Date",
            "Expect",
            "Host",
            "If-Modified-Since",
            "Keep-Alive",
            "Proxy-Connection",
            "Range",
            "Referer",
            "Transfer-Encoding",
            "User-Agent"
    };

        static Dictionary<string, PropertyInfo> HeaderProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        static HttpWebRequestExtensions()
        {
            Type type = typeof(HttpWebRequest);
            foreach (string header in RestrictedHeaders)
            {
                string propertyName = header.Replace("-", "");
                PropertyInfo headerProperty = type.GetProperty(propertyName);
                HeaderProperties[header] = headerProperty;
            }
        }

        public static void SetRawHeader(this HttpWebRequest request, string name, string value)
        {
            if (HeaderProperties.ContainsKey(name))
            {
                if (name == "Connection")
                {
                    if (value == "Keep-Alive")
                        request.KeepAlive = true;
                    else
                        request.KeepAlive = false;
                    return;
                }
                PropertyInfo property = HeaderProperties[name];
                if (property == null)
                    return;
                if (property.PropertyType == typeof(DateTime))
                    property.SetValue(request, DateTime.Parse(value), null);
                else if (property.PropertyType == typeof(bool))
                    property.SetValue(request, bool.Parse(value), null);
                else if (property.PropertyType == typeof(long))
                    property.SetValue(request, long.Parse(value), null);
                else
                    property.SetValue(request, value, null);
            }
            else
            {
                request.Headers[name] = value;
            }
        }

        public static void InitializeCustomHeaders(this WebRequest _request, WebHeaderCollection customHeaders)
        {
            if (!(_request is HttpWebRequest))
                return;
            HttpWebRequest request = (HttpWebRequest)_request;
            if (customHeaders == null || customHeaders.Count == 0)
            {
                request.KeepAlive = true;
                request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                return;
            }
            foreach (string key in customHeaders.AllKeys)
            {
                request.SetRawHeader(key, customHeaders[key]);
            }
        }
    }

}
