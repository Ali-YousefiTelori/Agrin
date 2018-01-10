using Agrin.BaseViewModels;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Framesoft.Helper;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using Agrin.IO.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Agrin.ViewModels.Helpers
{
    public abstract class InitializeApplication
    {
        public abstract string StoragePublicDirectory { get; set; }
        public abstract string DownloadsDirectory { get; set; }

        public void GoException(Exception error, string title = "")
        {
            //if (error is Exception)
            //{
            //    MainActivity.notify(error.Message, title + " GoException EX");
            //}
            //else if (error is Java.Lang.Exception)
            //{
            //    MainActivity.notify(((Java.Lang.Exception)error).Message, "GoException Java");
            //}
            if (Agrin.Log.AutoLogger.ApplicationDirectory == null)
                Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(StoragePublicDirectory, "AgrinData");
            Agrin.Log.AutoLogger.LogError(error, title, true);
            //ActivitesManager.MessageBoxActive();
        }

        public void GoException(string text)
        {
            if (Agrin.Log.AutoLogger.ApplicationDirectory == null)
                Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(StoragePublicDirectory, "AgrinData");

            Agrin.Log.AutoLogger.LogText(text);
        }

        public void SaveLanguage(string lang)
        {
            if (lang == "english")
            {
                IsSetLanguage = true;
                ViewsUtility.ProjectDirection = FlowDirection.LeftToRight;
                ViewsUtility.ApplicationLanguage = "_en";
            }
            else if (lang == "persian")
            {
                IsSetLanguage = true;
                ViewsUtility.ProjectDirection = FlowDirection.RightToLeft;
                ViewsUtility.ApplicationLanguage = "_fa";
            }
            ApplicationSetting.Current.ApplicationLanguage = lang;
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
        }

        public void LoadLanguage()
        {
            IsSetLanguage = true;
            string lang = ApplicationSetting.Current.ApplicationLanguage;
            if (lang == "english")
            {
                ViewsUtility.ProjectDirection = FlowDirection.LeftToRight;
                ViewsUtility.ApplicationLanguage = "_en";
            }
            else if (lang == "persian")
            {
                ViewsUtility.ProjectDirection = FlowDirection.RightToLeft;
                ViewsUtility.ApplicationLanguage = "_fa";
            }
            else
                IsSetLanguage = false;
        }

        public void CheckAddFrameSoftLink(LinkInfo link)
        {
            try
            {
                List<Uri> uri = new List<Uri>();
                uri.Add(new Uri(link.PathInfo.Address));
                foreach (var item in link.Management.MultiLinks)
                {
                    uri.Add(new Uri(item.Address));
                }

                List<string> guids = new List<string>();
                foreach (var item in uri)
                {
                    if (item.Host.ToLower().Contains(Framesoft.Helper.UserManagerHelper.domain.ToLower()))
                    {
                        string guidString = item.Segments.LastOrDefault();
                        Guid guid = Guid.Empty;
                        if (Guid.TryParse(guidString, out guid))
                        {
                            guids.Add(guid.ToString());
                        }
                    }
                }

                if (guids.Count > 0)
                {
                    foreach (var item in ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.ToList())
                    {
                        if (guids.Contains(item))
                            guids.Remove(item);
                    }
                    if (guids.Count > 0)
                    {
                        ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.AddRange(guids);
                        AsyncActions.Action(() =>
                        {
                            System.Threading.Thread.Sleep(2000);
                            List<Framesoft.UserFileInfoData> sendList = new List<Framesoft.UserFileInfoData>();
                            var sendingItems = ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.ToList();
                            foreach (var item in sendingItems)
                            {
                                sendList.Add(new Framesoft.UserFileInfoData() { FileGuid = Guid.Parse(item) });
                            }
                            if (sendList.Count > 0)
                            {
                                sendList.First().UserName = ApplicationSetting.Current.FramesoftSetting.UserName;
                                sendList.First().Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash();

                                var data = UserManagerHelper.SetCompleteUserFiles(sendList);
                                if (data.Message == "OK")
                                {
                                    ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.RemoveAll(x => sendingItems.Contains(x));
                                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                                }
                            }
                        });
                        Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    }
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "CheckAddFrameSoftLink");
            }

        }

        void NotificationInit()
        {
            ApplicationNotificationManager.Current.NotificationInfoChanged = (mode, linkInfo) =>
            {
                if (mode == Download.Web.NotificationMode.Complete)
                {
                    if (linkInfo != null)
                    {
                        Agrin.Download.Data.ApplicationServiceData.RemoveItem(linkInfo.PathInfo.Id);
                        try
                        {
                            AddToGallery(linkInfo.PathInfo.FullAddressFileName);
                            //Android.Media.MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { linkInfo.PathInfo.FullAddressFileName }, null, null);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            };
        }

        public abstract void AddToGallery(string fileName);
        public abstract void ShowToast(string msg, bool isLong);

        public bool Inited = false;
        public bool IsSetLanguage { get; set; }
        public bool Run()
        {
            //ClipboardHelper.InitializeClipboardChangedAction();
            //GoException("Run: " + Inited.ToString() + " " + DateTime.Now.ToString());
            if (!Inited)
            {
                Inited = true;
                Framesoft.Helper.FeedBackHelper.DefaultLimitMessage = Framesoft.Messages.LimitMessageEnum.Android;
                Agrin.Download.Engine.TimeDownloadEngine.IsNewVersionOfAndroid = true;

                Agrin.Download.Engine.TimeDownloadEngine.UpdatedAction = (update) =>
                {
                };

                Agrin.Download.Engine.TimeDownloadEngine.GetLastMessageAction = (update) =>
                {
                };

                Agrin.Download.Engine.TimeDownloadEngine.GetUserMessageAction = (userMessage) =>
                {
                };

                Agrin.Download.Web.Link.LinkInfoManagement.LinkInfoErrorAction = (error) =>
                {
                    this.RunOnUI(() =>
                    {
                        try
                        {
                            if (ApplicationSetting.Current.IsShowErrorMessageOnScreen)
                                ShowToast("خطا: " + Environment.NewLine + "" + error.Message, true);
                        }
                        catch (Exception ex)
                        {
                            GoException(ex, "LinkInfoErrorAction Toast");
                        }
                    });
                };
                Initialize();
                InitializeAppIO();
                ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
                ApplicationGroupManager.Current = new ApplicationGroupManager();
                ApplicationNotificationManager.Current = new ApplicationNotificationManager();
                ApplicationBalloonManager.Current = new ApplicationBalloonManager();


                ApplicationTaskManager.Current = new ApplicationTaskManager((taskInfo) =>
                {
                    if (taskInfo == null)
                    {
                        GoException("Start taskInfo == null");
                        return;
                    }
                    try
                    {
                        if (taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.WiFiOn))
                        {
                            DeviceHelper.Current.SetWifiEnable(true);
                        }
                        if (taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.InternetOn))
                        {
                            DeviceHelper.Current.SetMobileDataEnabled(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        GoException(ex, "ApplicationTaskManager.Current Start");
                    }
                }, (taskInfo) =>
                {
                    try
                    {
                        if (taskInfo == null)
                        {
                            GoException("Stop taskInfo == null");
                            return;
                        }
                        if (taskInfo.LinkCompleteTaskUtilityActions.Contains(TaskUtilityModeEnum.WiFiOff))
                        {
                            DeviceHelper.Current.SetWifiEnable(false);
                        }
                        if (taskInfo.LinkCompleteTaskUtilityActions.Contains(TaskUtilityModeEnum.InternetOff))
                        {
                            DeviceHelper.Current.SetMobileDataEnabled(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        GoException(ex, "ApplicationTaskManager.Current Stop");
                    }
                });


                ApplicationLinkInfoManager.Current.NotSupportResumableLinkAction = (link) =>
                {
                    ApplicationLinkInfoManager.Current.ClearDataForNotSupportResumableLink(link);
                    ApplicationLinkInfoManager.Current.PlayLinkInfo(link);
                };

                NotificationInit();

                Agrin.Download.Data.DeSerializeData.LoadApplicationData();
                bool mustSave = false;
                if (System.IO.Directory.Exists(ApplicationSetting.Current.PathSetting.DownloadsPath))
                    Agrin.IO.Helper.MPath.DownloadsPath = ApplicationSetting.Current.PathSetting.DownloadsPath;
                else
                {
                    mustSave = true;
                    ApplicationSetting.Current.PathSetting.DownloadsPath = Agrin.IO.Helper.MPath.DownloadsPath;
                }

                ApplicationBaseLoader.CreateGroups();
                //set app osSetting
                ApplicationSetting.Current.ApplicationOSSetting.Application = "Agrin Android";
                if (string.IsNullOrEmpty(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid))
                {
                    ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid = Guid.NewGuid().ToString();
                    mustSave = true;
                }

                ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion = DeviceHelper.Current.ApplicationVersion;
                ApplicationSetting.Current.ApplicationOSSetting.OSName = "Android";
                ApplicationSetting.Current.ApplicationOSSetting.OSVersion = DeviceHelper.Current.OSVersion;
                Agrin.Log.AutoLogger.AppVersion = ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion;
                if (!ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme)
                {
                    ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme = true;
                    ApplicationSetting.Current.IsSettingForAllLinks = true;
                    ApplicationSetting.Current.IsSettingForNewLinks = true;
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                }
                else if (mustSave)
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                LoadLanguage();
                //Agrin.Download.Manager.ApplicationGroupManager.Current.NoGroup.SavePath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath;

                Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();


            }

            //storage access framework
            IOHelper.OpenFileStreamForWriteAction = OpenFileStreamForWriteAction();

            if (Agrin.Download.Data.ApplicationServiceData.Current != null && Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks != null)
            {
                foreach (var id in Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks.ToArray())
                {
                    var link = Agrin.Download.Manager.ApplicationLinkInfoManager.Current.FindLinkInfoByID(id);
                    if (link != null)
                    {
                        if (link.DownloadingProperty.State == Agrin.Download.Web.ConnectionState.Complete)
                        {
                            CheckAddFrameSoftLink(link);
                            Agrin.Download.Data.ApplicationServiceData.RemoveItem(id);
                        }
                        else
                        {
                            Agrin.Download.Manager.ApplicationLinkInfoManager.Current.PlayLinkInfo(link);
                        }
                    }
                }
            }
            InitializeLimitDrawing();
            return true;
        }

        public abstract void InitializeLimitDrawing();
        public abstract void Initialize();
        public abstract void TriggerStorageAccessFramework();
        public abstract Func<string, string, FileMode, Action<string>, object, IStreamWriter> OpenFileStreamForWriteAction();
        
        public void InitializeAppIO()
        {
            Agrin.Log.AutoLogger.ApplicationDirectory = Agrin.IO.Helper.MPath.CurrentAppDirectory = System.IO.Path.Combine(StoragePublicDirectory, "AgrinData");

            string downloadsDirectory = DownloadsDirectory;
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

            Agrin.IO.Helper.MPath.InitializePath(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "DataBase"), downloadsDirectory, System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "DownloadingSaveData"));
        }
    }
}

