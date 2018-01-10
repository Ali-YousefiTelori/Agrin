using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class ApplicationOSSetting
    {
        public string Application { get; set; }
        public string ApplicationVersion { get; set; }
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public string ApplicationGuid { get; set; }
    }
}
