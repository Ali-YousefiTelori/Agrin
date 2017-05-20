using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Models
{
    /// <summary>
    /// فایلی که به سرور آپلود میشود
    /// </summary>
    public class UserFileInfo
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }

        public string Link { get; set; }
        public string DiskPath { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public byte Status { get; set; }
        public bool IsError { get; set; }
        public bool IsComplete { get; set; }
        public bool IsUserDownloadedThis { get; set; }
        public bool IsDeletedByApplication { get; set; }
        public int FormatCode { get; set; }

        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// برای دانلود فایل از این گوید استفاده میشه
        /// </summary>
        [Index]
        public Guid FileGuid { get; set; }

    }
}
