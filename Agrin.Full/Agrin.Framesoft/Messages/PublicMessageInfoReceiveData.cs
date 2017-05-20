using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft.Messages
{
    [Serializable]
    public class PublicMessageInfoReceiveData : ILinkMessage
    {
        public int MessageID { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }
}
