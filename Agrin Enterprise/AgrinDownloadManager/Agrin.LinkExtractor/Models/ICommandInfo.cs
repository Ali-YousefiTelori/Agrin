using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.LinkExtractor.Models
{
    public enum CommandType : byte
    {
        Text = 1,
        ListOfText = 2,
    }

    public interface ICommandInfo
    {
        CommandType Type { get; set; }
    }

    public interface ICommandInfo<T>
    {
        T Data { get; set; }
    }
}
