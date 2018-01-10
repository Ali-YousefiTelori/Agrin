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
using Agrin.Download.Web;
using Agrin.Download.Manager;
using Agrin.Helper.ComponentModel;
using Agrin.Download.Web.Tasks;
using Agrin.Download.Data;

namespace Agrin.MonoAndroid.UI
{
    public class LinksListDataViewModel : BaseAdapter<LinkInfo>, IBaseViewModel
    {
        public static LinksListDataViewModel This { get; set; }
        public LinksListDataViewModel(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }
        public Activity CurrentActivity { get; set; }

        bool _isDispose = false;
        object lockObj = new object();

        public void DisposeAll()
        {
            lock (lockObj)
            {
                _isDispose = true;
                LinksListDataViewModel.This = null;
                foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToList())
                {
                    BindingHelper.RemoveActionPropertyChanged(item.PathInfo);
                    BindingHelper.RemoveActionPropertyChanged(item.DownloadingProperty);
                }
                this.Dispose();
            }
        }

        public void Initialize()
        {
            //ListView listView = MainActivity.This.FindViewById<ListView> (Resource.LinksListData.mainListView);
            //ArrayAdapter CalculationAdapter = new ArrayAdapter();

            //for (int i = 0; i < 3; i++)
            //{
            //    CalculationAdapter.Add(i.ToString());
            //}
            //listView.Adapter=new HeaderViewListAdapter(
            //listView.AddView (new TextView(MainActivity.This));
            //listView.seta
        }

        private List<LinkInfo> _items;
        private int _templateResourceId;
        public ListView _listView;

        public void ValidateAllButtons()
        {
            CanDeleteComeplete();
            CanPlaySelection();
            CanStopSelection();
            CanDeleteSelection();
            CanQueueDownload();
            if (IsDownloadingTask())
                btnDownloadQueue.Text = ViewUtility.FindTextLanguage(CurrentActivity, "StopDownloadQueueList_Language");
            else
                btnDownloadQueue.Text = ViewUtility.FindTextLanguage(CurrentActivity, "DownloadQueueList_Language");
        }

        bool CanDeleteSelection()
        {
            if (IsDownloadingTask())
                return btnDelete.Enabled = false;
            return btnDelete.Enabled = selectedItems.Count > 0;
        }

        bool CanQueueDownload()
        {
            return btnDownloadQueue.Enabled = (selectedItems.Count > 0 || IsDownloadingTask());
        }

        bool CanDeleteComeplete()
        {
            if (IsDownloadingTask())
                return btnDeleteComplete.Enabled = false;
            foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToArray())
            {
                if (item.DownloadingProperty.State == ConnectionState.Complete)
                {
                    btnDeleteComplete.Enabled = true;
                    return false;
                }
            }
            return btnDeleteComplete.Enabled = false;
        }

        bool CanPlaySelection()
        {
            if (IsDownloadingTask())
                return btnPlay.Enabled = false;
            foreach (var item in selectedItems)
            {
                if (item.CanPlay)
                {
                    return btnPlay.Enabled = true;
                }
            }
            return btnPlay.Enabled = false;
        }

        bool CanStopSelection()
        {
            if (IsDownloadingTask())
                return btnStop.Enabled = false;
            foreach (var item in selectedItems)
            {
                if (item.CanStop)
                {
                    return btnStop.Enabled = true;
                }
            }
            return btnStop.Enabled = false;
        }

        Button btnDelete = null, btnPlay = null, btnStop = null, btnDeleteComplete = null, btnDownloadQueue = null;
        LinearLayout mainLayout = null;//, sizeLayout = null;

        public LinksListDataViewModel(Activity context, int templateResourceId, List<LinkInfo> items)
            : base()
        {
            This = this;
            CurrentActivity = context;
            _templateResourceId = templateResourceId;
            mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.LinksListData.mainLayout);
            //sizeLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.LinksListData.sizeLayout);
            _listView = CurrentActivity.FindViewById<ListView>(Resource.LinksListData.mainListView);
            _listView.Adapter = this;
            _listView.ItemClick += OnListItemClick;
            btnPlay = CurrentActivity.FindViewById<Button>(Resource.LinksListData.btnPlay);
            btnPlay.Click += btnPlayClick;
            btnStop = CurrentActivity.FindViewById<Button>(Resource.LinksListData.btnStop);
            btnStop.Click += btnStopClick;
            btnDownloadQueue = CurrentActivity.FindViewById<Button>(Resource.LinksListData.btnDownloadQueue);
            btnDownloadQueue.Click += btnDownloadQueue_Click;
            if (IsDownloadingTask())
                btnDownloadQueue.Text = ViewUtility.FindTextLanguage(CurrentActivity, "StopDownloadQueueList_Language");
            else
                btnDownloadQueue.Text = ViewUtility.FindTextLanguage(CurrentActivity, "DownloadQueueList_Language");

            //			Button btnOpen = CurrentActivity.FindViewById<Button> (Resource.LinksListData.btnOpen);
            //			btnOpen.Click += btnOpenClick;
            btnDelete = CurrentActivity.FindViewById<Button>(Resource.LinksListData.btnDelete);
            btnDelete.Click += btnDeleteClick;
            btnDeleteComplete = CurrentActivity.FindViewById<Button>(Resource.LinksListData.btnDeleteComplete);
            btnDeleteComplete.Click += btnDeleteCompleteClick;

            var btnSelectAll = CurrentActivity.FindViewById<Button>(Resource.LinksListData.btnSelectAll);
            btnSelectAll.Click += btnSelectAllClick;

            ValidateAllButtons();
        }

        bool IsDownloadingTask()
        {
            return ApplicationTaskManager.Current.TaskInfoes.Count == 1 && ApplicationTaskManager.Current.TaskInfoes[0].IsActive;
        }

        void btnDownloadQueue_Click(object sender, EventArgs e)
        {
            bool isStop = IsDownloadingTask();
            var builder = new AlertDialog.Builder(CurrentActivity);
            builder.SetTitle(ViewUtility.FindTextLanguage(CurrentActivity, "DownloadQueueList_Language"));
            LinearLayout layout = new LinearLayout(CurrentActivity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            TextView txtMessage = new TextView(CurrentActivity);
            txtMessage.SetTextAppearance(CurrentActivity, global::Android.Resource.Style.TextAppearanceSmall);
            txtMessage.SetSingleLine(false);
            txtMessage.VerticalScrollBarEnabled = true;
            if (isStop)
                txtMessage.Text = ViewUtility.FindTextLanguage(CurrentActivity, "DoYouWantToStopDownloadQueueList_Language");
            else
                txtMessage.Text = ViewUtility.FindTextLanguage(CurrentActivity, "DoYouDownloadQueueList_Language");

            layout.AddView(txtMessage);
            ScrollView scroll = new ScrollView(CurrentActivity);
            scroll.AddView(layout);
            builder.SetView(scroll);

            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "Yes_Language"), (s, ee) =>
            {
                try
                {
                    if (isStop)
                    {
                        ApplicationTaskManager.Current.DeActiveTask(ApplicationTaskManager.Current.TaskInfoes[0]);
                        btnDownloadQueue.Text = ViewUtility.FindTextLanguage(CurrentActivity, "DownloadQueueList_Language");
                    }
                    else
                    {
                        ApplicationTaskManager.Current.UserCompleteAction = () =>
                        {
                            ApplicationTaskManager.Current.DeActiveTask(ApplicationTaskManager.Current.TaskInfoes.FirstOrDefault());
                            CurrentActivity.RunOnUiThread(ValidateAllButtons);
                        };
                        if (ApplicationTaskManager.Current.TaskInfoes.Count == 0)
                        {
                            var task = new TaskInfo() { Name = "Agrin", IsActive = true, TaskItemInfoes = selectedItems.Select(x => x.PathInfo.Id).Select<int, TaskItemInfo>(x => new TaskItemInfo() { Mode = TaskItemMode.LinkInfo, Value = x }).ToList() };
                            task.IsStartNow = true;
                            task.TaskUtilityActions.Add(TaskUtilityModeEnum.StartLink);
                            ApplicationTaskManager.Current.AddTask(task);
                        }
                        else
                        {
                            ApplicationTaskManager.Current.TaskInfoes[0].TaskItemInfoes = selectedItems.Select(x => x.PathInfo.Id).Select<int, TaskItemInfo>(x => new TaskItemInfo() { Mode = TaskItemMode.LinkInfo, Value = x }).ToList();
                            ApplicationTaskManager.Current.ActiveTask(ApplicationTaskManager.Current.TaskInfoes[0]);
                        }
                        btnDownloadQueue.Text = ViewUtility.FindTextLanguage(CurrentActivity, "StopDownloadQueueList_Language");
                    }
                    ValidateAllButtons();
                    SerializeData.SaveApplicationTaskToFile();
                }
                catch (Exception err)
                {
                    Agrin.Log.AutoLogger.LogError(err, "OK_Language btnDownloadQueue_Click");
                }
            });
            builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "No_Language"), (s, ee) =>
            {
            });
            builder.Show();

        }

        public override LinkInfo this[int position]
        {
            get { return ApplicationLinkInfoManager.Current.LinkInfoes[position]; }
        }

        public override int Count
        {
            get { return ApplicationLinkInfoManager.Current.LinkInfoes.Count; }
        }

        public override long GetItemId(int position)
        {
            return ApplicationLinkInfoManager.Current.LinkInfoes[position].PathInfo.Id;
        }

        void btnPlayClick(object sender, EventArgs e)
        {
            foreach (var item in selectedItems)
            {
                if (item.CanPlay)
                    ApplicationLinkInfoManager.Current.PlayLinkInfo(item);
            }
            ValidateAllButtons();
        }

        void btnStopClick(object sender, EventArgs e)
        {
            foreach (var item in selectedItems)
            {
                if (item.CanStop)
                    ApplicationLinkInfoManager.Current.StopLinkInfo(item);
            }
            ValidateAllButtons();
        }

        bool selectedAll = false;

        void btnSelectAllClick(object sender, EventArgs e)
        {
            selectedItems.Clear();
            selectedAll = !selectedAll;
            int i = 0;
            foreach (var selItem in ApplicationLinkInfoManager.Current.LinkInfoes.ToArray())
            {
                var view = _listView.Adapter.GetView(i, _listView, null);
                var layout = view.FindViewById<LinearLayout>(Resource.ListLinkItem.mainLayout);
                var selectLayout = view.FindViewById<LinearLayout>(Resource.ListLinkItem.selectLayout);
                if (selectedAll)
                    selectedItems.Add(selItem);
                SetLayoutColor(selItem, layout, selectLayout);
                i++;
            }

            ValidateAllButtons();
            this.NotifyDataSetChanged();
            _listView.RefreshDrawableState();
        }

        void btnDeleteClick(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this.CurrentActivity);
            builder.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "Areyousureyouwanttodeleteselectedlinks_Language"));
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "Yes_Language"), (s, ee) =>
            {
                var list = selectedItems.ToList();
                ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(list);
                foreach (var item in list)
                {
                    InitializeApplication.CancelNotify(item.PathInfo.Id);
                }

                selectedItems.Clear();
                this.NotifyDataSetChanged();
                _listView.InvalidateViews();
            });
            builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "No_Language"), (s, ee) => { }).Create();
            builder.Show();
            ValidateAllButtons();
        }

        void btnDeleteCompleteClick(object sender, EventArgs e)
        {
            List<LinkInfo> completeLinks = new List<LinkInfo>();
            foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToArray())
            {
                if (item.DownloadingProperty.State == ConnectionState.Complete)
                {
                    completeLinks.Add(item);
                    selectedItems.Remove(item);
                }
            }
            ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(completeLinks);
            foreach (var item in completeLinks)
            {
                InitializeApplication.CancelNotify(item.PathInfo.Id);
            }
            this.NotifyDataSetChanged();
            _listView.InvalidateViews();
            ValidateAllButtons();
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var selItem = this[e.Position];
            var layout = e.View.FindViewById<LinearLayout>(Resource.ListLinkItem.mainLayout);
            var selectLayout = e.View.FindViewById<LinearLayout>(Resource.ListLinkItem.selectLayout);
            if (selectedItems.Contains(selItem))
            {
                selectedItems.Remove(selItem);
            }
            else
            {
                selectedItems.Add(selItem);
            }
            SetLayoutColor(selItem, layout, selectLayout);
            ValidateAllButtons();
        }
        //		void chkFileNameCheckedChange (object sender, EventArgs e)
        //		{
        //			CheckBox chkFileName = sender as CheckBox;
        //			HolderHelper<LinkInfo> item = chkFileName.Tag as HolderHelper<LinkInfo>;
        //			if (chkFileName.Checked)
        //				selectedItems.Add (item.Value);
        //			else
        //				selectedItems.Remove (item.Value);
        //		}

        int totalWidth = 0, totalHeight = 0;
        List<LinkInfo> selectedItems = new List<LinkInfo>();
        //List<LinkInfo> initedItems = new List<LinkInfo>();

        void SetLayoutColor(LinkInfo item, LinearLayout layout, LinearLayout selectLayout)
        {
            if (item.DownloadingProperty.State == ConnectionState.Complete)
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff70a541"));
            else if (item.DownloadingProperty.State == ConnectionState.Error)
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ffb4181b"));
            else if (item.IsDownloading)
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff22344d"));
            else
                layout.SetBackgroundColor(Android.Graphics.Color.Transparent);

            if (!selectedItems.Contains(item))
            {
                selectLayout.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
            else
            {
                selectLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#7F5c9ad5"));
            }
        }

        public static void CheckForWakeLock(bool isOn)
        {
            //bool wakeOn = false;
            //if (info.IsDownloading)
            //    wakeOn = true;
            //else
            //{
            //    foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToList())
            //    {
            //        if (item.IsDownloading)
            //        {
            //            wakeOn = true;
            //            break;
            //        }
            //    }
            //}

            if (isOn)
                MainActivity.AcquireWakeLock();
            else
                MainActivity.ReleaseWakeLock();
        }

        private void UpdateView(int index, LinkInfo item, string property, LinearLayout layout, LinearLayout selectLayout, View _view = null)
        {
            View view = _view;
            if (view == null)
                view = _listView.GetChildAt(index - _listView.FirstVisiblePosition);
            if (view == null)
                return;
            StringBuilder textBuilder = new StringBuilder();

            TextView chkFileName = view.FindViewById<TextView>(Resource.ListLinkItem.chkFileName);
            TextView txtData = view.FindViewById<TextView>(Resource.ListLinkItem.txtData);
            ProgressBar mainProgress = view.FindViewById<ProgressBar>(Resource.ListLinkItem.mainProgress);
            string fileName = item.PathInfo.FileName;
            textBuilder.Clear();
            textBuilder.Append(ViewUtility.FindTextLanguage(CurrentActivity, item.DownloadingProperty.State.ToString() + "_Language"));
            textBuilder.Append(" | " + ViewUtility.FindTextLanguage(CurrentActivity, "Size_Language") + " ");
            string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.Size);
            textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language"));
            textBuilder.Append(" | " + ViewUtility.FindTextLanguage(CurrentActivity, "Downloaded_Language") + " ");
            size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.DownloadedSize);
            textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language"));
            chkFileName.Text = fileName;
            txtData.Text = textBuilder.ToString();
            if (item.DownloadingProperty.Size >= 0.0)
                mainProgress.Progress = (int)(100 / (item.DownloadingProperty.Size / item.DownloadingProperty.DownloadedSize));
            else
                mainProgress.Progress = 0;

            if (property == "State")
                SetLayoutColor(item, layout, selectLayout);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (_isDispose)
                return null;
            View view = convertView;
            LinkInfo item = this[position];
            if (view == null || !(view is LinearLayout))
                view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, parent, false);
            bool isThread = false;


            LinearLayout selectLayout = view.FindViewById<LinearLayout>(Resource.ListLinkItem.selectLayout);
            //chkFileName.CheckedChange -= chkFileNameCheckedChange;
            //chkFileName.CheckedChange += chkFileNameCheckedChange;
            var layout = view.FindViewById<LinearLayout>(Resource.ListLinkItem.mainLayout);
            SetLayoutColor(item, layout, selectLayout);
            ProgressBar mainProgress = view.FindViewById<ProgressBar>(Resource.ListLinkItem.mainProgress);

            Action<string> setAllItems = (property) =>
            {
                AsyncActions.Action(() =>
                {
                    lock (lockObj)
                    {
                        if (_isDispose)
                            return;

                        CurrentActivity.RunOnUiThread(() =>
                        {
                            try
                            {
                                if (_isDispose)
                                    return;
                                UpdateView(position, item, property, layout, selectLayout);
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                                string st = ex.StackTrace;
                                var q = "";
                            }
                            //	Android.Widget.Toast.MakeText(this.CurrentActivity, "Complete: "+item.PathInfo.FileName, Android.Widget.ToastLength.Short).Show();
                            //else if (item.DownloadingProperty.State == ConnectionState.Stoped)
                            //	Android.Widget.Toast.MakeText(this.CurrentActivity, "Stoped: "+item.PathInfo.FileName, Android.Widget.ToastLength.Short).Show();
                            //txtFileName.Text = item.PathInfo.FileName;
                            //txtSize.Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum (item.DownloadingProperty.Size);
                            //txtStatus.Text = item.DownloadingProperty.State.ToString ();
                            //txtDownloaded.Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum (item.DownloadingProperty.DownloadedSize);
                        });
                    }
                }, null, isThread);
            };

            BindingHelper.RemoveActionPropertyChanged(item.PathInfo);
            BindingHelper.RemoveActionPropertyChanged(item.DownloadingProperty);

            BindingHelper.AddActionPropertyChanged(setAllItems, item.PathInfo, new List<string>() { "FileName" });
            BindingHelper.AddActionPropertyChanged(setAllItems, item.DownloadingProperty, new List<string>() {
				"DownloadedSize",
				"State",
				"Size"
			});

            BindingHelper.AddActionPropertyChanged((x) =>
            {
                AsyncActions.Action(() =>
                {
                    CurrentActivity.RunOnUiThread(() => ValidateAllButtons());
                });
            }, item.DownloadingProperty, new List<string>() {
				"State",
			});
            UpdateView(position, item, "State", layout, selectLayout, view);
            //setAllItems("State");
            if (view.MeasuredWidth > totalWidth)
            {
                view.Measure(0, 0);
                totalWidth = view.MeasuredWidth;
            }
            //var qqqqq = view.MeasuredHeight;
            totalHeight = (view.MeasuredHeight + 1) * Count;
            if (totalWidth != 0 && mainLayout.MeasuredWidth > 0 && totalWidth > mainLayout.MeasuredWidth)
                _listView.LayoutParameters.Width = totalWidth;

            //+ (_listView.DividerHeight * (Count - 1));

            if (totalHeight != 0 && totalHeight != Count)
                _listView.LayoutParameters.Height = totalHeight;

            _listView.SetMinimumHeight(totalHeight);
            if (mainLayout.Width > 0)
                mainProgress.LayoutParameters.Width = mainLayout.Width;
            //_listView.SetMinimumWidth (mainLayout.MeasuredWidth);
            layout.SetMinimumWidth(mainLayout.MeasuredWidth);
            //layout.RefreshDrawableState ();
            //_listView.RefreshDrawableState();

            isThread = true;
            //BindingHelper.AddBindingOneWay(this, item.DownloadingProperty, "DownloadedSize", txtDownloaded, "Text");
            //txtStatus.Text = item.DownloadingProperty.State.ToString();
            //			ProgressBar progDownloaded = view.FindViewById<ProgressBar>(Resource.Id.progDownloaded);
            //			progDownloaded.Max = 100;
            //			progDownloaded.Progress = 50;
            return view;
        }
    }
}

