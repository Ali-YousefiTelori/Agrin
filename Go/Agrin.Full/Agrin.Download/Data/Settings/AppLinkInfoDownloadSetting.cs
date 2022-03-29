using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class AppLinkInfoDownloadSetting
    {
        public bool IsExtreme { get; set; }
        public bool IsEndDownloaded { get; set; }
        public int EndDownloadSelectedIndex { get; set; }
        public int TryException { get; set; }
        public bool IsShowBalloon { get; set; }
    }
}
