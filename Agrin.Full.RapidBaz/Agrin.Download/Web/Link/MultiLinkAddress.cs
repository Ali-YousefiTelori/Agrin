using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web.Link
{
    [Serializable]
    public class MultiLinkAddress
    {
        public int ProxyID { get; set; }
        public string Address { get; set; }
        public bool IsSelected { get; set; }
        public bool IsApplicationAdded { get; set; }//if user added this link value is false
    }
}
