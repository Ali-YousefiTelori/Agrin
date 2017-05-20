using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class LinkInfoSerialize
    {
        public LinkInfoDownloadingPropertySerialize DownloadingProperty { get; set; }
        public LinkInfoPropertiesSerialize Properties { get; set; }
        public LinkInfoPathSerialize PathInfo { get; set; }
        public LinkInfoManagementSerialize Management { get; set; }
        public List<ConnectionInfoSerialize> Connections { get; set; }
    }
}
