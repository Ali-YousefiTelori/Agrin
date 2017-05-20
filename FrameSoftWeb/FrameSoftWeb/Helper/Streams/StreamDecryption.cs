using System.IO;

namespace Gita.Infrastructure.UI.IO
{
    public class StreamDecryption : Stream
    {
        Stream inner;
        byte mustno = 64;
        public StreamDecryption(Stream inner)
        {
            this.inner = inner;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = inner.Read(buffer, offset, count);
            for (int i = 0; i < result; i++)
            {
                buffer[i] -= mustno;
            }
            return result;
        }


        public override bool CanRead
        {
            get { return inner.CanRead; }
        }

        public override bool CanSeek
        {
            get { return inner.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return inner.CanWrite; }
        }

        public override void Flush()
        {
            inner.Flush();
        }

        public override long Length
        {
            get { return inner.Length; }
        }

        public override long Position
        {
            get
            {
                return inner.Position;
            }
            set
            {
                inner.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            inner.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            inner.Dispose();
        }
    }
}
