using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Agrin.Droid.Helpers;
using Agrin.Helper.ComponentModel;

namespace Agrin.Droid
{
	[Activity (Label = "Agrin", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
            try
            {
                if (Agrin.Log.AutoLogger.ApplicationDirectory == null)
                    Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");

                base.OnCreate(bundle);
                DeviceAndroidHelper.CreateInstance(this);
                ApplicationHelperBase.Current = new ViewModels.Helpers.ApplicationHelper();
                InitializeAndroidApplication.Current = new InitializeAndroidApplication();
                InitializeAndroidApplication.Current.DownloadsDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path;
                InitializeAndroidApplication.Current.StoragePublicDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path;
                InitializeAndroidApplication.Current.Initialize();
                global::Xamarin.Forms.Forms.Init(this, bundle);
                LoadApplication(new Agrin.App());
            }
            catch (Exception ex)
            {

            }
        }
	}
}

