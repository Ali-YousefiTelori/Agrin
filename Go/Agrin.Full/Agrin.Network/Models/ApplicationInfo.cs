using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Network.Models
{
    /// <summary>
    /// یک نرم افزار که پروکسی فیدلر ساپورت میکند
    /// </summary>
    [Serializable]
    public class ApplicationInfo
    {
        string _ApplicationPath;
        string _ProcessName;
        bool _IsEnabled = true;

        /// <summary>
        /// محل ذخیره ی نرم افزار اختیاری
        /// </summary>
        public string ApplicationPath
        {
            get
            {
                return _ApplicationPath;
            }
            set
            {
                _ApplicationPath = value;
            }
        }
        /// <summary>
        /// نام پروسه وقتی توی حافظه قرار میگیرد
        /// </summary>
        public string ProcessName
        {
            get
            {
                return _ProcessName;
            }
            set
            {
                _ProcessName = value;
            }
        }

        /// <summary>
        /// فعال باشد
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
                NetworkProxySettings.ChangedAction?.Invoke();
            }
        }
    }
}
