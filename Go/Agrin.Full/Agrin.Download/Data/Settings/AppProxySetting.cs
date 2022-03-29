using Agrin.Download.Web.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class AppProxySetting
    {
        public List<ProxyInfo> Items { get; set; }

        public ProxyInfo GetFirstActiveProxy()
        {
            return (from x in Items.ToList() where x.IsSelected select x).FirstOrDefault();
        }
    }
}
