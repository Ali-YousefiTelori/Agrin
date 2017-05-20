using Agrin.Download.Data.Serializition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data
{
    [Serializable]
    public class ApplicationNotificationSerialize
    {
        public List<NotificationSerialize> NotificationInfoes { get; set; }
    }
}
