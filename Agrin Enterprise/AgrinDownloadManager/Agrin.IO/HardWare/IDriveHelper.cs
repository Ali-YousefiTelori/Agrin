using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.IO.HardWare
{
    public interface IDriveHelper
    {
        long GetFreeSizeByPath(string path);
        string GetRootPath(string path);
        List<string> GetStorageDirectories();
    }
}
