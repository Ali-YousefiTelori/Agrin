using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.LinkExtractor.Models
{
    public class TextCommandInfo : ICommandInfo<string>
    {
        public CommandType Type { get; set; }
        public string Data { get; set; }
    }
}
