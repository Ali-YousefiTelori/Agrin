using Agrin.Download.ShortModels.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Helpers
{
    public abstract class LinkActionsBase
    {
        public static LinkActionsBase Current { get; set; }
        public abstract void OpenFile(string filePath);
        public abstract void OpenFileLocation(string directoryPath);
        public abstract void ShareFile(string filePath);
        public abstract void CopyLinkAddress(string linkAddress);
        public abstract void Delete(LinkInfoShort linkInfoShort);
    }
}
