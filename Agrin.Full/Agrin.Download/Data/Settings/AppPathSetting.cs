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
        /// <summary>
        /// محل ذخیره ی داده های موقت
        /// </summary>
        public string SaveDataPath { get; set; }
        public string RepairSaveDataPath { get; set; }
        public string SecurityPath { get; set; }
    }
}
