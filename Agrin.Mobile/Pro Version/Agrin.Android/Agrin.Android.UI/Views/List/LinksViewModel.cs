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
using Agrin.BaseViewModels.Lists;
using Agrin.Helper.Collections;
using Agrin.Download.Web;
using Agrin.Helpers;
using Agrin.Helper.ComponentModel;
using Agrin.Models;
using Agrin.Views.Controls;
using System.Globalization;
using Agrin.Download.Manager;
using Android.Animation;
using Agrin.Log;
using Agrin.IO.Mixer;

namespace Agrin.Views.List
{
    public class LinksViewModel : LinksBaseViewModel, IDisposable
    {
        public static LinksViewModel This { get; set; }
        public Activity CurrentActivity { get; set; }
        int _templateResourceId;
        LinearLayout _mainLayoutListView = null;
        LinearLayout _actionToolBoxLayout;
        LinearLayout _actionTopToolBoxLayout;
        LinearLayout _btnPlay, _btnPause, _btnDelete, _btnMenu, _btnPlayQueue, _btnPauseQueue, _btnDeleteQueue, _btnShowMenu;
        LinearLayout _mainLayout;
        View _CustomListView;
        View _linkInfoToolBox;
        View _findLinksToolBox;
        public TextView _txtTopToolbarHeader = null;
        public LinksViewModel(Activity context, int templateResourceId, LinearLayout mainLayout, LinearLayout actionToolBoxLayout, LinearLayout actionTopToolBoxLayout)
        {
            This = this;
            _mainLayout = mainLayout;
            CurrentActivity = context;
            _templateResourceId = templateResourceId;
            _CustomListView = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.CustomListView, mainLayout, false);
            _mainLayoutListView = _CustomListView.FindViewById<LinearLayout>(Resource.CustomListView.mainLayoutListView);
            _actionToolBoxLayout = actionToolBoxLayout;
            _actionTopToolBoxLayout = actionTopToolBoxLayout;

            _linkInfoToolBox = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.LinkInfoToolBox, mainLayout, false);

            _findLinksToolBox = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.FindLinksToolBox, mainLayout, false);
            _findLinksToolBox.Clickable = true;
            _findLinksToolBox.Click += _findLinksToolBox_Click;

            _txtTopToolbarHeader = _findLinksToolBox.FindViewById<TextView>(Resource.FindLinksToolBox.txtHeader);
            _txtTopToolbarHeader.Text = ViewsUtility.FindTextLanguage(CurrentActivity, "LinksSearchAll_Language");
            SetFindLayoutBackGroundAndForeground(MenuItemModeEnum.LinksSearchAll);
            ViewsUtility.SetFont(CurrentActivity, _txtTopToolbarHeader);

            _btnPlay = _linkInfoToolBox.FindViewById<LinearLayout>(Resource.LinkInfoToolBox.layoutPlay);
            _btnPause = _linkInfoToolBox.FindViewById<LinearLayout>(Resource.LinkInfoToolBox.layoutPause);
            _btnDelete = _linkInfoToolBox.FindViewById<LinearLayout>(Resource.LinkInfoToolBox.layoutDelete);
            _btnMenu = _linkInfoToolBox.FindViewById<LinearLayout>(Resource.LinkInfoToolBox.layoutMenu);
            _btnPlayQueue = _linkInfoToolBox.FindViewById<LinearLayout>(Resource.LinkInfoToolBox.layoutPlayQueue);
            _btnPauseQueue = _linkInfoToolBox.FindViewById<LinearLayout>(Resource.LinkInfoToolBox.layoutPauseQueue);
            _btnDeleteQueue = _linkInfoToolBox.FindViewById<LinearLayout>(Resource.LinkInfoToolBox.layoutDeleteQueue);
            _btnShowMenu = _linkInfoToolBox.FindViewById<LinearLayout>(Resource.LinkInfoToolBox.layoutMenu);


            _btnPlay.Click -= _btnPlay_Click;
            _btnPause.Click -= _btnPause_Click;
            _btnDelete.Click -= _btnDelete_Click;
            _btnShowMenu.Click -= _btnShowMenu_Click;
            _btnPlayQueue.Click -= _btnPlayQueue_Click;
            _btnDeleteQueue.Click -= _btnDeleteQueue_Click;
            _btnPauseQueue.Click -= _btnPauseQueue_Click;

            _btnPlay.Click += _btnPlay_Click;
            _btnPause.Click += _btnPause_Click;
            _btnDelete.Click += _btnDelete_Click;
            _btnShowMenu.Click += _btnShowMenu_Click;
            _btnPlayQueue.Click += _btnPlayQueue_Click;
            _btnDeleteQueue.Click += _btnDeleteQueue_Click;
            _btnPauseQueue.Click += _btnPauseQueue_Click;
            //if (tochLayout != null)
            //{
            //    tochLayout.RemoveAllViews();
            //    tochLayout.Dispose();
            //}
            //tochLayout = new TochDetectorLayout(CurrentActivity);
            //tochLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            InitializeView();
            getViewByLinkInfo.Clear();
            DrawAllItems();
            Items.CollectionChanged -= Items_CollectionChanged;
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public MenuItemModeEnum _cuurentMenuItemModeEnum = MenuItemModeEnum.LinksSearchAll;
        public void SetFindLayoutBackGroundAndForeground(MenuItemModeEnum selected)
        {
            CurrentActivity.RunOnUI(() =>
            {
                var shape = new Android.Graphics.Drawables.GradientDrawable();
                shape.SetCornerRadius(8);
                var mainLayout = _findLinksToolBox.FindViewById<View>(Resource.FindLinksToolBox.layoutBackgroud);

                float[] radii = new float[8];
                float round = 6;
                radii[0] = round;
                radii[1] = round;
                radii[2] = round;
                radii[3] = round;
                radii[4] = round;
                radii[5] = round;
                radii[6] = round;
                radii[7] = round;

                var footerBackground = new Android.Graphics.Drawables.ShapeDrawable();

                footerBackground.Shape = new Android.Graphics.Drawables.Shapes.RoundRectShape(radii, null, null);

                switch (selected)
                {
                    case MenuItemModeEnum.LinksSearchAll:
                        {
                            footerBackground.Paint.Color = ViewsUtility.GetColor(CurrentActivity, Resource.Color.ProgressStopedBackground);
                            //shape.SetColor(Resource.Color.ProgressStopedBackground);
                            ViewsUtility.SetTextViewTextColor(CurrentActivity, _txtTopToolbarHeader, Resource.Color.white);
                            break;
                        }
                    case MenuItemModeEnum.LinksSearchComplete:
                        {
                            footerBackground.Paint.Color = ViewsUtility.GetColor(CurrentActivity, Resource.Color.ProgressCompleteBackground);
                            //shape.SetColor(Resource.Color.ProgressCompleteBackground);
                            ViewsUtility.SetTextViewTextColor(CurrentActivity, _txtTopToolbarHeader, Resource.Color.LinkAddressForeground);
                            break;
                        }
                    case MenuItemModeEnum.LinksSearchDownloading:
                        {
                            footerBackground.Paint.Color = ViewsUtility.GetColor(CurrentActivity, Resource.Color.ProgressDownloadingBackground);
                            //shape.SetColor(Resource.Color.ProgressDownloadingBackground);
                            ViewsUtility.SetTextViewTextColor(CurrentActivity, _txtTopToolbarHeader, Resource.Color.LinkAddressForeground);
                            break;
                        }
                    case MenuItemModeEnum.LinksSearchNotComplete:
                        {
                            footerBackground.Paint.Color = ViewsUtility.GetColor(CurrentActivity, Resource.Color.LinkAddressForeground);
                            //shape.SetColor(Resource.Color.ProgressNotStartedBackground);
                            ViewsUtility.SetTextViewTextColor(CurrentActivity, _txtTopToolbarHeader, Resource.Color.white);
                            break;
                        }
                    case MenuItemModeEnum.LinksSearchError:
                        {
                            footerBackground.Paint.Color = ViewsUtility.GetColor(CurrentActivity, Resource.Color.ProgressErrorBackground);
                            //shape.SetColor(Resource.Color.ProgressErrorBackground);
                            ViewsUtility.SetTextViewTextColor(CurrentActivity, _txtTopToolbarHeader, Resource.Color.white);
                            break;
                        }
                    case MenuItemModeEnum.LinksSearchQueue:
                        {

                            footerBackground.Paint.Color = ViewsUtility.GetColor(CurrentActivity, Resource.Color.ProgressQueueBackground);
                            ViewsUtility.SetTextViewTextColor(CurrentActivity, _txtTopToolbarHeader, Resource.Color.white);
                            break;
                        }
                }

                // The corners are ordered top-left, top-right, bottom-right,
                // bottom-left. For each corner, the array contains 2 values, [X_radius,
                // Y_radius]

                ViewsUtility.SetBackgroundDrawable(mainLayout, footerBackground);
                //mainLayout.SetBackgroundDrawable(footerBackground);

                //mainLayout.SetBackgroundDrawable(shape);
                //mainLayout.Invalidate();
                //mainLayout.InvalidateDrawable(shape);
            }, true);
        }
        void _findLinksToolBox_Click(object sender, EventArgs e)
        {
            if (isDrawing)
                return;
            CurrentActivity.RegisterForContextMenu(_findLinksToolBox);
            CurrentActivity.OpenContextMenu(_findLinksToolBox);
            CurrentActivity.UnregisterForContextMenu(_findLinksToolBox);
        }

        //TochDetectorLayout tochLayout = null;

        //public void PaddingToZeroAnimation()
        //{
        //    ValueAnimator animator = ValueAnimator.OfInt(tochLayout.PaddingLeft, 0);
        //    animator.SetDuration(500);
        //    animator.Update += (s, e) =>
        //    {
        //        int value = (int)animator.AnimatedValue;
        //        tochLayout.SetPadding(value, tochLayout.PaddingTop, -value, tochLayout.PaddingBottom);
        //    };
        //    animator.AnimationEnd += (s, e) =>
        //    {
        //        tochLayout.IsAnimating = false;
        //        tochLayout.OneSwipe = false;
        //    };
        //    animator.Start();
        //}

        public void InitializeView()
        {
            CurrentActivity.RunOnUI(() =>
            {
                if (_actionToolBoxLayout.ChildCount > 0)
                    _actionToolBoxLayout.RemoveAllViews();

                _actionToolBoxLayout.AddView(_linkInfoToolBox);

                if (_actionTopToolBoxLayout.ChildCount > 0)
                    _actionTopToolBoxLayout.RemoveAllViews();

                _actionTopToolBoxLayout.AddView(_findLinksToolBox);

                _actionTopToolBoxLayout.Visibility = ViewStates.Visible;

                if (_mainLayout.ChildCount > 0)
                    _mainLayout.RemoveAllViews();
                //if (tochLayout.ChildCount > 0)
                //    tochLayout.RemoveAllViews();
                //tochLayout.OnSwipeLeftFunction = () =>
                //{
                //    int current = (int)_cuurentMenuItemModeEnum;
                //    if (current == 25)
                //        _cuurentMenuItemModeEnum = MenuItemModeEnum.LinksSearchQueue;
                //    else
                //    {
                //        current--;
                //        _cuurentMenuItemModeEnum = (MenuItemModeEnum)current;
                //    }
                //    tochLayout.IsAnimating = true;
                //    if ((int)Android.OS.Build.VERSION.SdkInt >= 11)
                //    {
                //        tochLayout.ClearAnimation();
                //        //var width = tochLayout.Width - tochLayout.PaddingLeft;
                //        ValueAnimator animator = ValueAnimator.OfInt(tochLayout.PaddingLeft, -tochLayout.Width);
                //        animator.SetDuration(500);
                //        animator.Update += (s, e) =>
                //        {
                //            int value = (int)animator.AnimatedValue;
                //            tochLayout.SetPadding(value, tochLayout.PaddingTop, -value, tochLayout.PaddingBottom);
                //        };
                //        animator.AnimationEnd += (s, e) =>
                //        {
                //            SearchLinksClickMenuItem(new MenuItem() { Mode = _cuurentMenuItemModeEnum }, CurrentActivity);
                //            PaddingToZeroAnimation();
                //        };
                //        animator.Start();
                //    }
                //    else
                //    {
                //        SearchLinksClickMenuItem(new MenuItem() { Mode = _cuurentMenuItemModeEnum }, CurrentActivity);
                //        tochLayout.SetPadding(0, tochLayout.PaddingTop, 0, tochLayout.PaddingBottom);
                //        tochLayout.IsAnimating = false;
                //    }
                //    return false;
                //};

                //tochLayout.OnSwipeRightFunction = () =>
                //{
                //    try
                //    {
                //        int current = (int)_cuurentMenuItemModeEnum;
                //        if (current == 30)
                //            _cuurentMenuItemModeEnum = MenuItemModeEnum.LinksSearchAll;
                //        else
                //        {
                //            current++;
                //            _cuurentMenuItemModeEnum = (MenuItemModeEnum)current;
                //        }

                //        //var width = tochLayout.Width - tochLayout.PaddingLeft;
                //        tochLayout.IsAnimating = true;
                //        if ((int)Android.OS.Build.VERSION.SdkInt >= 11)
                //        {
                //            tochLayout.ClearAnimation();
                //            ValueAnimator animator = ValueAnimator.OfInt(tochLayout.PaddingLeft, tochLayout.Width);
                //            animator.SetDuration(500);
                //            animator.Update += (s, e) =>
                //            {
                //                int value = (int)animator.AnimatedValue;
                //                tochLayout.SetPadding(value, tochLayout.PaddingTop, -value, tochLayout.PaddingBottom);
                //            };
                //            animator.AnimationEnd += (s, e) =>
                //            {
                //                SearchLinksClickMenuItem(new MenuItem() { Mode = _cuurentMenuItemModeEnum }, CurrentActivity);
                //                PaddingToZeroAnimation();
                //            };
                //            animator.Start();
                //        }
                //        else
                //        {
                //            SearchLinksClickMenuItem(new MenuItem() { Mode = _cuurentMenuItemModeEnum }, CurrentActivity);
                //            tochLayout.SetPadding(0, tochLayout.PaddingTop, 0, tochLayout.PaddingBottom);
                //            tochLayout.IsAnimating = false;
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        InitializeApplication.GoException(ex);
                //    }
                //    return false;
                //};
                //tochLayout.AddView(_CustomListView);
                //_mainLayout.AddView(tochLayout);
                _mainLayout.AddView(_CustomListView);
                //ViewsUtility.SetBackground(CurrentActivity, tochLayout, Resource.Color.background);
                ViewsUtility.SetBackground(CurrentActivity, _CustomListView, Resource.Color.background);
                //ViewsUtility.SetBackground(CurrentActivity, _mainLayout, Resource.Color.ProgressStopedBackground);
                GenerateButtonsVisibility();
            });

        }

        void _btnPauseQueue_Click(object sender, EventArgs e)
        {
            LinkInfoClickMenuItem(new MenuItem() { Mode = MenuItemModeEnum.StopByQueue }, null, CurrentActivity);
        }

        void _btnDeleteQueue_Click(object sender, EventArgs e)
        {
            LinkInfoClickMenuItem(new MenuItem() { Mode = MenuItemModeEnum.DeleteLinkInfoQueue }, null, CurrentActivity);
        }

        void _btnPlayQueue_Click(object sender, EventArgs e)
        {
            LinkInfoClickMenuItem(new MenuItem() { Mode = MenuItemModeEnum.DownloadByQueue }, null, CurrentActivity);
        }

        void _btnDelete_Click(object sender, EventArgs e)
        {
            LinkInfoClickMenuItem(new MenuItem() { Mode = MenuItemModeEnum.Delete }, null, CurrentActivity);
        }

        void _btnPause_Click(object sender, EventArgs e)
        {
            StopLinks();
            GenerateButtonsVisibility();
        }

        void _btnPlay_Click(object sender, EventArgs e)
        {
            PlayLinks(CurrentActivity);
        }

        void _btnShowMenu_Click(object sender, EventArgs e)
        {
            CurrentActivity.RegisterForContextMenu(_mainLayoutListView);
            MainActivity.IsLinkToolMenu = true;
            CurrentActivity.OpenContextMenu(_mainLayoutListView);
            CurrentActivity.UnregisterForContextMenu(_mainLayoutListView);
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_isDispose)
                return;
            CurrentActivity.RunOnUI(() =>
            {
                try
                {
                    NotifyChanged();
                }
                catch (Exception ex)
                {
                    InitializeApplication.GoException(ex, "notifyChanged");
                }
            });

        }

        public override IEnumerable<LinkInfo> GetSelectedItems()
        {
            return selectedItems;
        }

        public void SetProgressStyleByState(ProgressBar prog, View linearImage, LinkInfo linkInfo)
        {
            if (linkInfo.IsComplete || linkInfo.DownloadingProperty.State == ConnectionState.CopyingFile)
            {
                ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Complete);
                if (!selectedItems.Contains(linkInfo))
                    ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressCompleteBackground);
            }
            else if (linkInfo.IsDownloading)
            {
                ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Downloading);
                if (!selectedItems.Contains(linkInfo))
                    ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressDownloadingBackground);
            }
            else if (linkInfo.IsWaitingForPlayQueue)
            {
                ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Queue);
                if (!selectedItems.Contains(linkInfo))
                    ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressQueueBackground);
            }
            else if (linkInfo.IsError)
            {
                ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Error);
                if (!selectedItems.Contains(linkInfo))
                    ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressErrorBackground);
            }
            else if (!linkInfo.IsSizeValue)
            {
                ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_NotStarted);
                if (!selectedItems.Contains(linkInfo))
                    ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressNotStartedBackground);
            }
            else if (linkInfo.IsManualStop)
            {
                ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Stoped);
                if (!selectedItems.Contains(linkInfo))
                    ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressStopedBackground);
            }
        }

        public void SetTextViewTextColorByState(TextView txt, LinkInfo linkInfo)
        {
            if (linkInfo.IsError || (linkInfo.IsManualStop && !linkInfo.IsWaitingForPlayQueue && linkInfo.IsSizeValue && !linkInfo.IsComplete))
            {
                ViewsUtility.SetTextViewTextColor(CurrentActivity, txt, Resource.Color.white);
            }
            else
            {
                ViewsUtility.SetTextViewTextColor(CurrentActivity, txt, Resource.Color.LinkAddressForeground);
            }
        }

        List<LinkInfo> selectedItems = new List<LinkInfo>();

        public void SelectItem(LinkInfo item)
        {
            if (!selectedItems.Contains(item))
                selectedItems.Add(item);
            GenerateButtonsVisibility();
        }

        public void DeSelectItem(LinkInfo item)
        {
            selectedItems.Remove(item);
            GenerateButtonsVisibility();
        }

        void GenerateButtonsVisibility()
        {
            if (selectedItems.Count > 0 && IsSelectionMode)
                _actionToolBoxLayout.Visibility = ViewStates.Visible;
            else
                _actionToolBoxLayout.Visibility = ViewStates.Gone;

            _btnPlay.Enabled = CanPlayLinks();
            ViewsUtility.SetAlphaByEnabled(_btnPlay);

            //_btnPlayQueue.Enabled = CanPlaySelectionTask();
            //ViewsUtility.SetAlphaByEnabled(_btnPlayQueue);

            _btnPause.Enabled = CanStopLinks();
            ViewsUtility.SetAlphaByEnabled(_btnPause);

            //_btnPauseQueue.Enabled = CanStopSelectionTask();
            //ViewsUtility.SetAlphaByEnabled(_btnPauseQueue);

            _btnDelete.Enabled = CanDeleteLinks();
            ViewsUtility.SetAlphaByEnabled(_btnDelete);

            _btnDeleteQueue.Enabled = CanDeleteSelectedLinksTimes();
            ViewsUtility.SetAlphaByEnabled(_btnDeleteQueue);
        }

        bool _isDispose = false;
        public static Action DeSelectCurrentItem { get; set; }
        void layoutMain_Click(object sender, EventArgs e)
        {
            var layoutMain = sender as LinearLayout;
            var info = layoutMain.Tag.Cast<LinkInfo>();
            View view = null;
            bool isFind = getViewByLinkInfo.TryGetValue(info, out view);
            if (!isFind)
                return;

            if (view == null)
            {
                return;
            }
            if (IsSelectionMode)
            {
                var item = info;
                if (selectedItems.Contains(item))
                {
                    DeSelectItem(item);
                    DeSelectView(view, info);
                }
                else
                {
                    SelectItem(item);
                    SelectView(view, info);
                }
            }
            else
            {

            }
        }

        bool _isSelectionMode = true;

        public bool IsSelectionMode
        {
            get { return _isSelectionMode; }
            set { _isSelectionMode = value; }
        }

        public int SelectionCount
        {
            get
            {
                return selectedItems.Count;
            }
        }

        void layoutMain_LongClick(object sender, View.LongClickEventArgs e)
        {
            try
            {
                var layoutMain = sender as LinearLayout;
                var info = layoutMain.Tag.Cast<LinkInfo>();
                var view = getViewByLinkInfo[info];

                DeSelectAll();
                SelectItem(info);
                SelectView(view, info, false);
                MainActivity.SelectedLinkInfoFromMenu = info;
                DeSelectCurrentItem = () =>
                {
                    DeSelectItem(info);
                    DeSelectView(view, info, false);
                    DeSelectCurrentItem = null;
                };
                MainActivity.IsLinkToolMenu = false;
                CurrentActivity.RegisterForContextMenu(_mainLayoutListView);
                CurrentActivity.OpenContextMenu(_mainLayoutListView);
                CurrentActivity.UnregisterForContextMenu(_mainLayoutListView);

                //ViewsUtility.ShowMenuDialog<MenuItem>(CurrentActivity, info.PathInfo.FileName, menu, (mnu) => mnu.Content, (mnu) =>
                //{
                //    LinkInfoClickMenuItem(mnu, info);
                //}, () => DeSelectView(view, info, false));

                //View linkInfoMenu = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.LinkInfoMenu, _mainLayout, false);
                //PopupMenu popup = new PopupMenu(CurrentActivity, layoutMain);
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.Menu.Add("titleRes");
                //popup.DismissEvent += (s, ee) =>
                //{
                //    DeSelectView(view, info, false);
                //};
                //SelectView(view, info, false);
                //popup.Show();
            }
            catch (Exception ex)
            {
                var ms = ex.Message;
                var qq = ex.StackTrace;
            }
            //IsSelectionMode = !IsSelectionMode;
            //if (!IsSelectionMode)
            //{
            //DeSelectAll();
            //layoutMain_Click(sender, null);
            //}
            //else
            //{
            //    layoutMain_Click(sender, null);
            //}
        }

        public void DeSelectAll()
        {
            foreach (var item in selectedItems.ToArray())
            {
                DeSelectItem(item);
                if (getViewByLinkInfo.ContainsKey(item))
                    DeSelectView(getViewByLinkInfo[item], item);
            }
        }

        public void SelectAll()
        {
            foreach (var item in Items)
            {
                if (!selectedItems.Contains(item))
                {
                    SelectItem(item);
                    SelectView(getViewByLinkInfo[item], item);
                }
            }
        }

        public void SelectView(View view, LinkInfo item, bool setQueueNumber = true)
        {
            RelativeLayout linearImage = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.linearImage);
            ViewsUtility.SetBackground(CurrentActivity, view, Resource.Drawable.SelectedListItemBackground);

            view.FindViewById<ProgressBar>(Resource.LinkInfoListItem.prgDownload).Visibility = ViewStates.Invisible;
            ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.SelectedBackground);

            ImageView imgIcon = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgIcon);
            imgIcon.Visibility = ViewStates.Invisible;
            ImageView imgcheck = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgCheck);
            imgcheck.Visibility = ViewStates.Visible;

            if (setQueueNumber)
            {
                RelativeLayout layoutQueue = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.layoutQueue);
                layoutQueue.Visibility = ViewStates.Visible;
                TextView txtQueueNumber = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtQueueNumber);
                if (!BindingHelper.IsBinded(item, new List<string>() { "SelectedIndexNumber" }))
                    BindingHelper.BindAction(item, item, new List<string>() { "SelectedIndexNumber" }, (property) =>
                    {
                        txtQueueNumber.Text = item.SelectedIndexNumber;
                    });
                drawSelectionNumbers();
            }
        }

        public void DeSelectView(View view, LinkInfo item, bool setQueueNumber = true)
        {
            RelativeLayout linearImage = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.linearImage);
            ViewsUtility.SetBackground(CurrentActivity, view, Resource.Color.background);
            var progress = view.FindViewById<ProgressBar>(Resource.LinkInfoListItem.prgDownload);
            progress.Visibility = ViewStates.Visible;
            SetProgressStyleByState(progress, linearImage, item);
            var txtFileName = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtFileName);
            SetTextViewTextColorByState(txtFileName, item);
            ImageView imgIcon = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgIcon);
            imgIcon.Visibility = ViewStates.Visible;
            ImageView imgcheck = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgCheck);
            imgcheck.Visibility = ViewStates.Invisible;
            if (setQueueNumber)
            {
                RelativeLayout layoutQueue = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.layoutQueue);
                layoutQueue.Visibility = ViewStates.Invisible;
                TextView txtQueueNumber = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtQueueNumber);
                if (!BindingHelper.IsBinded(item, new List<string>() { "SelectedIndexNumber" }))
                    BindingHelper.BindAction(item, item, new List<string>() { "SelectedIndexNumber" }, (property) =>
                    {
                        CurrentActivity.RunOnUI(() =>
                        {
                            txtQueueNumber.Text = item.SelectedIndexNumber;
                        });
                    });

                if (!BindingHelper.IsBinded(item, new List<string>() { "IsWaitingForPlayQueue" }))
                    BindingHelper.BindAction(item, item, new List<string>() { "IsWaitingForPlayQueue" }, (property) =>
                    {
                        CurrentActivity.RunOnUI(() =>
                        {
                            SetProgressStyleByState(progress, linearImage, item);
                            SetTextViewTextColorByState(txtFileName, item);
                        });
                    });
                drawSelectionNumbers();
            }
        }

        Dictionary<LinkInfo, View> getViewByLinkInfo = new Dictionary<LinkInfo, View>();
        bool isDrawing = true;
        public void DrawAllItems()
        {
            lastChnages = Items.ToList();
            foreach (var item in Items)
            {
                if (CurrentActivity == null)
                    AutoLogger.LogText("CurrentActivity is null on DrawAllItems");
                CurrentActivity.RunOnUI(() =>
                {
                    try
                    {
                        var view = DrawOneView(item);
                        if (view == null)
                            AutoLogger.LogText("view is null on DrawAllItems");
                        if (item == null)
                            AutoLogger.LogText("item is null on DrawAllItems");
                        _mainLayoutListView.AddView(view, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                        if (!getViewByLinkInfo.ContainsKey(item))
                            getViewByLinkInfo.Add(item, view);
                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, $"on DrawAllItems {_templateResourceId} {_mainLayoutListView == null}");
                    }
                });
            }
            isDrawing = false;
        }

        List<LinkInfo> lastChnages = null;
        object ChangeItemsLock = new object();
        public void NotifyChanged()
        {
            int index = 0;
            var added = this.Items.Except(lastChnages).ToList();
            var removed = lastChnages.Except(this.Items).ToList();
            if (added.Count == 0 && removed.Count == 0)
                return;

            foreach (var item in added)
            {
                lock (ChangeItemsLock)
                {
                    if (_isDispose)
                        return;
                    index = Items.IndexOf(item);
                    if (!getViewByLinkInfo.ContainsKey(item))
                    {
                        var view = DrawOneView(item);
                        _mainLayoutListView.AddView(view, index, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
                        getViewByLinkInfo.Add(item, view);
                    }
                    else
                    {
                        _mainLayoutListView.AddView(getViewByLinkInfo[item], 0);
                    }
                }
            }

            foreach (var item in removed)
            {
                lock (ChangeItemsLock)
                {
                    if (_isDispose)
                        return;
                    BindingHelper.DisposeObject(item);
                    selectedItems.Remove(item);
                    _mainLayoutListView.RemoveView(getViewByLinkInfo[item]);
                    if (!IsSearch)
                    {
                        getViewByLinkInfo.Remove(item);
                    }
                }
            }

            lastChnages = Items.ToList();


            GenerateButtonsVisibility();
        }

        public View DrawOneView(LinkInfo item)
        {
            if (_isDispose)
                return null;

            bool isSelected = selectedItems.Contains(item);
            var view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, _mainLayoutListView, false);

            try
            {
                var layoutMain = view.FindViewById<LinearLayout>(Resource.LinkInfoListItem.layoutMain);
                RelativeLayout linearImage = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.linearImage);

                layoutMain.Tag = item.Cast();
                layoutMain.Clickable = true;
                layoutMain.LongClickable = true;
                layoutMain.Click -= layoutMain_Click;
                layoutMain.Click += layoutMain_Click;
                layoutMain.LongClickable = true;
                layoutMain.LongClick -= layoutMain_LongClick;
                layoutMain.LongClick += layoutMain_LongClick;

                InitializeNormalListViewItem(item, view, isSelected ? null : linearImage);

                if (!isSelected)
                {
                    DeSelectView(layoutMain, item);
                }
                else
                {
                    SelectView(layoutMain, item);
                }
            }
            catch (Exception e)
            {
                InitializeApplication.GoException(e, "DrawOneView");
            }
            return view;
        }

        void drawSelectionNumbers()
        {
            int i = 1;
            foreach (var item in selectedItems)
            {
                item.SelectedIndexNumber = i.ToString();
                i++;
            }
        }

        object lockObj = new object();
        void InitializeNormalListViewItem(LinkInfo item, View view, RelativeLayout linearImage)
        {
            try
            {

                var txtFileName = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtFileName);
                var txtSize = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtSize);
                var txtDownloadedSize = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtDownloadedSize);
                var txtState = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtState);
                var prgDownload = view.FindViewById<ProgressBar>(Resource.LinkInfoListItem.prgDownload);
                var layoutStateBottom = view.FindViewById<View>(Resource.LinkInfoListItem.layoutStateBottom);
                var layoutStateBottonCenter = view.FindViewById<View>(Resource.LinkInfoListItem.layoutStateBottonCenter);

                var txtTimeRemaning = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtTimeRemaning);
                var txtPercent = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtPercent);
                var txtSpeed = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtSpeed);
                var imgIcon = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgIcon);
                var layoutStateErrorTime = view.FindViewById<View>(Resource.LinkInfoListItem.layoutStateErrorTime);
                var txtErrorReconnectTimer = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtErrorReconnectTimer);

                ViewsUtility.SetFont(CurrentActivity, new List<View>() { txtFileName, txtSize, txtDownloadedSize, txtState });

                NewSetBinding:


                Action<string> renderDrawbles = (property) =>
                {
                    MainActivity.This.RunOnUI(() =>
                    {
                        try
                        {
                            if (_isDispose)
                                return;
                            string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.Size);
                            txtSize.Text = size[0] + " " + ViewsUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");

                            var downSize = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.DownloadedSize);
                            txtDownloadedSize.Text = downSize[0] + " " + ViewsUtility.FindTextLanguage(CurrentActivity, downSize[1] + "_Language");

                            txtState.Text = ViewsUtility.FindTextLanguage(CurrentActivity, item.DownloadingProperty.State.ToString() + "_Language");

                            if (property == "State" || property == null)
                            {
                                SetProgressStyleByState(prgDownload, linearImage, item);
                                SetTextViewTextColorByState(txtFileName, item);
                                //var old = prgDownload.Progress;
                                //if (old == 0)
                                //{
                                //    prgDownload.Progress++;
                                //    prgDownload.Progress--;
                                //}
                                //else
                                //{
                                //    prgDownload.Progress = 0;
                                //    prgDownload.Progress = old;
                                //}
                            }

                            txtTimeRemaning.Text = ViewsUtility.TimeToString(CurrentActivity, item.DownloadingProperty.TimeRemaining);
                            txtPercent.Text = item.DownloadingProperty.GetPercent;
                            size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.SpeedByteDownloaded);
                            txtSpeed.Text = size[0] + " " + ViewsUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");

                            if (item.IsDownloading || item.CanStop)
                                layoutStateBottom.Visibility = ViewStates.Visible;
                            else
                                layoutStateBottom.Visibility = ViewStates.Gone;
                            //prgDownload.Max = 200;
                            //prgDownload.Progress = 20;
                            //prgDownload.Progress = 0;
                            if (item.DownloadingProperty.Size >= 0.0)
                                prgDownload.Progress = (int)(100 / (item.DownloadingProperty.Size / item.DownloadingProperty.DownloadedSize));
                            else
                                prgDownload.Progress = 100;
                            GenerateButtonsVisibility();
                            //prgDownload.Max = 100;
                            //prgDownload.Progress = prgDownload.Progress;
                            //string fileName = item.PathInfo.FileName;
                            //textBuilder.Clear();
                            //textBuilder.Append(ViewUtility.FindTextLanguage(CurrentActivity, item.DownloadingProperty.State.ToString() + "_Language"));
                            //textBuilder.Append(" | " + ViewUtility.FindTextLanguage(CurrentActivity, "Size_Language") + " ");
                            //string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.Size);
                            //textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language"));
                            //textBuilder.Append(" | " + ViewUtility.FindTextLanguage(CurrentActivity, "Downloaded_Language") + " ");
                            //size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.DownloadedSize);
                            //textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language"));
                            //chkFileName.Text = fileName;
                            //txtData.Text = textBuilder.ToString();
                            //if (item.DownloadingProperty.Size >= 0.0)
                            //    mainProgress.Progress = (int)(100 / (item.DownloadingProperty.Size / item.DownloadingProperty.DownloadedSize));
                            //else
                            //    mainProgress.Progress = 0;
                            //if (property == "State")
                            //    SetLayoutColor(item, layout, selectLayout);
                        }
                        catch (Exception ex)
                        {
                            Agrin.Log.AutoLogger.LogError(ex, "InitializeNormalListViewItem 2", true);
                        }
                    });
                };

                Action<string> changedAction = (property) =>
                {
                    if (_isDispose)
                        return;
                    //ActionRunner.AddAction(item, () =>
                    //{
                    renderDrawbles(property);
                    //});
                };

                if (BindingHelper.IsBinded(item, new List<string>() { "FileName" }))
                {
                    BindingHelper.DisposeObject(item);
                }

                BindingHelper.BindAction(item, item.PathInfo, new List<string>() { "FileName" }, (p) =>
                {
                    MainActivity.This.RunOnUI(() =>
                    {
                        try
                        {
                            txtFileName.Text = item.PathInfo.FileName;
                            txtFileName.Visibility = ViewStates.Gone;
                            txtFileName.Visibility = ViewStates.Visible;

                            item.FileIcon = null;
                        }
                        catch (Exception ex)
                        {
                            Agrin.Log.AutoLogger.LogError(ex, "BindAction FileName", true);
                        }
                    });
                });

                if (BindingHelper.IsBinded(item, new List<string>() { "ReconnectTimer" }))
                {
                    BindingHelper.DisposeObject(item);
                }

                BindingHelper.BindAction(item, item.DownloadingProperty, new List<string>() { "ReconnectTimer" }, (p) =>
                {
                    MainActivity.This.RunOnUI(() =>
                    {
                        try
                        {
                            if (item.IsDownloading || item.CanStop)
                                layoutStateBottom.Visibility = ViewStates.Visible;
                            else
                                layoutStateBottom.Visibility = ViewStates.Gone;
                            txtErrorReconnectTimer.Text = item.DownloadingProperty.ReconnectTimer;

                            if (item.DownloadingProperty.ReconnectTimer == "0:0")
                            {
                                layoutStateErrorTime.Visibility = ViewStates.Gone;
                                layoutStateBottonCenter.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                layoutStateBottonCenter.Visibility = ViewStates.Gone;
                                layoutStateErrorTime.Visibility = ViewStates.Visible;
                            }
                        }
                        catch (Exception ex)
                        {
                            Agrin.Log.AutoLogger.LogError(ex, "BindAction ReconnectTimer", true);
                        }
                    });
                });

                if (BindingHelper.IsBinded(item, new List<string>() { "FileIcon" }))
                {
                    BindingHelper.DisposeObject(item);
                }

                BindingHelper.BindAction(item, item, new List<string>() { "FileIcon" }, (p) =>
                {
                    MainActivity.This.RunOnUI(() =>
                    {
                        try
                        {
                            if (item.FileIcon != null)
                            {
                                var bmp = ByteToBitmap(item.FileIcon);
                                imgIcon.SetImageBitmap(bmp);
                            }
                        }
                        catch (Exception ex)
                        {
                            Agrin.Log.AutoLogger.LogError(ex, "BindAction FileIcon", true);
                        }
                    });
                });

                // BindingHelper.BindOneWay(item, txtFileName, "Text", item.PathInfo, "FileName");
                if (item.FileIcon != null)
                {
                    var bmp = ByteToBitmap(item.FileIcon);
                    imgIcon.SetImageBitmap(bmp);
                }

                var listProps = new List<string>() { "DownloadedSize", "State", "Size" };

                if (BindingHelper.IsBinded(item.DownloadingProperty, listProps))
                {
                    BindingHelper.DisposeObject(item);
                    goto NewSetBinding;
                }

                BindingHelper.BindAction(item, item.DownloadingProperty, listProps, changedAction);

                renderDrawbles(null);
                txtFileName.Text = item.PathInfo.FileName;
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "InitializeNormalListViewItem 1", true);
            }
        }

        Android.Graphics.Bitmap ByteToBitmap(byte[] bytes)
        {
            var bmp = Android.Graphics.BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            return bmp;
        }

        public static List<MenuItem> GenerateLinkInfoMenues(LinkInfo linkInfo, Activity activity)
        {
            Func<string, string> getText = (text) =>
            {
                return ViewsUtility.FindTextLanguage(activity, text);
            };

            List<MenuItem> items = new List<MenuItem>();
            if (linkInfo.CanPlay)
                items.Add(new MenuItem() { Content = getText("Play_Language"), Mode = MenuItemModeEnum.Play });
            if (linkInfo.CanStop)
                items.Add(new MenuItem() { Content = getText("Stop_Language"), Mode = MenuItemModeEnum.Stop });
            if (!linkInfo.IsComplete)
            {
                items.Add(new MenuItem() { Content = getText("DownloadByQueue_Language"), Mode = MenuItemModeEnum.DownloadByQueue });
                items.Add(new MenuItem() { Content = getText("StopByQueue_Language"), Mode = MenuItemModeEnum.StopByQueue });
                items.Add(new MenuItem() { Content = getText("ForceStart_Language"), Mode = MenuItemModeEnum.ForcePlay });
                if (linkInfo.IsAddedToTaskForStart || linkInfo.IsAddedToTaskForStop)
                    items.Add(new MenuItem() { Content = getText("DeleteLinkInfoQueue_Language"), Mode = MenuItemModeEnum.DeleteLinkInfoQueue });
            }

            if (CanOpenFile(linkInfo))
            {
                items.Add(new MenuItem() { Content = getText("OpenFile_Language"), Mode = MenuItemModeEnum.OpenFile });
                items.Add(new MenuItem() { Content = getText("OpenFileLocation_Language"), Mode = MenuItemModeEnum.OpenFileLocation });
                items.Add(new MenuItem() { Content = getText("ShareFile_Language"), Mode = MenuItemModeEnum.ShareFile });
            }
            else
            {
                items.Add(new MenuItem() { Content = getText("ChangeLinkAddress_Language"), Mode = MenuItemModeEnum.ChangeLinkAddress });
            }

            if (linkInfo.IsComplete)
            {
                items.Add(new MenuItem() { Content = getText("RepairLink_Language"), Mode = MenuItemModeEnum.RepairLink });
            }

            items.Add(new MenuItem() { Content = getText("CopyLinkAddress_Language"), Mode = MenuItemModeEnum.CopyLink });
            items.Add(new MenuItem() { Content = getText("ChangeFileLocation_Language"), Mode = MenuItemModeEnum.ChangeSavePath });
            items.Add(new MenuItem() { Content = getText("ChangeFileName_Language"), Mode = MenuItemModeEnum.RenameFileName });
            items.Add(new MenuItem() { Content = getText("ShowLastError_Language"), Mode = MenuItemModeEnum.CheckLastError });
            items.Add(new MenuItem() { Content = getText("Delete_Language"), Mode = MenuItemModeEnum.Delete });
            items.Add(new MenuItem() { Content = getText("GenerateAutoMixerReport_Language"), Mode = MenuItemModeEnum.GenerateAutoMixerReport });

            return items;
        }

        public static List<MenuItem> GenerateSearchMenues(Activity activity)
        {
            Func<string, string> getText = (text) =>
            {
                return ViewsUtility.FindTextLanguage(activity, text);
            };
            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem() { Content = getText("LinksSearchAll_Language"), Mode = MenuItemModeEnum.LinksSearchAll });
            items.Add(new MenuItem() { Content = getText("LinksSearchComplete_Language"), Mode = MenuItemModeEnum.LinksSearchComplete });
            items.Add(new MenuItem() { Content = getText("LinksSearchDownloading_Language"), Mode = MenuItemModeEnum.LinksSearchDownloading });
            items.Add(new MenuItem() { Content = getText("LinksSearchNotComplete_Language"), Mode = MenuItemModeEnum.LinksSearchNotComplete });
            items.Add(new MenuItem() { Content = getText("LinksSearchError_Language"), Mode = MenuItemModeEnum.LinksSearchError });
            items.Add(new MenuItem() { Content = getText("LinksSearchQueue_Language"), Mode = MenuItemModeEnum.LinksSearchQueue });

            return items;
        }

        public void DisposeSelection()
        {
            foreach (var item in selectedItems)
            {
                BindingHelper.DisposeObject(item);
            }
        }

        public override DateTime GetDateTimeForAddTask()
        {
            Calendar pCal = new PersianCalendar();
            var dt = pCal.ToDateTime(AddDateYear, AddDateMonth, AddDateDay, 0, 0, 0, 0);
            dt = dt.AddHours(AddTimeHours).AddMinutes(AddTimeMinutes);
            return dt;
        }

        public override DateTime GetDateTimeForStopTask()
        {
            Calendar pCal = new PersianCalendar();
            var dt = pCal.ToDateTime(AddDateYear, AddDateMonth, AddDateDay, 0, 0, 0, 0);
            dt = dt.AddHours(StopTimeHours).AddMinutes(StopTimeMinutes);
            return dt;
        }

        public static void StopLinks(Activity activity)
        {
            Action<string, Action> showdialog = (message, ok) =>
            {
                TextView txt = new TextView(activity);
                //txt.SetMaxWidth(200);
                ViewsUtility.SetTextViewTextColor(activity, txt, Resource.Color.foreground);
                txt.Text = ViewsUtility.FindTextLanguage(activity, message);
                txt.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                txt.SetMaxWidth(600);
                ViewsUtility.ShowCustomResultDialog(activity, ViewsUtility.FindTextLanguage(activity, "StopAll_Language"), txt, () =>
                {
                    ok();
                });
            };

            bool isNotSupportResumable = false;

            foreach (var item in This.GetSelectedItems())
            {
                if (item.IsDownloading && (item.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.No || item.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Unknown) && item.DownloadingProperty.DownloadedSize > 0)
                {
                    isNotSupportResumable = true;
                    break;
                }
            }

            if (isNotSupportResumable)
            {
                showdialog("Areyousureyouwanttostopnotsupportlinks_Language", () =>
                {
                    This.StopSelectionTask();
                    This.StopLinks();
                });
            }
            else
            {
                This.StopSelectionTask();
                This.StopLinks();
            }
        }

        public static void PlayLinks(Activity activity)
        {
            bool isInQueue = false;

            foreach (var item in This.GetSelectedItems())
            {
                if (!item.CanPlay)
                    continue;
                var tasks = Agrin.Download.Manager.ApplicationTaskManager.Current.FindTaskInfoByLinkInfo(item);

                foreach (var task in tasks)
                {
                    isInQueue = true;
                    break;
                }
                if (isInQueue)
                    break;
            }
            if (!isInQueue)
            {
                This.PlayLinks();
                This.GenerateButtonsVisibility();
            }
            else
            {
                TextView txt = new TextView(activity);
                //txt.SetMaxWidth(200);
                ViewsUtility.SetTextViewTextColor(activity, txt, Resource.Color.foreground);
                txt.Text = ViewsUtility.FindTextLanguage(activity, "Areyouwanttoplaylinksqueues_Language");
                txt.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                txt.SetMaxWidth(600);
                ViewsUtility.ShowCustomResultDialog(activity, "PlayLink_Language", txt, () =>
                {
                    This.PlayLinks();
                    This.GenerateButtonsVisibility();
                }, noAction: () =>
                {
                    foreach (var item in This.GetSelectedItems())
                    {
                        if (!item.CanPlay)
                            continue;
                        var tasks = Agrin.Download.Manager.ApplicationTaskManager.Current.FindTaskInfoByLinkInfo(item);

                        foreach (var task in tasks)
                        {
                            Agrin.Download.Manager.ApplicationTaskManager.Current.DeActiveTask(task);
                            Agrin.Download.Manager.ApplicationTaskManager.Current.ActiveTask(task);
                            break;
                        }
                    }
                    This.GenerateButtonsVisibility();
                }, okButtonText: "PlayLink_Language", noButtonText: "ActiveQueue_Language");
            }
        }

        public static void DownloadByQueue(Activity activity, Func<bool> clickOK)
        {
            LinearLayout mainLayout = new LinearLayout(activity);
            mainLayout.Orientation = Orientation.Vertical;
            var viewObj = activity.LayoutInflater.Inflate(Resource.Layout.PlayQueueDateTimeSelector, mainLayout, false);
            //btnUp.Touch += mainL_Touch;
            //txtValue.Touch += mainL_Touch;
            //btnDown.Touch += mainL_Touch;


            Calendar cal = new PersianCalendar();

            var hourNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.PlayQueueDateTimeSelector.hourNumeric);
            hourNumeric.Minimum = 0;
            hourNumeric.Maximum = 23;
            hourNumeric.Value = DateTime.Now.Hour;

            var minuteNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.PlayQueueDateTimeSelector.minuteNumeric);
            minuteNumeric.Minimum = 0;
            minuteNumeric.Maximum = 59;
            minuteNumeric.Value = DateTime.Now.Minute;

            var yearNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.PlayQueueDateTimeSelector.yearNumeric);
            yearNumeric.Minimum = 1394;
            yearNumeric.Maximum = 1400;
            yearNumeric.Value = cal.GetYear(DateTime.Now);

            var monthNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.PlayQueueDateTimeSelector.monthNumeric);
            monthNumeric.Minimum = 1;
            monthNumeric.Maximum = 12;
            monthNumeric.Value = cal.GetMonth(DateTime.Now);

            var dayNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.PlayQueueDateTimeSelector.dayNumeric);
            dayNumeric.Minimum = 1;
            dayNumeric.Maximum = cal.GetDaysInMonth(cal.GetYear(DateTime.Now), cal.GetMonth(DateTime.Now));
            dayNumeric.Value = cal.GetDayOfMonth(DateTime.Now);

            var chkStartNow = viewObj.FindViewById<CheckBox>(Resource.PlayQueueDateTimeSelector.chkStartNow);
            var enableLayout = viewObj.FindViewById<LinearLayout>(Resource.PlayQueueDateTimeSelector.enableLayout);
            var settingLayout = viewObj.FindViewById<LinearLayout>(Resource.PlayQueueDateTimeSelector.settingLayout);
            //var txtTime = viewObj.FindViewById<TextView>(Resource.PlayQueueDateTimeSelector.txtTime);
            //var txtDate = viewObj.FindViewById<TextView>(Resource.PlayQueueDateTimeSelector.txtDate);
            //var layout = viewObj.FindViewById<LinearLayout>(Resource.PlayQueueDateTimeSelector.LinearLayoutReverce2);

            if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                //SetFont(activity, titleView);
                ViewsUtility.SetRightToLeftLayout(viewObj, new List<int>() { Resource.PlayQueueDateTimeSelector.LinearLayoutReverce2, Resource.PlayQueueDateTimeSelector.rightToLeftlayout1, Resource.PlayQueueDateTimeSelector.rightToLeftlayout2 });
                ViewsUtility.SetRightToLeftViews(viewObj, new List<int>() { Resource.PlayQueueDateTimeSelector.chkStartNow });
            }

            ViewsUtility.SetTextLanguage(activity, viewObj, new List<int>() { Resource.PlayQueueDateTimeSelector.btnAdd, Resource.PlayQueueDateTimeSelector.btnCancel, Resource.PlayQueueDateTimeSelector.txtDate, Resource.PlayQueueDateTimeSelector.txtTime, Resource.PlayQueueDateTimeSelector.chkStartNow, Resource.PlayQueueDateTimeSelector.btnSetting, Resource.PlayQueueDateTimeSelector.chkDataOffAfterComplete, Resource.PlayQueueDateTimeSelector.chkDataOnAfterStart, Resource.PlayQueueDateTimeSelector.chkWiFiOffAfterComplete, Resource.PlayQueueDateTimeSelector.chkWiFiOnAfterStart });
            chkStartNow.CheckedChange += (s, e) =>
            {
                ViewsUtility.SetAlpha(enableLayout, !chkStartNow.Checked, 0.2f);
            };

            ViewsUtility.SetAlpha(enableLayout, !chkStartNow.Checked, 0.2f);
            mainLayout.AddView(viewObj);
            var manual = ViewsUtility.ShowControlDialog(activity, mainLayout, "Queue_Language", null);

            var btnAdd = viewObj.FindViewById<Button>(Resource.PlayQueueDateTimeSelector.btnAdd);
            var btnCancel = viewObj.FindViewById<Button>(Resource.PlayQueueDateTimeSelector.btnCancel);
            var btnSetting = viewObj.FindViewById<ToggleButton>(Resource.PlayQueueDateTimeSelector.btnSetting);

            var chkDataOffAfterComplete = viewObj.FindViewById<CheckBox>(Resource.PlayQueueDateTimeSelector.chkDataOffAfterComplete);
            var chkDataOnAfterStart = viewObj.FindViewById<CheckBox>(Resource.PlayQueueDateTimeSelector.chkDataOnAfterStart);
            var chkWiFiOffAfterComplete = viewObj.FindViewById<CheckBox>(Resource.PlayQueueDateTimeSelector.chkWiFiOffAfterComplete);
            var chkWiFiOnAfterStart = viewObj.FindViewById<CheckBox>(Resource.PlayQueueDateTimeSelector.chkWiFiOnAfterStart);

            btnAdd.Click += (s, e) =>
            {
                This.LinkCompleteTaskUtilityActions.Clear();
                This.TaskUtilityActions.Clear();
                if (chkDataOffAfterComplete.Checked)
                {
                    This.LinkCompleteTaskUtilityActions.Add(TaskUtilityModeEnum.InternetOff);
                }
                if (chkDataOnAfterStart.Checked)
                {
                    This.TaskUtilityActions.Add(TaskUtilityModeEnum.InternetOn);
                }

                if (chkWiFiOffAfterComplete.Checked)
                {
                    This.LinkCompleteTaskUtilityActions.Add(TaskUtilityModeEnum.WiFiOff);
                }
                if (chkWiFiOnAfterStart.Checked)
                {
                    This.TaskUtilityActions.Add(TaskUtilityModeEnum.WiFiOn);
                }
                This.StartNow = chkStartNow.Checked;

                This.AddTimeHours = hourNumeric.Value;
                This.AddTimeMinutes = minuteNumeric.Value;

                This.AddDateYear = yearNumeric.Value;
                This.AddDateMonth = monthNumeric.Value;
                This.AddDateDay = dayNumeric.Value;
                if (clickOK())
                    manual.ManualClose();
            };

            btnCancel.Click += (s, e) =>
            {
                manual.ManualClose();
            };

            btnSetting.CheckedChange += (s, e) =>
            {
                if (btnSetting.Checked)
                {
                    chkStartNow.Visibility = enableLayout.Visibility = ViewStates.Gone;
                    settingLayout.Visibility = ViewStates.Visible;
                    enableLayout.ClearAnimation();
                }
                else
                {
                    chkStartNow.Visibility = enableLayout.Visibility = ViewStates.Visible;
                    settingLayout.Visibility = ViewStates.Gone;
                    ViewsUtility.SetAlpha(enableLayout, !chkStartNow.Checked, 0.2f);
                }
            };
        }

        public static void StopByQueue(Activity activity, Func<bool> clickOK, LinkInfo info)
        {
            LinearLayout mainLayout = new LinearLayout(activity);
            mainLayout.Orientation = Orientation.Vertical;
            var viewObj = activity.LayoutInflater.Inflate(Resource.Layout.StopQueueDateTimeSelector, mainLayout, false);
            //btnUp.Touch += mainL_Touch;
            //txtValue.Touch += mainL_Touch;
            //btnDown.Touch += mainL_Touch;


            Calendar cal = new PersianCalendar();

            var hourNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.StopQueueDateTimeSelector.hourNumeric);
            hourNumeric.Minimum = 0;
            hourNumeric.Maximum = 23;
            hourNumeric.Value = DateTime.Now.Hour;

            var minuteNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.StopQueueDateTimeSelector.minuteNumeric);
            minuteNumeric.Minimum = 0;
            minuteNumeric.Maximum = 59;
            minuteNumeric.Value = DateTime.Now.Minute;

            var yearNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.StopQueueDateTimeSelector.yearNumeric);
            yearNumeric.Minimum = 1394;
            yearNumeric.Maximum = 1400;
            yearNumeric.Value = cal.GetYear(DateTime.Now);

            var monthNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.StopQueueDateTimeSelector.monthNumeric);
            monthNumeric.Minimum = 1;
            monthNumeric.Maximum = 12;
            monthNumeric.Value = cal.GetMonth(DateTime.Now);

            var dayNumeric = viewObj.FindViewById<DateTimeSelectorLayout>(Resource.StopQueueDateTimeSelector.dayNumeric);
            dayNumeric.Minimum = 1;
            dayNumeric.Maximum = cal.GetDaysInMonth(cal.GetYear(DateTime.Now), cal.GetMonth(DateTime.Now));
            dayNumeric.Value = cal.GetDayOfMonth(DateTime.Now);

            var rdoStopLink = viewObj.FindViewById<RadioButton>(Resource.StopQueueDateTimeSelector.rdoStopLink);
            var rdoStopTask = viewObj.FindViewById<RadioButton>(Resource.StopQueueDateTimeSelector.rdoStopTask);
            var enableLayout = viewObj.FindViewById<LinearLayout>(Resource.StopQueueDateTimeSelector.enableLayout);
            var settingLayout = viewObj.FindViewById<LinearLayout>(Resource.StopQueueDateTimeSelector.settingLayout);
            var radiosLayout = viewObj.FindViewById<RadioGroup>(Resource.StopQueueDateTimeSelector.radiosLayout);
            //var txtTime = viewObj.FindViewById<TextView>(Resource.PlayQueueDateTimeSelector.txtTime);
            //var txtDate = viewObj.FindViewById<TextView>(Resource.PlayQueueDateTimeSelector.txtDate);
            //var layout = viewObj.FindViewById<LinearLayout>(Resource.PlayQueueDateTimeSelector.LinearLayoutReverce2);

            if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                //SetFont(activity, titleView);
                ViewsUtility.SetRightToLeftLayout(viewObj, new List<int>() { Resource.StopQueueDateTimeSelector.LinearLayoutReverce2, Resource.StopQueueDateTimeSelector.rightToLeftlayout1, Resource.StopQueueDateTimeSelector.rightToLeftlayout2 });
                ViewsUtility.SetRightToLeftViews(viewObj, new List<int>() { Resource.StopQueueDateTimeSelector.rdoStopLink, Resource.StopQueueDateTimeSelector.rdoStopTask });
            }

            ViewsUtility.SetTextLanguage(activity, viewObj, new List<int>() { Resource.StopQueueDateTimeSelector.btnAdd, Resource.StopQueueDateTimeSelector.btnCancel, Resource.StopQueueDateTimeSelector.txtDate, Resource.StopQueueDateTimeSelector.txtTime, Resource.StopQueueDateTimeSelector.rdoStopLink, Resource.StopQueueDateTimeSelector.rdoStopTask, Resource.StopQueueDateTimeSelector.btnSetting, Resource.StopQueueDateTimeSelector.chkDataOffAfterComplete, Resource.StopQueueDateTimeSelector.chkWiFiOffAfterComplete });

            mainLayout.AddView(viewObj);
            var manual = ViewsUtility.ShowControlDialog(activity, mainLayout, "Queue_Language", null);

            var btnAdd = viewObj.FindViewById<Button>(Resource.StopQueueDateTimeSelector.btnAdd);
            var btnCancel = viewObj.FindViewById<Button>(Resource.StopQueueDateTimeSelector.btnCancel);
            var btnSetting = viewObj.FindViewById<ToggleButton>(Resource.StopQueueDateTimeSelector.btnSetting);

            var chkDataOffAfterComplete = viewObj.FindViewById<CheckBox>(Resource.StopQueueDateTimeSelector.chkDataOffAfterComplete);
            var chkWiFiOffAfterComplete = viewObj.FindViewById<CheckBox>(Resource.StopQueueDateTimeSelector.chkWiFiOffAfterComplete);

            btnAdd.Click += (s, e) =>
            {
                This.LinkCompleteTaskUtilityActions.Clear();
                This.TaskUtilityActions.Clear();
                if (chkDataOffAfterComplete.Checked)
                {
                    This.LinkCompleteTaskUtilityActions.Add(TaskUtilityModeEnum.InternetOff);
                }

                if (chkWiFiOffAfterComplete.Checked)
                {
                    This.LinkCompleteTaskUtilityActions.Add(TaskUtilityModeEnum.WiFiOff);
                }

                This.IsStopTask = rdoStopTask.Checked;

                This.StopTimeHours = hourNumeric.Value;
                This.StopTimeMinutes = minuteNumeric.Value;

                This.AddDateYear = yearNumeric.Value;
                This.AddDateMonth = monthNumeric.Value;
                This.AddDateDay = dayNumeric.Value;
                if (clickOK())
                    manual.ManualClose();
            };
            bool isFindedStart = false;
            if (info != null)
            {
                isFindedStart = info.IsAddedToTaskForStart;
            }
            else
            {
                foreach (var item in This.selectedItems)
                {
                    if (item.IsAddedToTaskForStart)
                    {
                        isFindedStart = true;
                        break;
                    }
                }
            }

            if (!isFindedStart)
            {
                rdoStopLink.Checked = true;
                rdoStopTask.Enabled = false;
                ViewsUtility.SetAlpha(rdoStopTask, rdoStopTask.Enabled, 0.2f);
            }

            btnCancel.Click += (s, e) =>
            {
                manual.ManualClose();
            };

            btnSetting.CheckedChange += (s, e) =>
            {
                if (btnSetting.Checked)
                {
                    radiosLayout.Visibility = enableLayout.Visibility = ViewStates.Gone;
                    settingLayout.Visibility = ViewStates.Visible;
                    rdoStopTask.ClearAnimation();
                }
                else
                {
                    radiosLayout.Visibility = enableLayout.Visibility = ViewStates.Visible;
                    settingLayout.Visibility = ViewStates.Gone;
                    if (!isFindedStart)
                    {
                        rdoStopTask.Enabled = false;
                        ViewsUtility.SetAlpha(rdoStopTask, rdoStopTask.Enabled, 0.2f);
                    }
                }
            };
        }

        public bool IsSearch { get; set; }
        public static void SearchLinksClickMenuItem(MenuItem menu, Activity activity)
        {
            This.IsSearch = true;
            This.DeSelectAll();
            switch (menu.Mode)
            {
                case MenuItemModeEnum.LinksSearchAll:
                    {
                        This.IsSearch = false;
                        This._txtTopToolbarHeader.Text = ViewsUtility.FindTextLanguage(This.CurrentActivity, "LinksSearchAll_Language");
                        Agrin.Download.Engine.SearchEngine.FilterBy(isAll: true);
                        break;
                    }
                case MenuItemModeEnum.LinksSearchComplete:
                    {
                        This._txtTopToolbarHeader.Text = ViewsUtility.FindTextLanguage(This.CurrentActivity, "LinksSearchComplete_Language");
                        Agrin.Download.Engine.SearchEngine.FilterBy(isComplete: true);
                        break;
                    }
                case MenuItemModeEnum.LinksSearchDownloading:
                    {
                        This._txtTopToolbarHeader.Text = ViewsUtility.FindTextLanguage(This.CurrentActivity, "LinksSearchDownloading_Language");
                        Agrin.Download.Engine.SearchEngine.FilterBy(isDownloading: true);
                        break;
                    }
                case MenuItemModeEnum.LinksSearchNotComplete:
                    {
                        This._txtTopToolbarHeader.Text = ViewsUtility.FindTextLanguage(This.CurrentActivity, "LinksSearchNotComplete_Language");
                        Agrin.Download.Engine.SearchEngine.FilterBy(isNoComplete: true);
                        break;
                    }
                case MenuItemModeEnum.LinksSearchError:
                    {
                        This._txtTopToolbarHeader.Text = ViewsUtility.FindTextLanguage(This.CurrentActivity, "LinksSearchError_Language");
                        Agrin.Download.Engine.SearchEngine.FilterBy(isError: true);
                        break;
                    }
                case MenuItemModeEnum.LinksSearchQueue:
                    {
                        This._txtTopToolbarHeader.Text = ViewsUtility.FindTextLanguage(This.CurrentActivity, "LinksSearchQueue_Language");
                        Agrin.Download.Engine.SearchEngine.FilterBy(isQueue: true);
                        break;
                    }
            }
            This._cuurentMenuItemModeEnum = menu.Mode;
            This.SetFindLayoutBackGroundAndForeground(menu.Mode);
        }

        public static void LinkInfoClickMenuItem(MenuItem menu, LinkInfo info, Activity activity)
        {
            try
            {
                if (menu.Mode == MenuItemModeEnum.Play)
                {
                    if (CanPlayLinkInfo(info))
                        PlayLinks(activity);
                }
                else if (menu.Mode == MenuItemModeEnum.ForcePlay)
                {
                    if (!LinksBaseViewModel.CanPlayForce(info))
                        ViewsUtility.ShowMessageBoxOnlyOkButton(activity, "شروع لینک دارای مشکل...", "برای شروع به این روش باید حداقل 95 درصد فایل شما دانلود شده باشد.", null);
                    else
                        This.PlayForceLinkInfo(info);
                }
                else if (menu.Mode == MenuItemModeEnum.Stop)
                {
                    if (CanStopLinkInfo(info))
                        StopLinks(activity);
                }
                else if (menu.Mode == MenuItemModeEnum.DownloadByQueue)
                {
                    DownloadByQueue(activity, () =>
                    {
                        var span = This.GetDateTimeForAddTask() - DateTime.Now;
                        if (This.StartNow || (span.Ticks > 0 && This.CanActiveTaskByDateTimes(This.GetDateTimeForAddTask())))
                        {
                            string msg = "";
                            if (This.StartNow)
                            {
                                msg = "صف مورد نظر شما شروع شده است";
                            }
                            else
                            {
                                msg = ViewsUtility.TimeSpanToText(span);
                            }
                            if (This.selectedItems.Count == 0)
                                This.AddTimeTask(new List<LinkInfo>() { info });
                            else
                                This.AddTimeTask();
                            This.GenerateButtonsVisibility();
                            Toast.MakeText(activity, "تسک مورد نظر شما " + msg + " دیگر شروع می شود.", ToastLength.Long).Show();
                            return true;
                        }
                        else
                        {
                            Toast.MakeText(activity, "لطفاً تاریخ و زمان را بزرگتر از حالا وارد کنید", ToastLength.Long).Show();
                            return false;
                        }
                    });
                }
                else if (menu.Mode == MenuItemModeEnum.DeleteLinkInfoQueue)
                {
                    if (info != null)
                    {
                        Agrin.Download.Manager.ApplicationTaskManager.Current.RemoveLinkInfoFromStartTasks(info);
                        Agrin.Download.Manager.ApplicationTaskManager.Current.RemoveLinkInfoFromStopedTasks(info);
                    }
                    else
                    {
                        foreach (var item in This.selectedItems)
                        {
                            Agrin.Download.Manager.ApplicationTaskManager.Current.RemoveLinkInfoFromStartTasks(item);
                            Agrin.Download.Manager.ApplicationTaskManager.Current.RemoveLinkInfoFromStopedTasks(item);
                        }
                    }
                    This.GenerateButtonsVisibility();
                }
                else if (menu.Mode == MenuItemModeEnum.StopByQueue)
                {
                    This.IsStopForMinutes = false;
                    StopByQueue(activity, () =>
                    {
                        var span = This.GetDateTimeForStopTask() - DateTime.Now;
                        if (span.Ticks > 0 && This.CanActiveTaskByDateTimes(This.GetDateTimeForStopTask()))
                        {
                            string msg = "";
                            msg = ViewsUtility.TimeSpanToText(span);

                            if (This.selectedItems.Count == 0)
                                This.AddStopTimeTask(new List<LinkInfo>() { info });
                            else
                                This.AddStopTimeTask();
                            This.GenerateButtonsVisibility();
                            Toast.MakeText(activity, "عملیات مورد نظر شما " + msg + " دیگر متوقف می شود.", ToastLength.Long).Show();
                            Services.AgrinService.This.SetAlarmWithClosedTaskTime();
                            return true;
                        }
                        else
                        {
                            Toast.MakeText(activity, "لطفاً تاریخ و زمان را بزرگتر از حالا وارد کنید", ToastLength.Long).Show();
                            return false;
                        }
                    }, info);
                }
                else if (menu.Mode == MenuItemModeEnum.OpenFile)
                {
                    if (CanOpenFile(info))
                        OpenFile(info, activity);
                }
                else if (menu.Mode == MenuItemModeEnum.OpenFileLocation)
                {
                    if (CanOpenFile(info))
                        OpenFileLocation(info, activity);
                }
                else if (menu.Mode == MenuItemModeEnum.ShareFile)
                {
                    ShareFile(info, activity);
                }
                else if (menu.Mode == MenuItemModeEnum.ChangeLinkAddress)
                {
                    LinearLayout layout = new LinearLayout(activity);
                    layout.Orientation = Orientation.Vertical;
                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

                    layoutParams.SetMargins(5, 5, 5, 5);
                    layout.LayoutParameters = layoutParams;

                    EditText txtAddress = new EditText(new ContextThemeWrapper(activity, Resource.Style.editText));
                    txtAddress.Hint = ViewsUtility.FindTextLanguage(activity, "LinkAddress_Language");
                    txtAddress.VerticalScrollBarEnabled = true;
                    layout.AddView(txtAddress);
                    ScrollView scroll = new ScrollView(activity);
                    scroll.AddView(layout);

                    ViewsUtility.ShowCustomResultDialog(activity, "ChangeLinkAddress_Language", scroll, () =>
                    {
                        try
                        {
                            Uri uri = null;
                            if (Uri.TryCreate(txtAddress.Text, UriKind.Absolute, out uri))
                            {
                                foreach (var link in info.Management.MultiLinks)
                                {
                                    link.IsSelected = false;
                                }
                                info.Management.MultiLinks.Add(new Download.Web.Link.MultiLinkAddress() { IsSelected = true, Address = txtAddress.Text });
                                info.SaveThisLink();
                                Toast.MakeText(activity, ViewsUtility.FindTextLanguage(activity, "SaveSuccess_Language"), ToastLength.Long).Show();
                            }
                            else
                            {
                                Toast.MakeText(activity, ViewsUtility.FindTextLanguage(activity, "LinkInvariable_Language"), ToastLength.Long).Show();
                            }
                        }
                        catch (Exception e)
                        {
                            Toast.MakeText(activity, e.Message, ToastLength.Long).Show();
                        }
                    });
                }
                else if (menu.Mode == MenuItemModeEnum.RepairLink)
                {
                    try
                    {
                        //change link here
                        AlertDialog.Builder builder = new AlertDialog.Builder(activity);
                        builder.SetTitle(ViewsUtility.FindTextLanguage(activity, "RepairLink_Language"));
                        LinearLayout layout = new LinearLayout(activity);
                        layout.Orientation = Orientation.Vertical;
                        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

                        layoutParams.SetMargins(5, 5, 5, 5);
                        layout.LayoutParameters = layoutParams;

                        bool fileExist = Agrin.Download.Engine.Repairer.LinkRepairer.FileExist(info);
                        TextView txtState = null;
                        ProgressBar progressStage = null, progressMain = null;
                        if (!fileExist)
                        {
                            TextView txtMSG = new TextView(activity);
                            txtMSG.Text = ViewsUtility.FindTextLanguage(activity, "CompleteFileForRepairNotFound_Language");
                            txtMSG.Text += System.Environment.NewLine + info.PathInfo.BackUpCompleteAddress;
                            txtMSG.VerticalScrollBarEnabled = true;
                            layout.AddView(txtMSG);
                        }
                        else
                        {
                            LayoutInflater inflater = activity.LayoutInflater;
                            View dialoglayout = inflater.Inflate(Resource.Layout.RepairLink, null);

                            txtState = dialoglayout.FindViewById(Resource.RepairLink.txtState) as TextView;
                            txtState.Text = ViewsUtility.FindTextLanguage(activity, "Stop_Language");

                            progressStage = dialoglayout.FindViewById(Resource.RepairLink.stageProgress) as ProgressBar;
                            progressMain = dialoglayout.FindViewById(Resource.RepairLink.mainProgress) as ProgressBar;

                            progressMain.Max = 4;
                            progressMain.Progress = 0;
                            progressStage.Progress = 0;
                            layout.AddView(dialoglayout);
                        }


                        ScrollView scroll = new ScrollView(activity);
                        scroll.AddView(layout);
                        builder.SetView(scroll);
                        if (fileExist)
                            builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "StartRepairLink_Language"), (EventHandler<DialogClickEventArgs>)null);
                        else
                            builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (EventHandler<DialogClickEventArgs>)null);

                        builder.SetNegativeButton(ViewsUtility.FindTextLanguage(activity, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);
                        AlertDialog dialogW = null;
                        builder.SetCancelable(false);
                        dialogW = builder.Create();
                        dialogW.Show();
                        // Get the buttons.
                        var yesBtn = dialogW.GetButton((int)DialogButtonType.Positive);
                        var noBtn = dialogW.GetButton((int)DialogButtonType.Negative);
                        bool isStop = false;
                        System.Threading.Thread thread = new System.Threading.Thread(() =>
                        {
                            Action<string> runOnUI = (msg) =>
                            {
                                activity.RunOnUiThread(() =>
                                {
                                    txtState.Text = msg;
                                });
                            };
                            try
                            {
                                Agrin.Download.Engine.Repairer.LinkRepairer.LinkRepairerProcessAction = (val, max, state) =>
                                {
                                    if (isStop)
                                        return;

                                    if (state == Download.Engine.Repairer.LinkRepairerState.FindConnectionProblems)
                                    {
                                        activity.RunOnUiThread(() =>
                                        {
                                            txtState.Text = ViewsUtility.FindTextLanguage(activity, "FindConnectionProblems_Language");
                                            progressMain.Progress = 1;
                                        });
                                    }
                                    else if (state == Download.Engine.Repairer.LinkRepairerState.FindPositionOfProblems)
                                    {
                                        activity.RunOnUiThread(() =>
                                        {
                                            txtState.Text = ViewsUtility.FindTextLanguage(activity, "FindPositionOfProblems_Language");
                                            progressMain.Progress = 2;
                                        });
                                    }
                                    else if (state == Download.Engine.Repairer.LinkRepairerState.DownloadingProblems)
                                    {
                                        activity.RunOnUiThread(() =>
                                        {
                                            txtState.Text = ViewsUtility.FindTextLanguage(activity, "DownloadingProblems_Language");
                                            progressMain.Progress = 3;
                                        });
                                    }
                                    else if (state == Download.Engine.Repairer.LinkRepairerState.FixingProblems)
                                    {
                                        activity.RunOnUiThread(() =>
                                        {
                                            txtState.Text = ViewsUtility.FindTextLanguage(activity, "FixingProblems_Language");
                                            progressMain.Progress = 4;
                                        });
                                    }

                                    activity.RunOnUiThread(() =>
                                    {
                                        int newMax = 0, newVal = 0;
                                        if (max > int.MaxValue)
                                        {
                                            newMax = int.MaxValue;
                                            long nval = max - int.MaxValue;
                                            if (val - nval > 0)
                                            {
                                                newVal = (int)(val - nval);
                                            }
                                            else
                                                newVal = 0;
                                        }
                                        else
                                        {
                                            newMax = (int)max;
                                            newVal = (int)val;
                                        }
                                        progressStage.Max = newMax;
                                        progressStage.Progress = newVal;
                                    });
                                };
                                activity.RunOnUiThread(() =>
                                {
                                    txtState.Text = ViewsUtility.FindTextLanguage(activity, "Connecting_Language");
                                });
                                var repair = Agrin.Download.Engine.Repairer.LinkRepairer.RepairFile(info.PathInfo.BackUpCompleteAddress, info.PathInfo.FullAddressFileName);
                                if (isStop)
                                    return;
                                if (repair == "OK")
                                {
                                    activity.RunOnUiThread(() =>
                                    {
                                        txtState.Text = ViewsUtility.FindTextLanguage(activity, "SuccessRepairLink_Language");
                                        noBtn.Text = ViewsUtility.FindTextLanguage(activity, "OK_Language");
                                    });
                                }
                                else
                                {
                                    runOnUI(repair);
                                }
                            }
                            catch (Exception e)
                            {
                                if (isStop)
                                    return;
                                runOnUI(e.Message);
                            }
                        });
                        // Set up the buttons
                        yesBtn.Click += (s, args) =>
                        {
                            if (!fileExist)
                            {
                                dialogW.Dismiss();
                                return;
                            }
                            yesBtn.Enabled = false;
                            thread.Start();
                        };
                        noBtn.Click += (s, args) =>
                        {
                            isStop = true;
                            try
                            {
                                thread.Abort();
                            }
                            catch
                            {

                            }
                            dialogW.Dismiss();
                        };
                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, "RepairLink", true);
                        var builder = new AlertDialog.Builder(activity);
                        builder.SetMessage(ViewsUtility.FindTextLanguage(activity, "Error_Language"));
                        builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                        {
                        });
                        builder.Show();
                    }
                }
                else if (menu.Mode == MenuItemModeEnum.CopyLink)
                {
                    if (info.Management.MultiLinks.Count > 0)
                        ClipboardHelper.Text = info.Management.MultiLinks.FirstOrDefault().Address;
                    else
                        ClipboardHelper.Text = info.PathInfo.Address;
                }
                else if (menu.Mode == MenuItemModeEnum.ChangeSavePath)
                {
                    try
                    {
                        var manualDialog = ViewsUtility.ShowControlDialog(activity, null, "BrowseFolder_Language", null);

                        ViewsUtility.ShowFolderBrowseInLayout(activity, manualDialog.Layout, info.PathInfo.SavePath, (path, isSecurityPath) =>
                        {
                            if (info.PathInfo.AppSavePath != path)
                            {
                                if (isSecurityPath)
                                    info.PathInfo.UserSecurityPath = path;
                                else
                                {
                                    info.PathInfo.UserSavePath = path;
                                    info.PathInfo.UserSecurityPath = null;
                                }
                                info.SaveThisLink();
                            }
                            manualDialog.ManualClose();
                        }, manualDialog.ManualClose);
                    }
                    catch (Exception error)
                    {
                        Agrin.Log.AutoLogger.LogError(error, "TTTTTTTT 5 " + info.PathInfo.FullAddressFileName + " TT " + ViewsUtility.GetMimeType(info.PathInfo.FullAddressFileName) + " S! ", true);
                        var builder = new AlertDialog.Builder(activity);
                        builder.SetMessage(ViewsUtility.FindTextLanguage(activity, "Error_Language"));
                        builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                        {
                        });
                        builder.Show();
                    }
                }
                else if (menu.Mode == MenuItemModeEnum.CheckLastError)
                {
                    string text = "";
                    string error = info.Management.LastErrorDescription;
                    if (error == "No Error Found!")
                        text = ViewsUtility.FindTextLanguage(activity, "NoErrorFound_Language");
                    else
                        text = error;
                    ViewsUtility.ShowMessageBoxOnlyOkButton(activity, ViewsUtility.FindTextLanguage(activity, "ShowLastError_Language"), text, null);
                }
                else if (menu.Mode == MenuItemModeEnum.Delete)
                {
                    LinearLayout layout = new LinearLayout(activity);
                    layout.Orientation = Orientation.Vertical;
                    TextView txt = new TextView(activity);
                    //txt.SetMaxWidth(200);
                    ViewsUtility.SetTextViewTextColor(activity, txt, Resource.Color.foreground);
                    txt.Text = ViewsUtility.FindTextLanguage(activity, "Areyousureyouwanttodeleteselectedlinks_Language") + " " + ViewsUtility.FindTextLanguage(activity, "AreyousureyouwanttodeleteselectedlinksafterDelete_Language");
                    txt.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    txt.SetMaxWidth(600);
                    var view = ((View)activity.LayoutInflater.Inflate(Resource.Layout.CheckBoxControl, null));
                    CheckBox chkDeleteFile = (CheckBox)view.FindViewById(Resource.CheckBoxControl.checkbox);
                    chkDeleteFile.Checked = false;
                    ViewsUtility.SetTextLanguage(activity, view, new List<int>() { Resource.CheckBoxControl.checkbox });
                    layout.AddView(txt);
                    layout.AddView(view);

                    ViewsUtility.ShowCustomResultDialog(activity, "Delete_Language", layout, () =>
                    {
                        if (info != null)
                            This.SelectItem(info);
                        This.DisposeSelection();
                        if (chkDeleteFile.Checked)
                        {
                            foreach (var item in This.GetSelectedItems())
                            {
                                if (item.DownloadingProperty.State == ConnectionState.Complete)
                                {
                                    if (System.IO.File.Exists(item.PathInfo.FullAddressFileName))
                                    {
                                        try
                                        {
                                            System.IO.File.Delete(item.PathInfo.FullAddressFileName);
                                        }
                                        catch (Exception ex)
                                        {
                                            InitializeApplication.GoException(ex, "Delete Link And Complete File");
                                        }
                                    }
                                }
                            }
                        }

                        This.DeleteLinks();
                        This.selectedItems.Clear();
                    });
                }
                else if (menu.Mode == MenuItemModeEnum.RenameFileName)
                {
                    LinearLayout layout = new LinearLayout(activity);
                    layout.Orientation = Orientation.Vertical;
                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

                    layoutParams.SetMargins(5, 5, 5, 5);
                    layout.LayoutParameters = layoutParams;

                    EditText txtAddress = new EditText(new ContextThemeWrapper(activity, Resource.Style.editText));
                    txtAddress.Hint = info.PathInfo.FileName;
                    txtAddress.VerticalScrollBarEnabled = true;
                    layout.AddView(txtAddress);
                    ScrollView scroll = new ScrollView(activity);
                    scroll.AddView(layout);

                    ViewsUtility.ShowCustomResultDialog(activity, "ChangeFileName_Language", scroll, () =>
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(txtAddress.Text))
                            {
                                info.PathInfo.UserFileName = txtAddress.Text;
                                info.SaveThisLink();
                                Toast.MakeText(activity, ViewsUtility.FindTextLanguage(activity, "SaveSuccess_Language"), ToastLength.Long).Show();
                            }
                            else
                            {
                                Toast.MakeText(activity, ViewsUtility.FindTextLanguage(activity, "FileNameInvariable_Language"), ToastLength.Long).Show();
                            }
                        }
                        catch (Exception e)
                        {
                            Toast.MakeText(activity, e.Message, ToastLength.Long).Show();
                        }
                    });
                }
                else if (menu.Mode == MenuItemModeEnum.GenerateAutoMixerReport)
                {
                    List<string> files = new List<string>();
                    foreach (Download.Web.Link.LinkWebRequest item in Download.Helper.LinkHelper.SortByPosition(info.Connections))
                    {
                        files.Add(item.SaveFileName);
                    }

                    string report;
                    if (files.Count == 0)
                        report = "محل ذخیره: " + info.PathInfo.SavePath;
                    else
                        report = MixerInfo.ReportAutoMixerMode(info.PathInfo.SavePath, (long)info.DownloadingProperty.Size, files);
                    try
                    {
                        if (!System.IO.File.Exists(info.PathInfo.FullAddressFileName))
                        {
                            var extdirs = activity.GetExternalFilesDirs(null);
                            foreach (var item in extdirs)
                            {
                                var path = System.IO.Path.Combine(item.Path, info.PathInfo.FileName);
                                if (System.IO.File.Exists(path))
                                {
                                    report += System.Environment.NewLine + "موجود در: " + path;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, "GenerateAutoMixerReport");
                    }
                    ViewsUtility.ShowMessageBoxOnlyOkButton(activity, "گزارش لینک", report, null);
                }
            }
            catch (Exception e)
            {
                InitializeApplication.GoException(e, "LinkInfoClickMenuItem");
            }
        }

        public static void OpenFile(LinkInfo linkInfo, Activity activity)
        {
            try
            {
                if (System.IO.File.Exists(linkInfo.PathInfo.FullAddressFileName))
                {
                    Java.IO.File file = new Java.IO.File(linkInfo.PathInfo.FullAddressFileName);
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetDataAndType(Android.Net.Uri.FromFile(file), ViewsUtility.GetMimeType(linkInfo.PathInfo.FullAddressFileName));
                    activity.StartActivity(intent);
                }
                else if (!string.IsNullOrEmpty(linkInfo.PathInfo.SecurityFileSavePath))
                {
                    var builder = new AlertDialog.Builder(activity);
                    builder.SetMessage("باز کردن فایل ها در این مسیر در حال حاضر ممکن نیست");
                    builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                    {
                    });
                    builder.Show();
                }
                else
                {
                    var builder = new AlertDialog.Builder(activity);
                    builder.SetMessage(ViewsUtility.FindTextLanguage(activity, "Filedoesnotexist_Language"));
                    builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                    {
                    });
                    builder.Show();
                }
            }
            catch (Exception error)
            {
                Agrin.Log.AutoLogger.LogError(error, "TTTTTTTT 1 " + linkInfo.PathInfo.FullAddressFileName + " TT " + ViewsUtility.GetMimeType(linkInfo.PathInfo.FullAddressFileName) + " S! ", true);
                var builder = new AlertDialog.Builder(activity);
                builder.SetMessage(ViewsUtility.FindTextLanguage(activity, "CouldNotOpenThisFile_Language"));
                builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                {
                });
                builder.Show();
            }
        }

        public static void OpenFileLocation(LinkInfo linkInfo, Activity activity)
        {
            try
            {
                if (System.IO.Directory.Exists(linkInfo.PathInfo.SavePath))
                {
                    Android.Net.Uri selectedUri = Android.Net.Uri.Parse(linkInfo.PathInfo.SavePath);
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetDataAndType(selectedUri, "resource/folder");
                    activity.StartActivity(intent);
                }
                else
                {
                    var builder = new AlertDialog.Builder(activity);
                    builder.SetMessage(ViewsUtility.FindTextLanguage(activity, "Folderdoesnotexist_Language"));
                    builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                    {
                    });
                    builder.Show();
                }
            }
            catch (Exception error)
            {
                Agrin.Log.AutoLogger.LogError(error, "TTTTTTTT 5 " + linkInfo.PathInfo.FullAddressFileName + " TT " + ViewsUtility.GetMimeType(linkInfo.PathInfo.FullAddressFileName) + " S! ", true);
                var builder = new AlertDialog.Builder(activity);
                builder.SetMessage(ViewsUtility.FindTextLanguage(activity, "CouldNotOpenThisLocation_Language"));
                builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                {
                });
                builder.Show();
            }
        }

        public static void ShareFile(LinkInfo linkInfo, Activity activity)
        {
            try
            {
                if (System.IO.File.Exists(linkInfo.PathInfo.FullAddressFileName))
                {
                    Java.IO.File file = new Java.IO.File(linkInfo.PathInfo.FullAddressFileName);
                    Intent intent = new Intent(Intent.ActionSend);
                    intent.SetType("*/*");//getMimeType(listItemName.PathInfo.FullAddressFileName)
                    intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(file));
                    activity.StartActivity(Intent.CreateChooser(intent, ViewsUtility.FindTextLanguage(activity, "ShareFileUsing_Language")));
                }
                else
                {
                    var builder = new AlertDialog.Builder(activity);
                    builder.SetMessage(ViewsUtility.FindTextLanguage(activity, "Filedoesnotexist_Language"));
                    builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                    {
                    });
                    builder.Show();
                }
            }
            catch (Exception error)
            {
                Agrin.Log.AutoLogger.LogError(error, "TTTTTTTT 1 " + linkInfo.PathInfo.FullAddressFileName + " TT " + ViewsUtility.GetMimeType(linkInfo.PathInfo.FullAddressFileName) + " S! ", true);
                var builder = new AlertDialog.Builder(activity);
                builder.SetMessage(ViewsUtility.FindTextLanguage(activity, "ErrorShareFile_Language"));
                builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (s, ee) =>
                {
                });
                builder.Show();
            }
        }

        public void Dispose()
        {
            try
            {
                lock (ChangeItemsLock)
                {
                    _isDispose = true;
                    This = null;
                    Items.CollectionChanged -= Items_CollectionChanged;
                    foreach (var item in getViewByLinkInfo)
                    {
                        item.Value.Dispose();
                        BindingHelper.DisposeObject(item.Key);
                    }
                }

                getViewByLinkInfo.Clear();

                selectedItems.Clear();
                selectedItems = null;

                _btnPlay.Click -= _btnPlay_Click;
                _btnPause.Click -= _btnPause_Click;
                _btnDelete.Click -= _btnDelete_Click;
                _btnShowMenu.Click -= _btnShowMenu_Click;
                _btnPlayQueue.Click -= _btnPlayQueue_Click;
                _btnDeleteQueue.Click -= _btnDeleteQueue_Click;
                _btnPauseQueue.Click -= _btnPauseQueue_Click;
                if (_mainLayoutListView.ChildCount > 0)
                    _mainLayoutListView.RemoveAllViews();
                lastChnages = null;
                CurrentActivity = null;
                _mainLayoutListView.Dispose();
                _mainLayoutListView = null;
                _actionToolBoxLayout.Dispose();
                _actionToolBoxLayout = null;

                _btnPlay.Dispose();
                _btnPause.Dispose();
                _btnDelete.Dispose();
                _btnMenu.Dispose();
                _btnPlayQueue.Dispose();
                _btnPauseQueue.Dispose();
                _btnDeleteQueue.Dispose();
                _btnShowMenu.Dispose();
                _mainLayout.Dispose();
                _CustomListView.Dispose();
                _linkInfoToolBox.Dispose();

                _findLinksToolBox.Dispose();
                _txtTopToolbarHeader.Dispose();

                _btnPlay = _btnPause = _btnDelete = _btnMenu = _btnPlayQueue = _btnPauseQueue = _btnDeleteQueue = _btnShowMenu = null;
                _mainLayout = null;
                _CustomListView = null;
                _linkInfoToolBox = null;

                GC.Collect();
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "LinksViewModel Dispose");
            }

        }
    }
}