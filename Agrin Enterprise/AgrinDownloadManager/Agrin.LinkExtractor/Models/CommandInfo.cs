using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.LinkExtractor.Models
{
    /// <summary>
    /// command of extractor
    /// command is a script to do extract text or list items from strings
    /// </summary>
    public class CommandInfo
    {
        /// <summary>
        /// command type
        /// what command must do?
        /// </summary>
        public CommandType Type { get; set; }
        /// <summary>
        /// parameters
        /// exmaple: if Type was extract between two text paramerters will be two, first text and second text for extract
        /// </summary>
        public List<CommandParameterInfo> CommandParameters { get; set; }
    }
}
