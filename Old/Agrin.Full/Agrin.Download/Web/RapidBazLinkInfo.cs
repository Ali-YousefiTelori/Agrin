using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web
{
    [Serializable]
    public class RapidBazLinkInfo
    {
        string _QueueID;

        public string QueueID
        {
            get { return _QueueID; }
            set { _QueueID = value; }
        }

        bool _SendToQueueAfterComplete = false;

        public bool SendToQueueAfterComplete
        {
            get { return _SendToQueueAfterComplete; }
            set { _SendToQueueAfterComplete = value; }
        }
    }
}
