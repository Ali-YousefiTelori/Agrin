using Agrin.IO.Streams;
using Agrin.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Helper
{
    public class IOHelperBase : CrossFileInfo
    {
        public new static IOHelperBase Current { get; set; }

        public virtual IStreamWriter OpenFileStreamForRead(string fileName, FileMode fileMode, FileAccess fileAccess = FileAccess.ReadWrite)
        {
            IStreamWriter retStream = null;
            if (retStream == null)
            {
                retStream = StreamCross.OpenFile(fileName, fileMode, fileAccess);
                AutoLogger.LogText($"OpenFileStreamForRead new {fileName}");
            }
            return retStream;
        }

        public virtual IStreamWriter OpenFileStreamForWrite(string fileAddress, FileMode fileMode, FileAccess fileAccess = FileAccess.ReadWrite, string fileName = null, Action<string> newSecurityFileName = null, object data = null)
        {
            IStreamWriter retStream = null;

            if (retStream == null)
            {
                string path = fileAddress;
                if (!string.IsNullOrEmpty(fileName))
                    path = Path.Combine(fileAddress, fileName);
                retStream = StreamCross.OpenFile(path, fileMode, fileAccess);
            }

            return retStream;
        }
    }
}
