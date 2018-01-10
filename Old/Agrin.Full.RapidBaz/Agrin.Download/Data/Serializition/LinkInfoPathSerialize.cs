using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class LinkInfoPathSerialize
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string ConnectionsSavedAddress { get; set; }
        public string UserSavePath { get; set; }
        public string AddressFileName { get; set; }
        public string UserFileName { get; set; }
        public string UserGroupInfo { get; set; }
        public string ApplicationGroupInfo { get; set; }
    }
}
