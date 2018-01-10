using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models.Link
{
    /// <summary>
    /// link address information
    /// </summary>
    public class LinkAddressInfo
    {
        /// <summary>
        /// default proxy id zero is system proxy and -1 is null proxy and -2 is geted from app setting
        /// </summary>
        public int ProxyID { get; set; } = -2;
        /// <summary>
        /// uri address or link
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// is enabled
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// if application added this link
        /// </summary>
        public bool IsApplicationAdded { get; set; }//if user added this link value is false
    }
}
