using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft
{
    public class UserFileInfoData
    {
        [JsonIgnore]
        public Action ChangedValuesAction { get; set; }

        public int ID { get; set; }
        public long Size { get; set; }
        public long DownloadedSize { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Link { get; set; }
        public byte Status { get; set; }
        public int FormatCode { get; set; }
        public Guid FileGuid { get; set; }
        public string FileName { get; set; }
    }
}
