using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Agrin.IO.Streams;
using Agrin.Log;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Agrin.Helpers
{
    public class AndroidStreamCross : IStreamWriter
    {
        Java.IO.RandomAccessFile AccessFile { get; set; }
        Java.IO.File JavaFile { get; set; }

        public Guid Guid { get; set; }
        public AndroidStreamCross(string fileName, FileMode mode, FileAccess access)
        {
            Guid = Guid.NewGuid();
            string readWriteMode = "rw";
            if (access == FileAccess.Read)
                readWriteMode = "r";
            else if (access == FileAccess.Write)
                readWriteMode = "w";
            if (mode == FileMode.Open)
            {
                JavaFile = new Java.IO.File(fileName);
                if (!JavaFile.Exists())
                    throw new FileNotFoundException();
                AccessFile = new Java.IO.RandomAccessFile(JavaFile, readWriteMode);
            }
            else if (mode == FileMode.Create || mode == FileMode.CreateNew || mode == FileMode.OpenOrCreate)
            {
                JavaFile = new Java.IO.File(fileName);
                if (!JavaFile.Exists())
                    JavaFile.CreateNewFile();
                AccessFile = new Java.IO.RandomAccessFile(JavaFile, readWriteMode);
            }
            else
                throw new NotSupportedException("not support for " + mode);
            AutoLogger.LogText($"AndroidStreamCross {Guid} {fileName} {mode} {access} {Length}");
        }

        public long Length
        {
            get
            {
                return AccessFile.Length();
            }
        }

        public long Position
        {
            get
            {
                return AccessFile.FilePointer;
            }
            set
            {
                AccessFile.Seek(value);
            }
        }

        bool isDispose = false;
        public void Dispose()
        {
            if (isDispose)
                return;
            isDispose = true;
            AutoLogger.LogText($"AndroidStreamCross {Guid} dispose");
            try
            {
                AccessFile.FD.Sync();
            }
            catch (Exception ex)
            {

            }
            AccessFile.Dispose();
            JavaFile.Dispose();
        }

        public void Flush()
        {
            AccessFile.FD.Sync();
        }

        public int Read(byte[] bytes, int offest, int count)
        {
            return AccessFile.Read(bytes, offest, count);
        }

        public long Seek(long position, SeekOrigin seek)
        {
            if (seek == SeekOrigin.Begin)
                AccessFile.Seek(0);
            else if (seek == SeekOrigin.End)
                AccessFile.Seek(Length);
            else
                AccessFile.Seek(position);
            return Position;
        }

        public void SetLength(long lenght)
        {
            AccessFile.SetLength(lenght);
        }

        public void Write(byte[] bytes, int offest, int count)
        {
            AccessFile.Write(bytes, offest, count);
        }
    }
}