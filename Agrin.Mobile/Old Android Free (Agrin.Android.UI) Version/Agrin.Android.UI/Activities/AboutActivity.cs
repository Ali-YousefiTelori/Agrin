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
using Agrin.Helper.ComponentModel;
using Agrin.Download.Data.Settings;
using Android.Webkit;
using Android.Graphics.Drawables;
using Agrin.Download.Engine;

namespace Agrin.MonoAndroid.UI
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden, AlwaysRetainTaskState = true)]
    public class AboutActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);

            this.SetContentView(Resource.Layout.About);
            this.Title = "About_Language";
            if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.AboutPage.ReverceLinearLayout1, Resource.AboutPage.ReverceLinearLayout2, Resource.AboutPage.ReverceLinearLayout3 });
                ViewUtility.SetRightToLeftViews(this, new List<int>() { Resource.AboutPage.chkCheckLastVesrion, Resource.AboutPage.chkGetApplicationMessage });
            }
            else
            {
                ViewUtility.SetLeftToRightCheckBox(this, Resource.AboutPage.chkGetApplicationMessage);
                ViewUtility.SetLeftToRightCheckBox(this, Resource.AboutPage.chkCheckLastVesrion);
            }

            ViewUtility.SetTextLanguage(this, new List<int>() { Resource.AboutPage.TxtProgrammer, Resource.AboutPage.TxtProgrammerTitle, Resource.AboutPage.TxtSite, Resource.AboutPage.TxtSiteTitle, Resource.AboutPage.TxtVersionTitle, Resource.AboutPage.TxtMessage, Resource.AboutPage.btnSendFeedBack, Resource.AboutPage.btnLearning, Resource.AboutPage.chkCheckLastVesrion, Resource.AboutPage.chkGetApplicationMessage, Resource.AboutPage.btnCheckManualVersion });
            // Create your application here
            var textVersion = this.FindViewById<TextView>(Resource.AboutPage.TxtVersion);
            textVersion.Text = Agrin.Download.Data.Settings.ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion;

            var btnSendFeedBack = this.FindViewById<Button>(Resource.AboutPage.btnSendFeedBack);
            btnSendFeedBack.Click += btnSendFeedBack_Click;
            var btnLearning = this.FindViewById<Button>(Resource.AboutPage.btnLearning);
            btnLearning.Click += btnLearning_Click;

            var chkGetLastVersion = this.FindViewById<CheckBox>(Resource.AboutPage.chkCheckLastVesrion);
            chkGetLastVersion.Checked = !ApplicationSetting.Current.NoCheckLastVersion;
            chkGetLastVersion.CheckedChange += chkGetLastVersion_CheckedChange;

            var chkGetLastMessage = this.FindViewById<CheckBox>(Resource.AboutPage.chkGetApplicationMessage);
            chkGetLastMessage.Checked = !ApplicationSetting.Current.NoGetApplicationMessage;
            chkGetLastMessage.CheckedChange += chkGetLastMessage_CheckedChange;

            var btnCheckVersion = this.FindViewById<Button>(Resource.AboutPage.btnCheckManualVersion);
            btnCheckVersion.Click += btnCheckVersion_Click;
        }

        void btnCheckVersion_Click(object sender, EventArgs e)
        {
            var btnCheckVersion = (Button)sender;
            btnCheckVersion.Enabled = false;
            Toast.MakeText(this, ViewUtility.FindTextLanguage(this, "Checking_Language"), ToastLength.Short).Show();
            AsyncActions.Action(() =>
            {
                TimeDownloadEngine.CheckLastVersion(true);
                this.RunOnUiThread(() =>
                {
                    btnCheckVersion.Enabled = true;
                    Toast.MakeText(this, ViewUtility.FindTextLanguage(this, "CheckCompleteMessageYouInNotification_Language"), ToastLength.Long).Show();
                });
            }, (error) =>
            {
                this.RunOnUiThread(() =>
                {
                    btnCheckVersion.Enabled = true;
                    Toast.MakeText(this, ViewUtility.FindTextLanguage(this, "ErrorChecking_Language"), ToastLength.Long).Show();
                });
            });
        }

        void chkGetLastMessage_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var chkGetLastMessage = (CheckBox)sender;
            ApplicationSetting.Current.NoGetApplicationMessage = !chkGetLastMessage.Checked;
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
        }

        void chkGetLastVersion_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var chkGetLastVersion = (CheckBox)sender;
            ApplicationSetting.Current.NoCheckLastVersion = !chkGetLastVersion.Checked;
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
        }

        void btnLearning_Click(object sender, EventArgs e)
        {
            ViewUtility.ShowPageDialog("http://framesoft.ir/Learning/Android", "AboutLearning_Language", null, this);
            //AlertDialog.Builder builder = new AlertDialog.Builder(this);
            //builder.SetTitle(ViewUtility.FindTextLanguage(this, ""));
            //LinearLayout layout = new LinearLayout(this);
            //layout.Orientation = Orientation.Vertical;
            //LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            //layoutParams.SetMargins(5, 5, 5, 5);
            //layout.LayoutParameters = layoutParams;

            //var mWebview = new WebView(this);

            //mWebview.Settings.JavaScriptEnabled = true; // enable javascript
            //var wclient = new CustomWebViewClient() { activity = this };

            //mWebview.SetWebViewClient(wclient);
            //TextView txt = new TextView(this);
            //txt.Text = ViewUtility.FindTextLanguage(this, "Loading_Language");
            //txt.SetTextAppearance(this, Android.Resource.Attribute.TextAppearanceLarge);
            //wclient.LoadComepleteAction = () =>
            //    {
            //        mWebview.Visibility = ViewStates.Visible;
            //        txt.Visibility = ViewStates.Gone;
            //    };
            //mWebview.LoadUrl("");
            //mWebview.SetMinimumHeight(200);
            //mWebview.Visibility = ViewStates.Gone;

            //layout.AddView(mWebview);
            //layout.AddView(txt);

            //builder.SetView(layout);
            //AlertDialog dialogW = null;

            //builder.SetNegativeButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (EventHandler<DialogClickEventArgs>)null);

            //dialogW = builder.Create();
            //dialogW.Show();

            //// Get the buttons.
            //var noBtn = dialogW.GetButton((int)DialogButtonType.Negative);

            //noBtn.Click += (s, args) =>
            //{
            //    dialogW.Dismiss();
            //};
        }

        void btnSendFeedBack_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(ViewUtility.FindTextLanguage(this, "AboutQuestion_Language"));
            LinearLayout layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            EditText txtMessage = new EditText(this);
            txtMessage.SetTextColor(Android.Graphics.Color.Black);
            txtMessage.SetTextAppearance(this, global::Android.Resource.Style.TextAppearanceSmall);
            txtMessage.SetSingleLine(false);
            txtMessage.VerticalScrollBarEnabled = true;
            txtMessage.Hint = ViewUtility.FindTextLanguage(this, "TypeYourMessage_Language");
            txtMessage.Gravity = GravityFlags.Top | GravityFlags.Left;
            txtMessage.SetMinimumHeight(200);
            layout.AddView(txtMessage);
            builder.SetView(layout);
            bool isSending = false;
            AlertDialog dialogW = null;
            builder.SetCancelable(false);

            builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "Send_Language"), (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(ViewUtility.FindTextLanguage(this, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);

            dialogW = builder.Create();

            dialogW.Show();
            dialogW.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Black));
            // Get the buttons.
            var yesBtn = dialogW.GetButton((int)DialogButtonType.Positive);
            var noBtn = dialogW.GetButton((int)DialogButtonType.Negative);

            // Assign our handlers.
            yesBtn.Click += (s, args) =>
            {
                if (isSending)
                    return;
                isSending = true;
                string msg = txtMessage.Text;
                txtMessage.Text = ViewUtility.FindTextLanguage(this, "Sending_Language");
                txtMessage.Enabled = false;
                yesBtn.Enabled = false;
                noBtn.Enabled = false;
                AsyncActions.Action(() =>
                {
                    var send = Agrin.About.SendMessage.SendFeedBack(new About.UserMessage() { GUID = ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid, message = msg, LastUserMessageID = ApplicationSetting.Current.LastUserMessageID });
                    this.RunOnUiThread(() =>
                    {
                        txtMessage.Text = send ? ViewUtility.FindTextLanguage(this, "SendSuccess_Language") : ViewUtility.FindTextLanguage(this, "SendError_Language");
                    });
                    System.Threading.Thread.Sleep(2000);
                    this.RunOnUiThread(() =>
                    {
                        if (send)
                            dialogW.Dismiss();
                        else
                        {
                            txtMessage.Text = msg;
                            txtMessage.Enabled = true;
                        }
                        isSending = false;
                        yesBtn.Enabled = true;
                        noBtn.Enabled = true;
                    });
                }, (ex) =>
                    {
                        this.RunOnUiThread(() =>
                        {
                            txtMessage.Text = ex.Message;
                            System.Threading.Thread.Sleep(2000);
                            txtMessage.Text = msg;
                            txtMessage.Enabled = true;
                            yesBtn.Enabled = true;
                            noBtn.Enabled = true;
                        });
                        isSending = false;
                    });
            };
            noBtn.Click += (s, args) =>
            {
                dialogW.Dismiss();
            };

        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            ActivitesManager.ToolbarActive(null);
            base.Finish();
        }
    }

    public class CustomWebViewClient : WebViewClient
    {
        public Activity activity { get; set; }
        public Action<bool> LoadComepleteAction { get; set; }
        public Action ErrorLoadingAction { get; set; }
        bool isError = false;
        public override void OnLoadResource(WebView view, string url)
        {
            isError = false;
            base.OnLoadResource(view, url);
        }

        public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
        {
            isError = true;
            if (ErrorLoadingAction != null)
                ErrorLoadingAction();
            Toast.MakeText(activity, description, ToastLength.Short).Show();
        }

        public override void OnPageFinished(WebView view, string url)
        {
            if (LoadComepleteAction != null)
                LoadComepleteAction(isError);
            base.OnPageFinished(view, url);
        }
    }
}

