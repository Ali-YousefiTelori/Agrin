using Agrin.IO;
using Agrin.IO.HardWare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.Download.Mixers
{
    public static class MixerInfo
    {
        public static Func<string, long, List<string>, MixerTypeEnum> GenerateAutoMixerByDriveSizeAction { get; set; }
        
        public static Func<int, MixerInfoBase> LoadAction { get; set; }

        public static Action<MixerInfoBase> SaveAction { get; set; }

        //public void SaveToFile()
        //{
        //    //SerializationData.SaveToFile(MixerPath, this);
        //    //SerializationData.SaveToFile(MixerBackupPath, this);
        //}
        
        //public static MixerInfo LoadFromFile(string savePath, string securitySavePath, string backUpFileName, bool createNewIfNotFound = false)
        //{
        //    if (createNewIfNotFound && !PathHelper.FileExist(savePath) && !PathHelper.FileExist(backUpFileName))
        //        return new MixerInfo() { MixerPath = savePath, MixerBackupPath = backUpFileName, MixerType = MixerTypeEnum.Normal };
        //    else if (!createNewIfNotFound && !System.IO.File.Exists(savePath) && !System.IO.File.Exists(backUpFileName))
        //        return null;

        //    var mixer = DeserializationData.OpenFromFile<MixerInfo>(savePath);
        //    if (mixer == null)
        //        mixer = DeserializationData.OpenFromFile<MixerInfo>(backUpFileName);
        //    mixer.MixerPath = savePath;
        //    mixer.MixerBackupPath = backUpFileName;
        //    return mixer;
        //}



        public static string ReportAutoMixerMode(string savePath, long fileSize, List<string> partOfFiles)
        {
            var freeSpace = DriveHelperBase.Current.GetFreeSizeByPath(savePath);
            StringBuilder reportResult = new StringBuilder();
            reportResult.AppendLine($"حجم فایل: {fileSize}");
            reportResult.AppendLine($"فضای خالی محل ذخیره: {freeSpace}");
            reportResult.AppendLine($"محل ذخیره: {savePath}");
            var canCompelete = GenerateAutoMixerByDriveSizeAction(savePath, fileSize, partOfFiles);
            if (canCompelete == MixerTypeEnum.NoSpace)
                reportResult.AppendLine($"مد ذخیره سازی: عدم وجود حجم کافی برای ذخیره سازی");
            else if (canCompelete == MixerTypeEnum.Normal)
                reportResult.AppendLine($"مد ذخیره سازی: موفق، مد معمولی");
            else if (canCompelete == MixerTypeEnum.Deleter)
                reportResult.AppendLine($"مد ذخیره سازی: موفق، مد دوم");
            else if (canCompelete == MixerTypeEnum.Revercer)
                reportResult.AppendLine($"مد ذخیره سازی: موفق، مد سوم");
            return reportResult.ToString();
        }
    }
}
