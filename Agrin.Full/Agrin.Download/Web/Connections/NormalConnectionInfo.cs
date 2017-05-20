using Agrin.Download.Web.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web.Connections
{
    public class NormalConnectionInfo : AConnectionInfo
    {
        public NormalConnectionInfo(string address, LinkWebRequest parentLinkWebRequest)
        {
            ParentLinkWebRequest = parentLinkWebRequest;
            ParentLinkWebRequest.UriDownload = new Uri(address);
        }
    }
}
