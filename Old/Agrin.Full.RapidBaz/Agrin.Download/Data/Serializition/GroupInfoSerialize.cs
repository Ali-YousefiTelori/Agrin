using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class GroupInfoSerialize
    {
        public string Name { get; set; }
        public string SavePath { get; set; }
        public List<string> Extentions { get; set; }
        public long Id { get; set; }
        public bool IsExpanded { get; set; }
    }
}
