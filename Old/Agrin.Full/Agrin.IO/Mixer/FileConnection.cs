using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.IO.Mixer
{
    public enum IsCompleteEnum
    {
        None,
        Comeplete,
        Reverce
    }

    [Serializable]
    public class FileConnection
    {
        public string Path { get; set; }
        public long Lenght { get; set; }
        public IsCompleteEnum IsComplete { get; set; }
        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(Path);
            }
        }
    }
}
