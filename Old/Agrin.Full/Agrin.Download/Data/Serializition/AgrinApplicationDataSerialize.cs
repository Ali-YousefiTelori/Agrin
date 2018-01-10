using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class AgrinApplicationDataSerialize
    {
        public Dictionary<int,string> LinkInfoAddresses { get; set; }
    }
}
