using System;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Agrin.Helpers;
using System.Threading.Tasks;
using System.Threading;
using Agrin.Download.Manager;

namespace Agrin.Views.Settings
{
    public class ApplicationSettingsView : IDisposable
    {
        Activity CurrentActivity { get; set; }

        LinearLayout _mainLayout;
        LinearLayout _browsLayout;
        LinearLayout _actionToolBoxLayout;
        LinearLayout _actionTopToolBoxLayout;
        View mainView;
        public ApplicationSettingsView(Activity context, LinearLayout mainLayout, LinearLayout actionToolBoxLayout, LinearLayout actionTopToolBoxLayout)
        {
            CurrentActivity = context;
            _mainLayout = mainLayout;
            _actionToolBoxLayout = actionToolBoxLayout;
            _actionTopToolBoxLayout = actionTopToolBoxLayout;

            mainView = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.ApplicationSettings, mainLayout, false);

            var btnbrowseDownloadsPath = mainView.FindViewById<Button>(Resource.ApplicationSettings.btn_browseDownloadsPath);
            btnbrowseDownloadsPath.Click += btnbrowseDownloadsPath_Click;
            var btnbrowseDataDownloadsPath = mainView.FindViewById<Button>(Resource.ApplicationSettings.btn_browseDataDownloadsPath);
            btnbrowseDataDownloadsPath.Click += BtnbrowseDataDownloadsPath_Click;
            var btnSaveSetting = mainView.FindViewById<Button>(Resource.ApplicationSettings.btnSaveSetting);
            btnSaveSetting.Click += btnSaveSetting_Click;

            _browsLayout = mainView.FindViewById<LinearLayout>(Resource.ApplicationSettings.mainLayout);

            txtDownloadsPath = mainView.FindViewById<EditText>(Resource.ApplicationSettings.txt_DownloadsPath);
            txtDataDownloadsPath = mainView.FindViewById<EditText>(Resource.ApplicationSettings.txt_DataDownloadsPath);
            context.RunOnUI(() =>
            {
                txtDownloadsPath.Enabled = false;
                txtDataDownloadsPath.Enabled = false;
            });
            txtConnectionCount = mainView.FindViewById<TextView>(Resource.ApplicationSettings.txtConnectionCount);
            txtLimit = mainView.FindViewById<TextView>(Resource.ApplicationSettings.txtLimit);

            chkIsLimit = mainView.FindViewById<CheckBox>(Resource.ApplicationSettings.chkIsLimitTitle);
            chkIsScreenOnWhenDownloading = mainView.FindViewById<CheckBox>(Resource.ApplicationSettings.chkIsScreenOnWhenDownloading);
            chkIsShowErrorOnScreen = mainView.FindViewById<CheckBox>(Resource.ApplicationSettings.chkIsShowErrorOnScreen);
            chkIsForceScreenWhenQueueActive = mainView.FindViewById<CheckBox>(Resource.ApplicationSettings.chkIsForceScreenWhenQueueActive);
            chkIsAutoClipboard = mainView.FindViewById<CheckBox>(Resource.ApplicationSettings.chkIsAutoClipboard);
            //chkIsEnableNotify = CurrentActivity.FindViewById<CheckBox>(Resource.ApplicationSettings.chkEnableNotifyTitle);
            seekBarConnectionCount = mainView.FindViewById<SeekBar>(Resource.ApplicationSettings.seekBarConnectionCount);
            seekBarConnectionCount.ProgressChanged += seekBarConnectionCount_ProgressChanged;

            seekBarLimit = mainView.FindViewById<SeekBar>(Resource.ApplicationSettings.seekBarLimit);
            seekBarLimit.IncrementProgressBy(5);
            seekBarLimit.ProgressChanged += seekBarLimit_ProgressChanged;

            ViewsUtility.SetRightToLeftLayout(mainView, new List<int>() { Resource.ApplicationSettings.LinearLayoutRightToLeft1, Resource.ApplicationSettings.LinearLayoutRightToLeft2, Resource.ApplicationSettings.LinearLayoutRightToLeft3, Resource.ApplicationSettings.LinearLayoutRightToLeft4, Resource.ApplicationSettings.LinearLayoutRightToLeft5, Resource.ApplicationSettings.LinearLayoutRightToLeft6, Resource.ApplicationSettings.LinearLayoutRightToLeft7, Resource.ApplicationSettings.LinearLayoutRightToLeft8 });

            //ViewsUtility.SetRightToLeftViews(mainView, new List<int>() { Resource.ApplicationSettings.chkIsLimitTitle, Resource.ApplicationSettings.txtConnectionCountTitle, Resource.ApplicationSettings.txtDownloadsPathTitle });
            ViewsUtility.SetTextLanguage(CurrentActivity, mainView, new List<int>() { Resource.ApplicationSettings.btn_browseDownloadsPath, Resource.ApplicationSettings.btnSaveSetting, Resource.ApplicationSettings.chkIsLimitTitle, Resource.ApplicationSettings.chkIsScreenOnWhenDownloading, Resource.ApplicationSettings.chkIsShowErrorOnScreen, Resource.ApplicationSettings.txtConnectionCountTitle, Resource.ApplicationSettings.txtDownloadsPathTitle, Resource.ApplicationSettings.txtDataDownloadsPathTitle, Resource.ApplicationSettings.btn_browseDataDownloadsPath, Resource.ApplicationSettings.chkIsAutoClipboard, Resource.ApplicationSettings.chkIsForceScreenWhenQueueActive });

            InitializeView();
        }


        public void InitializeView()
        {
            InitializeData();
            if (_actionToolBoxLayout.ChildCount > 0)
                _actionToolBoxLayout.RemoveAllViews();
            _actionToolBoxLayout.Visibility = ViewStates.Gone;

            if (_actionTopToolBoxLayout.ChildCount > 0)
                _actionTopToolBoxLayout.RemoveAllViews();
            _actionTopToolBoxLayout.Visibility = ViewStates.Gone;

            if (_mainLayout.ChildCount > 0)
                _mainLayout.RemoveAllViews();
            _mainLayout.AddView(mainView);
        }

        EditText txtDownloadsPath = null, txtDataDownloadsPath = null;
        TextView txtConnectionCount = null, txtLimit = null;
        CheckBox chkIsLimit = null, chkIsEnableNotify = null, chkIsScreenOnWhenDownloading = null, chkIsShowErrorOnScreen = null, chkIsForceScreenWhenQueueActive = null, chkIsAutoClipboard = null;
        SeekBar seekBarConnectionCount = null, seekBarLimit = null;
        public void InitializeData()
        {
            _SecurityPath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.SecurityPath;
            txtDownloadsPath.Text = string.IsNullOrEmpty(_SecurityPath) ? Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath : _SecurityPath;
            txtDataDownloadsPath.Text = IO.Helper.MPath.SaveDataPath;
            seekBarConnectionCount.Progress = 1;
            seekBarConnectionCount.Progress = Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.ConnectionCount - 1;
            seekBarLimit.Progress = (Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.SpeedSize / 1024) - 1;

            chkIsLimit.Checked = Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.IsLimit;
            chkIsScreenOnWhenDownloading.Checked = Agrin.Download.Data.Settings.ApplicationSetting.Current.IsTurnOnScreenWhenDownloading;
            chkIsShowErrorOnScreen.Checked = Agrin.Download.Data.Settings.ApplicationSetting.Current.IsShowErrorMessageOnScreen;

            chkIsForceScreenWhenQueueActive.Checked = Agrin.Download.Data.Settings.ApplicationSetting.Current.IsTurnOnScreenWhenQueueIsActivated;
            chkIsAutoClipboard.Checked = Services.AgrinService.IsClipboardOn;

            //chkIsEnableNotify.Checked = Agrin.Download.Data.Settings.ApplicationSetting.Current.IsShowNotification;

            seekBarLimit_ProgressChanged(null, new SeekBar.ProgressChangedEventArgs(seekBarLimit, seekBarLimit.Progress, false));
            seekBarConnectionCount_ProgressChanged(null, new SeekBar.ProgressChangedEventArgs(seekBarConnectionCount, seekBarConnectionCount.Progress, false));
        }

        void seekBarLimit_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            var prog = e.Progress + 1;
            prog = prog / 5;
            prog = prog * 5;
            if (prog == 0)
                prog = 1;
            var str = prog + " " + ViewsUtility.FindTextLanguage(CurrentActivity, "KB_Language");
            txtLimit.Text = str;
        }

        void seekBarConnectionCount_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            txtConnectionCount.Text = (e.Progress + 1).ToString();
        }

        string _SecurityPath = "";
        void btnSaveSetting_Click(object sender, EventArgs e)
        {
            Agrin.Download.Data.Settings.ApplicationSetting.Current.IsSettingForNewLinks = true;
            Agrin.Download.Data.Settings.ApplicationSetting.Current.IsSettingForAllLinks = true;
            Button btnSaveSetting = sender as Button;
            btnSaveSetting.Enabled = false;
            if (!string.IsNullOrEmpty(_SecurityPath) || System.IO.Directory.Exists(txtDownloadsPath.Text))
            {
                if (!string.IsNullOrEmpty(_SecurityPath))
                {
                    if ((int)Build.VERSION.SdkInt == 19)
                    {
                        Toast.MakeText(CurrentActivity, "این مسیر برای نرم افزار قابل دسترسی نیست", ToastLength.Short).Show();
                        return;
                    }
                    Agrin.IO.Helper.MPath.SecurityPath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.SecurityPath = _SecurityPath;
                    try
                    {

                        Android.Net.Uri path = Android.Net.Uri.Parse(_SecurityPath);
                        Android.Net.Uri docUri = null;
                        docUri = Android.Provider.DocumentsContract.BuildDocumentUriUsingTree(path, Android.Provider.DocumentsContract.GetTreeDocumentId(path));

                        foreach (var item in ApplicationGroupManager.Current.GroupInfoes)
                        {
                            Android.Net.Uri childUri = Android.Provider.DocumentsContract.CreateDocument(CurrentActivity.ContentResolver, docUri, Android.Provider.DocumentsContract.Document.MimeTypeDir, item.SaveFolderName);
                            item.UserSecurityPath = childUri.ToString();
                        }
                        Agrin.Download.Data.SerializeData.SaveGroupInfoesToFile();
                    }
                    catch (Exception ex)
                    {
                        InitializeApplication.GoException(ex);
                    }
                }
                else
                    Agrin.IO.Helper.MPath.DownloadsPath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath = txtDownloadsPath.Text;
            }
            else
            {
                Toast.MakeText(CurrentActivity, ViewsUtility.FindTextLanguage(CurrentActivity, "InvalidAddress_Language"), ToastLength.Short).Show();
                return;
            }
            if (string.IsNullOrEmpty(_SecurityPath))
                Agrin.IO.Helper.MPath.SecurityPath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.SecurityPath = _SecurityPath;

            Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.ConnectionCount = seekBarConnectionCount.Progress + 1;
            var prog = seekBarLimit.Progress + 1;
            prog = prog / 5;
            prog = prog * 5;
            if (prog == 0)
                prog = 1;
            Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.SpeedSize = prog * 1024;

            Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.IsLimit = chkIsLimit.Checked;
            Agrin.Download.Data.Settings.ApplicationSetting.Current.IsTurnOnScreenWhenDownloading = chkIsScreenOnWhenDownloading.Checked;
            Agrin.Download.Data.Settings.ApplicationSetting.Current.IsShowErrorMessageOnScreen = chkIsShowErrorOnScreen.Checked;

            //Agrin.Download.Data.Settings.ApplicationSetting.Current.IsShowNotification = chkIsEnableNotify.Checked;
            if (chkIsAutoClipboard.Checked)
            {
                ClipboardHelper.InitializeClipboardChangedAction();
                Services.AgrinService.IsClipboardOn = true;
            }
            else
            {
                ClipboardHelper.Stop();
                Services.AgrinService.IsClipboardOn = false;
            }


            Agrin.Download.Data.Settings.ApplicationSetting.Current.IsTurnOnScreenWhenQueueIsActivated = chkIsForceScreenWhenQueueActive.Checked;
            Services.AgrinService.This.CheckAppForeground();
            Services.AgrinService.StopServiceIfNotNeed();
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            Toast.MakeText(CurrentActivity, "تنظیمات شما با موفقیت ذخیره شد", ToastLength.Long).Show();
            Task task = new Task(() =>
            {
                Thread.Sleep(2000);
                CurrentActivity?.RunOnUI(() =>
                {
                    btnSaveSetting.Enabled = true;
                });
            });
            task.Start();
        }

        void btnbrowseDownloadsPath_Click(object sender, EventArgs e)
        {
            ViewsUtility.ShowFolderBrowseInLayout(CurrentActivity, _browsLayout, Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath, (path, isSecurityPath) =>
            {
                if (isSecurityPath)
                {
                    _SecurityPath = path;
                    txtDownloadsPath.Text = _SecurityPath;
                }
                else
                {
                    _SecurityPath = null;
                    txtDownloadsPath.Text = path;
                }
            });
        }

        private void BtnbrowseDataDownloadsPath_Click(object sender, EventArgs e)
        {
            ViewsUtility.ShowFolderBrowseInLayout(CurrentActivity, _browsLayout, IO.Helper.MPath.SaveDataPath, (path, isSecurityPath) =>
            {
                if (isSecurityPath)
                {
                    Toast.MakeText(CurrentActivity, "آدرس انتخاب شده نمی تواند یک آدرس امنیتی باشد!", ToastLength.Long).Show();
                }
                else
                {
                    if (path.TrimEnd('/').ToLower().Contains(Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.SaveDataPath.TrimEnd('/').ToLower()))
                    {
                        Toast.MakeText(CurrentActivity, "آدرس انتخاب شده نباید درون آدرس آدرس قبلی انتخاب شود لطفا محل دیگری را انتخاب کنید.", ToastLength.Long).Show();
                        return;
                    }
                    ViewsUtility.ShowYesCancelMessageBox(CurrentActivity, "تغییر مسیر داده های موقتی", "هنگام تغییر مسیر موقتی فایل ها تمامی دانلود ها و وظیفه ها باید متوقف باشند و تا پایان عملیات هیچ دانلودی نباید انجام گیرد و نرم افزار نباید بسته شود، در غیر این صورت مسئولیت از بین رفتن داده ها به عهده ی کاربر می باشد.", () =>
                    {
                        LinearLayout linearLayout = new LinearLayout(CurrentActivity);
                        linearLayout.Orientation = Orientation.Vertical;
                        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

                        layoutParams.SetMargins(5, 5, 5, 5);
                        linearLayout.LayoutParameters = layoutParams;
                        ProgressBar progress = new ProgressBar(new ContextThemeWrapper(CurrentActivity, Resource.Style.ProgressBar), null, 0);// (ProgressBar)CurrentActivity.LayoutInflater.Inflate(Resource.Style.ProgressBar, null);
                        //style = "@style/ProgressBar"
                        progress.ProgressDrawable = ViewsUtility.GetDrawable(CurrentActivity, Resource.Drawable.ProgressBarShape_Downloading);
                        linearLayout.AddView(progress);

                        var dialog = ViewsUtility.ShowControlDialog(CurrentActivity, linearLayout, "در حال انتقال داده ها...", null);
                        ApplicationLinkInfoManager.Current.ChangeSaveDataPath(path, (ex, ex2) =>
                        {
                            CurrentActivity?.RunOnUI(() =>
                            {
                                if (ex == null && ex2 == null)
                                {
                                    ViewsUtility.ShowMessageBoxOnlyOkButton(CurrentActivity, "عملیات موفق", "مسیر موقتی با موفقیت تغییر کرد", null);
                                    txtDataDownloadsPath.Text = path;
                                }
                                else
                                {
                                    StringBuilder str = new StringBuilder();
                                    str.AppendLine("خظا در انجام عملیت رخ داده است:");
                                    if (ex != null)
                                        str.AppendLine(ex.Message);
                                    if (ex2 != null)
                                        str.AppendLine(ex2.Message);
                                    ViewsUtility.ShowMessageBoxOnlyOkButton(CurrentActivity, "عملیات ناموفق", str.ToString(), null);
                                }
                                dialog.ManualClose();
                            });
                        }, (source, target, len, pos) =>
                        {
                            CurrentActivity?.RunOnUI(() =>
                            {
                                progress.Max = len;
                                progress.Progress = pos;
                            });
                        });
                    }, () => { });
                }
            });
        }

        public void Dispose()
        {
            try
            {
                CurrentActivity = null;
                _SecurityPath = null;
                txtDownloadsPath.Dispose();
                txtConnectionCount.Dispose();
                txtLimit.Dispose();
                chkIsLimit.Dispose();
                chkIsScreenOnWhenDownloading.Dispose();
                chkIsShowErrorOnScreen.Dispose();
                seekBarConnectionCount.Dispose();
                seekBarLimit.Dispose();
                _mainLayout.Dispose();
                txtDataDownloadsPath.Dispose();
                txtConnectionCount = txtLimit = null;
                txtDownloadsPath = null;
                txtDataDownloadsPath = null;
                chkIsShowErrorOnScreen = chkIsScreenOnWhenDownloading = chkIsLimit = chkIsEnableNotify = null;
                seekBarConnectionCount = null;
                seekBarLimit = null;
                _mainLayout = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "ApplicationSettingsView Dispose");
            }
        }
    }
}