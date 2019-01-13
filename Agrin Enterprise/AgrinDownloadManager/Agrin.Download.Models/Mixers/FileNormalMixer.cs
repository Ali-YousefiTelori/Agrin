using Agrin.IO.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltraStreamGo;

namespace Agrin.Download.Mixers
{
    /// <summary>
    /// میکسر تا زمانی که فایل را کامل کپی نکند ان را حذف نمی کند
    /// </summary>
    public class FileNormalMixer : FileMixer
    {
        List<string> _newFiles = null;
        public FileNormalMixer(List<string> files)
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

        }

        public void CreateFile()
        {
            if (isCanceled)
                return;
            MixedSize = 0;
            using (var stream = IOHelperBase.Current.OpenFileStreamForWrite(CurrentMixer.FilePath, System.IO.FileMode.OpenOrCreate, fileName: CurrentMixer.FileName, newSecurityFileName: (newPath) => CurrentMixer.SecurityAddress = newPath))
            {
                if (isCanceled)
                    return;
                stream.SetLength(0);
                stream.Flush();
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                foreach (var file in Files)
                {
                    long currentMixed = MixedSize;
                    using (var copystream = IOHelperBase.Current.OpenFileStreamForRead(file.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        int len = 1024 * 1024 * 2;
                        byte[] read = new byte[len];
                        int rCount;
                        while ((rCount = copystream.Read(read, 0, len)) > 0)
                        {
                            System.Threading.Thread.Sleep(2);
                            stream.Write(read, 0, rCount);
                            stream.Flush();
                            MixedSize = currentMixed + copystream.Position;
                            if (isCanceled)
                                return;
                        }
                    }
                    if (isCanceled)
                        return;
                    file.IsComplete = IsCompleteEnum.Comeplete;
                    CurrentMixer.MixedCompletedLen = stream.Length;
                }
                Size = stream.Length;
                stream.Flush();
            }
            try
            {
                if (isCanceled)
                    return;
                foreach (var file in Files)
                {
                    CrossFileInfo.Current.Delete(file.Path);
                }
            }
            catch (Exception ex)
            {
                Log.AutoLogger.LogError(ex, "FileNormalMixer CreateFile");
            }
            Complete();
        }

        public static bool CanStartByThisMixer(long size, long freeSpace)
        {
            if (size > freeSpace)
                return false;
            return true;
        }

        public override void Dispose()
        {
            isCanceled = true;
        }
    }
}
