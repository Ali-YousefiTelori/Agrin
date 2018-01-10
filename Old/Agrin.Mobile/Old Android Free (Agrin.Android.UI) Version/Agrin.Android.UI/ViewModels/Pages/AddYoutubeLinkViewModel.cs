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
using System.ComponentModel;
using Agrin.Helper.ComponentModel;
using Agrin.Framesoft.Helper;
using Agrin.Download.Data.Settings;
using Agrin.MonoAndroid.UI.ViewModels.Lists;

namespace Agrin.MonoAndroid.UI
{
    public class AddYoutubeLinkViewModel : IBaseViewModel
    {
        public Activity CurrentActivity { get; set; }

        public static string CurrentSelectedURL = "";
        public static int CurrentSelectedFormatCode = -1;
        public static string FileName = "";

        public AddYoutubeLinkViewModel(Activity activity)
        {
            CurrentActivity = activity;
            Initialize();
        }

        Spinner spinner = null;
        EditText txt_Address = null;
        Button btnSelect = null, btnUploadToServer = null;

        public void Initialize()
        {
            var btnCheck = CurrentActivity.FindViewById<Button>(Resource.AddYoutubeLink.btnFindVideo);
            btnCheck.Click += btnCheckClick;

            btnSelect = CurrentActivity.FindViewById<Button>(Resource.AddYoutubeLink.btnSelect);
            btnSelect.Click += btnSelectClick;

            var btn_Extract = CurrentActivity.FindViewById<Button>(Resource.AddYoutubeLink.btn_Extract);
            btn_Extract.Click += btnExtractClick;

            btnUploadToServer = CurrentActivity.FindViewById<Button>(Resource.AddYoutubeLink.btnUploadToServer);
            btnUploadToServer.Click += btnUploadToServer_Click;
            btnUploadToServer.Visibility = (!string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.UserName) && !string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.Password)) ? ViewStates.Visible : ViewStates.Gone;
            btnUploadToServer.Enabled = false;

            spinner = CurrentActivity.FindViewById<Spinner>(Resource.AddYoutubeLink.cboFormats);
            spinner.Enabled = btnSelect.Enabled = false;

            txt_Address = CurrentActivity.FindViewById<EditText>(Resource.AddYoutubeLink.txt_Address);
            txt_Address.Text = AddYoutubeLinkViewModel.CurrentSelectedURL;
            if (!string.IsNullOrEmpty(AddYoutubeLinkViewModel.CurrentSelectedURL))
                btnExtractClick(null, null);
            CurrentSelectedURL = "";
            CurrentSelectedFormatCode = -1;
            //txt_Address.Text = "http://www.youtube.com/watch?v=7tPVAMXl6ds&feature=youtu.be";
        }

        void btnUploadToServer_Click(object sender, EventArgs e)
        {
            Agrin.MonoAndroid.UI.ViewModels.Toolbox.VIPToolbarViewModel.UploadLinkToServer(CurrentActivity, () =>
            {
                FramesoftLinksListDataViewModel.MustRefresh = true;
                ActivitesManager.VIPLinksActive(CurrentActivity);
            }, txt_Address.Text, true, linksItems[spinner.SelectedItemPosition].FormatCode);
        }

        List<Agrin.LinkExtractor.VideoInfo> linksItems = null;
        public bool IsFinished { get; set; }
        void btnCheckClick(object sender, EventArgs e)
        {
            var btn = sender as Button;
            txt_Address.Enabled = btn.Enabled = false;
            var progressDialog = ProgressDialog.Show(CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "FindingYoutubeVideos_Language"), ViewUtility.FindTextLanguage(CurrentActivity, "Downloading_Language"), true, false);
            string link = txt_Address.Text;
            AsyncActions.Action(() =>
            {
                Action<string> showMessageException = (message) =>
                {
                    CurrentActivity.RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();
                        progressDialog.Hide();
                        if (IsFinished)
                            return;
                        var builder = new AlertDialog.Builder(this.CurrentActivity);
                        builder.SetMessage(message);
                        builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "OK_Language"), (s, ee) =>
                        {
                        }).Create();
                        builder.Show();
                        txt_Address.Enabled = btn.Enabled = true;
                    });
                };
                try
                {
                    List<string> items = new List<string>();
                    bool mustGet = true;
                    if ((!string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.UserName) && !string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.Password)))
                    {
                        try
                        {
                            var data = UserManagerHelper.GetYoutubeVideoList(new Framesoft.UserFileInfoData() { UserName = ApplicationSetting.Current.FramesoftSetting.UserName, Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash(), Link = link });
                            if (data.Data != null)
                            {
                                linksItems = data.Data;
                                mustGet = false;
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                    if (mustGet)
                        linksItems = Agrin.LinkExtractor.DownloadUrlResolver.GetDownloadUrls(link).ToList();
                    if (IsFinished)
                    {
                        CurrentActivity.RunOnUiThread(() =>
                        {
                            progressDialog.Dismiss();
                            progressDialog.Hide();
                        });
                        return;
                    }

                    foreach (var item in linksItems)
                    {
                        string noSoundNoVideo = "";
                        if (item.AudioType == LinkExtractor.AudioType.Unknown && string.IsNullOrEmpty(item.AudioExtension))
                            noSoundNoVideo = ViewUtility.FindTextLanguage(CurrentActivity, "NoSound_Language");
                        if (item.VideoType == LinkExtractor.VideoType.Unknown && string.IsNullOrEmpty(item.VideoExtension))
                            noSoundNoVideo += ViewUtility.FindTextLanguage(CurrentActivity, "NoVideo_Language");
                        items.Add(ViewUtility.FindTextLanguage(CurrentActivity, "TypeTitle_Language") + " " + item.VideoType + " " + ViewUtility.FindTextLanguage(CurrentActivity, "ResolutionTitle_Language") + " " + item.Resolution + " " + noSoundNoVideo);
                        //items.Add ("item.VideoType" +" Resolution: "+"item.Resolution");
                    }
                    CurrentActivity.RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();
                        progressDialog.Hide();
                        var adapter = new ArrayAdapter<String>(CurrentActivity, Android.Resource.Layout.SimpleSpinnerItem, items);
                        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                        spinner.Adapter = adapter;
                        btnUploadToServer.Enabled = btnSelect.Enabled = spinner.Enabled = true;
                    });
                }
                catch (AggregateException c)
                {
                    if (c.InnerExceptions != null && c.InnerExceptions.Count > 0)
                    {
                        if (c.InnerExceptions[0].InnerException != null)
                        {
                            showMessageException(c.InnerExceptions[0].InnerException.Message);
                        }
                        else
                            showMessageException(c.InnerExceptions[0].Message);
                    }
                    else
                        showMessageException(c.Message);
                }
                catch (Exception ex)
                {
                    showMessageException(ex.Message);
                }
            });
        }

        void btnSelectClick(object sender, EventArgs e)
        {
            if (linksItems == null)
                return;
            IsFinished = true;
            var selItem = linksItems[spinner.SelectedItemPosition];
            FileName = Agrin.IO.Helper.MPath.GetFileNameValidChar(selItem.Title) + selItem.VideoExtension;
            CurrentSelectedURL = txt_Address.Text;//selItem.DownloadUrl;
            CurrentSelectedFormatCode = selItem.FormatCode;
            ActivitesManager.AddNewLinkActive(CurrentActivity);
        }

        void btnExtractClick(object sender, EventArgs e)
        {
            var list = Agrin.IO.Strings.HtmlPage.ExtractLinksFromHtml(txt_Address.Text);
            if (list.Count > 1)
            {
                (CurrentActivity as AddYoutubeLinkActivity).ShowListDialog(list.ToArray(), (value) =>
                {
                    txt_Address.Text = value;
                });
            }
            else
                txt_Address.Text = list.FirstOrDefault();
        }
    }
}
