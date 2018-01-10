using Agrin.Download.CoreModels.Link;
using Agrin.Download.ShortModels.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.EntireModels.Link
{
    /// <summary>
    /// link information
    /// use for desktop maximum data
    /// </summary>
    public class LinkInfo : LinkInfoShort
    {
        LinkInfoProperties _Properties;
        /// <summary>
        /// properties of link
        /// </summary>
        public override LinkInfoPropertiesShort Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new LinkInfoProperties();
                return _Properties;
            }
            set
            {
                _Properties = (LinkInfoProperties)value;
            }
        }
    }
}
