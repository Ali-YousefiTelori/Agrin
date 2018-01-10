using AmarGiri.Foundation;
using Gita.DataBase.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmarGiri.Model
{
    public class BaseAmarDTO : QueryTable<BaseAmarDTO>, IBaseAmarDTO
    {
        public long ID { get; set; }
        public string Application { get; set; }
        public string ApplicationVersion { get; set; }
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public string ApplicationGuid { get; set; }
    }
}
