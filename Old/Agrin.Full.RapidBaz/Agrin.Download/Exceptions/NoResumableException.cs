using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Agrin.Download.Exceptions
{
    [Serializable]
    public class NoResumableException : Exception
    {
        public NoResumableException()
            : base() { }

        public NoResumableException(string message)
            : base(message) { }

        public NoResumableException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public NoResumableException(string message, Exception innerException)
            : base(message, innerException) { }

        public NoResumableException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }
        protected NoResumableException(SerializationInfo information, StreamingContext context)
        {

        }
    }
}
