using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models.Serialization.Link
{
    public class LinkInfoDownloadSerialization
    {
        public ResumeCapabilityEnum ResumeCapability { get; set; }
        public byte? ConcurrentConnectionCount { get; set; }
    }
}
