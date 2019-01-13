using Agrin.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Streams
{
    public static class StreamCross
    {
        public static Func<string, FileMode, FileAccess, IStreamWriter> OpenFile { get; set; } = (fileName, mode, access) => throw new NotImplementedException();
    }
}
