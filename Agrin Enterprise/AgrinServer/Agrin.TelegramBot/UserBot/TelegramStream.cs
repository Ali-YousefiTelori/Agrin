using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Upload;
using TLSharp.Core;

namespace Agrin.TelegramBot.UserBot
{
    public class TelegramStream : Stream
    {
        public TelegramStream(TelegramClient client, TLAbsInputFileLocation location, long length)
        {
            _length = length;
            _client = client;
            _location = location;
        }

        readonly long _length = 0;
        long _position = 0;
        public readonly int filePart = 512 * 1024;
        TLFile _resFile = null;
        readonly TelegramClient _client = null;
        readonly TLAbsInputFileLocation _location = null;
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
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                return _length;
            }
        }

        public override long Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        public override void Flush()
        {

        }

        public static SemaphoreSlim lockobj = new SemaphoreSlim(1);
        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                lockobj.Wait();
                TryAgain:
                try
                {
                    Thread.Sleep(400);
                    Console.WriteLine($"reading... {Position} {buffer.Length} {filePart}");
                    _resFile = _client.GetFile(_location, filePart, (int)Position).Result;
                    var resultBytes = _resFile.Bytes;
                    for (int i = 0; i < resultBytes.Length; i++)
                    {
                        if (i >= buffer.Length)
                            break;
                        buffer[i] = resultBytes[i];
                    }
                    Console.WriteLine($"readed {resultBytes.Length} {Position} {buffer.Length}");

                    Position += resultBytes.Length;
                    return resultBytes.Length;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception to read stream :" + ex.ToString());
                    Thread.Sleep(1000);
                    goto TryAgain;
                }

            }
            finally
            {
                lockobj.Release();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
