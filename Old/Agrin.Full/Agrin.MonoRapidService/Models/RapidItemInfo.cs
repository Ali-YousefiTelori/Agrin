using Agrin.IO.File;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidService.Models
{
    public class RapidItemInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Size { get; set; }
        public string Status { get; set; }
        public string StatusString
        {
            get
            {
                return Agrin.RapidService.Web.WebManager.GetStatusString(Status);
            }
        }
        public string Progress { get; set; }
        public string Url { get; set; }

        public string FileType
        {
            get
            {
                return FileProperties.GetFileType(Name);
            }
        }
    }
}
