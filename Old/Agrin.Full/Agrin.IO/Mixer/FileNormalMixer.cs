using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.IO.Mixer
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

        public override void Start(MixerInfo currentMixer)
        {
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
            MixedSize = 0;
            using (var stream = IOHelper.OpenFileStreamForWrite(CurrentMixer.FilePath, System.IO.FileMode.OpenOrCreate, fileName: CurrentMixer.FileName))
            {
                stream.SetLength(0);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                foreach (var file in Files)
                {
                    long currentMixed = MixedSize;
                    using (var copystream = IOHelper.OpenFileStreamForRead(file.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        int len = 1024 * 1024 * 2;
                        byte[] read = new byte[len];
                        int rCount;
                        while ((rCount = copystream.Read(read, 0, len)) > 0)
                        {
                            System.Threading.Thread.Sleep(2);
                            stream.Write(read, 0, rCount);
                            MixedSize = currentMixed + copystream.Position;
                        }
                    }
                    file.IsComplete = IsCompleteEnum.Comeplete;
                    CurrentMixer.MixedCompletedLen = stream.Length;
                }
                Size = stream.Length;
            }
            try
            {
                foreach (var file in Files)
                {
                    IOHelper.DeleteFile(file.Path);
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
    }
}
