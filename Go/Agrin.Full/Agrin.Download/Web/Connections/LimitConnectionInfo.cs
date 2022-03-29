using Agrin.Download.Web.Link;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Download.Web.Connections
{
    public class LimitConnectionInfo : AConnectionInfo
    {
        public LimitConnectionInfo(Uri address, LinkWebRequest parentLinkWebRequest)
        {
            ParentLinkWebRequest = parentLinkWebRequest;
            ParentLinkWebRequest.UriDownload = address;
        }
    }
}
