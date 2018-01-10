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
using Agrin.Framesoft;
using Agrin.Helper.ComponentModel;
using Agrin.Framesoft.Helper;
using Agrin.Download.Data.Settings;
using System.Threading;

namespace Agrin.MonoAndroid.UI.ViewModels.Lists
{
    public class FileStatusGetter : IDisposable
    {
        UserFileInfoData CurrentUserFileInfoData { get; set; }
        public FileStatusGetter(UserFileInfoData userFileInfoData)
        {
            CurrentUserFileInfoData = userFileInfoData;
            AsyncActions.Action(() =>
            {
                while (!_isDispose)
                {
                    try
                    {
                        CurrentUserFileInfoData.UserName = ApplicationSetting.Current.FramesoftSetting.UserName;
                        CurrentUserFileInfoData.Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash();
                        var response = UserManagerHelper.GetOneFileStatus(CurrentUserFileInfoData);
                        if (response.Data != null)
                        {
                            CurrentUserFileInfoData.DownloadedSize = response.Data.DownloadedSize;
                            CurrentUserFileInfoData.FileGuid = response.Data.FileGuid;
                            CurrentUserFileInfoData.FileName = response.Data.FileName;
                            CurrentUserFileInfoData.ID = response.Data.ID;
                            CurrentUserFileInfoData.Size = response.Data.Size;
                            CurrentUserFileInfoData.Status = response.Data.Status;
                            if (CurrentUserFileInfoData.ChangedValuesAction != null)
                                CurrentUserFileInfoData.ChangedValuesAction();
                            if (CurrentUserFileInfoData.Status == 4 || CurrentUserFileInfoData.Status == 5)
                            {
                                Dispose();
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    Thread.Sleep(5000);
                }
            });
        }
        bool _isDispose = false;
        public void Dispose()
        {
            _isDispose = true;
        }
    }

    //FramesoftLinksListDataViewModel
    public class FramesoftLinksListDataViewModel : BaseAdapter<UserFileInfoData>, IBaseViewModel
    {

        public static FramesoftLinksListDataViewModel This { get; set; }
        public FramesoftLinksListDataViewModel(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        Dictionary<UserFileInfoData, FileStatusGetter> FileStatusUpdaterList = new Dictionary<UserFileInfoData, FileStatusGetter>();
        bool _isDispose = false;

        public void DisposeAll()
        {
            lock (lockOBJ)
            {
                _isDispose = true;
                DisposeAllGetters();
            }
        }

        void DisposeAllGetters()
        {
            foreach (var item in FileStatusUpdaterList.ToArray())
            {
                item.Value.Dispose();
            }
            FileStatusUpdaterList.Clear();
        }

        public void AddToUpdater(UserFileInfoData file)
        {
            if (file == null || _isDispose)
                return;
            if (!FileStatusUpdaterList.ContainsKey(file))
                FileStatusUpdaterList.Add(file, new FileStatusGetter(file) { });
        }

        public Activity CurrentActivity { get; set; }

        public static List<UserFileInfoData> LastRefreshList { get; set; }
        private List<UserFileInfoData> _items;
        private int _templateResourceId;
        public ListView _listView;

        LinearLayout mainLayout = null;//, sizeLayout = null;

        public FramesoftLinksListDataViewModel(Activity context, int templateResourceId, List<UserFileInfoData> items)
            : base()
        {
            _items = LastRefreshList == null ? items : LastRefreshList;
            This = this;
            CurrentActivity = context;
            _templateResourceId = templateResourceId;
            mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.FramesoftLinksListData.mainLayout);
            //sizeLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.LinksListData.sizeLayout);
            _listView = CurrentActivity.FindViewById<ListView>(Resource.FramesoftLinksListData.mainListView);
            _listView.Adapter = this;
            _listView.ItemClick += OnListItemClick;

            var refreshButton = CurrentActivity.FindViewById<Button>(Resource.FramesoftLinksListData.btnRefersh);
            refreshButton.Click += refreshButton_Click;
            if (MustRefresh)
                RefreshList();
        }

        void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        public static bool MustRefresh = true;
        public void RefreshList()
        {
            var progressDialog = ProgressDialog.Show(CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "RefreshList_Language"), ViewUtility.FindTextLanguage(CurrentActivity, "Refreshing_Language"), true, false);
            Action<string> errorRegister = (msg) =>
            {
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "RefreshListError_Language") + ": " + System.Environment.NewLine + msg);
                });
                Thread.Sleep(2000);
            };

            AsyncActions.Action(() =>
            {
                var buyStorage = UserManagerHelper.GetListOfFiles(new UserInfoData() { UserName = ApplicationSetting.Current.FramesoftSetting.UserName, Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash() });
                if (buyStorage.Data != null)
                {
                    MustRefresh = false;
                    DisposeAllGetters();
                    _items = LastRefreshList = buyStorage.Data;
                    CurrentActivity.RunOnUiThread(() =>
                    {
                        foreach (var item in _items)
                        {
                            if (item.Status != 4 && item.Status != 5)
                                AddToUpdater(item);
                        }
                        this.NotifyDataSetChanged();
                        _listView.InvalidateViews();
                    });
                    Thread.Sleep(1000);
                }
                else
                {
                    errorRegister(ViewUtility.FindTextLanguage(CurrentActivity, UserManagerHelper.GetServerResponseMessageValue(buyStorage.Message, "")));
                }
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.Dismiss();
                    progressDialog.Hide();
                });
            }, (error) =>
            {
                errorRegister(error.Message);
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.Dismiss();
                    progressDialog.Hide();
                });
            });
        }

        public void RemoveLink(UserFileInfoData userFileInfoData)
        {
            var progressDialog = ProgressDialog.Show(CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "DeleteFilesFromServer_Language"), ViewUtility.FindTextLanguage(CurrentActivity, "DeletingFilesFromServer_Language"), true, false);
            Action<string> errorRegister = (msg) =>
            {
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "DeletingFilesError_Language") + ": " + System.Environment.NewLine + msg);
                });
                Thread.Sleep(2000);
            };

            AsyncActions.Action(() =>
            {
                userFileInfoData.UserName = ApplicationSetting.Current.FramesoftSetting.UserName;
                userFileInfoData.Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash();

                var buyStorage = UserManagerHelper.SetCompleteUserFiles(new List<UserFileInfoData>() { userFileInfoData });
                if (buyStorage.Message == "OK")
                {
                    CurrentActivity.RunOnUiThread(() =>
                    {
                        RefreshList();
                    });
                }
                else
                {
                    errorRegister(ViewUtility.FindTextLanguage(CurrentActivity,UserManagerHelper.GetServerResponseMessageValue(buyStorage.Message, "")));
                }
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.Dismiss();
                    progressDialog.Hide();
                });
            }, (error) =>
            {
                errorRegister(error.Message);
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.Dismiss();
                    progressDialog.Hide();
                });
            });
        }

        public override UserFileInfoData this[int position]
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override long GetItemId(int position)
        {
            return _items[position].ID;
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //var listView = sender as ListView;
            //var selItem = this[e.Position];
            //_listView.ShowContextMenu();
        }

        int totalWidth = 0, totalHeight = 0;

        public string GetState(int state)
        {
            switch (state)
            {
                case 0:
                    {
                        return "Unknown_Language";
                    }
                case 1:
                    {
                        return "CreatingRequest_Language";
                    }
                case 2:
                    {
                        return "Connecting_Language";
                    }
                case 3:
                    {
                        return "Downloading_Language";
                    }
                case 4:
                    {
                        return "Error_Language";
                    }
                case 5:
                    {
                        return "Complete_Language";
                    }
            }
            return "";
        }

        void SetLayoutColor(UserFileInfoData item, LinearLayout layout, LinearLayout selectLayout)
        {
            if (item.Status == 5)
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff70a541"));
            else if (item.Status == 4)
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ffb4181b"));
            else if (item.Status == 3)
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff22344d"));
            else
                layout.SetBackgroundColor(Android.Graphics.Color.Transparent);

            selectLayout.SetBackgroundColor(Android.Graphics.Color.Transparent);

        }

        object lockOBJ = new object();
        private void UpdateView(int index, UserFileInfoData item, View _view = null)
        {
            View view = _view;
            if (view == null)
                view = _listView.GetChildAt(index - _listView.FirstVisiblePosition);
            if (view == null)
                return;
            TextView chkFileName = view.FindViewById<TextView>(Resource.ListLinkItem.chkFileName);
            TextView txtData = view.FindViewById<TextView>(Resource.ListLinkItem.txtData);
            ProgressBar mainProgress = view.FindViewById<ProgressBar>(Resource.ListLinkItem.mainProgress);
            LinearLayout selectLayout = view.FindViewById<LinearLayout>(Resource.ListLinkItem.selectLayout);
            //chkFileName.CheckedChange -= chkFileNameCheckedChange;
            //chkFileName.CheckedChange += chkFileNameCheckedChange;
            var layout = view.FindViewById<LinearLayout>(Resource.ListLinkItem.mainLayout);
            StringBuilder textBuilder = new StringBuilder();
            if (item.Status == 5)
                item.DownloadedSize = item.Size;
            string fileName = item.FileName;
            textBuilder.Clear();
            textBuilder.Append(ViewUtility.FindTextLanguage(CurrentActivity, GetState(item.Status)));
            textBuilder.Append(" | " + ViewUtility.FindTextLanguage(CurrentActivity, "Size_Language") + " ");
            string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.Size);
            textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language"));
            textBuilder.Append(" | " + ViewUtility.FindTextLanguage(CurrentActivity, "Downloaded_Language") + " ");
            size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadedSize);
            textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language"));
            chkFileName.Text = fileName;
            txtData.Text = textBuilder.ToString();

            if (item.Size >= 0 && item.DownloadedSize > 0)
                mainProgress.Progress = (int)(100.0 / ((double)item.Size / (double)item.DownloadedSize));
            else
                mainProgress.Progress = 0;
            SetLayoutColor(item, layout, selectLayout);
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (_isDispose)
                return null;
            View view = convertView;
            UserFileInfoData item = this[position];
            if (view == null || !(view is LinearLayout))
                view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, parent, false);
            if (item == null)
                return view;
            

            item.ChangedValuesAction = () =>
                {
                    lock (lockOBJ)
                    {
                        if (_isDispose)
                            return;
                        CurrentActivity.RunOnUiThread(() =>
                        {
                            UpdateView(position, item);
                            //	Android.Widget.Toast.MakeText(this.CurrentActivity, "Complete: "+item.PathInfo.FileName, Android.Widget.ToastLength.Short).Show();
                            //else if (item.DownloadingProperty.State == ConnectionState.Stoped)
                            //	Android.Widget.Toast.MakeText(this.CurrentActivity, "Stoped: "+item.PathInfo.FileName, Android.Widget.ToastLength.Short).Show();
                            //txtFileName.Text = item.PathInfo.FileName;
                            //txtSize.Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum (item.DownloadingProperty.Size);
                            //txtStatus.Text = item.DownloadingProperty.State.ToString ();
                            //txtDownloaded.Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum (item.DownloadingProperty.DownloadedSize);
                        });
                    }
                };
            UpdateView(position, item, view);
            //if (view.MeasuredWidth > totalWidth)
            //{
            //    view.Measure(0, 0);
            //    totalWidth = view.MeasuredWidth;
            //}

            //totalHeight = (view.MeasuredHeight + 1) * Count;
            //if (totalWidth != 0 && mainLayout.MeasuredWidth > 0 && totalWidth > mainLayout.MeasuredWidth)
            //    _listView.LayoutParameters.Width = totalWidth;

            //if (totalHeight != 0 && totalHeight != Count)
            //    _listView.LayoutParameters.Height = totalHeight;

            //_listView.SetMinimumHeight(totalHeight);
            //if (mainLayout.Width > 0)
            //    mainProgress.LayoutParameters.Width = mainLayout.Width;
            //layout.SetMinimumWidth(mainLayout.MeasuredWidth);
            return view;
        }
    }
}

