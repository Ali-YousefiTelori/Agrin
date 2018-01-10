using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Helper.Messaging
{
    public enum MessageMode
    {
        RenameFile,
        DateTime,
    }

    public class AcceptMessage
    {
        object _messageValue;

        public object MessageValue
        {
            get { return _messageValue; }
            set { _messageValue = value; }
        }
    }
}
