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
using Agrin.Download.Web.Link;
using Agrin.Download.Data.Settings;

namespace Agrin.MonoAndroid.UI.ViewModels.Lists
{
    public class UserAuthorizationListViewModel : BaseAdapter<NetworkCredentialInfo>, IBaseViewModel
    {
        public UserAuthorizationListViewModel(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }
        public Activity CurrentActivity { get; set; }

        //bool _isDispose = false;
        //object lockObj = new object();

        //public void DisposeAll()
        //{
        //    lock (lockObj)
        //    {
        //        //_isDispose = true;
        //        this.Dispose();
        //        foreach (var item in ApplicationGroupManager.Current.GroupInfoes.ToList())
        //        {
        //            //BindingHelper.RemoveActionPropertyChanged (item.SavePath);
        //            //BindingHelper.RemoveActionPropertyChanged (item.Name);
        //        }
        //    }
        //}

        private List<NetworkCredentialInfo> _items;
        private int _templateResourceId;
        ListView _listView;
        LinearLayout mainLayout = null;

        public UserAuthorizationListViewModel(Activity context, int templateResourceId, List<NetworkCredentialInfo> items)
            : base()
        {
            CurrentActivity = context;
            _templateResourceId = templateResourceId;
            _items = items;
            mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.UserAuthorizationList.mainLayout);
            _listView = CurrentActivity.FindViewById<ListView>(Resource.UserAuthorizationList.mainListView);
            _listView.Adapter = this;
            _listView.ItemClick += OnListItemClick;
            Button btnAddUserAuthorization = CurrentActivity.FindViewById<Button>(Resource.UserAuthorizationList.btnAddUserAuthorization);
            btnAddUserAuthorization.Click += btnAddUserAuthorizationClick;
            Button btnDelete = CurrentActivity.FindViewById<Button>(Resource.UserAuthorizationList.btnDelete);
            btnDelete.Click += btnDeleteClick;
        }

        public override NetworkCredentialInfo this[int position]
        {
            get { return ApplicationSetting.Current.UserAccountsSetting.Items[position]; }
        }

        public override int Count
        {
            get { return ApplicationSetting.Current.UserAccountsSetting.Items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position + 1;
        }

        void btnDeleteClick(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this.CurrentActivity);
            builder.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "AreyousureyouwanttodeleteselectedItems_Language"));
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "Yes_Language"), (s, ee) =>
            {
                ApplicationSetting.Current.UserAccountsSetting.Items.RemoveAll(x => selectedItems.Contains(x));
                selectedItems.Clear();
                this.NotifyDataSetChanged();
            });
            builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "No_Language"), (s, ee) => { }).Create();
            builder.Show();

        }

        void btnAddUserAuthorizationClick(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this.CurrentActivity);
            builder.SetTitle(ViewUtility.FindTextLanguage(CurrentActivity, "AddUserAuthorization_Language"));
            LinearLayout layout = new LinearLayout(CurrentActivity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            EditText txtServerAddress = new EditText(this.CurrentActivity);
            txtServerAddress.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "ServerAddressExample_Language");

            EditText txtUserName = new EditText(this.CurrentActivity);
            txtUserName.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "UserName_Language");

            EditText txtPassword = new EditText(this.CurrentActivity);
            txtPassword.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "Password_Language");

            layout.AddView(txtServerAddress);
            layout.AddView(txtUserName);
            layout.AddView(txtPassword);
            builder.SetView(layout);
            // Set up the buttons
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "OK_Language"), (dialog, which) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtServerAddress.Text) || string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        Toast.MakeText(this.CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "EmptyValidation_Language"), ToastLength.Short).Show();
                        return;
                    }
                    var user = new NetworkCredentialInfo() { IsUsed = true };
                    user.ServerAddress = txtServerAddress.Text;
                    user.UserName = txtUserName.Text;
                    user.Password = txtPassword.Text;
                    ApplicationSetting.Current.UserAccountsSetting.Items.Add(user);
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    this.NotifyDataSetChanged();
                }
                catch (Exception ex)
                {
                    Agrin.Log.AutoLogger.LogError(ex, "NetworkCredentialInfo Creation", true);
                }
                //m_Text = input.Text;
                builder.Dispose();
            });
            builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "Cancel_Language"), (dialog, which) =>
            {
                builder.Dispose();
            });

            builder.Show();
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

        int totalWidth = 0, totalHeight = 0;
        List<NetworkCredentialInfo> selectedItems = new List<NetworkCredentialInfo>();

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            NetworkCredentialInfo item = this[position];
            if (view == null || !(view is LinearLayout))
                view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, parent, false);

            TextView txtServerAddress = view.FindViewById<TextView>(Resource.GroupListItem.txtGroupName);
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

            txtServerAddress.Text = item.ServerAddress;
            txtData.Text = item.UserName;
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
