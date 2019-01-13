using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.IO.Spliter
{
    public static class FileSpliter
    {
        public static void SplitFile(string fileName, string saveToDirectory, string startFileName, int splitelength)
        {
            using (var stream = IOHelperBase.Current.OpenFileStreamForRead(fileName, System.IO.FileMode.Open))
            {
                var oneFileSize = stream.Length / splitelength;
                var lastFileSize = stream.Length % splitelength;
                for (int i = 0; i <= oneFileSize; i++)
                {
                    var readLen = 0;
                    if (i == oneFileSize)
                        splitelength = (int)lastFileSize;
                    string splitFileName = System.IO.Path.GetFileNameWithoutExtension(startFileName) + i + MPath.GetFileExtention(startFileName);
                    using (var writeStream = IOHelperBase.Current.OpenFileStreamForWrite(saveToDirectory, System.IO.FileMode.Create, fileName: splitFileName))
                    {
                        while (readLen < splitelength)
                        {
                            var blockSize = 1024 * 1024 * 2;
                            if (blockSize + readLen > splitelength)
                                blockSize = splitelength - readLen;

                            var readbytes = new byte[blockSize];
                            var readCount = stream.Read(readbytes, 0, readbytes.Length);
                            writeStream.Write(readbytes, 0, readCount);
                            readLen += readCount;
                        }
                    }
                }
            }
        }
    }
}
