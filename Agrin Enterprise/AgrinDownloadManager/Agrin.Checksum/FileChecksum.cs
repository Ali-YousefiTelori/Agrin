using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Checksum
{
    public class CheckSumItem
    {
        public string Hash { get; set; }
        public long StartPosition { get; set; }
        public long EndPosition { get; set; }

        public long Size
        {
            get
            {
                return EndPosition - StartPosition;
            }
        }
        public string FileName { get; set; }
    }

    public class FileCheckSumData
    {
        public List<CheckSumItem> CheckSums { get; set; }
        public int ReadBufferCount { get; set; }
    }

    public static class FileChecksum
    {
        public static Action<long, long> ProgressAction { get; set; }
        public static byte[] GetBytesPerBuffer(Stream stream, int bufferCount)
        {
            List<byte> read = new List<byte>();
            int totalRead = bufferCount;
            while (stream.Position != stream.Length)
            {
                byte[] readBytes = new byte[totalRead];
                int readCount = stream.Read(readBytes, 0, totalRead);
                if (readCount == bufferCount)
                {
                    read.AddRange(readBytes);
                    break;
                }
                else
                {
                    totalRead -= readCount;
                    read.AddRange(readBytes.ToList().GetRange(0, readCount));
                    if (totalRead == 0)
                        break;
                }
            }
            return read.ToArray();
        }

        public static byte[] GetBytesPerBufferNet(Stream stream, int bufferCount, long len, long pos)
        {
            List<byte> read = new List<byte>();
            int totalRead = bufferCount;
            while (true)
            {
                byte[] readBytes = new byte[totalRead];
                int readCount = stream.Read(readBytes, 0, totalRead);
                if (readCount == bufferCount)
                {
                    read.AddRange(readBytes);
                    break;
                }
                else if (readCount == 0)
                {
                    if (read.Count + pos >= len)
                        break;
                }
                else
                {
                    totalRead -= readCount;
                    read.AddRange(readBytes.ToList().GetRange(0, readCount));
                    if (totalRead == 0)
                        break;
                }
            }
            return read.ToArray();
        }

        public static string GetMD5(byte[] data)
        {
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(data);
            string encoded = BitConverter.ToString(hash)
               // without dashes
               .Replace("-", string.Empty)
               // make lowercase
               .ToLower();
            return encoded;
        }

        public static FileCheckSumData GetFileCheckSum(Stream fileStream, int checkSize = 1024 * 1024)
        {
            FileCheckSumData data = new FileCheckSumData() { ReadBufferCount = checkSize };
            List<CheckSumItem> items = new List<CheckSumItem>();
            while (fileStream.Position != fileStream.Length)
            {
                CheckSumItem checkSum = new CheckSumItem();
                checkSum.StartPosition = fileStream.Position;
                var bytes = GetBytesPerBuffer(fileStream, checkSize);
                checkSum.EndPosition = fileStream.Position == fileStream.Length ? fileStream.Length : checkSum.StartPosition + checkSize;
                checkSum.Hash = GetMD5(bytes);
                items.Add(checkSum);
                if (ProgressAction != null)
                    ProgressAction(fileStream.Position, fileStream.Length);
            }
            data.CheckSums = items;
            return data;
        }

        public static byte[] GetListOfBytes(Stream stream, long position, int lenght)
        {
            stream.Seek(position, SeekOrigin.Begin);
            var bytes = GetBytesPerBuffer(stream, lenght);
            return bytes;
        }

        //public static void SaveToFile(FileCheckSumData data, string fileName)
        //{
        //    //SerializeStream.SaveSerializeStream(fileName, data);
        //}

        public static List<CheckSumItem> GetErrorsFromTwoCheckSum(FileCheckSumData downloadData, FileCheckSumData trueData)
        {
            List<CheckSumItem> items = new List<CheckSumItem>();
            if (trueData.CheckSums.Count != downloadData.CheckSums.Count)
            {
                throw new Exception("Not true");
            }
            for (int i = 0; i < trueData.CheckSums.Count; i++)
            {
                if (trueData.CheckSums[i].Hash != downloadData.CheckSums[i].Hash)
                {
                    if (items.Count > 0 && trueData.CheckSums[i].StartPosition == items.Last().EndPosition)
                    {
                        items.Last().EndPosition = trueData.CheckSums[i].EndPosition;
                    }
                    else
                    {
                        items.Add(trueData.CheckSums[i]);
                    }
                }
            }
            return items;
        }
    }
}
