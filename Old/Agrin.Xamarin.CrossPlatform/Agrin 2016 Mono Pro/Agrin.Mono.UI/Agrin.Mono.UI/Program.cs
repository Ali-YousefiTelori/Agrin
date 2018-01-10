using System;
using Gtk;
using Agrin.Download.Manager;
using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.BaseViewModels;
using System.Linq;
using Agrin.Log;
using Agrin.Mono.UI;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace System
{
	public static class Extensions
	{
		public static void RunOnUI (this object obj, Action action)
		{
			if (Thread.CurrentThread == MainClass.MainThread) {
				try {
					action ();
				} catch (Exception ex) {
					Agrin.Log.AutoLogger.LogError (ex, "RunOnUI MainThread");
				}
				return;
			}
			//bool existOnBinding = BindingHelper.ExistObject (obj);
			ManualResetEvent resetEvent = new ManualResetEvent (false);
			Gtk.Application.Invoke (delegate {
				try {
					//if (existOnBinding) {
					//if (BindingHelper.ExistObject (obj))
					action ();
					//} else
					//	action ();
				} catch (Exception ex) {
					Agrin.Log.AutoLogger.LogError (ex, "RunOnUI");
				}
				resetEvent.Set ();
			});
			resetEvent.WaitOne ();
		}
	}
}
namespace Agrin.Mono.UI
{
	class MainClass
	{
		public static Thread MainThread = null;

		public static void Main (string[] args)
		{
			try {
				MainThread = Thread.CurrentThread;
				Application.Init ();
//				Gtk.Settings.Default.ThemeName = "Theme/gtk-2.0/gtkrc";
//				Gtk.Rc.Parse ("./Theme/gtk-2.0/gtkrc");

				Initialize ();
				MainWindow win = new MainWindow ();
				win.Show ();
				Application.Run ();
				System.Diagnostics.Process.GetCurrentProcess ().Kill ();
			} catch (Exception ex) {
				AutoLogger.LogError (ex, "MainClass Main", true);
			}

		}

		static void Initialize ()
		{
			ApplicationTaskManager.Current = new ApplicationTaskManager ((info) => {

			}, (info) => {

			});

			ApplicationTaskManager.Current.UserCompleteAction = () => {
				ApplicationTaskManager.Current.DeActiveTask (ApplicationTaskManager.Current.TaskInfoes.FirstOrDefault ());
			};

			Agrin.IO.Helper.MPath.InitializePath ();
			ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager ();
			ApplicationGroupManager.Current = new ApplicationGroupManager ();
			ApplicationNotificationManager.Current = new ApplicationNotificationManager ();
			ApplicationBalloonManager.Current = new ApplicationBalloonManager ();

			DeSerializeData.LoadApplicationData ();
			//Agrin.UI.ViewModels.Popups.BalloonViewModel.CreateBalloon();

			//notify.Click += notify_Click;
			//System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Agrin.UI;component/Project1.ico")).Stream;
			//notify.Icon = new System.Drawing.Icon(iconStream);

			//downloadManager.linksListData.CurrentToolbox = mainTopToolBox;

			//ser app setting
			ApplicationSetting.Current.ApplicationOSSetting.Application = "Agrin Mono UI";
			if (string.IsNullOrEmpty (ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid)) {
				ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid = Guid.NewGuid ().ToString ();
				SerializeData.SaveApplicationSettingToFile ();
			}

			ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			ApplicationSetting.Current.ApplicationOSSetting.OSName = SystemManager.GetOperatingSystem ().ToString ();
			ApplicationSetting.Current.ApplicationOSSetting.OSVersion = Environment.OSVersion.VersionString;


			ApplicationBaseLoader.CreateGroups ();
			Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate ();

			Agrin.Download.Engine.TimeDownloadEngine.UpdatedAction = (update) => {
				ApplicationLinkInfoManager.Current.RunOnUI (() => {
					Gtk.MessageDialog m = new Gtk.MessageDialog (MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Info, Gtk.ButtonsType.Ok, false, "نسخه ی جدید دانلود منیجر آگرین برای لینوکس و مک منتشر شده است");
					Gtk.ResponseType result = (Gtk.ResponseType)m.Run ();
					System.Diagnostics.Process.Start ("http://framesoft.ir");
					m.Destroy ();
				});
			};
//			Agrin.Download.Engine.TimeDownloadEngine.GetLastMessageAction = (update) =>
//			{
//				ApplicationHelper.EnterDispatcherThreadAction(() =>
//					{
//						if (!busyApplicationMessageControl.IsBusy)
//						{
//							busyApplicationMessageControl.Message = update.Message;
//							busyApplicationMessageControl.Command = new RelayCommand(() =>
//								{
//									ApplicationSetting.Current.LastApplicationMessageID = update.LastApplicationMessageID;
//									busyApplicationMessageControl.IsBusy = false;
//									Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
//								});
//							busyApplicationMessageControl.IsBusy = true;
//						}
//					});
//				//var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);
//
//				//var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
//				//var mainActivity = new Intent(currentActivity, typeof(MainActivity));
//				//mainActivity.SetFlags(ActivityFlags.NewTask);
//				//mainActivity.SetFlags(ActivityFlags.ClearTop);
//				//mainActivity.PutExtra("AgrinApplicationMessage", Newtonsoft.Json.JsonConvert.SerializeObject(update));
//				//var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
//				//notification.SetLatestEventInfo(currentActivity, ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageTitle_Language"), ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageMessage_Language"), pendingIntent);
//				//notification.Flags = NotificationFlags.AutoCancel;
//
//				//nMgr.Notify(MaxNotifyID + 2, notification);
//			};

			Agrin.Download.Engine.TimeDownloadEngine.GetUserMessageAction = (user) => {
				ApplicationLinkInfoManager.Current.RunOnUI (() => {
					Gtk.MessageDialog m = new Gtk.MessageDialog (MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Info, Gtk.ButtonsType.Ok, false, user.Message);
					Gtk.ResponseType result = (Gtk.ResponseType)m.Run ();
					ApplicationSetting.Current.LastUserMessageID = user.LastUserMessageID;
					Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile ();
					m.Destroy ();
				});
			};
		}
	}
}
