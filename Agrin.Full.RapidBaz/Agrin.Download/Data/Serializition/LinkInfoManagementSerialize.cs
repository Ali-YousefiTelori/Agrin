using Agrin.Download.Web.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class LinkInfoManagementSerialize
    {
        public int ReadBuffer { get; set; }
        public int ConnectionThreadCount { get; set; }
        public int TryAginCount { get; set; }
        public bool IsTryExtreme { get; set; }
        public List<ExceptionSerializable> Errors { get; set; }
        public List<MultiLinkAddress> MultiLinks { get; set; }
        public List<object> SharingSettings { get; set; }
        public List<ProxyInfo> MultiProxy { get; set; }
        public bool IsLimit { get; set; }
        public int LimitPerSecound { get; set; }
        public bool IsEndDownload { get; set; }
        public CompleteDownloadSystemMode EndDownloadSystemMode { get; set; }
        public NetworkCredentialInfo NetworkUserPass { get; set; }
        public string Description { get; set; }
        public bool IsApplicationSetting { get; set; }
    }
}
