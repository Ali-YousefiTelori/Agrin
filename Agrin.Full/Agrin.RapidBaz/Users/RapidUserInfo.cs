using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Users
{
    public class RapidUserInfo
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonProperty("spc_used")]
        public string UploadSize { get; set; }
        [JsonProperty("spc_total")]
        public string TotalSize { get; set; }
    }
}
