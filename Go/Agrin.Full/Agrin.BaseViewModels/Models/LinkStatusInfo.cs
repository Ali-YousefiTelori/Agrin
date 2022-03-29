using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Models
{
    public class LinkStatusInfo
    {
        public double TotalSpeed { get; set; }
        public double TotalMaximumProgress { get; set; }
        public double TotalProgressValue { get; set; }
        public bool IsConnecting { get; set; }
        public bool IsError { get; set; }
        public bool IsComplete { get; set; }

        public LinkStatusInfo Clone()
        {
            return (LinkStatusInfo)MemberwiseClone();
        }
    }
}
