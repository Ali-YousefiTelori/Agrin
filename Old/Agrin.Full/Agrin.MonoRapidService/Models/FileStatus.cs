using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidService.Models
{
    public class FileStatus
    {
        public string Status { get; set; }
        public string Progress { get; set; }
        public string Url { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Oid { get; set; }
        public string Store { get; set; }
        public string Size { get; set; }
    }
}
