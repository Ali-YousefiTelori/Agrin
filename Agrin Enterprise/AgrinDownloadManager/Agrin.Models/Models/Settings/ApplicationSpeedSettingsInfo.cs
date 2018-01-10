using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models.Settings
{
    public class ApplicationSpeedSettingsInfo
    {
        private volatile byte maximumConnectionCount = 4;

        public byte MaximumConnectionCount { get => maximumConnectionCount; set => maximumConnectionCount = value; }
    }
}
