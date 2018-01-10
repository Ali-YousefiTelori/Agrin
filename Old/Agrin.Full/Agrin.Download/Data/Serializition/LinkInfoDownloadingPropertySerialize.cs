using Agrin.Download.Web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class LinkInfoDownloadingPropertySerialize
    {
        public int ConnectionCount { get; set; }
        public DateTime DateLastDownload { get; set; }
        public DateTime CreateDateTime { get; set; }
        public double Size { get; set; }
        public List<double> ListSpeedByteDownloaded { get; set; }
        public double SpeedByteDownloaded { get; set; }
        public ResumeCapabilityEnum ResumeCapability { get; set; }
        public AlgoritmEnum DownloadAlgoritm { get; set; }
        public List<long> DownloadRangePositions { get; set; }
        public ConnectionState State { get; set; }
        public ConcurrentDictionary<string, string> CustomHeaders { get; set; }
    }
}
