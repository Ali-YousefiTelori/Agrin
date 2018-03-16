using Agrin.IO.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Helpers
{
    public static class IOHelperBase
    {
        public static Func<string, Stream> OpenFileStreamForReadAction { get; set; }
        public static Func<string, string, FileMode, Action<string>, object, IStreamWriter> OpenFileStreamForWriteAction { get; set; }
        //public static List<string> FilesList { get; set; } = new List<string>();

        public static Stream OpenFileStreamForRead(string fileName, FileMode fileMode, FileAccess fileAccess = FileAccess.ReadWrite)
        {
            //FilesList.Add(fileName);
            Stream retStream = null;
            if (OpenFileStreamForReadAction != null)
                retStream = OpenFileStreamForReadAction(fileName);
            if (retStream == null)
                retStream = new FileStream(fileName, fileMode, fileAccess);
            return retStream;
        }

        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool DirectoryExists(string filePath)
        {
            return Directory.Exists(filePath);
        }

        public static IStreamWriter OpenFileStreamForWrite(string fileAddress, FileMode fileMode, FileAccess fileAccess = FileAccess.ReadWrite, string fileName = null, Action<string> newSecurityFileName = null, object data = null)
        {
            IStreamWriter retStream = null;
            //if (OpenFileStreamForWriteAction == null)
            //    Agrin.Log.AutoLogger.LogText("OpenFileStreamForWriteAction is null");
            if (OpenFileStreamForWriteAction != null)
                retStream = OpenFileStreamForWriteAction(fileAddress, fileName, fileMode, newSecurityFileName, data);
            if (retStream == null)
            {
                string path = fileAddress;
                if (!string.IsNullOrEmpty(fileName))
                    path = Path.Combine(fileAddress, fileName);
                retStream = new Agrin.IO.Streams.StreamWriter(new FileStream(path, fileMode, fileAccess));
            }

            return retStream;
        }



        public static byte[] ReadAllBytes(string fileName)
        {
            return System.IO.File.ReadAllBytes(fileName);
        }

        public static string[] ReadAllLines(string fileName, Encoding encoding)
        {
            return System.IO.File.ReadAllLines(fileName, encoding);
        }

        public static void WriteAllBytes(string fileName, byte[] bytes)
        {
            System.IO.File.WriteAllBytes(fileName, bytes);
        }
        public static void WriteAllLines(string fileName, string[] lines, Encoding encoding)
        {
            System.IO.File.WriteAllLines(fileName, lines, encoding);
        }
        public static void FileMove(string oldFile, string newFile)
        {
            System.IO.File.Move(oldFile, newFile);
        }

        public static void DeleteFile(string fileName)
        {
            System.IO.File.Delete(fileName);
        }

        public static void CreateDirectory(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        public static void DeleteDirectory(string path, bool recurcive)
        {
            System.IO.Directory.Delete(path, recurcive);
        }
        //public static Stream CreateFileStream(string fileName, FileAccess fileAccess)
        //{
        //    if (CreateFileStreamAction != null)
        //        return CreateFileStreamAction(fileName);
        //    return new FileStream(fileName, FileMode.Create, fileAccess);
        //}

        //public static Stream OpenOrCreateFileStream(string fileName, FileAccess fileAccess)
        //{
        //    if (OpenOrCreateFileStreamAction != null)
        //        return OpenOrCreateFileStreamAction(fileName);
        //    return new FileStream(fileName, FileMode.OpenOrCreate, fileAccess);
        //}
    }
}
