using Agrin.IO.HardWare;
using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Mixer
{
    public enum MixerStatusEnum
    {
        Copying = 0,
        Stoped = 1,
        Revercing = 2,
        Complete = 3
    }

    public enum MixerTypeEnum
    {
        Normal = 0,
        Deleter = 1,
        Revercer = 2,
        NoSpace = 3
    }

    [Serializable]
    public class MixerInfo
    {
        public MixerInfo()
        {
            Files = new List<FileConnection>();
        }
        /// <summary>
        /// آی دی لینک
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// محل ذخیره فایل تکمیل شده
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }

        public string FullAddress
        {
            get
            {
                return Path.Combine(FilePath, FileName);
            }
        }
        /// <summary>
        /// محل ذخیره ی کلاس میکسر
        /// </summary>
        public string MixerPath { get; set; }
        /// <summary>
        /// بکاپ ذخیره شده ی میکسر
        /// </summary>
        public string MixerBackupPath { get; set; }
        /// <summary>
        /// برای رزیوم باید این هش چک شود که اگر برعکس شده بود دیگر دوباره ان را برعکس نکند
        /// </summary>
        public string BeforeMixHashStart { get; set; }
        /// <summary>
        /// همان توضیحات برای انتهای فایل
        /// </summary>
        public string BeforeMixHashEnd { get; set; }
        /// <summary>
        /// فایل جدید ساخته شده و حجمی که میکس شده روی آن
        /// </summary>
        public long MixedCompletedLen { get; set; }
        /// <summary>
        /// فایل جدید ساخته شده و حجمی که برعکس شده روی آن
        /// </summary>
        public long RevercedCompletedLen { get; set; }


        public byte[] BackUpBytes { get; set; }
        /// <summary>
        /// برعکس شده از شروع فایل
        /// </summary>
        //public long RevecredStartLen { get; set; }
        /// <summary>
        /// برعکس شده از انتهاب فایل
        /// </summary>
        public long RevecredLen { get; set; }

        /// <summary>
        /// وضعیت
        /// </summary>
        public MixerStatusEnum Status { get; set; }
        /// <summary>
        /// نوع میکسر که آخرین بار برای ساخت فایل ذخیره شده بود
        /// </summary>
        public MixerTypeEnum MixerType { get; set; } = MixerTypeEnum.Normal;

        public List<FileConnection> Files { get; set; }

        public void SaveToFile()
        {
            SerializeStream.SaveSerializeStream(MixerPath, this);
            SerializeStream.SaveSerializeStream(MixerBackupPath, this);
        }

        public static MixerInfo LoadFromFile(string fileName, string backUpFileName, bool createNewIfNotFound = false)
        {
            if (createNewIfNotFound && !System.IO.File.Exists(fileName) && !System.IO.File.Exists(backUpFileName))
                return new MixerInfo() { MixerPath = fileName, MixerBackupPath = backUpFileName, MixerType = MixerTypeEnum.Normal };
            else if (!createNewIfNotFound && !System.IO.File.Exists(fileName) && !System.IO.File.Exists(backUpFileName))
                return null;

            var mixer = (MixerInfo)SerializeStream.OpenSerializeStream(fileName);
            if (mixer == null)
                mixer = (MixerInfo)SerializeStream.OpenSerializeStream(backUpFileName);
            mixer.MixerPath = fileName;
            mixer.MixerBackupPath = backUpFileName;
            return mixer;
        }

        public static MixerInfo InstanceMixerByType(MixerTypeEnum type, string fileName, string backUpFileName)
        {
            return new MixerInfo() { MixerPath = fileName, MixerBackupPath = backUpFileName, MixerType = type };
        }

        public static MixerTypeEnum GenerateAutoMixerByDriveSize(string savePath, long fileSize, List<string> partOfFiles)
        {
            try
            {
                var freeSpace = ADriveHelper.Current.GetFreeSizeByPath(savePath);

                if (FileNormalMixer.CanStartByThisMixer(fileSize, freeSpace))
                    return MixerTypeEnum.Normal;
                else if (FileDeleterMixer.CanStartByThisMixer(partOfFiles, freeSpace, ADriveHelper.Current.GetRootPath(partOfFiles.First()), ADriveHelper.Current.GetRootPath(savePath)))
                    return MixerTypeEnum.Deleter;
                else if (FileRevercerMixer.CanStartByThisMixer(freeSpace, ADriveHelper.Current.GetRootPath(partOfFiles.First()), ADriveHelper.Current.GetRootPath(savePath)))
                    return MixerTypeEnum.Revercer;
                return MixerTypeEnum.NoSpace;
            }
            catch(Exception ex)
            {
                return MixerTypeEnum.Normal;
            }
        }

        public static string ReportAutoMixerMode(string savePath, long fileSize, List<string> partOfFiles)
        {
            var freeSpace = ADriveHelper.Current.GetFreeSizeByPath(savePath);
            StringBuilder reportResult = new StringBuilder();
            reportResult.AppendLine($"حجم فایل: {fileSize}");
            reportResult.AppendLine($"فضای خالی محل ذخیره: {freeSpace}");
            reportResult.AppendLine($"محل ذخیره: {savePath}");
            var canCompelete = GenerateAutoMixerByDriveSize(savePath, fileSize, partOfFiles);
            if (canCompelete == MixerTypeEnum.NoSpace)
                reportResult.AppendLine($"مد ذخیره سازی: عدم وجود حجم کافی برای ذخیره سازی");
            else if (canCompelete == MixerTypeEnum.Normal)
                reportResult.AppendLine($"مد ذخیره سازی: موفق، مد معمولی");
            else if (canCompelete == MixerTypeEnum.Deleter)
                reportResult.AppendLine($"مد ذخیره سازی: موفق، مد دوم");
            else if (canCompelete == MixerTypeEnum.Revercer)
                reportResult.AppendLine($"مد ذخیره سازی: موفق، مد سوم");
            return reportResult.ToString();
        }
    }
}
