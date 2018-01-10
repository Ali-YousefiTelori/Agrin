using Agrin.Download.ShortModels.Link;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.EntireModels.Link
{
    /// <summary>
    /// entire link info download properties
    /// </summary>
    public class LinkInfoDownload : LinkInfoDownloadShort
    {
        /// <summary>
        /// custom headers from user for download link
        /// </summary>
        public ConcurrentDictionary<string, string> CustomHeaders { get; internal set; }
    }
}
