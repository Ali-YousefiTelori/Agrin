using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models.Settings
{
    /// <summary>
    /// setting of application
    /// </summary>
    public class ApplicationSettingsInfo
    {
        volatile ApplicationPathSettingsInfo _PathSettings = null;
        volatile ApplicationSpeedSettingsInfo _SpeedSettingInfo = null;

        volatile static ApplicationSettingsInfo _Current = null;
        /// <summary>
        /// current setting of application
        /// </summary>
        public static ApplicationSettingsInfo Current
        {
            get
            {
                if (_Current == null)
                    _Current = new ApplicationSettingsInfo();
                return _Current;
            }
            set => _Current = value;
        }

        /// <summary>
        /// setting of path and addresses
        /// </summary>
        public ApplicationPathSettingsInfo PathSettings
        {
            get
            {
                if (_PathSettings == null)
                    _PathSettings = new ApplicationPathSettingsInfo();
                return _PathSettings;
            }
            set
            {
                _PathSettings = value;
            }
        }
        /// <summary>
        /// seeting of speed download manager
        /// </summary>
        public ApplicationSpeedSettingsInfo SpeedSettingInfo
        {
            get
            {
                if (_SpeedSettingInfo == null)
                    _SpeedSettingInfo = new ApplicationSpeedSettingsInfo();
                return _SpeedSettingInfo;
            }
            set
            {
                _SpeedSettingInfo = value;
            }
        }
    }
}
