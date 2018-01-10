using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class LinkInfoShortSerialize
    {
        public DateTime CreateDateTime { get; set; }
        public int Id { get; set; }
        public string Address { get; set; }
        public string UserSavePath { get; set; }
        public string AddressFileName { get; set; }
        public string UserFileName { get; set; }
        public string UserGroupInfo { get; set; }
        public string ApplicationGroupInfo { get; set; }
        public double Size { get; set; }
        public ConnectionState State { get; set; }
        public string UserNameAuthorization { get; set; }
        public string PasswordAuthorization { get; set; }
        public bool IsApplicationSetting { get; set; }
        public string BackUpCompleteAddress { get; set; }
    }
}
