using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.ViewModels.Helpers
{
    public abstract class DeviceHelper
    {
        public static DeviceHelper Current { get; set; }

        public abstract string ApplicationVersion { get; }

        public abstract string OSVersion { get; }

        public abstract bool IsRootMode();

        public abstract void SetMobileDataEnabled(bool enabled);

        public abstract void SetMobileDataEnabledNormal(bool enabled);

        public abstract void SetMobileDataEnabledRoot( bool mobileDataEnabled);

        public abstract void SetWifiEnable(bool enabled);
    }
}
