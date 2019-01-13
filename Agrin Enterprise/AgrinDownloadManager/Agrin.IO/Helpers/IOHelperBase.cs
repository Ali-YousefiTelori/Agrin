using Agrin.IO.Streams;
using Agrin.Log;
using System;
using System.IO;
using UltraStreamGo;

namespace Agrin.IO.Helpers
{
    public class IOHelperBase : CrossFileInfo
    {
        static IOHelperBase _Current;
        public new static IOHelperBase Current
        {
            get
            {
                return _Current;
            }
            set
            {
                CrossFileInfo.Current = _Current = value;
            }
        }
        
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
