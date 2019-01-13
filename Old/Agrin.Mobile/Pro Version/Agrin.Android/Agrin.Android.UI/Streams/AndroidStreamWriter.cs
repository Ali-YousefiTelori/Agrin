using Agrin.IO.Streams;
using Java.IO;
using Java.Nio.Channels;
using System.Linq;
using System;

namespace Agrin.Streams
{
    public class AndroidStreamWriter : IStreamWriter
    {
        public FileChannel CurrentStream { get; set; }
        public OutputStream disposeStrream { get; set; }
        public FileDescriptor FileDescriptor { get; set; }
        public Android.OS.ParcelFileDescriptor ParcelFileDescriptor { get; set; }
        public AndroidStreamWriter(FileOutputStream currentStream)
        {
            CurrentStream = currentStream.Channel;
            disposeStrream = currentStream;
        }

        public long Length
        {
            get { return CurrentStream.Size(); }
        }

        public long Position
        {
            get { return CurrentStream.Position(); }
            set { CurrentStream.Position(value); }
        }

        public void SetLength(long lenght)
        {

        }

        public long Seek(long posigion, System.IO.SeekOrigin seek)
        {
            return CurrentStream.Position(posigion).Position();
        }

        public void Write(byte[] bytes, int offest, int count)
        {
            if (count == 0)
                return;
            //Java.Nio.ByteBuffer buf = Java.Nio.ByteBuffer.AllocateDirect(bytes.Length);
            //if (count < bytes.Length)
            //    bytes = bytes.ToList().GetRange(0, count).ToArray();
            //buf.Put(bytes);
            //CurrentStream.Write(buf);
            disposeStrream.Write(bytes, offest, count);
            disposeStrream.Flush();
        }

        public void Dispose()
        {
            try
            {
                CurrentStream.Dispose();
                disposeStrream.Dispose();
                FileDescriptor = null;
            }
            catch
            {

            }
        }


        public System.IO.Stream GetStream()
        {
            return null;
        }

        public int Read(byte[] bytes, int offest, int count)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            disposeStrream.Flush();
        }
    }
}