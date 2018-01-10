using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft.Upload
{
    /// <summary>
    /// کلید منحصر بفرد فایل برای شناسایی در سرور
    /// </summary>
    public class FileUniqueData
    {
        /// <summary>
        /// تاریخ ساخت فایل
        /// </summary>
        public DateTime CreateFileDateTime { get; set; }
        /// <summary>
        /// آخرین تاریخ ویرایش
        /// </summary>
        public DateTime LastModifiedFileDateTime { get; set; }
        /// <summary>
        /// اندازه ی فایل
        /// </summary>
        public long FileLenght { get; set; }
        /// <summary>
        /// آدرس فایل
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// کد منحصر به فرد کل فایل
        /// </summary>
        public string UniqueHash { get; set; }
        /// <summary>
        /// هش های قطعه های یک مگابایتی فایل
        /// </summary>
        public List<IOHash> FileDataHashes { get; set; }
    }
}
