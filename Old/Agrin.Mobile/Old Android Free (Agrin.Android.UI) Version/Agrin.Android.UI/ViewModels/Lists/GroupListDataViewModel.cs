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
using Agrin.Download.Web;
using Agrin.Download.Manager;

namespace Agrin.MonoAndroid.UI
{
    public class GroupListDataViewModel : BaseAdapter<GroupInfo>, IBaseViewModel
    {
        public GroupListDataViewModel(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }
        public Activity CurrentActivity { get; set; }

        //bool _isDispose = false;
        object lockObj = new object();

        public void DisposeAll()
        {
            lock (lockObj)
            {
                //_isDispose = true;
                this.Dispose();
                foreach (var item in ApplicationGroupManager.Current.GroupInfoes.ToList())
                {
                    //BindingHelper.RemoveActionPropertyChanged (item.SavePath);
                    //BindingHelper.RemoveActionPropertyChanged (item.Name);
                }
            }
        }

        private List<GroupInfo> _items;
        private int _templateResourceId;
        ListView _listView;
        LinearLayout mainLayout = null;

        public GroupListDataViewModel(Activity context, int templateResourceId, List<GroupInfo> items)
            : base()
        {
            CurrentActivity = context;
            _templateResourceId = templateResourceId;
            _items = items;
            mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.GroupListData.mainLayout);
            _listView = CurrentActivity.FindViewById<ListView>(Resource.GroupListData.mainListView);
            _listView.Adapter = this;
            _listView.ItemClick += OnListItemClick;
            Button btnAddGroup = CurrentActivity.FindViewById<Button>(Resource.GroupListData.btnAddGroup);
            btnAddGroup.Click += btnAddGroupClick;
            Button btnDelete = CurrentActivity.FindViewById<Button>(Resource.GroupListData.btnDelete);
            btnDelete.Click += btnDeleteClick;
        }

        public override GroupInfo this[int position]
        {
            get { return ApplicationGroupManager.Current.GroupInfoes[position]; }
        }

        public override int Count
        {
            get { return ApplicationGroupManager.Current.GroupInfoes.Count; }
        }

        public override long GetItemId(int position)
        {
            return ApplicationGroupManager.Current.GroupInfoes[position].Id;
        }

        void btnDeleteClick(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this.CurrentActivity);
            builder.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "Areyousureyouwanttodeleteselectedgroups_Language"));
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "Yes_Language"), (s, ee) =>
            {
                ApplicationGroupManager.Current.DeleteRangeGroupInfo(selectedItems);
                selectedItems.Clear();
                this.NotifyDataSetChanged();
            });
            builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "No_Language"), (s, ee) => { }).Create();
            builder.Show();

        }

        void btnAddGroupClick(object sender, EventArgs e)
        {
            ActivitesManager.AddGroupActive(CurrentActivity);
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var selItem = this[e.Position];
            var layout = e.View.FindViewById<LinearLayout>(Resource.GroupListItem.mainLayout);
            if (selectedItems.Contains(selItem))
            {
                selectedItems.Remove(selItem);
                layout.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
            else
            {
                selectedItems.Add(selItem);
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff5c9ad5"));
            }
        }

        void chkFileNameCheckedChange(object sender, EventArgs e)
        {
            CheckBox chkFileName = sender as CheckBox;
            HolderHelper<GroupInfo> item = chkFileName.Tag as HolderHelper<GroupInfo>;
            if (chkFileName.Checked)
                selectedItems.Add(item.Value);
            else
                selectedItems.Remove(item.Value);
        }

        int totalWidth = 0, totalHeight = 0;
        List<GroupInfo> selectedItems = new List<GroupInfo>();

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            GroupInfo item = this[position];
            if (view == null || !(view is LinearLayout))
                view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, parent, false);

            TextView txtGroupName = view.FindViewById<TextView>(Resource.GroupListItem.txtGroupName);
            TextView txtData = view.FindViewById<TextView>(Resource.GroupListItem.txtSaveAddress);
            var layout = view.FindViewById<LinearLayout>(Resource.GroupListItem.mainLayout);

            if (!selectedItems.Contains(item))
            {
                layout.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
            else
            {
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff007faa"));
            }
            //chkFileName.Tag = new HolderHelper<GroupInfo> (item);
            //chkFileName.CheckedChange -= chkFileNameCheckedChange;
            //chkFileName.CheckedChange += chkFileNameCheckedChange;

            txtGroupName.Text = item.Name;
            txtData.Text = item.SavePath;
            if (view.MeasuredWidth >= totalWidth)
            {
                view.Measure(0, 0);
                totalWidth = view.MeasuredWidth;
            }
            totalHeight = (view.MeasuredHeight + 1) * Count;
            if (totalWidth < mainLayout.MeasuredWidth)
                totalWidth = mainLayout.MeasuredWidth;
            //			if (totalWidth != 0 && mainLayout.MeasuredWidth > 0 && totalWidth > mainLayout.MeasuredWidth) {
            //				_listView.LayoutParameters.Width = totalWidth;
            //				txtData.LayoutParameters.Width = _listView.LayoutParameters.Width;
            //			}
            //			else if (mainLayout.MeasuredWidth > 0 && mainLayout.MeasuredWidth < _listView.LayoutParameters.Width)
            //				txtData.LayoutParameters.Width = _listView.LayoutParameters.Width;
            if (totalHeight > _listView.LayoutParameters.Height)
                _listView.LayoutParameters.Height = totalHeight;

            if (totalWidth != 0)
            {
                //_listView.LayoutParameters.Width = totalWidth;
                //txtData.LayoutParameters.Width = totalWidth;
                txtData.SetMinWidth(totalWidth);
            }

            _listView.SetMinimumHeight(totalHeight);
            //_listView.SetMinimumWidth (mainLayout.MeasuredWidth);
            //			if (mainLayout.MeasuredWidth > 0)
            //				layout.SetMinimumWidth (mainLayout.MeasuredWidth);
            _listView.RefreshDrawableState();
            return view;
        }
    }
}

