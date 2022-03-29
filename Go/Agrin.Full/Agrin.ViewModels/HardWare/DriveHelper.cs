using Agrin.IO.HardWare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.HardWare
{
    public class DriveHelper : ADriveHelper
    {
        public override long GetFreeSizeByPath(string path)
        {
            string root = Path.GetPathRoot(path);
            DriveInfo driveInfo = new DriveInfo(root);
            return driveInfo.AvailableFreeSpace;
        }

        public override string GetRootPath(string path)
        {
            return Path.GetPathRoot(path);
        }
    }
}
