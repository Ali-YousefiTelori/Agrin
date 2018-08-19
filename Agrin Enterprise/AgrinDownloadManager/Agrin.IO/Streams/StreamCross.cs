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
        //public Guid Guid { get; set; }
        public static Func<string, FileMode, FileAccess, IStreamWriter> OpenFile { get; set; } = (fileName, mode, access) => throw new NotImplementedException();
        //{
        //    Guid = Guid.NewGuid();
        //    AutoLogger.LogText($"StreamWriter new {Guid} {path} {CurrentStream.Length}");
        //}


        //bool _isDispose = false;
        //public void Dispose()
        //{
        //    if (_isDispose)
        //        return;
        //    _isDispose = true;
        //    try
        //    {
        //        CurrentStream.Flush();
        //    }
        //    catch (Exception ex)
        //    {
        //        AutoLogger.LogError(ex, $"StreamWriter dispose flush exception {Guid} : ");
        //    }
        //    try
        //    {
        //        AutoLogger.LogText($"StreamWriter dispose {Guid}");
        //        CurrentStream.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        AutoLogger.LogError(ex, $"StreamWriter dispose exception {Guid} : ");
        //    }
        //}

        //public Stream GetStream()
        //{
        //    return CurrentStream;
        //}

        //public void Flush()
        //{
        //    CurrentStream.Flush();
        //}
    }
}
