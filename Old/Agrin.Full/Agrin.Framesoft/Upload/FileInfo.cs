using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft.Upload
{
    /// <summary>
    /// مشخصات یک فایل
    /// </summary>
    public class FileInfo
    {
        public int ID { get; set; }
        public int FolderID { get; set; }
        public int UserID { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
    }
}
