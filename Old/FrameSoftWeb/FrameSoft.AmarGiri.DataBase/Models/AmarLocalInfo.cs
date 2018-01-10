using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class AmarLocalInfo
    {
        public AmarLocalInfo()
        {
            DateTime = DateTime.Now;
        }

        public string IPAddress { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public Guid ApplicationGuid { get; set; }

        public int IpId { get; set; }
        public int GuidID { get; set; }
        public DateTime DateTime { get; set; }
    }

}
