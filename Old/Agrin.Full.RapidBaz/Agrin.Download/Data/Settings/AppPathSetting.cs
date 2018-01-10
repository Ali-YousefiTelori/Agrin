using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class AppPathSetting
    {
        public string DownloadsPath { get; set; }
        public string SaveDataPath { get; set; }
    }
}
