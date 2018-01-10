using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Agrin.IO.File
{
    [Serializable]
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
        [field: NonSerialized]
        public string FileName { get; set; }
    }

    [Serializable]
    public class FileCheckSumData
    {
        public List<CheckSumItem> CheckSums { get; set; }
        public int ReadBufferCount { get; set; }
    }

    public static class FileCheckSum
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

        public static FileCheckSumData GetFileCheckSum(string fileName, int checkSize = 1024*1024)
        {
            try
            {
                FileCheckSumData data = new FileCheckSumData() { ReadBufferCount = checkSize };
                List<CheckSumItem> items = new List<CheckSumItem>();
                using (var stream = IOHelper.OpenFileStreamForRead(fileName, FileMode.Open, FileAccess.Read))
                {
                    while (stream.Position != stream.Length)
                    {
                        CheckSumItem checkSum = new CheckSumItem();
                        checkSum.StartPosition = stream.Position;
                        var bytes = GetBytesPerBuffer(stream, checkSize);
                        checkSum.EndPosition = stream.Position == stream.Length ? stream.Length : checkSum.StartPosition + checkSize;
                        checkSum.Hash = GetMD5(bytes);
                        items.Add(checkSum);
                        if (ProgressAction != null)
                            ProgressAction(stream.Position, stream.Length);
                    }
                }
                data.CheckSums = items;
                return data;
            }
            catch (Exception e)
            {
                Log.AutoLogger.LogError(e, "GetFileCheckSum");
                return null;
            }
        }

        public static List<byte> GetListOfBytes(string fileName, long position, int lenght)
        {
            try
            {
                using (var stream = IOHelper.OpenFileStreamForRead(fileName, FileMode.Open, FileAccess.Read))
                {
                    stream.Seek(position, SeekOrigin.Begin);
                    var bytes = GetBytesPerBuffer(stream, lenght);
                    return bytes.ToList();
                }
            }
            catch (Exception e)
            {
                Log.AutoLogger.LogError(e, "GetListOfBytes");
                return null;
            }
        }

        public static void SaveToFile(FileCheckSumData data, string fileName)
        {
            SerializeStream.SaveSerializeStream(fileName, data);
        }

        public static List<CheckSumItem> GetErrorsFromTwoCheckSum(string downloadedFile, string trueFile)
        {
            try
            {
                var downloadData = SerializeStream.OpenSerializeStream<FileCheckSumData>(downloadedFile);
                var trueData = SerializeStream.OpenSerializeStream<FileCheckSumData>(trueFile);
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
            catch (Exception e)
            {
                Log.AutoLogger.LogError(e, "GetErrorsFromTwoCheckSum");
                return null;
            }
        }
    }
}
