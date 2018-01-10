using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Framesoft.Services.Models
{
    public class ClientInfo
    {
        public string SessionKey { get; set; }
        public List<string> SessionIds { get; set; }
        public UserInfo User { get; set; }
        public IClientChannel ClientChannel { get; set; }
    }
}