using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Streams
{
    public class StreamWriter : IStreamWriter
    {
        public Stream CurrentStream { get; set; }

        public StreamWriter(Stream currentStream)
        {
            CurrentStream = currentStream;
        }

        public long Length
        {
            get { return CurrentStream.Length; }
        }

        public long Position
        {
            get { return CurrentStream.Position; }
            set { CurrentStream.Position = value; }
        }

        public void SetLength(long lenght)
        {
            CurrentStream.SetLength(lenght);
        }

        public long Seek(long posigion, System.IO.SeekOrigin seek)
        {
            return CurrentStream.Seek(posigion, seek);
        }

        public void Write(byte[] bytes, int offest, int count)
        {
            CurrentStream.Write(bytes, offest, count);
        }

        public int Read(byte[] bytes, int offest, int count)
        {
           return CurrentStream.Read(bytes, offest, count);
        }

        public void Dispose()
        {
            CurrentStream.Dispose();
        }


        public Stream GetStream()
        {
            return CurrentStream;
        }

        public void Flush()
        {
            CurrentStream.Flush();
        }
    }
    public class NormalStream : Stream, IStreamWriter
    {
        public IStreamWriter CurrentStream { get; set; }

        public NormalStream(IStreamWriter currentStream)
        {
            CurrentStream = currentStream;
        }
        

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return CurrentStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return CurrentStream.Position;
            }

            set
            {
                CurrentStream.Position = value;
            }
        }

        public override void SetLength(long lenght)
        {
            CurrentStream.SetLength(lenght);
        }

        public override long Seek(long posigion, System.IO.SeekOrigin seek)
        {
            return CurrentStream.Seek(posigion, seek);
        }

        public override void Write(byte[] bytes, int offest, int count)
        {
            CurrentStream.Write(bytes, offest, count);
        }

        public override int Read(byte[] bytes, int offest, int count)
        {
            return CurrentStream.Read(bytes, offest, count);
        }

        public new void Dispose()
        {
            CurrentStream.Dispose();
        }
        
        public override void Flush()
        {
            CurrentStream.Flush();
        }
    }
}
