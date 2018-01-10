using Agrin.Download.Data.Serializition;
using Agrin.Download.Web;
using System;
using System.Collections.Generic;

namespace Agrin.Download.Data
{
    [Serializable]
    public class AgrinApplicationDataSerialize
    {
        List<LinkInfoSerialize> _linkInfoes;
        public List<LinkInfoSerialize> LinkInfoes
        {
            get
            {
                if (_linkInfoes == null)
                    _linkInfoes = new List<LinkInfoSerialize>();
                return _linkInfoes;
            }
            set { _linkInfoes = value; }
        }
    }

    [Serializable]
    public class AgrinApplicationDataShortSerialize
    {
        public List<LinkInfoShortSerialize> LinkInfoes { get; set; }
    }

    [Serializable]
    public class AgrinApplicationGroupDataSerialize
    {
        public List<GroupInfoSerialize> GroupInfoes { get; set; }
    }
}
