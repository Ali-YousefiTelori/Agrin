using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web.Link
{
    [Serializable]
    public class ProxyInfo
    {
        public string FullAddress
        {
            get
            {
                if (Port == -1)
                    return ServerAddress;
                return ServerAddress + ":" + Port + (string.IsNullOrEmpty(UserName) ? "" : ":" + UserName);
            }
        }
        public int Id { get; set; }
        public string ServerAddress { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsUserPass { get; set; }
        public bool IsSelected { get; set; }
        public bool IsSystemProxy { get; set; }

        public IWebProxy GetProxy()
        {
            if (IsSystemProxy)
                return WebProxy.GetDefaultProxy();
            if (Port == -1)
                return null;
            if (IsUserPass)
                return new WebProxy(ServerAddress, Port) { Credentials = new NetworkCredential(UserName, Password), UseDefaultCredentials = false };
            return new WebProxy(ServerAddress, Port);
        }

        public static IWebProxy GetProxyByID(List<ProxyInfo> items, int id)
        {
            if (id == 0 || id == 1)
                return null;
            foreach (var item in items)
            {
                if (item.Id == id)
                    return item.GetProxy();
            }
            return null;
        }
    }
}
