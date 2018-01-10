using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models.Serialization.Link
{
    public class LinkInfoPathSerialization
    {
        public string MainUriAddress { get; set; }

        public string AppDirectorySavePath { get; set; }
        public string UserDirectorySavePath { get; set; }
        public string SecurityDirectorySavePath { get; set; }

        public string AppFileName { get; set; }
        public string UserFileName { get; set; }
        public string SecurityFileName { get; set; }
        public string MixerSavePath { get; set; }
        public string SecurityMixerSavePath { get; set; }
        public string BackUpMixerSavePath { get; set; }
    }
}
