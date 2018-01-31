using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models.Serialization.Link
{
    public class LinkRequestSerialization
    {
        [BsonId]
        public int Id { get; set; }
        public int LinkId { get; set; }
        public long StartPosition { get; set; }
        public long EndPosition { get; set; }
        public int BufferRead { get; set; }
        public int LimitPerSecound { get; set; }
        public long DownloadedSize { get; set; }
    }
}
