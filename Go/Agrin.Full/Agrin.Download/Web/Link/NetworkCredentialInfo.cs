using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web.Link
{
    [Serializable]
    public class NetworkCredentialInfo
    {
        public string ServerAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsUsed { get; set; }
    }
}
