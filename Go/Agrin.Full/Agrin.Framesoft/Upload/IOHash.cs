using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft.Upload
{
    /// <summary>
    /// هش قسمتی از فایل
    /// </summary>
    public class IOHash
    {
        /// <summary>
        /// شروع محل هش
        /// </summary>
        public long StartPosition { get; set; }
        /// <summary>
        /// پایان محل هش
        /// </summary>
        public long EndPosition { get; set; }
        /// <summary>
        /// هش ساخته شده از شروع تا پایان
        /// </summary>
        public string HashCode { get; set; }
    }
}
