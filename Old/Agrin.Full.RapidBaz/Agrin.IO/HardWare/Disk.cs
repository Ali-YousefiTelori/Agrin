using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.HardWare
{
    public static class Disk
    {
        public static long GetDriveSize(string path)
        {
            try
            {
                string root = Path.GetPathRoot(path);
                System.IO.Directory.GetLogicalDrives();
                DriveInfo driveInfo = new DriveInfo(root);
                return driveInfo.TotalFreeSpace;
            }
            catch
            {
                return 0;
            }
        }
    }
}
