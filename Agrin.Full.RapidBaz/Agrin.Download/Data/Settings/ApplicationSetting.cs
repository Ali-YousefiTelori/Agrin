using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class ApplicationSetting
    {
        [NonSerialized]
        private static ApplicationSetting _Current;
        public static ApplicationSetting Current
        {
            get { return ApplicationSetting._Current; }
            set { ApplicationSetting._Current = value; }
        }

        public bool IsSettingForAllLinks { get; set; }
        public bool IsSettingForNewLinks { get; set; }
        public bool IsShowNotification { get; set; }

        public bool NoCheckLastVersion { get; set; }
        public bool NoGetApplicationMessage { get; set; }

        public bool IsRemoveBackupWhenCreatingFile { get; set; }

        public int LastUserMessageID { get; set; }//آخرین باری که از سرور چک شد آی دی میخوره و چیزی که از سایت دریافت میکنه یک متن و یک نسخه هست و نرم افزار چک میکنه اگر اخرین ایدی رو در ستینگ ذخیره کرده بود اتفاقی نمیوفته وگرنه میره میبینه که نسخه اش بزرگتر از نسخه ی کنونی هست یا نه اگر نبود میره مسج رو نوتیفیشن میکنه و در انتها در ستینگ ای دی جدید رو ذخیره میکنه
        public int LastApplicationMessageID { get; set; }
        public string ApplicationLanguage { get; set; }

        public long OldDownloadedSize { get; set; }

        public AppPathSetting PathSetting { get; set; }
        public AppLinkInfoDownloadSetting LinkInfoDownloadSetting { get; set; }
        public AppProxySetting ProxySetting { get; set; }
        public AppSpeedSetting SpeedSetting { get; set; }
        public AppUserAccountsSetting UserAccountsSetting { get; set; }
        public ApplicationOSSetting ApplicationOSSetting { get; set; }
        public RapidBazSetting RapidBazSetting { get; set; }
        public FramesoftSetting FramesoftSetting { get; set; }
    }
}
