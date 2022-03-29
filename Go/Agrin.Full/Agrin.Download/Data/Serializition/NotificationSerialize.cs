using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class NotificationSerialize
    {
        public DateTime NotifyDateTime { get; set; }
        public bool IsRead { get; set; }
        public List<int> Items { get; set; }
        public NotificationMode Mode { get; set; }
    }
}
