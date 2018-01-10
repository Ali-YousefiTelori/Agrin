using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class FramesoftSetting
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool ConfirmUserPermissions { get; set; }
        public List<string> PurchaseProductIds { get; set; }
        public List<string> CompleteFileAddress { get; set; }

    }
}
