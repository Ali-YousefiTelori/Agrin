using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.LinkExtractor.Models
{
    /// <summary>
    /// script of site to extract data
    /// </summary>
    public class ScriptInfo
    {
        /// <summary>
        /// host name like "framesoft.ir"
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// commands of script
        /// </summary>
        public List<CommandInfo> Commands { get; set; }
    }
}
