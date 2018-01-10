using Agrin.Download.CoreModels.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.ShortModels.Link
{
    /// <summary>
    /// link information
    /// use for mobile minimum data
    /// </summary>
    public class LinkInfoShort : LinkInfoCore
    {
        volatile LinkInfoPathShort _PathInfo;
        volatile LinkInfoPropertiesShort _Properties;
        volatile LinkInfoDownloadShort _LinkInfoDownloadCore;
        volatile LinkInfoManagementShort _LinkInfoManagementCore;
        /// <summary>
        /// properties of link
        /// </summary>
        public virtual LinkInfoPropertiesShort Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new LinkInfoPropertiesShort() { LinkInfo = this };
                return _Properties;
            }
            set => _Properties = value;
        }

        /// <summary>
        /// path and addresses of link
        /// </summary>
        public virtual LinkInfoPathShort PathInfo
        {
            get
            {
                if (_PathInfo == null)
                    _PathInfo = new LinkInfoPathShort() { LinkInfo = this };
                return _PathInfo;
            }
            set => _PathInfo = value;
        }
        /// <summary>
        /// downloading settings of link info
        /// </summary>
        public LinkInfoDownloadShort LinkInfoDownloadCore
        {
            get
            {
                if (_LinkInfoDownloadCore == null)
                    _LinkInfoDownloadCore = new LinkInfoDownloadShort() { LinkInfo = this };
                return _LinkInfoDownloadCore;
            }
            set => _LinkInfoDownloadCore = value;
        }


        /// <summary>
        /// management seetings of link info
        /// </summary>
        public virtual LinkInfoManagementShort LinkInfoManagementCore
        {
            get
            {
                if (_LinkInfoManagementCore == null)
                    _LinkInfoManagementCore = new LinkInfoManagementShort() { LinkInfo = this };
                return _LinkInfoManagementCore;
            }
            set => _LinkInfoManagementCore = value;
        }
    }
}
