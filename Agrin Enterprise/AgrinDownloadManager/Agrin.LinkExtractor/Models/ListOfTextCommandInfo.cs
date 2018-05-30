using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.LinkExtractor.Models
{
    public class ListOfTextCommandInfo : ICommandInfo<List<string>>
    {
        public CommandType Type { get; set; }
        public List<string> Data { get; set; }
    }
}
