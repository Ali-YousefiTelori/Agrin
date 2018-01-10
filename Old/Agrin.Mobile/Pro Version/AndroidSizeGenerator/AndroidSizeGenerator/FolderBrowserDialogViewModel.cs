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
using System;
//using Android.Support.V4.Provider;

namespace AndroidSizeGenerator
{
    public class FolderInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

    }

    public class FolderBrowserDialogViewModel : BaseAdapter<FolderInfo>
    {
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
            catch (System.UnauthorizedAccessException ex)
            {
                Toast.MakeText(this.CurrentActivity, "Need To Access to Path", ToastLength.Short).Show();
                MainActivity.triggerStorageAccessFramework();
            }
            catch (Exception e)
            {
                Toast.MakeText(this.CurrentActivity, "Cannot Load Path", ToastLength.Short).Show();
            }
        }

        public Activity CurrentActivity { get; set; }
        EditText txt_GoPath = null;
        public FolderBrowserDialogViewModel(Activity activity, View context, int templateResourceId, List<FolderInfo> items)
            : base()
        {
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
            txt_GoPath = viewObj.FindViewById<EditText>(Resource.FolderBrowserDialog.txt_GoPath);

        }

        void btnGo_Click(object sender, EventArgs e)
        {
            SelectFolder(txt_GoPath.Text);
        }

        Android.Net.Uri getedAccessPath = null;
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

                    //var files = Android.Content.Context.GetExternalFilesDirs(null);

                    //string path = System.IO.Path.Combine(CurrentFolder, input.Text);
                    //if (!System.IO.Directory.Exists(path))
                    //    System.IO.Directory.CreateDirectory(path);

                    //ContentResolver resolver = MainActivity.This.ContentResolver;
                    ////Android.Net.Uri path = Android.Net.Uri.FromFile(new Java.IO.File(CurrentFolder));
                    //Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUri(path.Authority, Android.Provider.DocumentsContract.GetTreeDocumentId(path));
                    //Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUriUsingTree(path, Android.Provider.DocumentsContract.GetTreeDocumentId(path));
                    //Android.Net.Uri childUri = Android.Provider.DocumentsContract.CreateDocument(resolver, docUri, Android.Provider.DocumentsContract.Document.MimeTypeDir, input.Text);
                    //if (childUri == null)
                    //{
                    //    TextView text = new TextView(MainActivity.This);
                    //    ViewsUtility.SetTextViewTextColor(MainActivity.This, text, Resource.Color.darkForeground);
                    //    StringBuilder str = new StringBuilder();
                    //    str.AppendLine("agrinPath:" + path.Path);
                    //    str.AppendLine("name:" + input.Text);
                    //    str.AppendLine("docUri:" + docUri.Path);
                    //    text.Text = str.ToString();
                    //    ViewsUtility.ShowControlDialog(CurrentActivity, text, "title", null);
                    //}
                    //else
                    //{
                    //    Toast.MakeText(CurrentActivity, "YESS", ToastLength.Short).Show();
                    //    SelectFolder(CurrentFolder);
                    //}

                    MainActivity.ActionSetPermission = (path) =>
                    {
                        try
                        {
                            var patheddd = GetFilesystemPath(CurrentActivity, path);
                            getedAccessPath = path;
                            //var newpath = System.IO.Path.Combine(path.Path, input.Text);
                            ContentResolver resolver = MainActivity.This.ContentResolver;
                            //Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUri(path.Authority, Android.Provider.DocumentsContract.GetTreeDocumentId(path));
                            Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUriUsingTree(path, Android.Provider.DocumentsContract.GetTreeDocumentId(path));
                            Android.Net.Uri childUri = Android.Provider.DocumentsContract.CreateDocument(resolver, docUri, Android.Provider.DocumentsContract.Document.MimeTypeDir, input.Text);
                            //var childUri = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(docUri.ToString(), input.Text));
                            if (childUri == null)
                            {
                                TextView text = new TextView(MainActivity.This);
                                StringBuilder str = new StringBuilder();
                                str.AppendLine("agrinPath:" + path.Path);
                                str.AppendLine("name:" + input.Text);
                                str.AppendLine("docUri:" + docUri.Path);
                                text.Text = str.ToString();
                                ViewsUtility.ShowControlDialog(MainActivity.This, text, "title", null);
                            }
                            else
                                SelectFolder(CurrentFolder);

                        }
                        catch (Exception newEX)
                        {
                            Toast.MakeText(this.CurrentActivity, "newEX " + newEX.Message + " uri: " + path.ToString(), ToastLength.Long).Show();
                        }
                    };
                    MainActivity.triggerStorageAccessFramework();
                    //if (getedAccessPath != null)
                    //    MainActivity.ActionSetPermission(getedAccessPath);
                    //else
                    //    MainActivity.triggerStorageAccessFramework();
                    //DocumentFile targetDocument = DocumentFile.FromFile(new Java.IO.File(CurrentFolder));
                    //var createdFile = targetDocument.CreateDirectory(input.Text);
                    //string path = System.IO.Path.Combine(CurrentFolder, input.Text);
                    //if (!System.IO.Directory.Exists(path))
                    //    System.IO.Directory.CreateDirectory(path);

                }
                catch (Exception ex)
                {
                    MainActivity.triggerStorageAccessFramework();
                    Toast.MakeText(CurrentActivity, "err: " + ex.Message, ToastLength.Long).Show();
                    //try
                    //{
                    //    Toast.MakeText(this.CurrentActivity, ex.Message, ToastLength.Long).Show();
                    //    MainActivity.ActionSetPermission = (path) =>
                    //    {
                    //        try
                    //        {
                    //            //var newpath = System.IO.Path.Combine(path.Path, input.Text);
                    //            ContentResolver resolver = MainActivity.This.ContentResolver;
                    //            Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUri(path.Authority, Android.Provider.DocumentsContract.GetTreeDocumentId(path));
                    //            Android.Net.Uri childUri = Android.Provider.DocumentsContract.CreateDocument(resolver, docUri, Android.Provider.DocumentsContract.Document.MimeTypeDir, input.Text);
                    //            if (childUri == null)
                    //            {
                    //                Toast.MakeText(this.CurrentActivity, "cannot create", ToastLength.Long).Show();
                    //            }
                    //            else
                    //                SelectFolder(childUri.Path);
                    //        }
                    //        catch (Exception newEX)
                    //        {
                    //            Toast.MakeText(this.CurrentActivity, "newEX " + newEX.Message, ToastLength.Long).Show();
                    //        }
                    //    };
                    //    MainActivity.triggerStorageAccessFramework();
                    //}
                    //catch (Exception eeee)
                    //{
                    //    Toast.MakeText(this.CurrentActivity, "eee " + eeee.Message, ToastLength.Long).Show();
                    //}
                }
            });
        }

        public string GetFilesystemPath(Context context, Android.Net.Uri uri)
        {
            var docUri = Android.Provider.DocumentsContract.BuildDocumentUriUsingTree(uri,
                                    Android.Provider.DocumentsContract.GetTreeDocumentId(uri));
            string path = getPath(CurrentActivity, docUri);
            return path;
        }

        public string getPath(Context context, Android.Net.Uri uri)
        {
            bool isKitKat = Build.VERSION.SdkInt >= Build.VERSION_CODES.Kitkat;
            //uri = Android.Net.Uri.Parse(Android.OS.Environment.ExternalStorageDirectory.Path);
            var path = Android.OS.Environment.DataDirectory.Path;
            bool checkexist = System.IO.Directory.Exists(uri.Path);
            Java.IO.File file = new Java.IO.File(uri.ToString());
            var exist = file.Exists();

            var cursor = context.ContentResolver.Query(uri, null, null, null, null);
            if (cursor != null && cursor.MoveToFirst())
            {
                var columns = cursor.GetColumnNames();
            }
            //uri = Android.Net.Uri.Parse("content://com.android.externalstorage.documents/tree/19FF-3905%3ADownload/document/19FF-3905%3ADownloads");
            //context.ContentResolver.ex
            //cursor = context.ContentResolver.Query(uri, null, null, null, null);
            //if (cursor != null && cursor.MoveToFirst())
            //{
            //  var columns=  cursor.GetColumnNames();
            //}

            //uri = Android.Net.Uri.Parse(Android.OS.Environment.ExternalStorageDirectory.Path);

            //cursor = context.ContentResolver.Query(uri, null, null, null, null);
            //if (cursor != null && cursor.MoveToFirst())
            //{
            //    var columns = cursor.GetColumnNames();
            //}
            // DocumentProvider
            if (isKitKat && Android.Provider.DocumentsContract.IsDocumentUri(context, uri))
            {
                //Android.Provider.DocumentsContract.
                // ExternalStorageProvider
                if (isExternalStorageDocument(uri))
                {
                    Java.Lang.String docId = new Java.Lang.String(Android.Provider.DocumentsContract.GetDocumentId(uri));
                    string[] split = docId.Split(":");
                    string type = split[0];

                    if (new Java.Lang.String("primary").EqualsIgnoreCase(type))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }

                    // TODO handle non-primary volumes
                }
                // DownloadsProvider
                else if (isDownloadsDocument(uri))
                {

                    Java.Lang.String id = new Java.Lang.String(Android.Provider.DocumentsContract.GetDocumentId(uri));
                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                            Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id.ToString()));

                    return getDataColumn(context, contentUri, null, null);
                }
                // MediaProvider
                else if (isMediaDocument(uri))
                {
                    Java.Lang.String docId = new Java.Lang.String(Android.Provider.DocumentsContract.GetDocumentId(uri));
                    string[] split = docId.Split(":");
                    Java.Lang.String type = new Java.Lang.String(split[0]);

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Audio.Media.ExternalContentUri;
                    }

                    string selection = "_id=?";
                    string[] selectionArgs = new string[] {
                        split[1] };

                    return getDataColumn(context, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if (new Java.Lang.String("content").EqualsIgnoreCase(uri.Scheme))
            {

                // Return the remote address
                if (isGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                return getDataColumn(context, uri, null, null);
            }
            // File
            else if (new Java.Lang.String("file").EqualsIgnoreCase(uri.Scheme))
            {
                return uri.Path;
            }

            return null;
        }
        public string getDataColumn(Context context, Android.Net.Uri uri, string selection,
                                    string[] selectionArgs)
        {

            Android.Database.ICursor cursor = null;
            string column = "_data";
            string[] projection = {
                column
        };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,
                        null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }


        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is ExternalStorageProvider.
         */
        public bool isExternalStorageDocument(Android.Net.Uri uri)
        {
            return new Java.Lang.String("com.android.externalstorage.documents").Equals(uri.Authority);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is DownloadsProvider.
         */
        public bool isDownloadsDocument(Android.Net.Uri uri)
        {
            return new Java.Lang.String("com.android.providers.downloads.documents").Equals(uri.Authority);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is MediaProvider.
         */
        public bool isMediaDocument(Android.Net.Uri uri)
        {
            return new Java.Lang.String("com.android.providers.media.documents").Equals(uri.Authority);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is Google Photos.
         */
        public bool isGooglePhotosUri(Android.Net.Uri uri)
        {
            return new Java.Lang.String("com.google.android.apps.photos.content").Equals(uri.Authority);
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