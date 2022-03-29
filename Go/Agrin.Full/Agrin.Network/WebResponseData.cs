using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Network
{
    public class MultipeWebResponseData
    {
        /// <summary>
        /// ادرس صفحه
        /// </summary>
        public string Reference { get; set; }
        public List<WebResponseData> Items { get; set; } = new List<WebResponseData>();
    }

    /// <summary>
    /// کلاس درخواست داده وب
    /// </summary>
    public class WebResponseData
    {
        /// <summary>
        /// آدرس لینک
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// پسوند
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }
}
