using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.IO.HardWare
{
    public abstract class ADriveHelper : IDriveHelper
    {
        public static IDriveHelper Current { get; set; }

        public abstract long GetFreeSizeByPath(string path);

        public abstract string GetRootPath(string path);
    }
}
