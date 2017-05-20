using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.File
{
    public class TestStream
    {
        public TestStream()
        {

        }

        public void CreateFile(int length)
        {
            using (var stream = IOHelper.OpenFileStreamForWrite("d:\\text.apk", FileMode.Create, FileAccess.ReadWrite))
            {
                for (int i = 0; i < length; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(i);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public void ReadFile(int length)
        {
            List<int> items = new List<int>();
            using (var stream = IOHelper.OpenFileStreamForRead("d:\\text.apk", FileMode.Open, FileAccess.ReadWrite))
            {
                while (stream.Position != stream.Length)
                {
                    byte[] bytes = new byte[4];
                    stream.Read(bytes, 0, 4);
                    items.Add(BitConverter.ToInt32(bytes, 0));
                }
            }
            for (int i = 0; i < length; i++)
            {
                var item = items[i];
                if (item != i)
                {
                    System.Diagnostics.Debug.Assert(false, "Read False");
                }
            }
        }

        public void WriteToStream()
        {

        }
    }
}
