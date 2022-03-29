using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft.Upload
{
    /// <summary>
    /// مشخصات یک پوشه
    /// </summary>
    public class FolderInfo
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
    }
}
