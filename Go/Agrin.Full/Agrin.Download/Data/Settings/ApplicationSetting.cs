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
            get { return _Current; }
            set { _Current = value; }
        }

        public bool IsSettingForAllLinks { get; set; }
        public bool IsSettingForNewLinks { get; set; }
        public bool IsShowNotification { get; set; }
        /// <summary>
        /// عدم نمایش راهنمای ابتدای صفحه هنگام اجرا
        /// </summary>
        public bool DontShowHelpPage { get; set; }

        public bool NoCheckLastVersion { get; set; }
        public bool NoGetApplicationMessage { get; set; }

        public bool IsRemoveBackupWhenCreatingFile { get; set; }

        public int LastUserMessageID { get; set; }//آخرین باری که از سرور چک شد آی دی میخوره و چیزی که از سایت دریافت میکنه یک متن و یک نسخه هست و نرم افزار چک میکنه اگر اخرین ایدی رو در ستینگ ذخیره کرده بود اتفاقی نمیوفته وگرنه میره میبینه که نسخه اش بزرگتر از نسخه ی کنونی هست یا نه اگر نبود میره مسج رو نوتیفیشن میکنه و در انتها در ستینگ ای دی جدید رو ذخیره میکنه
        public int LastApplicationMessageID { get; set; }
        public string ApplicationLanguage { get; set; }

        public long OldDownloadedSize { get; set; }

        private bool _IsTurnOnScreenWhenDownloading = true;//روشن نگه داشتن صفحه هنگام دانلود

        public bool IsTurnOnScreenWhenDownloading
        {
            get { return _IsTurnOnScreenWhenDownloading; }
            set { _IsTurnOnScreenWhenDownloading = value; }
        }

        private bool _IsTurnOnScreenWhenQueueIsActivated = true;//روشن نگه داشتن صفحه هنگامی که یک صف در حال انتظار است

        public bool IsTurnOnScreenWhenQueueIsActivated
        {
            get
            {
                return _IsTurnOnScreenWhenQueueIsActivated;
            }

            set
            {
                _IsTurnOnScreenWhenQueueIsActivated = value;
            }
        }

        private bool _IsShowErrorMessageOnScreen = true;//نمایش خطا روی صفحه

        public bool IsShowErrorMessageOnScreen
        {
            get { return _IsShowErrorMessageOnScreen; }
            set { _IsShowErrorMessageOnScreen = value; }
        }

        /// <summary>
        /// مدی از نوع دانلود در  صف که بدرد اندروید میخورد جهت اینکه کاربر انتخاب کند که به چه صورتی صفش دانلود شود
        /// </summary>
        public byte QueueDownloadBackgroundWorkerMode { get; set; }

        public AppPathSetting PathSetting { get; set; }
        public AppLinkInfoDownloadSetting LinkInfoDownloadSetting { get; set; }
        public AppProxySetting ProxySetting { get; set; }
        public AppSpeedSetting SpeedSetting { get; set; }
        public AppUserAccountsSetting UserAccountsSetting { get; set; }
        public ApplicationOSSetting ApplicationOSSetting { get; set; }
        public FramesoftSetting FramesoftSetting { get; set; }

    }
}
