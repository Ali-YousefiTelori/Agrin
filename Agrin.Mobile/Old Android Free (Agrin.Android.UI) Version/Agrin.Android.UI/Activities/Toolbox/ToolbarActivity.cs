
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
using Agrin.Download.Engine;
using Agrin.Download.Manager;

namespace Agrin.MonoAndroid.UI
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    //[IntentFilter (new[] { Intent.ActionSend }, Categories = new[] {Intent.CategoryDefault,Intent.CategoryBrowsable}, DataMimeType = "text/*")]
    public class ToolbarActivity : Activity
    {
        //public static ToolbarActivity This;
        protected override void OnCreate(Bundle bundle)
        {
            if (MainActivity.This == null)
                MainActivity.This = this;
            ActivitesManager.AddActivity(this);
            //This = this;
            base.OnCreate(bundle);
            // Create your application here
            SetContentView(Resource.Layout.Toolbar);
            this.Title = "AgrinDownloadManager_Language";
            ViewUtility.SetTextLanguage(this, new List<int>() { Resource.Toolbars.btn_AddYoutubeLinks, Resource.Toolbars.btn_AddLinks, Resource.Toolbars.btnGoToHome, Resource.Toolbars.btn_GroupManager, Resource.Toolbars.btn_Links, Resource.Toolbars.btnAbout, Resource.Toolbars.btn_Setting, Resource.Toolbars.btnVIP });
            //, Resource.Toolbars.btn_QueueManager

            ToolbarViewModel toolbar = new ToolbarViewModel(this);
            toolbar.Initialize();
            if (!string.IsNullOrEmpty(MainActivity.AgrinApplicationMessage))
            {
                try
                {
                    var appMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateInformation>(MainActivity.AgrinApplicationMessage);
                    InitializeApplication.ShowMessageDialog(appMessage, this);
                    MainActivity.AgrinApplicationMessage = "";
                }
                catch (Exception e)
                {
                    Agrin.Log.AutoLogger.LogError(e, "AgrinApplicationMessage");
                }
            }
            else if (!string.IsNullOrEmpty(MainActivity.AgrinApplicationUserMessage))
            {
                try
                {
                    var appMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageInformation>(MainActivity.AgrinApplicationUserMessage);
                    InitializeApplication.ShowMessageDialog(appMessage, this);
                    MainActivity.AgrinApplicationUserMessage = "";
                }
                catch (Exception e)
                {
                    Agrin.Log.AutoLogger.LogError(e, "AgrinApplicationMessage");
                }
            }
            else if (MainActivity.NotSupportResumableLinkAction != -1)
            {
                try
                {
                    var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID(MainActivity.NotSupportResumableLinkAction);
                    var nMgr = (NotificationManager)this.GetSystemService(Context.NotificationService);
                    nMgr.Cancel(InitializeApplication.MaxNotifyID + 4);
                    if (linkInfo != null)
                    {
                        var builder = new AlertDialog.Builder(this);
                        builder.SetTitle(linkInfo.PathInfo.FileName);
                        LinearLayout layout = new LinearLayout(this);
                        layout.Orientation = Orientation.Vertical;
                        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

                        layoutParams.SetMargins(5, 5, 5, 5);
                        layout.LayoutParameters = layoutParams;

                        TextView txtMessage = new TextView(this);
                        txtMessage.SetTextAppearance(this, global::Android.Resource.Style.TextAppearanceSmall);
                        txtMessage.SetSingleLine(false);
                        txtMessage.VerticalScrollBarEnabled = true;
                        txtMessage.Text = ViewUtility.FindTextLanguage(this, "YourLinkNotSupportResumable_Language");

                        layout.AddView(txtMessage);
                        ScrollView scroll = new ScrollView(this);
                        scroll.AddView(layout);
                        builder.SetView(scroll);

                        builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (s, ee) =>
                        {
                            try
                            {
                                //ApplicationLinkInfoManager.Current.StopLinkInfo(linkInfo);
                                //foreach (var item in linkInfo.Connections)
                                //{
                                //    System.IO.File.Delete(item.SaveFileName);
                                //    item.DownloadedSize = 0;
                                //}
                                //linkInfo.DownloadingProperty.DownloadedSize = 0;
                                //linkInfo.SaveThisLink();
                                ApplicationLinkInfoManager.Current.ClearDataForNotSupportResumableLink(linkInfo);
                                ApplicationLinkInfoManager.Current.PlayLinkInfo(linkInfo);
                            }
                            catch (Exception e)
                            {
                                Agrin.Log.AutoLogger.LogError(e, "OK_Language NotSupportResumableLinkAction");
                            }
                        });

                        builder.SetNegativeButton(ViewUtility.FindTextLanguage(this, "Cancel_Language"), (s, ee) =>
                        {
                            ApplicationLinkInfoManager.Current.StopLinkInfo(linkInfo);
                            MainActivity.NotSupportResumableLinkAction = -1;
                        });
                        builder.Show();

                        MainActivity.NotSupportResumableLinkAction = -1;
                    }

                }
                catch (Exception e)
                {
                    Agrin.Log.AutoLogger.LogError(e, "NotSupportResumableLinkAction");
                }
            }
        }

        public override void OnBackPressed()
        {
            ActivitesManager.GoToHomeApplication(this);
        }
        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            base.Finish();
        }

    }
}

