using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Streams
{
    public interface IStreamWriter : IDisposable
    {
        long Length { get; }
        long Position { get; set; }
        void SetLength(long lenght);

        long Seek(long posigion, System.IO.SeekOrigin seek);
        void Write(byte[] bytes, int offest, int count);
        int Read(byte[] bytes, int offest, int count);

        Stream GetStream();
        void Flush();
    }
}
