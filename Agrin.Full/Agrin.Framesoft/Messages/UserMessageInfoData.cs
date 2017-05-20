using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft.Messages
{
    public enum LimitMessageEnum
    {
        All = 0,
        Windows = 1,
        Android = 2,
        Linux = 3,
        Mac = 4,
        IOS = 5,
        WindowsPhone = 6
    }

    [Serializable]
    public class UserMessageInfoData : ILinkMessage
    {
        public int MessageID { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
    }
}
