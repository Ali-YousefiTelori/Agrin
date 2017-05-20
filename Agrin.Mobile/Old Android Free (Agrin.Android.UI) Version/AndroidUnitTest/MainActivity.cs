using System.Reflection;
using Android.App;
using Android.OS;
using Xamarin.Android.NUnitLite;
using Android.Widget;
using System.IO;
using Agrin.IO.Helper;
using Agrin.Download.Data.Serializition;
using System.Text;
using System.Linq;
using Agrin.Download.Data.Settings;
using System;
using Android.Content;
using Agrin.Download.Manager;
using System.Collections.Generic;
using Android.Content.PM;

namespace AndroidUnitTest
{

    public static class InitializeApplication
    {
        public static void CancelNotify(int id)
        {
            if (_notifyManager != null)
                _notifyManager.Cancel(id);
        }

        static List<int> downloadingIDS = new List<int>();
        static NotificationManager _notifyManager = null;
        static Notification _notification = null;

        static bool initLimit = false;
        static Context _context = null;

        public static bool Inited = false;
        public static bool IsSetLanguage { get; set; }

        public static bool Run(Context currentActivity)
        {
            _context = currentActivity;
            if (!Inited)
            {
                Inited = true;

                InitializeApplication.InitializeAppIO();
                ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
                ApplicationGroupManager.Current = new ApplicationGroupManager();
                ApplicationNotificationManager.Current = new ApplicationNotificationManager();
                ApplicationBalloonManager.Current = new ApplicationBalloonManager();

                Agrin.Download.Data.DeSerializeData.LoadApplicationData();
                bool mustSave = false;
                if (System.IO.Directory.Exists(ApplicationSetting.Current.PathSetting.DownloadsPath))
                    Agrin.IO.Helper.MPath.DownloadsPath = ApplicationSetting.Current.PathSetting.DownloadsPath;
                else
                {
                    mustSave = true;
                    ApplicationSetting.Current.PathSetting.DownloadsPath = Agrin.IO.Helper.MPath.DownloadsPath;
                }
                //set app osSetting
                ApplicationSetting.Current.ApplicationOSSetting.Application = "Agrin Android";
                if (string.IsNullOrEmpty(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid))
                {
                    ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid = Guid.NewGuid().ToString();
                    mustSave = true;
                }
                PackageManager manager = currentActivity.PackageManager;
                PackageInfo info = manager.GetPackageInfo(currentActivity.PackageName, 0);
                ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion = info.VersionName;
                ApplicationSetting.Current.ApplicationOSSetting.OSName = "Android";
                ApplicationSetting.Current.ApplicationOSSetting.OSVersion = Android.OS.Build.VERSION.Release + " Api Level " + Android.OS.Build.VERSION.Sdk;

                if (!ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme)
                {
                    ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme = true;
                    ApplicationSetting.Current.IsSettingForAllLinks = true;
                    ApplicationSetting.Current.IsSettingForNewLinks = true;
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                }
                else if (mustSave)
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                Agrin.Download.Manager.ApplicationGroupManager.Current.NoGroup.SavePath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath;

                Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();
            }

            return true;
        }

        public static void InitializeAppIO()
        {
            Agrin.Log.AutoLogger.ApplicationDirectory = Agrin.IO.Helper.MPath.CurrentAppDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");

            string downloadsDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path;
            try
            {
                System.IO.Directory.CreateDirectory(Agrin.IO.Helper.MPath.CurrentAppDirectory);
            }
            catch (System.UnauthorizedAccessException e)
            {
                Agrin.Log.AutoLogger.ApplicationDirectory = downloadsDirectory = Agrin.IO.Helper.MPath.CurrentAppDirectory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Agrin");
                Agrin.Log.AutoLogger.LogError(e, "Storage 1: ", true);
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Storage 2: ", true);
            }
            //try
            //{
            //    //System.IO.Directory.CreateDirectory (Agrin.IO.Helper.MPath.CurrentAppDirectory);
            //    System.IO.File.Create(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory , "/test.txt");
            //}
            //catch (System.UnauthorizedAccessException e)
            //{
            //    Agrin.Log.AutoLogger.LogError(e, "Storage 3: ", true);
            //}
            //catch (Exception e)
            //{
            //    Agrin.Log.AutoLogger.LogError(e, "Storage 4: ", true);
            //}
            Agrin.IO.Helper.MPath.InitializePath(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "DataBase"), downloadsDirectory, System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "DownloadingSaveData"));
        }
    }
    [Activity(Label = "AndroidUnitTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : TestSuiteActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            AddTest(Assembly.GetExecutingAssembly());
            // or in any reference assemblies
            // AddTest (typeof (Your.Library.TestClass).Assembly);

            // Once you called base.OnCreate(), you cannot add more assemblies.
            var activity = this;
            base.OnCreate(bundle);
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle("AgrinDownloadManager_Language");
            LinearLayout layout = new LinearLayout(activity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            TextView txtMessage = new TextView(activity);
            txtMessage.SetTextAppearance(activity, global::Android.Resource.Style.TextAppearanceSmall);
            txtMessage.SetSingleLine(false);
            txtMessage.VerticalScrollBarEnabled = true;
            string fileName = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "Link.agn");
            InitializeApplication.Run(this);

            StringBuilder str = new StringBuilder();
            int foundCount = 0;
            int checkCount = 0;
            int fixedError = 0;

            foreach (var linkInfo in ApplicationLinkInfoManager.Current.LinkInfoes)
            {
                foreach (var item in linkInfo.Connections)
                {
                    if (File.Exists(item.SaveFileName))
                    {
                        FileInfo fileInfo = new FileInfo(item.SaveFileName);
                        item.DownloadedSize = fileInfo.Length;
                        if (item.DownloadedSize > item.Length)
                        {
                            foundCount++;
                            try
                            {
                                using (FileStream stream = new FileStream(item.SaveFileName, FileMode.Open))
                                {
                                    stream.SetLength((long)item.Length);
                                    stream.Flush();
                                }
                                fileInfo.Refresh();
                                if (fileInfo.Length <= item.Length)
                                    fixedError++;
                            }
                            catch (Exception e)
                            {
                                str.AppendLine("Error to fixed:" + e.Message);
                            }
                        }
                    }
                }
                checkCount++;

            }
            str.AppendLine("Check Links: " + checkCount.ToString());
            str.AppendLine("Error Founds: " + foundCount.ToString());
            str.AppendLine("Fixes Errors: " + fixedError.ToString());
            txtMessage.Text = str.ToString();

            layout.AddView(txtMessage);
            ScrollView scroll = new ScrollView(activity);
            scroll.AddView(layout);
            builder.SetView(scroll);
            // Set up the buttons
            builder.SetPositiveButton("OK", (dialog, which) =>
            {
                builder.Dispose();
            });

            builder.Show();
        }
    }
}

