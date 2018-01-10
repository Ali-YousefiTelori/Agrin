using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Network.Models
{
    /// <summary>
    /// تنظیمات پروکسی فیدلر
    /// </summary>
    [Serializable]
    public class NetworkProxySettings
    {
        public static Action ChangedAction { get; set; }

        public static NetworkProxySettings Current { get; set; }

        public List<ApplicationInfo> SupportApps { get; set; }

        public List<ExtensionInfo> SupportFileExtensions { get; set; }

        public static bool ExistProcesses(string processName)
        {
            return (from x in Current.SupportApps where x.IsEnabled && x.ProcessName.ToLower() == processName.ToLower() select x).FirstOrDefault() != null;
        }

        public static bool IsSupportExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;
            return (from x in Current.SupportFileExtensions where x.IsEnabled && (x.Extension == ".*" || x.Extension.ToLower() == extension.ToLower()) select x).FirstOrDefault() != null;
        }

        public static void InitializeDefaultSettings()
        {
            Current.SupportApps = new List<ApplicationInfo>();
            Current.SupportFileExtensions = new List<ExtensionInfo>();

            Current.SupportApps.Add(new ApplicationInfo() { ProcessName = "firefox", IsEnabled = true });
            Current.SupportApps.Add(new ApplicationInfo() { ProcessName = "chrome", IsEnabled = true });

            Current.SupportFileExtensions = new List<ExtensionInfo>();
            Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".mp3", IsEnabled = true });
            Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".apk", IsEnabled = true });
            Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".exe", IsEnabled = true });
            Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".msi", IsEnabled = true });
            //NetworkProxySettings.Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".png", IsEnabled = true });
            //NetworkProxySettings.Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".jpg", IsEnabled = true });
            Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".rar", IsEnabled = true });
            Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".zip", IsEnabled = true });
            Current.SupportFileExtensions.Add(new ExtensionInfo() { Extension = ".mkv", IsEnabled = true });
        }
    }
}
