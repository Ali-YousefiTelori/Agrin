using Agrin.IO.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.IO.Mixer
{
    public abstract class FileMixer : IDisposable
    {
        public static FileMixer GetMixerByType(MixerTypeEnum type, List<string> files)
        {
            if (type == MixerTypeEnum.Normal)
                return new FileNormalMixer(files);
            else if (type == MixerTypeEnum.Deleter)
                return new FileDeleterMixer(files);
            else if (type == MixerTypeEnum.Revercer)
                return new FileRevercerMixer(files);
            return null;
        }

        public Action OnChangedDataAction { get; set; }

        public MixerInfo CurrentMixer { get; set; }

        public List<FileConnection> Files
        {
            get
            {
                return CurrentMixer.Files;
            }
        }

        long _Size;
        public long Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
                OnChangedDataAction?.Invoke();
            }
        }

        long _MixedSize;
        public long MixedSize
        {
            get
            {
                return _MixedSize;
            }
            set
            {
                _MixedSize = value;
                OnChangedDataAction?.Invoke();
            }
        }

        public virtual void Start(MixerInfo currentMixer)
        {
            CurrentMixer = currentMixer;
        }

        public virtual void GenerateFiles(MixerInfo currentMixer, List<string> newFiles)
        {
            long mainLen = 0;
            foreach (var file in newFiles)
            {
                var find = (from x in Files where x.FileName == System.IO.Path.GetFileName(file) select x).FirstOrDefault();
                if (find == null)
                {
                    find = new FileConnection() { Path = file };
                    Files.Add(find);
                }
                else
                    find.Path = file;
                if (!(find.IsComplete == IsCompleteEnum.Comeplete) && !System.IO.File.Exists(file))
                    throw new Exception("mixer file not found");
                if (find.IsComplete == IsCompleteEnum.Comeplete && !System.IO.File.Exists(currentMixer.FullAddress))
                    throw new Exception("complete file moved or deleted!");
                if (System.IO.File.Exists(file) && find.IsComplete == IsCompleteEnum.None)
                {
                    find.Lenght = new System.IO.FileInfo(file).Length;
                    mainLen += find.Lenght;
                }
                else
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

        public void Complete()
        {
            try
            {
                IOHelper.DeleteFile(CurrentMixer.MixerBackupPath);
            }
            catch (Exception ex)
            {
                Log.AutoLogger.LogError(ex, "FileMixer Complete Delete MixerBackupPath");
            }
            try
            {
                IOHelper.DeleteFile(CurrentMixer.MixerPath);
            }
            catch (Exception ex)
            {
                Log.AutoLogger.LogError(ex, "FileMixer Complete Delete MixerPath");
            }
        }

        public abstract void FixCompleteFileSizeFotResume();

        public void Dispose()
        {

        }
    }
}
