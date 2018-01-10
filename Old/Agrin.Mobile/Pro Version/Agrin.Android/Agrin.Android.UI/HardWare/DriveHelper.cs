using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Agrin.IO.HardWare;
using Java.IO;

namespace Agrin.HardWare
{
    public class DriveHelper : ADriveHelper
    {
        public override long GetFreeSizeByPath(string path)
        {
            StatFs stat = new StatFs(path);
            long bytesAvailable;
            if ((int)Build.VERSION.SdkInt >= 18)
            {
                bytesAvailable = stat.BlockSizeLong * stat.AvailableBlocksLong;
            }
            else
            {
                //noinspection deprecation
                bytesAvailable = (long)stat.BlockSize * (long)stat.AvailableBlocks;
            }

            return bytesAvailable;
        }

        public override string GetRootPath(string path)
        {
            File file = new File(path);
            if (file == null)
                return null;
            long totalSpace = file.TotalSpace;
            while (true)
            {
                File parentFile = file.ParentFile;
                if (parentFile == null || parentFile.TotalSpace != totalSpace)
                    return file.AbsolutePath;
                file = parentFile;
            }
        }
    }
}