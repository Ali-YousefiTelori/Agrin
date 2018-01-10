using Agrin.ComponentModels;
using Agrin.Download.ShortModels.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.CoreModels.Link
{
    /// <summary>
    /// properties of link info
    /// </summary>
    public class LinkInfoPropertiesCore : NotifyPropertyChanged
    {
        volatile LinkInfoShort _LinkInfo;

        /// <summary>
        /// parent of link info
        /// </summary>
        public LinkInfoShort LinkInfo { get => _LinkInfo; set => _LinkInfo = value; }
    }
}
