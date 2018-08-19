using Agrin.Log;
using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// save path of setting
        /// </summary>
        public static string SettingSavePath { get; set; }
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

        /// <summary>
        /// load settings from
        /// </summary>
        /// <param name="directoryPath">from path</param>
        public static void Load(string directoryPath)
        {
            try
            {
                SettingSavePath = directoryPath;
                var path = Path.Combine(directoryPath, "settings.json");
                if (File.Exists(path))
                    Current = Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationSettingsInfo>(File.ReadAllText(path, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "Load Setting");
            }
        }

        /// <summary>
        /// save settings
        /// </summary>
        public static void Save()
        {
            File.WriteAllText(Path.Combine(SettingSavePath, "settings.json"), Newtonsoft.Json.JsonConvert.SerializeObject(Current), Encoding.UTF8);
        }
    }
}
