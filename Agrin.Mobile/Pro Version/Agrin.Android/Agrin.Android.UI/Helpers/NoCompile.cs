//public class LinksViewModel : BaseAdapter<LinkInfo>
//{
//    LinksBaseViewModel vm = new LinksBaseViewModel();
//    #region Interace LinksBaseViewModel

//    public int AddTimeHours
//    {
//        get
//        {
//            return vm.AddTimeHours;
//        }
//        set
//        {
//            vm.AddTimeHours = value;
//        }
//    }
//    public int AddTimeMinutes
//    {
//        get
//        {
//            return vm.AddTimeMinutes;
//        }
//        set
//        {
//            vm.AddTimeMinutes = value;
//        }
//    }
//    public FastCollection<GroupInfo> GrouInfoes
//    {
//        get
//        {
//            return vm.GrouInfoes;
//        }
//    }
//    public bool IsStopForMinutes
//    {
//        get
//        {
//            return vm.IsStopForMinutes;
//        }
//        set
//        {
//            vm.IsStopForMinutes = value;
//        }
//    }

//    public FastCollection<LinkInfo> Items
//    {
//        get
//        {
//            return vm.Items;
//        }
//    }

//    public string SeachAddress
//    {
//        get
//        {
//            return vm.SeachAddress;
//        }
//        set
//        {
//            vm.SeachAddress = value;
//        }
//    }
//    public LinkInfo SelectedItem
//    {
//        get
//        {
//            return vm.SelectedItem;
//        }
//        set
//        {
//            vm.SelectedItem = value;
//        }
//    }
//    public bool StartNow
//    {
//        get
//        {
//            return vm.StartNow;
//        }
//        set
//        {
//            vm.StartNow = value;
//        }
//    }
//    public int StopTimeForMinutes
//    {
//        get
//        {
//            return vm.StopTimeForMinutes;
//        }
//        set
//        {
//            vm.StopTimeForMinutes = value;
//        }
//    }
//    public int StopTimeHours
//    {
//        get
//        {
//            return vm.StopTimeHours;
//        }
//        set
//        {
//            vm.StopTimeHours = value;
//        }
//    }
//    public int StopTimeMinutes
//    {
//        get
//        {
//            return vm.StopTimeMinutes;
//        }
//        set
//        {
//            vm.StopTimeMinutes = value;
//        }
//    }

//    public void AddGroupInfo()
//    {
//        vm.AddGroupInfo();
//    }
//    public void AddStopTimeTask()
//    {
//        vm.AddStopTimeTask();
//    }
//    public void AddTimeTask()
//    {
//        vm.AddTimeTask();
//    }
//    public bool CanChangeUserSavePath(LinkInfo obj)
//    {
//        return vm.CanChangeUserSavePath(obj);
//    }
//    public bool CanDeleteGroupInfo(object obj)
//    {
//        return vm.CanDeleteGroupInfo(obj);
//    }
//    public bool CanDeleteLinks()
//    {
//        return vm.CanDeleteLinks();
//    }
//    public bool CanDeleteSelectedLinksTimes()
//    {
//        return vm.CanDeleteSelectedLinksTimes();
//    }
//    public bool CanOpenFile()
//    {
//        return vm.CanOpenFile();
//    }
//    public bool CanPlayLinkInfo()
//    {
//        return vm.CanPlayLinkInfo();
//    }
//    public bool CanPlayLinks()
//    {
//        return vm.CanPlayLinks();
//    }
//    public bool CanPlaySelectionTask()
//    {
//        return vm.CanPlaySelectionTask();
//    }
//    public bool CanReconnectSelectedLinks()
//    {
//        return vm.CanReconnectSelectedLinks();
//    }
//    public bool CanStopLinkInfo()
//    {
//        return vm.CanStopLinkInfo();
//    }
//    public bool CanStopLinks()
//    {
//        return vm.CanStopLinks();
//    }
//    public bool CanStopSelectionTask()
//    {
//        return vm.CanStopSelectionTask();
//    }
//    public virtual void ChangeUserSavePath(LinkInfo obj, string fileName)
//    {
//        vm.ChangeUserSavePath(obj, fileName);
//    }
//    public virtual string CopyLinkLocation()
//    {
//        return vm.CopyLinkLocation();
//    }
//    public void DeleteGroupInfo(object obj)
//    {
//        vm.DeleteGroupInfo(obj);
//    }
//    public virtual void DeleteLinks()
//    {
//        vm.DeleteLinks();
//    }
//    public void DeleteSelectedLinksTimes()
//    {
//        vm.DeleteSelectedLinksTimes();
//    }
//    public void LinkInfoSetting(LinkInfo item)
//    {
//        vm.LinkInfoSetting(item);
//    }
//    public void MoveDownFromTask()
//    {
//        vm.MoveDownFromTask();
//    }
//    public void MoveUpFromTask()
//    {
//        vm.MoveUpFromTask();
//    }
//    public virtual void OpenFile()
//    {
//        vm.OpenFile();
//    }
//    public virtual void OpenFileLocation()
//    {
//        vm.OpenFileLocation();
//    }
//    public void PlayLinkInfo()
//    {
//        vm.PlayLinkInfo();
//    }
//    public void PlayLinks()
//    {
//        vm.PlayLinks();
//    }
//    public void PlaySelectionTask()
//    {
//        vm.PlaySelectionTask();
//    }
//    public void ReconnectSelectedLinks()
//    {
//        vm.ReconnectSelectedLinks();
//    }
//    public void RemoveAllStartTimeTask(LinkInfo linkInfo)
//    {
//        vm.RemoveAllStartTimeTask(linkInfo);
//    }
//    public void RemoveAllStopTimeTask(LinkInfo linkInfo)
//    {
//        vm.RemoveAllStopTimeTask(linkInfo);
//    }
//    public void ReportLink(string savePath)
//    {
//        vm.ReportLink(savePath);
//    }
//    public void Search()
//    {
//        vm.Search();
//    }
//    public void SendToGroupInfo(GroupInfo obj, List<LinkInfo> links)
//    {
//        vm.SendToGroupInfo(obj, links);
//    }
//    public virtual void SettingLinks()
//    {
//        vm.SettingLinks();
//    }
//    public void StopLinkInfo()
//    {
//        vm.StopLinkInfo();
//    }
//    public void StopLinks()
//    {
//        vm.StopLinks();
//    }
//    public void StopSelectionTask()
//    {
//        vm.StopSelectionTask();
//    }

//    #endregion

//    public Activity CurrentActivity { get; set; }
//    int _templateResourceId;
//    ListView mainListView = null;
//    public LinksViewModel(Activity context, int templateResourceId, LinearLayout mainLayout)
//        : base()
//    {
//        CurrentActivity = context;
//        _templateResourceId = templateResourceId;
//        var view = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.LinksListData, mainLayout, false);
//        mainListView = view.FindViewById<ListView>(Resource.LinksListData.mainListView);
//        mainListView.Adapter = this;
//        mainLayout.AddView(view);
//        mainListView.ScrollStateChanged += mainListView_ScrollStateChanged;
//        //var scroll = view.FindViewById<ScrollView>(Resource.LinksListData.mainScroll);
//        //LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,LinearLayout.LayoutParams.MatchParent);
//        //scroll.LayoutParameters = lp;
//    }


//    public override LinkInfo this[int position]
//    {
//        get { return Items[position]; }
//    }

//    public override int Count
//    {
//        get { return Items.Count; }
//    }

//    public override long GetItemId(int position)
//    {
//        return Items[position].PathInfo.Id;
//    }


//    public void SetProgressStyleByState(ProgressBar prog, View linearImage, LinkInfo linkInfo)
//    {
//        if (linkInfo.IsComplete)
//        {
//            ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Complete);
//            ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressCompleteBackground);
//        }
//        else if (linkInfo.IsDownloading)
//        {
//            ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Downloading);
//            ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressDownloadingBackground);
//        }
//        else if (linkInfo.IsError)
//        {
//            ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Error);
//            ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressErrorBackground);
//        }
//        else if (!linkInfo.IsSizeValue)
//        {
//            ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_NotStarted);
//            ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressNotStartedBackground);
//        }
//        else if (linkInfo.IsWaitingForPlayQueue)
//        {
//            ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Queue);
//            ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressQueueBackground);
//        }
//        else if (linkInfo.IsManualStop)
//        {
//            ViewsUtility.SetProgressBarProgressDrawable(CurrentActivity, prog, Resource.Drawable.ProgressBarShape_Stoped);
//            ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.ProgressStopedBackground);
//        }
//    }

//    public void SetTextViewTextColorByState(TextView txt, LinkInfo linkInfo)
//    {
//        if (linkInfo.IsError || (linkInfo.IsManualStop && !linkInfo.IsWaitingForPlayQueue && linkInfo.IsSizeValue && !linkInfo.IsComplete))
//        {
//            ViewsUtility.SetTextViewTextColor(CurrentActivity, txt, Resource.Color.white);
//        }
//        else
//        {
//            ViewsUtility.SetTextViewTextColor(CurrentActivity, txt, Resource.Color.LinkAddressForeground);
//        }
//    }

//    List<LinkInfo> selectedItems = new List<LinkInfo>();

//    public void SelectItem(LinkInfo item)
//    {
//        if (!selectedItems.Contains(item))
//            selectedItems.Add(item);
//    }

//    public void DeSelectItem(LinkInfo item)
//    {
//        selectedItems.Remove(item);
//    }


//    bool _isDispose = false;
//    public override View GetView(int position, View convertView, ViewGroup parent)
//    {
//        if (_isDispose)
//            return null;

//        var view = convertView;
//        LinkInfo item = this[position];
//        bool isSelected = selectedItems.Contains(item);
//        if (view == null)
//            view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, parent, false);

//        try
//        {
//            var layoutMain = view.FindViewById<LinearLayout>(Resource.LinkInfoListItem.layoutMain);
//            RelativeLayout linearImage = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.linearImage);

//            //if (isSelected && height > 0)
//            //{
//            //    ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(LinearLayout.LayoutParams.MatchParent, height);
//            //    view.LayoutParameters = lp;
//            //}
//            LinkInfoPosition pos = new LinkInfoPosition() { Position = position, LinkInfo = item };
//            layoutMain.Tag = pos.Cast();
//            layoutMain.Clickable = true;
//            layoutMain.Click -= layoutMain_Click;
//            layoutMain.Click += layoutMain_Click;
//            InitializeNormalListViewItem(item, view, isSelected ? null : linearImage);

//            if (!isSelected)
//            {
//                DeSelectView(layoutMain, pos);
//            }
//            else
//            {
//                SelectView(layoutMain, pos);
//            }
//        }
//        catch (Exception e)
//        {

//        }
//        return view;
//    }

//    void layoutMain_Click(object sender, EventArgs e)
//    {
//        var layoutMain = sender as LinearLayout;
//        LinkInfoPosition pos = layoutMain.Tag.Cast<LinkInfoPosition>();
//        //var view = mainListView.GetChildAt(pos.Position - mainListView.FirstVisiblePosition);
//        var view = getViewByPosition(pos.Position);
//        if (view == null)
//        {
//            return;
//        }

//        var item = pos.LinkInfo;
//        if (selectedItems.Contains(item))
//        {
//            DeSelectItem(item);
//            DeSelectView(view, pos);
//        }
//        else
//        {
//            SelectItem(item);
//            SelectView(view, pos);
//        }
//    }

//    public void SelectView(View view, LinkInfoPosition item)
//    {
//        RelativeLayout linearImage = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.linearImage);
//        ViewsUtility.SetBackground(CurrentActivity, view, Resource.Drawable.SelectedListItemBackground);

//        view.FindViewById<ProgressBar>(Resource.LinkInfoListItem.prgDownload).Visibility = ViewStates.Invisible;
//        ViewsUtility.SetBackground(CurrentActivity, linearImage, Resource.Color.SelectedBackground);
//        RelativeLayout layoutQueue = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.layoutQueue);
//        layoutQueue.Visibility = ViewStates.Visible;
//        ImageView imgIcon = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgIcon);
//        imgIcon.Visibility = ViewStates.Invisible;
//        ImageView imgcheck = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgCheck);
//        imgcheck.Visibility = ViewStates.Visible;

//        if (!BindingHelper.IsBinded(item.LinkInfo, new List<string>() { "SelectedIndexNumber" }))
//            BindingHelper.BindAction(item.LinkInfo, item.LinkInfo, new List<string>() { "SelectedIndexNumber" }, (property) =>
//            {
//                RefreshOnlyOneViewText(item.Position, item.LinkInfo.SelectedIndexNumber);
//            });
//        //if (noSelect)
//        //{
//        //    TextView txtQueueNumber = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtQueueNumber);
//        //    txtQueueNumber.Text = item.LinkInfo.SelectedIndexNumber;
//        //}
//        //else
//        drawSelectionNumbers();
//    }



//    public void DeSelectView(View view, LinkInfoPosition item)
//    {
//        RelativeLayout linearImage = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.linearImage);
//        ViewsUtility.SetBackground(CurrentActivity, view, Resource.Color.background);
//        view.FindViewById<ProgressBar>(Resource.LinkInfoListItem.prgDownload).Visibility = ViewStates.Visible;
//        SetProgressStyleByState(null, linearImage, item.LinkInfo);
//        RelativeLayout layoutQueue = view.FindViewById<RelativeLayout>(Resource.LinkInfoListItem.layoutQueue);
//        layoutQueue.Visibility = ViewStates.Invisible;
//        ImageView imgIcon = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgIcon);
//        imgIcon.Visibility = ViewStates.Visible;
//        ImageView imgcheck = view.FindViewById<ImageView>(Resource.LinkInfoListItem.imgCheck);
//        imgcheck.Visibility = ViewStates.Invisible;

//        if (!BindingHelper.IsBinded(item.LinkInfo, new List<string>() { "SelectedIndexNumber" }))
//            BindingHelper.BindAction(item.LinkInfo, item.LinkInfo, new List<string>() { "SelectedIndexNumber" }, (property) =>
//            {
//                RefreshOnlyOneViewText(item.Position, item.LinkInfo.SelectedIndexNumber);
//            });

//        drawSelectionNumbers();
//    }

//    public void RefreshOnlyOneViewText(int position, string text)
//    {
//        //var view = mainListView.GetChildAt(position - mainListView.FirstVisiblePosition);
//        var view = getViewByPosition(position);
//        if (view == null)
//        {
//            return;
//        }
//        TextView txtQueueNumber = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtQueueNumber);
//        txtQueueNumber.Text = text;
//    }

//    public View getViewByPosition(int pos)
//    {
//        try
//        {
//            View retView = null;
//            int firstListItemPosition = mainListView.FirstVisiblePosition;
//            int lastListItemPosition = firstListItemPosition + mainListView.ChildCount - 1;

//            if (pos < firstListItemPosition || pos > lastListItemPosition)
//            {
//                //noSelect = true;
//                //retView = GetView(pos, null, mainListView);
//                //noSelect = false;
//                //NotifyDataSetChanged();
//            }
//            else
//            {
//                int childIndex = pos - firstListItemPosition;
//                retView = mainListView.GetChildAt(childIndex);
//            }
//            if (retView == null)
//            {

//            }
//            return retView;
//        }
//        catch (Java.Lang.Exception c)
//        {
//            return null;
//        }
//        catch (Exception e)
//        {
//            return null;
//        }
//    }

//    void drawSelectionNumbers()
//    {
//        int i = 1;
//        foreach (var item in selectedItems)
//        {
//            item.SelectedIndexNumber = i.ToString();
//            i++;
//        }
//    }

//    void mainListView_ScrollStateChanged(object sender, AbsListView.ScrollStateChangedEventArgs e)
//    {
//        if (e.ScrollState == ScrollState.Idle)
//        {
//            NotifyDataSetChanged();
//        }
//    }


//    //object lockObj = new object();
//    //bool _isDispose = false;
//    //public override View GetView(int position, View convertView, ViewGroup parent)
//    //{
//    //    if (_isDispose)
//    //        return null;

//    //    var view = convertView;
//    //    LinkInfo item = this[position];
//    //    bool isSelected = selectedItems.Contains(item);
//    //    if (isSelected)
//    //        view = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.LinkInfoListItemSelected, parent, false);
//    //    else
//    //        view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, parent, false);

//    //    LinearLayout layoutMain = null;
//    //    int layoutID = 0;
//    //    if (isSelected)
//    //    {
//    //        layoutID = Resource.LinkInfoListItemSelected.layoutMain;
//    //    }
//    //    else
//    //        layoutID = Resource.LinkInfoListItem.layoutMain;

//    //    layoutMain = view.FindViewById<LinearLayout>(layoutID);
//    //    //if (isSelected && height > 0)
//    //    //{
//    //    //    ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(LinearLayout.LayoutParams.MatchParent, height);
//    //    //    view.LayoutParameters = lp;
//    //    //}
//    //    AndroidViewHelper helper = new AndroidViewHelper() { LayoutID = layoutID, LinkInfo = item };
//    //    layoutMain.Tag = helper.Cast();
//    //    layoutMain.Clickable = true;
//    //    layoutMain.Click += layoutMain_Click;

//    //    if (isSelected)
//    //        return layoutMain;
//    //    InitializeNormalListViewItem(item, view);

//    //    return view;
//    //}

//    void InitializeNormalListViewItem(LinkInfo item, View view, RelativeLayout linearImage)
//    {
//        try
//        {
//            var txtFileName = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtFileName);
//            var txtSize = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtSize);
//            var txtDownloadedSize = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtDownloadedSize);
//            var txtState = view.FindViewById<TextView>(Resource.LinkInfoListItem.txtState);
//            var prgDownload = view.FindViewById<ProgressBar>(Resource.LinkInfoListItem.prgDownload);

//            //if (selectedItems.Contains(item))
//        //    SelectItem(item, layoutSelected);
//        //else
//        //    DeSelectItem(item, layoutSelected); 
//        //ViewsUtility.SetFont(CurrentActivity, new List<View>() { txtFileName, txtSize, txtDownloadedSize, txtState });

//        NewSetBinding:
//            BindingHelper.BindOneWay(item, txtFileName, "Text", item.PathInfo, "FileName");

//            Action<string> changedAction = (property) =>
//            {


//                //AsyncActions.Action(() =>
//                //{
//                //    lock (lockObj)
//                //    {
//                if (_isDispose)
//                    return;

//                MainActivity.This.RunOnUiThread(() =>
//                {
//                    try
//                    {
//                        string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.Size);
//                        txtSize.Text = size[0] + " " + ViewsUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");

//                        var downSize = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.DownloadedSize);
//                        txtDownloadedSize.Text = downSize[0] + " " + ViewsUtility.FindTextLanguage(CurrentActivity, downSize[1] + "_Language");

//                        txtState.Text = ViewsUtility.FindTextLanguage(CurrentActivity, item.DownloadingProperty.State.ToString() + "_Language");



//                        if (property == "State" || property == null)
//                        {
//                            SetProgressStyleByState(prgDownload, linearImage, item);
//                            SetTextViewTextColorByState(txtFileName, item);
//                            var old = prgDownload.Progress;
//                            if (old == 0)
//                            {
//                                prgDownload.Progress++;
//                                prgDownload.Progress--;
//                            }
//                            else
//                            {
//                                prgDownload.Progress = 0;
//                                prgDownload.Progress = old;
//                            }
//                        }
//                        //prgDownload.Max = 200;
//                        //prgDownload.Progress = 20;
//                        //prgDownload.Progress = 0;
//                        if (item.DownloadingProperty.Size >= 0.0)
//                            prgDownload.Progress = (int)(100 / (item.DownloadingProperty.Size / item.DownloadingProperty.DownloadedSize));
//                        else
//                            prgDownload.Progress = 100;

//                        //prgDownload.Max = 100;
//                        //prgDownload.Progress = prgDownload.Progress;
//                        //string fileName = item.PathInfo.FileName;
//                        //textBuilder.Clear();
//                        //textBuilder.Append(ViewUtility.FindTextLanguage(CurrentActivity, item.DownloadingProperty.State.ToString() + "_Language"));
//                        //textBuilder.Append(" | " + ViewUtility.FindTextLanguage(CurrentActivity, "Size_Language") + " ");
//                        //string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.Size);
//                        //textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language"));
//                        //textBuilder.Append(" | " + ViewUtility.FindTextLanguage(CurrentActivity, "Downloaded_Language") + " ");
//                        //size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.DownloadedSize);
//                        //textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language"));
//                        //chkFileName.Text = fileName;
//                        //txtData.Text = textBuilder.ToString();
//                        //if (item.DownloadingProperty.Size >= 0.0)
//                        //    mainProgress.Progress = (int)(100 / (item.DownloadingProperty.Size / item.DownloadingProperty.DownloadedSize));
//                        //else
//                        //    mainProgress.Progress = 0;
//                        //if (property == "State")
//                        //    SetLayoutColor(item, layout, selectLayout);
//                    }
//                    catch (Exception ex)
//                    {

//                    }
//                });
//                //    }
//                //}, null, true);
//            };

//            var listProps = new List<string>() { "DownloadedSize", "State", "Size" };

//            if (BindingHelper.IsBinded(item.DownloadingProperty, listProps))
//            {
//                BindingHelper.DisposeObject(item);
//                goto NewSetBinding;
//            }
//            BindingHelper.BindAction(item, item.DownloadingProperty, listProps, changedAction);

//            changedAction(null);
//            txtFileName.Text = item.PathInfo.FileName;
//        }
//        catch (Exception e)
//        {

//        }
//    }

//    //    int height = 0;
//    //    void layoutMain_Click(object sender, EventArgs e)
//    //    {
//    //        var layoutMain = sender as LinearLayout;
//    //        AndroidViewHelper item = layoutMain.Tag.Cast<AndroidViewHelper>();
//    //        if (height <= 0)
//    //            height = layoutMain.Height - 20;
//    //        if (selectedItems.Contains(item.LinkInfo))
//    //        {
//    //            DeSelectItem(item.LinkInfo);

//    //            if (layoutMain.ChildCount == 1)
//    //            {
//    //                layoutMain.GetChildAt(0).Visibility = ViewStates.Gone;
//    //                var view = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.LinkInfoListItem, null, false);
//    //                layoutMain.AddView(view, new ViewGroup.LayoutParams(LinearLayout.LayoutParams.MatchParent, height));
//    //                InitializeNormalListViewItem(item.LinkInfo, view);
//    //            }
//    //            else
//    //            {
//    //                if (item.LayoutID == Resource.LinkInfoListItem.layoutMain)
//    //                {
//    //                    layoutMain.GetChildAt(1).Visibility = ViewStates.Gone;
//    //                    layoutMain.GetChildAt(0).Visibility = ViewStates.Visible;
//    //                }
//    //                else
//    //                {
//    //                    layoutMain.GetChildAt(0).Visibility = ViewStates.Gone;
//    //                    layoutMain.GetChildAt(1).Visibility = ViewStates.Visible;
//    //                }
//    //            }
//    //        }
//    //        else
//    //        {
//    //            SelectItem(item.LinkInfo);
//    //            if (layoutMain.ChildCount == 1)
//    //            {
//    //                layoutMain.GetChildAt(0).Visibility = ViewStates.Gone;
//    //                var view = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.LinkInfoListItemSelected, null, false);
//    //                layoutMain.AddView(view, new ViewGroup.LayoutParams(LinearLayout.LayoutParams.MatchParent, height));
//    //            }
//    //            else
//    //            {
//    //                if (item.LayoutID == Resource.LinkInfoListItem.layoutMain)
//    //                {
//    //                    layoutMain.GetChildAt(0).Visibility = ViewStates.Gone;
//    //                    layoutMain.GetChildAt(1).Visibility = ViewStates.Visible;
//    //                }
//    //                else
//    //                {
//    //                    layoutMain.GetChildAt(1).Visibility = ViewStates.Gone;
//    //                    layoutMain.GetChildAt(0).Visibility = ViewStates.Visible;
//    //                }
//    //            }
//    //        }
//    //    }
//    //}

//    //public class AndroidViewHelper
//    //{
//    //    public int LayoutID { get; set; }
//    //    public LinkInfo LinkInfo { get; set; }
//    //}
//}
//public class LinkInfoPosition
//{
//    public int Position { get; set; }
//    public LinkInfo LinkInfo { get; set; }
//}
