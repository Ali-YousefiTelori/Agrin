using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Network.Models
{
    [Serializable]
    public class ExtensionInfo
    {
        string _Extension;
        bool _IsEnabled = true;

        public string Extension
        {
            get
            {
                return _Extension;
            }
            set
            {
                _Extension = value;
            }
        }

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
