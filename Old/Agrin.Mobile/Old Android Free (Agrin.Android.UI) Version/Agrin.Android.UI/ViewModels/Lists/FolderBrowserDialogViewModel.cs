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
using Agrin.MonoAndroid.UI.Models;
using Agrin.Log;

namespace Agrin.MonoAndroid.UI.ViewModels.Lists
{
    public class FolderBrowserDialogViewModel : BaseAdapter<FolderInfo>, IBaseViewModel
    {
        public FolderBrowserDialogViewModel(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }
        public static Action<string> SelectedFolder { get; set; }
        public Activity CurrentActivity { get; set; }
        private List<FolderInfo> _items;
        private int _templateResourceId;
        GridView _gridView;
        LinearLayout mainLayout = null;

        public void BackFolder(string folder = null)
        {
            try
            {
                System.IO.DirectoryInfo dir = null;
                if (folder == null)
                    dir = System.IO.Directory.GetParent(CurrentFolder);
                else
                    dir = System.IO.Directory.GetParent(folder);
                SelectFolder(dir.FullName);
            }
            catch (Exception e)
            {
                Toast.MakeText(this.CurrentActivity, "Cannot Load Path", ToastLength.Short).Show();
            }

        }

        string _currentFolder = "";

        public string CurrentFolder
        {
            get { return _currentFolder; }
            set
            {
                txt_GoPath.Text = _currentFolder = value;
            }
        }

        public void SelectFolder(string path)
        {
            try
            {
                if (!System.IO.Directory.Exists(path))
                {
                    BackFolder(path);
                    return;
                }
                List<FolderInfo> folders = new List<FolderInfo>();
                int id = 0;
                foreach (var item in System.IO.Directory.GetDirectories(path))
                {
                    folders.Add(new FolderInfo() { Name = System.IO.Path.GetFileName(item), Address = item, Id = id });
                    id++;
                }
                CurrentFolder = path;
                _items.Clear();
                _items.AddRange(folders);
                _gridView.InvalidateViews();
            }
            catch (Exception e)
            {
                Toast.MakeText(this.CurrentActivity, "Cannot Load Path", ToastLength.Short).Show();
            }
        }

        EditText txt_GoPath = null;
        public FolderBrowserDialogViewModel(Activity context, int templateResourceId, List<FolderInfo> items)
            : base()
        {
            CurrentActivity = context;
            _templateResourceId = templateResourceId;
            _items = items;
            //mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.FolderBrowserDialog.mainLayout);
            _gridView = CurrentActivity.FindViewById<GridView>(Resource.FolderBrowserDialog.mainGridView);
            _gridView.Adapter = this;
            _gridView.ItemClick += OnListItemClick;
            Button btnSelect = CurrentActivity.FindViewById<Button>(Resource.FolderBrowserDialog.btnSelect);
            btnSelect.Click += btnSelect_Click;
            Button btnNewFolder = CurrentActivity.FindViewById<Button>(Resource.FolderBrowserDialog.btnNewFolder);
            btnNewFolder.Click += btnNewFolder_Click;
            Button btnBack = CurrentActivity.FindViewById<Button>(Resource.FolderBrowserDialog.btnBack);
            btnBack.Click += btnBack_Click;
            Button btnCancel = CurrentActivity.FindViewById<Button>(Resource.FolderBrowserDialog.btnCancel);
            btnCancel.Click += btnCancel_Click;
            Button btnGo = CurrentActivity.FindViewById<Button>(Resource.FolderBrowserDialog.btnGo);
            btnGo.Click += btnGo_Click;
            txt_GoPath = CurrentActivity.FindViewById<EditText>(Resource.FolderBrowserDialog.txt_GoPath);

        }

        void btnGo_Click(object sender, EventArgs e)
        {
            SelectFolder(txt_GoPath.Text);
        }

        void btnNewFolder_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this.CurrentActivity);
            builder.SetTitle(ViewUtility.FindTextLanguage(CurrentActivity, "CreateFolderTitle_Language"));

            // Set up the input
            EditText input = new EditText(this.CurrentActivity);
            // Specify the type of input expected; this, for example, sets the input as a password, and will mask the text
            //input.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword;
            builder.SetView(input);

            // Set up the buttons
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "OK_Language"), (dialog, which) =>
                {

                    try
                    {
                        if (string.IsNullOrWhiteSpace(input.Text))
                        {
                            Toast.MakeText(this.CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "NameValidation_Language"), ToastLength.Short).Show();
                            return;
                        }
                        string path = System.IO.Path.Combine(CurrentFolder, input.Text);
                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);
                        SelectFolder(path);
                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, "Folder Creation", true);
                        Toast.MakeText(this.CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "ErrorCreateFolder_Language"), ToastLength.Short).Show();
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

        void btnCancel_Click(object sender, EventArgs e)
        {
            CurrentActivity.Finish();
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            BackFolder();
        }

        void btnSelect_Click(object sender, EventArgs e)
        {
            SelectedFolder(CurrentFolder);
            btnCancel_Click(null, null);
        }

        public override FolderInfo this[int position]
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override long GetItemId(int position)
        {
            return _items[position].Id;
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as GridView;
            var selItem = this[e.Position];
            SelectFolder(selItem.Address);
            //var layout = e.View.FindViewById<LinearLayout>(Resource.GroupListItem.mainLayout);
            //if (selectedItems.Contains(selItem))
            //{
            //    selectedItems.Remove(selItem);
            //    layout.SetBackgroundColor(Android.Graphics.Color.Transparent);
            //}
            //else
            //{
            //    selectedItems.Add(selItem);
            //    layout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff5c9ad5"));
            //}
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            FolderInfo item = this[position];
            if (view == null || !(view is LinearLayout))
                view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, parent, false);

            ImageView imgIcon = view.FindViewById<ImageView>(Resource.FolderInfoItem.imgIcon);
            TextView txtName = view.FindViewById<TextView>(Resource.FolderInfoItem.txtName);
            txtName.Text = item.Name;
            _gridView.RefreshDrawableState();
            return view;
        }
    }
}