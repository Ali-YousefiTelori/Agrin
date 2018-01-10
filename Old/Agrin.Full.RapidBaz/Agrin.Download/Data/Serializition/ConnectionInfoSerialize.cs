using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class ConnectionInfoSerialize
    {
        public int ConnectionId { get; set; }
        public double StartPosition { get; set; }
        public double EndPosition { get; set; }
        public double Length { get; set; }
        public Uri UriDownload { get; set; }
        public CookieContainer RequestCookieContainer { get; set; }
    }
}
