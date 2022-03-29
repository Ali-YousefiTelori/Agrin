using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Models
{
    public class FileStatus : DocumantInfo
    {
        public string Status { get; set; }
        public string Progress { get; set; }
        public string Url { get; set; }
        public string ID { get; set; }
        
        public string Oid { get; set; }
        public string Store { get; set; }
        public string Size { get; set; }

        string _Name = "";
        public override string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
    }
}
