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
using Agrin.Helpers;
using Agrin.Log;
using Agrin.Models;
using static Android.Views.View;
using Agrin.Views;
using Agrin.Streams;

namespace Agrin.ViewModels.Dialogs
{
    public class FolderInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

    }

    public class MenuItemOnMenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
    {
        public MenuItemClick MenuItemClick { get; set; }
        public List<MenuItem> Items { get; set; }
        public bool OnMenuItemClick(IMenuItem item)
        {
            if (item == null || Items == null)
                return true;
            var menuItem = Items[item.Order];
            if (MenuItemClick == MenuItemClick.SelectDataPath)
            {
                FolderBrowserDialogViewModel.This.SelectFolder(menuItem.Content);
            }
            return true;
        }
    }

    public class FolderBrowserDialogViewModel : BaseAdapter<FolderInfo>
    {
        public static FolderBrowserDialogViewModel This { get; set; }

        public Action<string> SelectFolderAction { get; set; }
        public Action CancelSelectFolderAction { get; set; }
        public FolderBrowserDialogViewModel(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }
        View viewObj { get; set; }
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
            //catch (System.UnauthorizedAccessException ex)
            //{
            //    Toast.MakeText(this.CurrentActivity, "Need To Access to Path", ToastLength.Short).Show();
            //    Agrin.Views.MainActivity.TriggerStorageAccessFramework();
            //}
            catch (Exception e)
            {
                Toast.MakeText(this.CurrentActivity, "Cannot Load Path", ToastLength.Short).Show();
            }
        }

        public Activity CurrentActivity { get; set; }
        EditText txt_GoPath = null;
        Button btnSelectDataFolder = null;
        public FolderBrowserDialogViewModel(Activity activity, View context, int templateResourceId, List<FolderInfo> items)
            : base()
        {
            This = this;
            viewObj = context;
            CurrentActivity = activity;
            _templateResourceId = templateResourceId;
            _items = items;
            //mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.FolderBrowserDialog.mainLayout);
            _gridView = viewObj.FindViewById<GridView>(Resource.FolderBrowserDialog.mainGridView);
            _gridView.Adapter = this;
            _gridView.ItemClick += OnListItemClick;
            Button btnSelect = viewObj.FindViewById<Button>(Resource.FolderBrowserDialog.btnSelect);
            btnSelect.Click += btnSelect_Click;
            Button btnNewFolder = viewObj.FindViewById<Button>(Resource.FolderBrowserDialog.btnNewFolder);
            btnNewFolder.Click += btnNewFolder_Click;
            Button btnBack = viewObj.FindViewById<Button>(Resource.FolderBrowserDialog.btnBack);
            btnBack.Click += btnBack_Click;
            Button btnCancel = viewObj.FindViewById<Button>(Resource.FolderBrowserDialog.btnCancel);
            btnCancel.Click += btnCancel_Click;
            Button btnGo = viewObj.FindViewById<Button>(Resource.FolderBrowserDialog.btnGo);
            btnGo.Click += btnGo_Click;
            btnSelectDataFolder = viewObj.FindViewById<Button>(Resource.FolderBrowserDialog.btnSelectDataFolder);
            btnSelectDataFolder.Click += BtnSelectDataFolder_Click;

            txt_GoPath = viewObj.FindViewById<EditText>(Resource.FolderBrowserDialog.txt_GoPath);

        }

        private void BtnSelectDataFolder_Click(object sender, EventArgs e)
        {
            CurrentActivity.RegisterForContextMenu(btnSelectDataFolder);
            CurrentActivity.OpenContextMenu(btnSelectDataFolder);
            CurrentActivity.UnregisterForContextMenu(btnSelectDataFolder);
        }

        private void BtnSelectDataFolder_ContextClick(object sender, View.ContextClickEventArgs e)
        {

        }

        public static List<MenuItem> GenerateDataDirectoriyMenues(Activity activity)
        {
            if (activity == null)
            {
                AutoLogger.LogText("GenerateDataDirectoriyMenues activity is null!");
            }
            List<MenuItem> items = new List<MenuItem>();
            var files = GetStorageDirectories(activity);
            foreach (var item in files)
            {
                items.Add(new MenuItem() { Content = item });
            }

            if (files == null || files.Count == 0)
            {
                ViewsUtility.ShowMessageBoxOnlyOkButton(activity, "بروز مشکل", "این گزینه برای اندروید شما کاربرد ندارد.", null);
            }
            return items;
        }

        public static List<string> GetStorageDirectories(Activity activity)
        {
            List<string> storageDirectories = new List<string>();
            try
            {
                string rawSecondaryStoragesStr = Java.Lang.JavaSystem.Getenv("SECONDARY_STORAGE");
                if (string.IsNullOrEmpty(rawSecondaryStoragesStr))
                {
                    rawSecondaryStoragesStr = Java.Lang.JavaSystem.Getenv("EXTERNAL_SDCARD_STORAGE");
                }
                if (ViewsUtility.GetApiVersion() >= 19)
                {
                    List<string> results = new List<string>();
                    var externalDirs = activity.GetExternalFilesDirs(null);
                    foreach (var file in externalDirs)
                    {
                        results.Add(file.Path);
                    }
                    storageDirectories.AddRange(results);
                }
                else
                {
                    var rv = new List<string>();

                    if (!Android.Text.TextUtils.IsEmpty(rawSecondaryStoragesStr))
                    {
                        string[] rawSecondaryStorages = rawSecondaryStoragesStr.Split(new string[] { Java.IO.File.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                        rv.AddRange(rawSecondaryStorages);
                    }
                    for (int i = 0; i < rv.Count; i++)
                    {
                        rv[i] = rv[i] + "/Android/data/Agrin.Android/files";
                    }
                    foreach (var item in rv)
                    {
                        try
                        {
                            if (!System.IO.Directory.Exists(item))
                            {
                                System.IO.Directory.CreateDirectory(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            AutoLogger.LogError(ex, "GetStorageDirectories 1");
                        }
                    }
                    storageDirectories.AddRange(rv);
                }
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "GetStorageDirectories 2");
            }
            if (storageDirectories.Count ==0)
            {
                var items = StorageInfo.GetStorageList();
                foreach (var item in items)
                {
                    storageDirectories.Add(item.path + "/Android/data/Agrin.Android/files");
                }
                foreach (var item in storageDirectories)
                {
                    try
                    {
                        if (!System.IO.Directory.Exists(item))
                        {
                            System.IO.Directory.CreateDirectory(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, "GetStorageDirectories 1");
                    }
                }
            }
            return storageDirectories;
        }



        void btnGo_Click(object sender, EventArgs e)
        {
            SelectFolder(txt_GoPath.Text);
        }

        void btnNewFolder_Click(object sender, EventArgs e)
        {
            EditText input = new EditText(new ContextThemeWrapper(CurrentActivity, Resource.Style.editText));

            ViewsUtility.SetBackground(CurrentActivity, input, Resource.Drawable.EditBoxShape);
            ViewsUtility.ShowCustomResultDialog(CurrentActivity, "CreateFolderTitle_Language", input, () =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(input.Text))
                    {
                        Toast.MakeText(CurrentActivity, ViewsUtility.FindTextLanguage(CurrentActivity, "NameValidation_Language"), ToastLength.Short).Show();
                        return;
                    }
                    string path = System.IO.Path.Combine(CurrentFolder, input.Text);
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    SelectFolder(path);
                }
                catch (Exception ex)
                {
                    AutoLogger.LogError(ex, "Folder Creation", true);
                    Toast.MakeText(this.CurrentActivity, ViewsUtility.FindTextLanguage(CurrentActivity, "ErrorCreateFolder_Language"), ToastLength.Short).Show();
                }
            });
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            CancelSelectFolderAction();
            //CurrentActivity.Finish();
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            BackFolder();
        }

        void btnSelect_Click(object sender, EventArgs e)
        {
            SelectFolderAction(CurrentFolder);
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