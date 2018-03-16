using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.Download.Mixers
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

    public enum IsCompleteEnum
    {
        None,
        Comeplete,
        Reverce
    }

    public class FileConnection
    {
        public string Path { get; set; }
        public long Lenght { get; set; }
        public IsCompleteEnum IsComplete { get; set; }
        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(Path);
            }
        }
    }

    public class MixerInfoBase
    {
        /// <summary>
        /// آی دی لینک
        /// </summary>
        [BsonId]
        public int Id { get; set; }
        /// <summary>
        /// آی دی لینک
        /// </summary>
        public int LinkId { get; set; }
        /// <summary>
        /// محل ذخیره فایل تکمیل شده
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }
        
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

        public List<FileConnection> Files { get; set; } = new List<FileConnection>();

        public string FullAddress
        {
            get
            {
                return Path.Combine(FilePath, FileName);
            }
        }

    }
}
