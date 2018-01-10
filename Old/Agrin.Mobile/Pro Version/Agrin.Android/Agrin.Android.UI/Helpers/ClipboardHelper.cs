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
using Agrin.Views;
using Agrin.IO.Strings;

namespace Agrin.Helpers
{
    public static class ClipboardHelper
    {
        public static string Text
        {
            get
            {
                try
                {

                    Android.Text.ClipboardManager clipboardManager = (Android.Text.ClipboardManager)InitializeApplication.CurrentContext.GetSystemService(Context.ClipboardService);
                    return clipboardManager.Text;
                }
                catch
                {

                }
                return "";
            }
            set
            {
                try
                {
                    Android.Text.ClipboardManager clipboardManager = (Android.Text.ClipboardManager)InitializeApplication.CurrentContext.GetSystemService(Context.ClipboardService);
                    clipboardManager.Text = value;
                }
                catch
                {

                }
            }
        }

        public static Action<string> ChangedClipboard { get; set; }

        static bool initialized = false;
        static ClipboardManager clipboard = null;
        public static void InitializeClipboardChangedAction()
        {
            if (initialized)
                return;
            initialized = true;
            int sdk = (int)Android.OS.Build.VERSION.SdkInt;
            if (sdk < 11)
            {
                string currentText = Text;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    while (initialized)
                    {
                        try
                        {
                            var newText = Text;
                            if (currentText != newText)
                            {
                                currentText = newText;
                                StartActivityByChangedClipboard(currentText);
                            }
                        }
                        catch
                        {

                        }
                        System.Threading.Thread.Sleep(1500);
                    }
                });
                thread.Start();
            }
            else
            {
                if (clipboard == null)
                    clipboard = (ClipboardManager)InitializeApplication.CurrentContext.GetSystemService(Context.ClipboardService);
                clipboard.PrimaryClipChanged -= clipboard_PrimaryClipChanged;
                clipboard.PrimaryClipChanged += clipboard_PrimaryClipChanged;
            }
        }

        public static void Stop()
        {
            initialized = false;
            int sdk = (int)Android.OS.Build.VERSION.SdkInt;
            if (sdk >= 11)
            {
                if (clipboard == null)
                    clipboard = (ClipboardManager)InitializeApplication.CurrentContext.GetSystemService(Context.ClipboardService);
                clipboard.PrimaryClipChanged -= clipboard_PrimaryClipChanged;
            }
        }

        static void clipboard_PrimaryClipChanged(object sender, EventArgs e)
        {
            StartActivityByChangedClipboard(Text);
        }

        static void StartActivityByChangedClipboard(string newText)
        {
            if (!initialized)
                return;
            var links = HtmlPage.ExtractLinksFromHtmlTwo(newText);
            if (links.Count > 0)
            {
                var link = links.First();
                //ActivityManager am = (ActivityManager)InitializeApplication.CurrentContext.GetSystemService(Context.ActivityService);

                // get the info from the currently running task
                //var taskInfo = am.GetRunningTasks(1);
                // ComponentName componentInfo = taskInfo[0].TopActivity;
                Intent intent = new Intent(InitializeApplication.CurrentContext, typeof(BrowseActivity));
                intent.PutExtra(Intent.ExtraText, link);
                intent.SetFlags(ActivityFlags.NewTask);
                InitializeApplication.CurrentContext.StartActivity(intent);
            }
        }
    }
}