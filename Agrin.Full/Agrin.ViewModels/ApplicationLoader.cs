using Agrin.BaseViewModels;
using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Threading;

namespace Agrin.ViewModels
{
    public static class ApplicationLoader
    {
        public static void LoadApplicationData(Dispatcher dispatcher, Version version)
        {
            ApplicationHelperBase.DispatcherThread = dispatcher;
            Agrin.IO.Helper.MPath.InitializePath();
            ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
            ApplicationGroupManager.Current = new ApplicationGroupManager();
            ApplicationNotificationManager.Current = new ApplicationNotificationManager();
            ApplicationBalloonManager.Current = new ApplicationBalloonManager();

            DeSerializeData.LoadApplicationData();
            //Agrin.UI.ViewModels.Popups.BalloonViewModel.CreateBalloon();

            //notify.Click += notify_Click;
            //System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Agrin.UI;component/Project1.ico")).Stream;
            //notify.Icon = new System.Drawing.Icon(iconStream);

            //downloadManager.linksListData.CurrentToolbox = mainTopToolBox;

            //ser app setting
            ApplicationSetting.Current.ApplicationOSSetting.Application = "Agrin WPF Windows";
            if (string.IsNullOrEmpty(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid))
            {
                ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid = Guid.NewGuid().ToString();
                SerializeData.SaveApplicationSettingToFile();
            }
            ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion = version.ToString();
            ApplicationSetting.Current.ApplicationOSSetting.OSName = Agrin.OS.Management.OSSystemInfo.GetSystemInformation();
            ApplicationSetting.Current.ApplicationOSSetting.OSVersion = Environment.OSVersion.VersionString;


            ApplicationHelperBase.DispatcherThread = dispatcher;
            //IsShowToolbar = true;
            //Agrin.UI.ViewModels.Toolbox.ToolbarViewModel.This.PinCommand.Execute();
            //var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            //txtVersion.Text = "نسخه آزمایشی " + version.ToString() + " بهمن 1393";
            ApplicationBaseLoader.CreateGroups();
            Agrin.Log.AutoLogger.AppVersion = ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion;
            Agrin.Log.AutoLogger.ApplicationGuid = ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid;
        }
    }
}
