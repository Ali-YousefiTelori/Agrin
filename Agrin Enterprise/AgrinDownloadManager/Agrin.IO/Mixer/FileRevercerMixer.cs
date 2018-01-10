using Agrin.Checksum;
using Agrin.IO.Helpers;
using Agrin.IO.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Mixer
{
    /// <summary>
    /// میکسر فایل را ابتدا برعکس ذخیره میکند و سپس ان را به حالت اصلی بر می گرداند
    /// هنگامی که برعکس ذخیره میکند از حجم فایل های موقتی کم میکند
    /// </summary>
    public class FileRevercerMixer : FileMixer
    {
        List<string> _newFiles = null;
        public FileRevercerMixer(List<string> files)
        {
            _newFiles = files;
        }

        public override void Start(MixerInfo currentMixer)
        {
            base.Start(currentMixer);
            GenerateFiles(currentMixer, _newFiles);
            CurrentMixer.SaveToFile();
            FixCompleteFileSizeFotResume();
            ReverceCreateFile();
        }
        public override void GenerateFiles(MixerInfo currentMixer, List<string> newFiles)
        {
            long mainLen = 0;
            foreach (var file in newFiles)
            {
                var find = (from x in Files where x.FileName == System.IO.Path.GetFileName(file) select x).FirstOrDefault();
                if (find == null)
                {
                    find = new FileConnection() { Path = file, IsComplete = IsCompleteEnum.None };
                    Files.Add(find);
                }
                else
                    find.Path = file;
                if (!(find.IsComplete == IsCompleteEnum.Comeplete || find.IsComplete == IsCompleteEnum.Reverce) && !System.IO.File.Exists(file))
                    throw new Exception("mixer file not found");
                if (find.IsComplete == IsCompleteEnum.Comeplete && !System.IO.File.Exists(currentMixer.FullAddress))
                    throw new Exception("complete file moved or deleted!");
                if (System.IO.File.Exists(file) && find.IsComplete == IsCompleteEnum.None)
                {
                    find.Lenght = new System.IO.FileInfo(file).Length;
                }
                mainLen += find.Lenght;
            }
            Size = mainLen;
            if (System.IO.File.Exists(currentMixer.FullAddress))
            {
                MixedSize = new System.IO.FileInfo(currentMixer.FullAddress).Length;
            }
            else
                MixedSize = currentMixer.MixedCompletedLen;
        }
        public override void FixCompleteFileSizeFotResume()
        {
            long size = 0;
            foreach (var file in Files)
            {
                if (System.IO.File.Exists(file.Path))
                    size += new System.IO.FileInfo(file.Path).Length;
            }

            using (var stream = IOHelper.OpenFileStreamForWrite(CurrentMixer.FilePath, System.IO.FileMode.OpenOrCreate, fileName: CurrentMixer.FileName))
            {
                if (size + stream.Length > Size)
                {
                    var len = (size + stream.Length) - Size;
                    stream.SetLength(stream.Length - len);
                }
            }
        }

        public void ReverceCreateFile()
        {
            MixedSize = 0;
            using (var stream = IOHelper.OpenFileStreamForWrite(CurrentMixer.FilePath, System.IO.FileMode.OpenOrCreate, fileName: CurrentMixer.FileName))
            {
                stream.Seek(0, System.IO.SeekOrigin.End);
                foreach (var file in Files.Reverse<FileConnection>())
                {
                    if (file.IsComplete == IsCompleteEnum.Comeplete)
                        continue;
                    long currentMixed = MixedSize;
                    using (var copystream = IOHelper.OpenFileStreamForWrite(file.Path, System.IO.FileMode.Open))
                    {
                        int len = 1024 * 1024 * 2;
                        byte[] read = new byte[len];
                        while (copystream.Length > 0)
                        {
                            System.Threading.Thread.Sleep(2);
                            var seek = copystream.Length - len;
                            if (seek < 0)
                                seek = 0;
                            copystream.Seek(seek, System.IO.SeekOrigin.Begin);
                            int rCount = copystream.Read(read, 0, len);
                            List<byte> comRead = null;
                            if (rCount == read.Length)
                                comRead = read.ToList();
                            else
                            {
                                comRead = read.ToList().GetRange(0, rCount);
                            }
                            comRead.Reverse();
                            stream.Write(comRead.ToArray(), 0, rCount);
                            copystream.SetLength(copystream.Length - rCount);
                            MixedSize = currentMixed + copystream.Position;
                            file.IsComplete = IsCompleteEnum.Reverce;
                            CurrentMixer.MixedCompletedLen = MixedSize = Size / 2;
                            CurrentMixer.SaveToFile();
                        }
                    }
                    CurrentMixer.MixedCompletedLen = MixedSize = Size / 2;
                    file.IsComplete = IsCompleteEnum.Comeplete;
                    CurrentMixer.SaveToFile();
                }
                Size = stream.Length;
            }

            try
            {
                foreach (var file in Files)
                {
                    if (System.IO.File.Exists(file.Path))
                        IOHelper.DeleteFile(file.Path);
                }
            }
            catch (Exception ex)
            {
                Log.AutoLogger.LogError(ex, "FileNormalMixer CreateFile");
            }
            ReverceFileData();
        }


        public void ReverceFileData()
        {
            using (var stream = IOHelper.OpenFileStreamForWrite(CurrentMixer.FilePath, System.IO.FileMode.OpenOrCreate, fileName: CurrentMixer.FileName))
            {
                CurrentMixer.MixedCompletedLen = MixedSize = Size / 2;
                CurrentMixer.Status = MixerStatusEnum.Revercing;
                long revecredSize = CurrentMixer.RevercedCompletedLen;
                MixedSize = revecredSize;
                //long revecredStart = CurrentMixer.RevecredStartLen;
                long revecredLen = CurrentMixer.RevecredLen;
                bool isResuming = true;
                while (revecredSize < stream.Length)
                {
                    MixedSize = revecredSize;
                    long doubleRead = stream.Length - revecredSize;
                    int readBlockSize = 1024 * 1024 * 2;
                    if (doubleRead < readBlockSize * 2)
                        readBlockSize = (int)doubleRead / 2;
                    if (readBlockSize == 0)
                        break;

                    stream.Seek(revecredLen, System.IO.SeekOrigin.Begin);
                    // start file bytes
                    var bytesOfStart = ReadCount(stream, readBlockSize);


                    stream.Seek(stream.Length - revecredLen - readBlockSize, System.IO.SeekOrigin.Begin);
                    //end file bytes
                    var bytesOfEnd = ReadCount(stream, readBlockSize);


                    CurrentMixer.RevecredLen = revecredLen;


                    var hashOfStart = FileChecksum.GetMD5(bytesOfStart);
                    var hashOfEnd = FileChecksum.GetMD5(bytesOfEnd);
                    bool canWriteStart = true;
                    if (isResuming)
                        canWriteStart = hashOfStart != CurrentMixer.BeforeMixHashStart && CurrentMixer.BeforeMixHashStart != FileChecksum.GetMD5(bytesOfEnd.Reverse().ToArray());
                    else
                        canWriteStart = true;

                    bool canWriteEnd = true;
                    if (isResuming)
                        canWriteEnd = hashOfEnd != CurrentMixer.BeforeMixHashEnd && CurrentMixer.BeforeMixHashEnd != FileChecksum.GetMD5(bytesOfStart.Reverse().ToArray());
                    else
                        canWriteEnd = true;

                    bytesOfStart = bytesOfStart.Reverse().ToArray();
                    bytesOfEnd = bytesOfEnd.Reverse().ToArray();

                    if (canWriteStart)
                        CurrentMixer.BackUpBytes = bytesOfEnd;
                    CurrentMixer.BeforeMixHashStart = hashOfStart;
                    CurrentMixer.BeforeMixHashEnd = hashOfEnd;
                    CurrentMixer.SaveToFile();

                    //hash1 = FileCheckSum.GetMD5(bytes1.ToArray());
                    stream.Seek(stream.Length - revecredLen - readBlockSize, System.IO.SeekOrigin.Begin);

                    if (!isResuming || canWriteStart)
                        stream.Write(bytesOfStart, 0, bytesOfStart.Length);
                    else
                        bytesOfEnd = CurrentMixer.BackUpBytes;

                    stream.Seek(revecredLen, System.IO.SeekOrigin.Begin);

                    if (!isResuming || canWriteEnd)
                        stream.Write(bytesOfEnd, 0, bytesOfEnd.Length);

                    revecredLen += readBlockSize;
                    isResuming = false;
                    CurrentMixer.RevecredLen = revecredLen;

                    revecredSize += (readBlockSize * 2);
                    CurrentMixer.RevercedCompletedLen = revecredSize;

                    CurrentMixer.MixedCompletedLen = MixedSize = (Size / 2) + (revecredSize / 2);
                }
                CurrentMixer.MixedCompletedLen = MixedSize = Size;
            }
        }

        static byte[] ReadCount(IStreamWriter stream, int count)
        {
            List<byte> bytes = new List<byte>();
            int lengthReaded = 0;
            while (lengthReaded < count)
            {
                int countToRead = count;
                if (lengthReaded + countToRead > count)
                {
                    countToRead = count - lengthReaded;
                }
                byte[] readBytes = new byte[countToRead];
                var readCount = stream.Read(readBytes, 0, countToRead);
                if (readCount == 0)
                    throw new Exception("read zero buffer!");
                lengthReaded += readCount;
                bytes.AddRange(readBytes.ToList().GetRange(0, readCount));
            }
            return bytes.ToArray();
        }

        public static bool CanStartByThisMixer(long freeSpace, string connectionSavePathRoot, string saveToRoot)
        {
            if (PathHelper.EqualPath(connectionSavePathRoot, saveToRoot))
            {
                if (freeSpace > 1024 * 1024 * 10)
                    return true;
                //else
                //    return MixerResult.NoSpace;
            }

            return false;
        }
    }
}
