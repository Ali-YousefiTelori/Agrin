using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Models
{
    public class FolderInfo : DocumantInfo
    {
        public string ID { get; set; }
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
        public int Count { get; set; }
        public long Size { get; set; }
        public string Link { get; set; }
    }
}
