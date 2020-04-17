using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.LinkExtractor.Models
{
    /// <summary>
    /// command type
    /// </summary>
    public enum CommandType : byte
    {
        /// <summary>
        /// extreact text between two text
        /// </summary>
        ExtractBetweenTwoText = 1,
        /// <summary>
        /// Extract list of models from a text
        /// </summary>
        ListOfModels = 2,
    }
}
