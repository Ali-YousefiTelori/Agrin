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
using Agrin.Helpers;
using Android.Content.PM;
using Agrin.Views.Link;
using Agrin.Services;

namespace Agrin.Views
{
    [Activity(Name = "agrin.android.browseactivity", Theme = "@style/Theme.IconSplash", LaunchMode = LaunchMode.SingleTask, AlwaysRetainTaskState = true, ExcludeFromRecents = true, TaskAffinity = "", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class BrowseActivity : Activity
    {
        public static bool IsDestroy = true;
        public static BrowseActivity This;
        public static bool IsBrowse { get; set; }
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                Title = ViewsUtility.FindTextLanguage(this, "App_Name_Language");
                IsDestroy = false;
                IsBrowse = true;
                This = this;
                if (MainActivity.This == null)
                {
                    MainActivity.This = this;
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MainActivity.HandleExceptions);
                    AndroidEnvironment.UnhandledExceptionRaiser += MainActivity.HandleAndroidException;
                }
                base.OnCreate(bundle);

                //StartService(new Intent(this, typeof(AgrinService)));

                System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
                {
                    //System.Threading.Thread.Sleep(1000);
                    this.RunOnUI(() =>
                    {
                        try
                        {
                            if (AgrinService.This != null)
                                RunDialog();
                            else
                                StartService(new Intent(this, typeof(AgrinService)));
                        }
                        catch (Exception e)
                        {
                            InitializeApplication.GoException(e, "task StartService browse");
                        }
                    });
                });
                task.Start();

            }
            catch (Exception error)
            {
                InitializeApplication.GoException(error, "OnCreate browse");
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            //Agrin.Helper.ComponentModel.ANotifyPropertyChanged.StopNotifyChanging();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Agrin.Helper.ComponentModel.ANotifyPropertyChanged.StartNotifyChanging();
        }

        public static void TriggerStorageAccessFramework()
        {
            if ((int)Build.VERSION.SdkInt == 19)
            {
                Intent intent = new Intent(Intent.ActionCreateDocument)
                    .AddCategory(Intent.CategoryOpenable)
                    .SetType(Android.Provider.DocumentsContract.Document.MimeTypeDir);

                MainActivity.This.StartActivityForResult(intent, 42);
            }
            else
            {
                Android.Content.Intent intent = new Android.Content.Intent(Intent.ActionOpenDocumentTree);
                MainActivity.This.StartActivityForResult(intent, 42);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 42)
            {
                Android.Net.Uri treeUri = null;
                if (resultCode == Result.Ok)
                {
                    // Get Uri from Storage Access Framework.
                    treeUri = data.Data;
                    var takeFlags = data.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                    // Check for the freshest data.
                    ContentResolver.TakePersistableUriPermission(treeUri, takeFlags);
                    if (MainActivity.ActionSetPermission != null)
                        MainActivity.ActionSetPermission(treeUri);
                }
                // Persist URI in shared preference so that you can use it later.
                // Use your own framework here instead of PreferenceUtil.
                //    PreferenceUtil.setSharedPreferenceUri(R.string.key_internal_uri_extsdcard, treeUri);

                //    // Persist access permissions.
                //    int takeFlags = resultData.getFlags()
                //        & (Android.Content.Intent.FLAG_GRANT_READ_URI_PERMISSION | Intent.FLAG_GRANT_WRITE_URI_PERMISSION);
                //getActivity().getContentResolver().takePersistableUriPermission(treeUri, takeFlags);
            }

        }
        public void RunDialog()
        {
            IsBrowse = false;
            AddLinks addLinks = new AddLinks(this, () =>
            {
                this.Finish();
            }, GetAllExtras(this), true);
        }

        protected override void OnDestroy()
        {
            IsDestroy = true;
            try
            {
                Agrin.Download.Data.SerializeData.CloseApplicationWaitForSavingAllComplete();
                AgrinService.StopServiceIfNotNeed();
            }
            catch(Exception ex)
            {
                Log.AutoLogger.LogError(ex, "BroweseActivity OnDestroy");
            }
            base.OnDestroy();
        }

        string GetAllExtras(Activity activity)
        {
            StringBuilder builder = new StringBuilder();
            if (activity.Intent.HasExtra(Intent.ExtraText) && !String.IsNullOrEmpty(activity.Intent.GetStringExtra(Intent.ExtraText)))
                builder.AppendLine(activity.Intent.GetStringExtra(Intent.ExtraText));
            else if (activity.Intent.HasExtra(Intent.ExtraTitle) && !String.IsNullOrEmpty(activity.Intent.GetStringExtra(Intent.ExtraTitle)))
                builder.AppendLine(activity.Intent.GetStringExtra(Intent.ExtraTitle));
            else if (activity.Intent.HasExtra(Intent.ExtraSubject) && !String.IsNullOrEmpty(activity.Intent.GetStringExtra(Intent.ExtraSubject)))
                builder.AppendLine(activity.Intent.GetStringExtra(Intent.ExtraSubject));
            else if (activity.Intent.ToUri(IntentUriType.None) != null)
            {
                builder.AppendLine(activity.Intent.ToUri(IntentUriType.None).Replace("#", " "));
            }
            Action<string> getItems = (name) =>
            {
                var text = activity.Intent.GetStringArrayExtra(name);
                if (text != null && text.Length > 0)
                    foreach (var item in text)
                    {
                        builder.AppendLine(item);
                    }
            };
            getItems(Intent.ExtraText);
            getItems(Intent.ExtraTitle);
            getItems(Intent.ExtraSubject);
            return builder.ToString();
        }

        public override void Finish()
        {
            Agrin.Download.Data.SerializeData.CloseApplicationWaitForSavingAllComplete();
            base.Finish();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }
    }
}