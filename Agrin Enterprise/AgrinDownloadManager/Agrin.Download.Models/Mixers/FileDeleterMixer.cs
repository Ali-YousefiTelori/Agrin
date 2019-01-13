using Agrin.IO;
using Agrin.IO.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltraStreamGo;

namespace Agrin.Download.Mixers
{
    /// <summary>
    /// میکسر بعد از کپی بخش فایل ان را حذف میکند
    /// </summary>
    public class FileDeleterMixer : FileMixer
    {
        List<string> _newFiles = null;
        public FileDeleterMixer(List<string> files)
        {
            _newFiles = files;
        }

        public override void Start(MixerInfoBase currentMixer)
        {
            if (isCanceled)
                return;
            base.Start(currentMixer);
            GenerateFiles(currentMixer, _newFiles);
            FixCompleteFileSizeFotResume();
            CreateFile();
        }

        public override void FixCompleteFileSizeFotResume()
        {
            if (isCanceled)
                return;
            long resumeSize = 0;
            foreach (var file in Files)
            {
                if (file.IsComplete == IsCompleteEnum.Comeplete)
                {
                    resumeSize += file.Lenght;
                }
                else
                {
                    break;
                }
            }
            if (isCanceled)
                return;
            using (var stream = IOHelperBase.Current.OpenFileStreamForWrite(CurrentMixer.FilePath, System.IO.FileMode.OpenOrCreate, fileName: CurrentMixer.FileName))
            {
                if (isCanceled)
                    return;
                stream.SetLength(resumeSize);
            }
        }

        public void CreateFile()
        {
            if (isCanceled)
                return;
            using (var stream = IOHelperBase.Current.OpenFileStreamForWrite(CurrentMixer.FilePath, System.IO.FileMode.OpenOrCreate, fileName: CurrentMixer.FileName, newSecurityFileName: (newPath) => CurrentMixer.SecurityAddress = newPath))
            {
                stream.Seek(0, System.IO.SeekOrigin.End);
                foreach (var file in Files)
                {
                    if (file.IsComplete == IsCompleteEnum.Comeplete)
                        continue;
                    if (isCanceled)
                        return;
                    long currentMixed = MixedSize;
                    using (var copystream = IOHelperBase.Current.OpenFileStreamForRead(file.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        int len = 1024 * 1024 * 2;
                        byte[] read = new byte[len];
                        int rCount;
                        while ((rCount = copystream.Read(read, 0, len)) > 0)
                        {
                            if (isCanceled)
                                return;
                            System.Threading.Thread.Sleep(2);
                            stream.Write(read, 0, rCount);
                            MixedSize = currentMixed + copystream.Position;
                        }
                    }
                    file.IsComplete = IsCompleteEnum.Comeplete;
                    CurrentMixer.MixedCompletedLen = stream.Length;
                    MixerInfo.SaveAction(CurrentMixer);
                    try
                    {
                        CrossFileInfo.Current.Delete(file.Path);
                    }
                    catch (Exception ex)
                    {
                        Log.AutoLogger.LogError(ex, "FileDeleterMixer CreateFile");
                    }
                }
                Size = stream.Length;
            }
            Complete();
        }

        public static bool CanStartByThisMixer(List<string> files, long freeSpace, string connectionSavePathRoot, string saveToRoot)
        {
            long max = 0;
            long size = 0;
            foreach (var fileName in files)
            {
                if (CrossFileInfo.Current.Exists(fileName))
                {
                    var fileSize = new System.IO.FileInfo(fileName).Length;
                    if (max < fileSize)
                        max = fileSize;
                    size += fileSize;
                }
            }
            if (PathHelper.EqualPath(connectionSavePathRoot, saveToRoot))
            {
                if (max < freeSpace)
                    return true;
                else
                    return false;
            }
            else
            {
                if (size < freeSpace)
                    return true;
                else
                    return false;
            }
        }

        public override void Dispose()
        {
            isCanceled = true;
        }
    }
}
