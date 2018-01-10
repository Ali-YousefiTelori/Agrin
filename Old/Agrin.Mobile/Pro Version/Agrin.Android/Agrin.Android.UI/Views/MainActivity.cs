using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.HardWare;
using Agrin.Helpers;
using Agrin.Log;
using Agrin.Models;
using Agrin.Services;
using Agrin.ViewModels.Dialogs;
using Agrin.ViewModels.Toolbox;
using Agrin.Views.List;
using Agrin.Views.Settings;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Android.Content.PM;
using Agrin.Views.Web;

namespace Agrin.Views
{
    [Activity(MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden, Icon = "@drawable/icon", Theme = "@style/Theme.Splash", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class MainActivity : Activity
    {
        public static Activity This { get; set; }
        public static bool IsDestroy = true;

        public ToolbarViewModel toolBarVM = null;
        public NoticesManager noticesManager = null;
        public TaskInfoesManager taskInfoesManager = null;
        public AgrinBrowser agrinBrowser = null;
        
        public ApplicationSettingsView appSettingsView = null;
        public static LinksViewModel linksVM = null;

        public static bool IsLoading { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //PermissiansHelper.NeedPermissians = new List<string>() { Android.Manifest.Permission.AccessWifiState, Android.Manifest.Permission.ChangeNetworkState, Android.Manifest.Permission.ModifyPhoneState,
                //Android.Manifest.Permission.ChangeWifiState,Android.Manifest.Permission.Internet,Android.Manifest.Permission.ReadExternalStorage,Android.Manifest.Permission.WakeLock,Android.Manifest.Permission.WriteExternalStorage,
                //    Android.Manifest.Permission.ReadPhoneState};
                //DeviceHelper.SetMobileDataEnabled(this, true);

                //PermissiansHelper.CheckPermissionsNeed(this);
                BindingExpresionEventManager.CurrentActivity = this;
                IsLoading = true;
                Title = ViewsUtility.FindTextLanguage(this, "App_Name_Language");

                IsDestroy = false;
                base.OnCreate(savedInstanceState);

                BrowseActivity.IsBrowse = false;
                NotificationClickActivity.IsBrowse = false;
                This = this;

                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleExceptions);
                AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

                var view = LayoutInflater.Inflate(Resource.Layout.InitializingApplicationProgress, new LinearLayout(this), false);
                SetContentView(view);
                ProgressBar progressInit = view.FindViewById<ProgressBar>(Resource.InitializingApplicationProgress.mainProgress);
                TextView txtTitle = view.FindViewById<TextView>(Resource.InitializingApplicationProgress.txtTitle);
                progressInit.Max = 9;
                //ViewsUtility.ShowPageDialog("http://framesoft.ir/Learning/AndroidPro", "AboutLearning_Language", null, this);
                txtTitle.Text = "در حال بارگزاری...";
                Agrin.Download.Data.DeSerializeData.ChangedStateAction = (state) =>
                {
                    this.RunOnUI(() =>
                    {
                        try
                        {
                            progressInit.Progress = (int)state;
                            switch (state)
                            {
                                case Download.Data.LoadingStateEnum.LoadApplicationIPsData:
                                    {
                                        txtTitle.Text = "بارگزاری آیکن ها...";
                                        break;
                                    }
                                case Download.Data.LoadingStateEnum.LoadAppServiceData:
                                    {
                                        txtTitle.Text = "بارگزاری داده ها...";
                                        break;
                                    }
                                case Download.Data.LoadingStateEnum.LoadFileApplicationSetting:
                                    {
                                        txtTitle.Text = "بارگزاری تنظیمات...";
                                        break;
                                    }
                                case Download.Data.LoadingStateEnum.LoadGroups:
                                    {
                                        txtTitle.Text = "بارگزاری گروه ها...";
                                        break;
                                    }
                                case Download.Data.LoadingStateEnum.LoadLinks:
                                    {
                                        txtTitle.Text = "بارگزاری لینک ها...";
                                        break;
                                    }
                                case Download.Data.LoadingStateEnum.LoadNoticesFromFile:
                                    {
                                        txtTitle.Text = "بارگزاری پیام ها...";
                                        break;
                                    }
                                case Download.Data.LoadingStateEnum.LoadTaskInfoesFromFile:
                                    {
                                        txtTitle.Text = "بارگزاری وظایف...";
                                        break;
                                    }
                                case Download.Data.LoadingStateEnum.InitializeUI:
                                    {
                                        txtTitle.Text = "بارگزاری رابط کاربری...";
                                        break;
                                    }
                                case Download.Data.LoadingStateEnum.InitializeServicesAndUI:
                                    {
                                        txtTitle.Text = "بارگزاری سرویس...";
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            InitializeApplication.GoException(ex);
                        }
                    });
                };
                if (AgrinService.This == null)
                {
                    AgrinService.MustInitializeUI = false;
                    StartService(new Intent(this, typeof(AgrinService)));
                }
                else
                {
                    AgrinService.MustInitializeUI = true;
                }
                System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
                {
                    while (true)
                    {
                        try
                        {
                            if (AgrinService.MustInitializeUI)
                            {
                                InitializeMain();
                                break;
                            }
                            System.Threading.Thread.Sleep(500);
                        }
                        catch (Exception ee)
                        {
                            InitializeApplication.GoException(ee, "loaded OnCreate");
                            var txt = ee.StackTrace;
                            var msg = ee.Message;
                            Toast.MakeText(this, msg, ToastLength.Long).Show();
                            break;
                        }
                    }
                    //int end = 0;
                });
                task.Start();
            }
            catch (Exception ee)
            {
                InitializeApplication.GoException(ee, "main OnCreate");
                var txt = ee.StackTrace;
                var msg = ee.Message;
                Toast.MakeText(this, msg, ToastLength.Long).Show();
            }
        }

        public static System.Action<Android.Net.Uri> ActionSetPermission = null;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 42)
            {
                Android.Net.Uri treeUri = null;
                if (resultCode == Result.Ok)
                {
                    // Get Uri from Storage Access Framework.
                    treeUri = data.Data;
                    var takeFlags = data.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                    // Check for the freshest data.
                    ContentResolver.TakePersistableUriPermission(treeUri, takeFlags);
                    if (ActionSetPermission != null)
                        ActionSetPermission(treeUri);
                }
                // Persist URI in shared preference so that you can use it later.
                // Use your own framework here instead of PreferenceUtil.
                //    PreferenceUtil.setSharedPreferenceUri(R.string.key_internal_uri_extsdcard, treeUri);

                //    // Persist access permissions.
                //    int takeFlags = resultData.getFlags()
                //        & (Android.Content.Intent.FLAG_GRANT_READ_URI_PERMISSION | Intent.FLAG_GRANT_WRITE_URI_PERMISSION);
                //getActivity().getContentResolver().takePersistableUriPermission(treeUri, takeFlags);
            }

        }

        public static void TriggerStorageAccessFramework()
        {
            if ((int)Build.VERSION.SdkInt == 19)
            {
                Intent intent = new Intent(Intent.ActionCreateDocument)
                    .AddCategory(Intent.CategoryOpenable)
                    .SetType(Android.Provider.DocumentsContract.Document.MimeTypeDir);

                MainActivity.This.StartActivityForResult(intent, 42);

                //Android.Content.Intent intent = new Android.Content.Intent(Intent.ActionOpenDocument);
                //intent.SetType(Android.Provider.DocumentsContract.Document.MimeTypeDir);
                //MainActivity.This.StartActivityForResult(intent, 42);
            }
            else
            {
                Android.Content.Intent intent = new Android.Content.Intent(Intent.ActionOpenDocumentTree);
                MainActivity.This.StartActivityForResult(intent, 42);
            }
        }

        volatile bool _doubleBackToExitPressedOnce = false;

        public override void OnBackPressed()
        {
            if (agrinBrowser != null && agrinBrowser.currentWebView != null && agrinBrowser.IsFocusOnAgrinBrowser())
            {
                if (agrinBrowser.currentWebView.CanGoBack())
                {
                    agrinBrowser.currentWebView.GoBack();
                    return;
                }
            }
            if (_doubleBackToExitPressedOnce)
            {
                base.OnBackPressed();
                return;
            }

            _doubleBackToExitPressedOnce = true;
            Toast.MakeText(this, "برای خروج مجدداً کلیک کنید", ToastLength.Short).Show();

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);
                _doubleBackToExitPressedOnce = false;
            });
        }

        public override void Finish()
        {
            Agrin.Download.Data.SerializeData.CloseApplicationWaitForSavingAllComplete();
            AgrinService.StopServiceIfNotNeed();
            try
            {
                BindingHelper.DisposeAll();
            }
            catch (Exception ex)
            {
                Agrin.Log.AutoLogger.LogError(ex, "Finish DisposeAll");
            }
            try
            {
                DisposeAllViews();
            }
            catch (Exception ex)
            {
                Agrin.Log.AutoLogger.LogError(ex, "Finish DisposeAllViews");
            }
            base.Finish();
        }

        public void DisposeAllViews()
        {
            if (toolBarVM != null)
                toolBarVM.Dispose();

            if (linksVM != null)
                linksVM.Dispose();

            if (noticesManager != null)
                noticesManager.Dispose();
            if (taskInfoesManager != null)
                taskInfoesManager.Dispose();
            if (agrinBrowser != null)
                agrinBrowser.Dispose();
            if (appSettingsView != null)
                appSettingsView.Dispose();

            toolBarVM = null;
            noticesManager = null;
            agrinBrowser = null;
            appSettingsView = null;
            linksVM = null;
        }

        public void InitializeMain()
        {
            try
            {
                if (!InitializeApplication.Inited)
                {
                    InitializeApplication.GoException("OK OK Initialize Release");
                    InitializeApplication.Run(this);
                }
                else if (ApplicationNoticeManager.Current == null)
                {
                    InitializeApplication.GoException("OK OK Must Release");
                    InitializeApplication.Inited = false;
                    InitializeApplication.Run(this);
                }

                this.RunOnUI(() =>
                {
                    SetContentView(Resource.Layout.MainActionBar);
                }, true);

                GenerateAlertCountData();

                toolBarVM = new ToolbarViewModel(this);
                var mainLayout = FindViewById<LinearLayout>(Resource.MainActionBar.contentLayout);

                var actionToolBoxLayout = FindViewById<LinearLayout>(Resource.MainActionBar.bottomToolboxMenuLayout);
                var actionTopToolBoxLayout = FindViewById<LinearLayout>(Resource.MainActionBar.topToolboxMenuLayout);
                linksVM = new LinksViewModel(this, Resource.Layout.LinkInfoListItem, mainLayout, actionToolBoxLayout, actionTopToolBoxLayout);


                ApplicationNoticeManager.Current.NoticeAddedAction = (notice) =>
                {
                    this.RunOnUI(() =>
                    {
                        this.GenerateAlertCountData();
                    });
                };
                IsLoading = false;

                //TochTest(mainLayout);

                //var viewObj = LayoutInflater.Inflate(Resource.Layout.MainActionBar, mainLayout, false);
                //mainLayout.AddView(viewObj);
            }
            catch (Exception ex)
            {
                IsLoading = false;
                InitializeApplication.GoException(ex, "main RunedService :" + (ApplicationNoticeManager.Current == null ? "is null" : "not null"));
                var txt = ex.StackTrace;
                var msg = ex.Message;
                this.RunOnUI(() =>
                {
                    Toast.MakeText(this, msg, ToastLength.Long).Show();
                });
            }
        }

        public void GenerateAlertCountData()
        {
            this.RunOnUI(() =>
            {
                try
                {
                    var relativeLayoutNoticeCount = FindViewById<View>(Resource.MainActionBar.AlertCycleLayout);
                    if (relativeLayoutNoticeCount == null || ApplicationNoticeManager.Current == null)
                        return;

                    if (ApplicationNoticeManager.Current.NotReadCount == 0)
                        relativeLayoutNoticeCount.Visibility = ViewStates.Gone;
                    else
                    {
                        var txtNoticeCount = FindViewById<TextView>(Resource.MainActionBar.txtNoticeCount);
                        if (txtNoticeCount == null)
                        {
                            AutoLogger.LogText("txtNoticeCount is null");
                            return;
                        }
                        txtNoticeCount.Text = ApplicationNoticeManager.Current.NotReadCount.ToString();
                        relativeLayoutNoticeCount.Visibility = ViewStates.Visible;
                    }
                }
                catch (Exception ex)
                {
                    InitializeApplication.GoException(ex, "GenerateAlertCountData :" + (ApplicationNoticeManager.Current == null ? "null" : "not null"));
                }
            }, true);
        }

        public static LinkInfo SelectedLinkInfoFromMenu { get; set; }
        public static TaskInfo SelectedTaskInfoFromMenu { get; set; }

        public static bool IsLinkToolMenu { get; set; }
        MenuItemClick _menuItemClick;
        List<MenuItem> items = null;
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id == Resource.CustomListView.mainLayoutListView)
            {
                _menuItemClick = MenuItemClick.LinkInfoes;
                if (!IsLinkToolMenu)
                {
                    if (SelectedLinkInfoFromMenu == null)
                        return;
                    menu.SetHeaderTitle(SelectedLinkInfoFromMenu.PathInfo.FileName);

                    items = LinksViewModel.GenerateLinkInfoMenues(SelectedLinkInfoFromMenu, this);
                    for (var i = 0; i < items.Count; i++)
                    {
                        var mnu = menu.Add(Menu.None, i, i, items[i].Content);
                    }
                }
                else
                {
                    items = GenerateLinkInfoToolbarMenus(this);
                    for (var i = 0; i < items.Count; i++)
                    {
                        var mnu = menu.Add(Menu.None, i, i, items[i].Content);
                    }
                }
            }
            else if (v.Id == Resource.MainActionBar.btnAllMenu)
            {
                _menuItemClick = MenuItemClick.Application;
                items = GenerateApplicationMenus(this);
                for (var i = 0; i < items.Count; i++)
                {
                    var mnu = menu.Add(Menu.None, i, i, items[i].Content);
                }
            }
            else if (v.Id == Resource.TaskInfoItem.mainLayout)
            {
                _menuItemClick = MenuItemClick.TaskInfoes;
                items = TaskInfoesManager.GenerateMenus(this);
                for (var i = 0; i < items.Count; i++)
                {
                    var mnu = menu.Add(Menu.None, i, i, items[i].Content);
                }
            }
            else if (v.Id == Resource.FindLinksToolBox.mainLayout)
            {
                _menuItemClick = MenuItemClick.Searchs;
                items = LinksViewModel.GenerateSearchMenues(this);
                for (var i = 0; i < items.Count; i++)
                {
                    var mnu = menu.Add(Menu.None, i, i, items[i].Content);
                }
            }
            else if (v.Id == Resource.FolderBrowserDialog.btnSelectDataFolder)
            {
                _menuItemClick = MenuItemClick.SelectDataPath;
                items = FolderBrowserDialogViewModel.GenerateDataDirectoriyMenues(this);
                List<IMenuItem> menues = new List<IMenuItem>();
                for (var i = 0; i < items.Count; i++)
                {
                    var mnu = menu.Add(Menu.None, i, i, items[i].Content);
                    menues.Add(mnu);
                }
                foreach (var item in menues)
                {
                    item.SetOnMenuItemClickListener(new MenuItemOnMenuItemClickListener() { MenuItemClick = _menuItemClick, Items = items });
                }
            }

        }

        public override void OnContextMenuClosed(IMenu menu)
        {
            if (LinksViewModel.DeSelectCurrentItem != null)
                LinksViewModel.DeSelectCurrentItem();
            SelectedLinkInfoFromMenu = null;
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            if (item == null || items == null)
                return true;
            var menuItem = items[item.Order];
            if (_menuItemClick == MenuItemClick.Application)
            {
                toolBarVM.ClickMenuItem(menuItem, SelectedLinkInfoFromMenu, this);
            }
            else if (_menuItemClick == MenuItemClick.LinkInfoes)
            {
                if (IsLinkToolMenu)
                {
                    if (SelectedLinkInfoFromMenu == null)
                        LinkInfoToolbarMenuClick(menuItem, this);
                }
                else
                    LinksViewModel.LinkInfoClickMenuItem(menuItem, SelectedLinkInfoFromMenu, this);
            }
            else if (_menuItemClick == MenuItemClick.TaskInfoes)
            {
                TaskInfoesManager.TaskInfoClickMenuItem(menuItem, SelectedTaskInfoFromMenu, this);
            }
            else if (_menuItemClick == MenuItemClick.Searchs)
            {
                LinksViewModel.SearchLinksClickMenuItem(menuItem, this);
            }
            else if (_menuItemClick == MenuItemClick.SelectDataPath)
            {
                FolderBrowserDialogViewModel.This.SelectFolder(menuItem.Content);
            }
            return true;
        }

        public override void OnLowMemory()
        {
            try
            {
                var runtime = Java.Lang.Runtime.GetRuntime();
                long usedMemInMB = (runtime.TotalMemory() - runtime.FreeMemory()) / 1048576L;
                long maxHeapSizeInMB = runtime.MaxMemory() / 1048576L;
                AutoLogger.LogText("used: " + usedMemInMB + " MB max: " + maxHeapSizeInMB + " MB");
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "get low memory!");
            }
            InitializeApplication.GoException("OnLowMemory Main");
            base.OnLowMemory();
        }

        protected override void OnPause()
        {
            base.OnPause();
            Agrin.Helper.ComponentModel.ANotifyPropertyChanged.StopNotifyChanging();
            notify("onPause");
        }

        protected override void OnResume()
        {
            base.OnResume();
            Agrin.Helper.ComponentModel.ANotifyPropertyChanged.StartNotifyChanging();
            notify("onResume");
        }

        protected override void OnStop()
        {
            base.OnStop();
            notify("onStop");
        }

        protected override void OnDestroy()
        {
            try
            {
                IsDestroy = true;
                BindingHelper.DisposeAll();
                Agrin.Download.Data.SerializeData.CloseApplicationWaitForSavingAllComplete();
                AgrinService.StopServiceIfNotNeed();
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "OnDestroy ainActivity");
            }
            base.OnDestroy();
            notify("onDestroy");
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            notify("onRestoreInstanceState");
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            notify("onSaveInstanceState");
        }

        public static void notify(string text, string title = "state")
        {
            //Notification noti = new Notification.Builder(MainActivity.This)
            //.SetContentTitle(title).SetAutoCancel(true)
            //.SetSmallIcon(Resource.Drawable.Icon)
            //.SetContentText(text).Build();
            //NotificationManager notificationManager =
            //    (NotificationManager)MainActivity.This.GetSystemService(Context.NotificationService);
            //notificationManager.Notify((int)DateTime.Now.TimeOfDay.TotalMilliseconds, noti);
        }

        public static void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
        {
            InitializeApplication.GoException(e.Exception, "HandleAndroidException");
            e.Handled = true;
            notify(e.Exception.Message);
        }

        public static void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                if (e.ExceptionObject is Exception)
                {
                    InitializeApplication.GoException(e.ExceptionObject as Exception, "unHandle EX");
                    notify(((Exception)e.ExceptionObject).Message, "unHandle EX");
                    return;
                }
                else if (e.ExceptionObject is Java.Lang.Exception)
                {
                    InitializeApplication.GoException(e.ExceptionObject as Java.Lang.Exception, "unHandle Java");
                    notify(((Java.Lang.Exception)e.ExceptionObject).Message, "unHandle Java");
                    return;
                }
            }
            InitializeApplication.GoException("HandleExceptions Null");
        }

        public static List<MenuItem> GenerateLinkInfoToolbarMenus(Activity activity)
        {
            Func<string, string> getText = (text) =>
            {
                return ViewsUtility.FindTextLanguage(activity, text);
            };

            List<MenuItem> items = new List<MenuItem>();
            //if (linksVM.IsSelectionMode)
            //    items.Add(new MenuItem() { Content = getText("MenuSelectionMode_Language"), Mode = MenuItemModeEnum.MenuSelection });
            //else
            //    items.Add(new MenuItem() { Content = getText("MultiSelectionMode_Language"), Mode = MenuItemModeEnum.MultiSelection });

            if (linksVM.SelectionCount >= linksVM.Items.Count)
                items.Add(new MenuItem() { Content = getText("DeSelectAll_Language"), Mode = MenuItemModeEnum.DeSelectAll });
            else
            {
                items.Add(new MenuItem() { Content = getText("SelectAll_Language"), Mode = MenuItemModeEnum.SelectAll });
                items.Add(new MenuItem() { Content = getText("DeSelectAll_Language"), Mode = MenuItemModeEnum.DeSelectAll });
            }

            if (ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Count > 0)
                items.Add(new MenuItem() { Content = getText("StopAll_Language"), Mode = MenuItemModeEnum.StopAll });

            items.Add(new MenuItem() { Content = getText("DeleteComplete_Language"), Mode = MenuItemModeEnum.DeleteComplete });

            return items;
        }

        public static List<MenuItem> GenerateApplicationMenus(Activity activity)
        {
            Func<string, string> getText = (text) =>
            {
                return ViewsUtility.FindTextLanguage(activity, text);
            };

            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem() { Content = getText("TaskInfoesManagement_Language"), Mode = MenuItemModeEnum.TaskInfoesManagement });
            items.Add(new MenuItem() { Content = getText("Learning_Language"), Mode = MenuItemModeEnum.Learning });
            items.Add(new MenuItem() { Content = getText("AgrinBrowser_Language"), Mode = MenuItemModeEnum.AgrinBrowser });
            items.Add(new MenuItem() { Content = getText("About_Language"), Mode = MenuItemModeEnum.About });

            return items;
        }

        public static void LinkInfoToolbarMenuClick(MenuItem menu, Activity activity)
        {
            if (menu.Mode == MenuItemModeEnum.MenuSelection)
            {
                linksVM.DeSelectAll();
                linksVM.IsSelectionMode = false;
            }
            else if (menu.Mode == MenuItemModeEnum.MultiSelection)
            {
                linksVM.IsSelectionMode = true;
            }
            else if (menu.Mode == MenuItemModeEnum.DeSelectAll)
            {
                linksVM.DeSelectAll();
            }
            else if (menu.Mode == MenuItemModeEnum.SelectAll)
            {
                linksVM.SelectAll();
            }
            else if (menu.Mode == MenuItemModeEnum.StopAll)
            {
                LinksViewModel.StopLinks(activity);
            }
            else if (menu.Mode == MenuItemModeEnum.DeleteComplete)
            {
                TextView txt = new TextView(activity);
                //txt.SetMaxWidth(200);
                ViewsUtility.SetTextViewTextColor(activity, txt, Resource.Color.foreground);
                txt.Text = ViewsUtility.FindTextLanguage(activity, "Areyousureyouwanttodeletecompletelinks_Language");
                txt.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                txt.SetMaxWidth(600);
                ViewsUtility.ShowCustomResultDialog(activity, "Delete_Language", txt, () =>
                {
                    linksVM.DeSelectAll();
                    foreach (var item in linksVM.Items)
                    {
                        if (item.IsComplete)
                        {
                            linksVM.SelectItem(item);
                        }
                    }
                    linksVM.DisposeSelection();
                    linksVM.DeleteLinks();
                    linksVM.DeSelectAll();
                });
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case 10:
                    {
                        foreach (var item in grantResults)
                        {
                            if (item != Permission.Granted)
                            {
                                ViewsUtility.ShowYesCancelMessageBox(this, "نیاز به دسترسی", "کاربر گرامی برای اینکه نرم افزار بتواند درست کار کند باید دسترسی های مورد نیاز را به آن بدهید", () =>
                                {
                                    PermissiansHelper.CheckPermissionsNeed(this);
                                });
                                break;
                            }
                        }
                        break;
                    }
            }
        }
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    // Menu items default to never show in the action bar. On most devices this means
        //    // they will show in the standard options menu panel when the menu button is pressed.
        //    // On xlarge-screen devices a "More" button will appear in the far right of the
        //    // Action Bar that will display remaining items in a cascading menu.

        //    menu.Add(new Java.Lang.String("Normal item"));

        //    var actionItem = menu.Add(new Java.Lang.String("Action Button"));

        //    // Items that show as actions should favor the "if room" setting, which will
        //    // prevent too many buttons from crowding the bar. Extra items will show in the
        //    // overflow area.
        //    MenuItemCompat.SetShowAsAction(actionItem, MenuItemCompat.ShowAsActionIfRoom);

        //    // Items that show as actions are strongly encouraged to use an icon.
        //    // These icons are shown without a text description, and therefore should
        //    // be sufficiently descriptive on their own.
        //    actionItem.SetIcon(Android.Resource.Drawable.IcMenuShare);
        //    return true;
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    Android.Widget.Toast.MakeText(this,
        //        "Selected Item: " +
        //        item.TitleFormatted,
        //        Android.Widget.ToastLength.Short).Show();

        //    return true;
        //}
        //public override bool OnPreferenceTreeClick(PreferenceScreen preferenceScreen, Preference preference)
        //{
        //    try
        //    {
        //        Toast.MakeText(this, "OK 2", ToastLength.Long).Show();

        //        Type cls = null;
        //        var title = preference.Title;


        //        if (title.Equals(GetString(Resource.String.anim_scale)))
        //            cls = typeof(ScaleAnimation);

        //        var intent = new Intent(this, cls);
        //        StartActivity(intent, Bundle.Empty);
        //        Toast.MakeText(this, "OK 3", ToastLength.Long).Show();

        //    }
        //    catch (Exception ee)
        //    {
        //        Toast.MakeText(this, "2" + ee.Message, ToastLength.Long).Show();
        //    }
        //    return true;
        //}

    }
    public enum MenuItemClick
    {
        Application,
        LinkInfoes,
        TaskInfoes,
        Searchs,
        SelectDataPath
    }
}

