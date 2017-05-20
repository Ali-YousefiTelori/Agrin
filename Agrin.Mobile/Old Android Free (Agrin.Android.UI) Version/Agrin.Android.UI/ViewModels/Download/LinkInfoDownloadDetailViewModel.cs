using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agrin.Download.Web;
using Agrin.Download.Manager;
using Agrin.Helper.ComponentModel;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Agrin.Download.Web.Link;
using Agrin.Log;

namespace Agrin.MonoAndroid.UI.ViewModels.Download
{
    public class ListViewRefreshingHelper : IDisposable
    {
        bool _isDispose = false;
        bool _invalidate = true;
        public void Start(ListView list, Activity currentActivity)
        {
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                while (!_isDispose)
                {
                    if (!_invalidate)
                        resetEvent.WaitOne();
                    if (_isDispose)
                        break;
                    if (_invalidate)
                    {
                        currentActivity.RunOnUiThread(() =>
                        {
                            list.InvalidateViews();
                        });
                        _invalidate = false;
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            });
            thread.Start();
        }

        System.Threading.ManualResetEvent resetEvent = new System.Threading.ManualResetEvent(false);
        public void InvalidateRefresh()
        {
            if (_isDispose)
                return;
            _invalidate = true;
            resetEvent.Set();
            resetEvent.Reset();
        }

        public void Dispose()
        {
            _isDispose = true;
            resetEvent.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class LinkInfoDownloadDetailViewModel : BaseAdapter<LinkWebRequest>, IBaseViewModel
    {
        public LinkInfoDownloadDetailViewModel(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
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
                _listViewRefreshingHelper.Dispose();
                BindingHelper.RemoveActionPropertyChanged(CurrentLinkInfo.PathInfo, setAllItems);
                BindingHelper.RemoveActionPropertyChanged(CurrentLinkInfo.DownloadingProperty, setAllItems);
                foreach (var item in CurrentLinkInfo.Connections.ToList())
                {
                    BindingHelper.RemoveActionPropertyChanged(item);
                }
                this.Dispose();
            }
        }

        private List<LinkWebRequest> _items;
        LinkInfo CurrentLinkInfo { get; set; }
        private int _templateResourceId;
        public ListView _listView;

        void ValidateAllButtons()
        {
            CanPlay();
            CanStop();
            CanReconnect();
        }

        bool CanPlay()
        {
            return btnPlay.Enabled = CurrentLinkInfo.CanPlay;
        }

        bool CanStop()
        {
            return btnStop.Enabled = CurrentLinkInfo.CanStop;
        }

        bool CanReconnect()
        {
            return btnReConnect.Enabled = CurrentLinkInfo.CanStop;
        }

        Button btnPlay = null, btnStop = null, btnReConnect = null;
        LinearLayout mainLayout = null;
        static Action<string> setAllItems = null;
        public LinkInfoDownloadDetailViewModel(Activity context, int templateResourceId, LinkInfo _currentLinkInfo)
            : base()
        {
            CurrentActivity = context;
            CurrentLinkInfo = _currentLinkInfo;

            _templateResourceId = templateResourceId;
            _items = CurrentLinkInfo.Connections.ToList();
            mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.LinkInfoDownloadDetail.mainLayout);
            _listView = CurrentActivity.FindViewById<ListView>(Resource.LinkInfoDownloadDetail.mainList);
            _listView.Adapter = this;
            btnPlay = CurrentActivity.FindViewById<Button>(Resource.LinkInfoDownloadDetail.btnPlay);
            btnPlay.Click += btnPlayClick;
            btnStop = CurrentActivity.FindViewById<Button>(Resource.LinkInfoDownloadDetail.btnStop);
            btnStop.Click += btnStopClick;
            btnReConnect = CurrentActivity.FindViewById<Button>(Resource.LinkInfoDownloadDetail.btnReConnect);
            var txtFileName = CurrentActivity.FindViewById<TextView>(Resource.LinkInfoDownloadDetail.txtFileName);
            var txtStatus = CurrentActivity.FindViewById<TextView>(Resource.LinkInfoDownloadDetail.txtStatus);
            var txtSize = CurrentActivity.FindViewById<TextView>(Resource.LinkInfoDownloadDetail.txtSize);
            var txtDownloadedSize = CurrentActivity.FindViewById<TextView>(Resource.LinkInfoDownloadDetail.txtDownloadedSize);
            var txtResumable = CurrentActivity.FindViewById<TextView>(Resource.LinkInfoDownloadDetail.txtResumable);
            var txtTimeRemainig = CurrentActivity.FindViewById<TextView>(Resource.LinkInfoDownloadDetail.txtTimeRemainig);
            var txtSpeed = CurrentActivity.FindViewById<TextView>(Resource.LinkInfoDownloadDetail.txtSpeed);

            _listViewRefreshingHelper.Start(_listView, CurrentActivity);
            CurrentLinkInfo.Connections.ChangedCollection = () =>
            {
                CurrentActivity.RunOnUiThread(() =>
                {
                    _items = CurrentLinkInfo.Connections.ToList();
                    if (!_isDispose)
                    {
                        NotifyDataSetChanged();
                        _listView.InvalidateViews();
                    }
                });
            };


            if (setAllItems != null)
            {
                BindingHelper.RemoveActionPropertyChanged(CurrentLinkInfo.PathInfo, setAllItems);
                BindingHelper.RemoveActionPropertyChanged(CurrentLinkInfo.DownloadingProperty, setAllItems);
            }

            btnReConnect.Click += btnStopClick;
            ValidateAllButtons();
            setAllItems = (property) =>
            {
                AsyncActions.Action(() =>
                {
                    lock (lockObj)
                    {
                        if (_isDispose)
                            return;
                        CurrentActivity.RunOnUiThread(() =>
                        {
                            txtFileName.Text = CurrentLinkInfo.PathInfo.FileName;
                            txtStatus.Text = ViewUtility.FindTextLanguage(CurrentActivity, CurrentLinkInfo.DownloadingProperty.State.ToString() + "_Language");
                            string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(CurrentLinkInfo.DownloadingProperty.Size);
                            txtSize.Text = size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");
                            size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(CurrentLinkInfo.DownloadingProperty.DownloadedSize);
                            txtDownloadedSize.Text = size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");
                            txtResumable.Text = ViewUtility.FindTextLanguage(CurrentActivity, CurrentLinkInfo.DownloadingProperty.ResumeCapability.ToString() + "_Language");
                            txtTimeRemainig.Text = ViewUtility.TimeToString(CurrentActivity, CurrentLinkInfo.DownloadingProperty.TimeRemaining);
                            size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(CurrentLinkInfo.DownloadingProperty.SpeedByteDownloaded);
                            txtSpeed.Text = size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");
                            ValidateAllButtons();
                            if (CurrentLinkInfo.DownloadingProperty.State == ConnectionState.Complete)
                                _listView.InvalidateViews();
                        });
                    }
                }, null, true);
            };

            BindingHelper.AddActionPropertyChanged(setAllItems, CurrentLinkInfo.PathInfo, new List<string>() { "FileName" });
            BindingHelper.AddActionPropertyChanged(setAllItems, CurrentLinkInfo.DownloadingProperty, new List<string>() {
				"DownloadedSize",
				"State",
				"Size",
                "ResumeCapability",
                "TimeRemaining",
                "SpeedByteDownloaded"
			});

            BindingHelper.AddActionPropertyChanged((x) =>
            {
                AsyncActions.Action(() =>
                {
                    CurrentActivity.RunOnUiThread(() => ValidateAllButtons());
                });
            }, CurrentLinkInfo.DownloadingProperty, new List<string>() {
				"State",
			});

            setAllItems("State");
        }

        public override LinkWebRequest this[int position]
        {
            get
            {
                try
                {
                    return _items[position];
                }
                catch (Exception e)
                {
                    AutoLogger.LogError(e, "ERRRRRRRR 1: ", true);
                }
                return null;
            }
        }

        public override int Count
        {
            get
            {
                try
                {
                    if (CurrentLinkInfo == null)
                        return 0;
                    return _items.Count;
                }
                catch (Exception e)
                {
                    AutoLogger.LogError(e, "ERRRRRRRR 2: ", true);
                }
                return 0;
            }
        }

        public override long GetItemId(int position)
        {
            try
            {
                return _items[position].ConnectionId;
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, "ERRRRRRRR 3: ", true);
            }
            return 0;
        }

        void btnPlayClick(object sender, EventArgs e)
        {
            ApplicationLinkInfoManager.Current.PlayLinkInfo(CurrentLinkInfo);
        }

        void btnStopClick(object sender, EventArgs e)
        {
            ApplicationLinkInfoManager.Current.StopLinkInfo(CurrentLinkInfo);
        }

        void btnReConnectClick(object sender, EventArgs e)
        {
            if (CurrentLinkInfo.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Yes)
            {
                foreach (var connectionInfo in _items.ToList())
                {
                    connectionInfo.ReConnect();
                }
            }
        }

        //private void updateItemAtPosition(int position)
        //{
        //    int visiblePosition = _listView.FirstVisiblePosition;
        //    View view = _listView.GetChildAt(position - visiblePosition);
        //    _listView.Adapter.GetView(position, view, _listView);
        //}

        int totalWidth = 0, totalHeight = 0;
        ListViewRefreshingHelper _listViewRefreshingHelper = new ListViewRefreshingHelper();
        //List<LinkWebRequest> initedItems = new List<LinkWebRequest>();
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (_isDispose)
                    return null;
                View view = convertView;
                LinkWebRequest item = this[position];
                if (view == null || !(view is LinearLayout))
                    view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, parent, false);
                var layout = view.FindViewById<RelativeLayout>(Resource.ConnectionInfoListItem.mainLayout);
                bool isThread = false;
                TextView txtStatus = view.FindViewById<TextView>(Resource.ConnectionInfoListItem.txtStatus);
                TextView txtSize = view.FindViewById<TextView>(Resource.ConnectionInfoListItem.txtSize);
                TextView txtDownloaded = view.FindViewById<TextView>(Resource.ConnectionInfoListItem.txtDownloaded);
                LinearLayout linearLayoutRTL1 = view.FindViewById<LinearLayout>(Resource.ConnectionInfoListItem.LinearLayoutRightToLeft1);
                LinearLayout linearLayoutRTL2 = view.FindViewById<LinearLayout>(Resource.ConnectionInfoListItem.LinearLayoutRightToLeft2);

                ProgressBar mainProgress = view.FindViewById<ProgressBar>(Resource.ConnectionInfoListItem.mainProgress);
                //var layout = view.FindViewById<RelativeLayout>(Resource.ConnectionInfoListItem.mainLayout);
                ViewUtility.SetRightToLeftViews(new List<View>() { txtStatus, txtSize, txtDownloaded, linearLayoutRTL1, txtDownloaded, linearLayoutRTL2 });

                Action<string> setAllItemsClient = (property) =>
                {
                    AsyncActions.Action(() =>
                    {
                        try
                        {
                            lock (lockObj)
                            {
                                if (_isDispose)
                                    return;
                                CurrentActivity.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        txtStatus.Text = ViewUtility.FindTextLanguage(CurrentActivity, item.State.ToString() + "_Language");
                                        string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.Length);
                                        txtSize.Text = ViewUtility.FindTextLanguage(CurrentActivity, "Size_Language") + " " + size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");
                                        size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadedSize);
                                        txtDownloaded.Text = ViewUtility.FindTextLanguage(CurrentActivity, "Downloaded_Language") + " " + size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");

                                        if (item.Length >= 0.0)
                                            mainProgress.Progress = (int)(100 / (item.Length / item.DownloadedSize));
                                        else
                                            mainProgress.Progress = 0;
                                        if (!_isDispose)
                                            _listViewRefreshingHelper.InvalidateRefresh();
                                    }
                                    catch (Exception e)
                                    {
                                        AutoLogger.LogError(e, "ui thread:", true);
                                    }
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            AutoLogger.LogError(e, "lockError:", true);
                        }
                    }, null, isThread);
                };

                BindingHelper.RemoveActionPropertyChanged(item);
                BindingHelper.AddActionPropertyChanged(setAllItemsClient, item, new List<string>() {
				"DownloadedSize",
				"State",
				"Length"});

                setAllItemsClient("State");
                //if (view.MeasuredWidth > totalWidth)
                //{
                //    view.Measure(0, 0);
                //    totalWidth = view.MeasuredWidth;
                //}
                ////var qqqqq = view.MeasuredHeight;
                //totalHeight = (view.MeasuredHeight + 1) * Count;
                //if (totalWidth != 0 && mainLayout.MeasuredWidth > 0 && totalWidth > mainLayout.MeasuredWidth)
                //    _listView.LayoutParameters.Width = totalWidth;

                ////+ (_listView.DividerHeight * (Count - 1));

                //if (totalHeight != 0 && totalHeight != Count)
                //    _listView.LayoutParameters.Height = totalHeight;

                //_listView.SetMinimumHeight(totalHeight);
                //if (mainLayout.Width > 0)
                //    mainProgress.LayoutParameters.Width = mainLayout.Width;
                ////_listView.SetMinimumWidth (mainLayout.MeasuredWidth);
                //layout.SetMinimumWidth(mainLayout.MeasuredWidth);
                ////layout.RefreshDrawableState ();
                //_listView.RefreshDrawableState();
                isThread = true;
                return view;
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, "TTTTTTTT 1 ", true);
            }
            return null;
        }
    }
}

