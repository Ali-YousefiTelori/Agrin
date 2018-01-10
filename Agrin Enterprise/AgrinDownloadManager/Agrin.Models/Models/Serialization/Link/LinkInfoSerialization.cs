using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models.Serialization.Link
{
    public class LinkInfoSerialization
    {
        [BsonId]
        public int Id { get; set; }
        public bool IsComplete { get; set; }
        public bool IsError { get; set; }
        public long Size { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastDownloadedDateTime { get; set; }

        public List<LinkRequestSerialization> Connections { get; set; }

        public LinkInfoPathSerialization PathInfo { get; set; }
        public LinkInfoPropertiesSerialization Properties { get; set; }
        public LinkInfoDownloadSerialization LinkInfoDownloadCore { get; set; }
        public LinkInfoManagementSerialization LinkInfoManagementCore { get; set; }
    }
}
