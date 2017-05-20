using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class AppSpeedSetting
    {
        public int ConnectionCount { get; set; }
        public int BufferSize { get; set; }
        public int SpeedSize { get; set; }
        public bool IsLimit { get; set; }
    }
}
