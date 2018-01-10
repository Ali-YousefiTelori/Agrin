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
using Agrin.Views.Link;
using Agrin.Views;
using Agrin.Views.Settings;
using Agrin.Views.List;
using Agrin.Views.Web;

namespace Agrin.ViewModels.Toolbox
{
    public class ToolbarViewModel : IDisposable
    {
        public static ToolbarViewModel This { get; set; }
        public MainActivity CurrentActivity { get; set; }
        public LinearLayout btnAddLink = null;
        public ToolbarViewModel(MainActivity activity)
        {
            This = this;
            CurrentActivity = activity;
            btnAddLink = activity.FindViewById<LinearLayout>(Resource.MainActionBar.btnAddLink);
            btnAddLink.Click += btnAddLink_Click;

            var btnAlert = activity.FindViewById<LinearLayout>(Resource.MainActionBar.btnAlert);
            btnAlert.Click += btnAlert_Click;

            var btnAllMenu = activity.FindViewById<LinearLayout>(Resource.MainActionBar.btnAllMenu);
            btnAllMenu.Click += btnAllMenu_Click;

            var btnListlinks = activity.FindViewById<LinearLayout>(Resource.MainActionBar.btnListlinks);
            btnListlinks.Click += btnListlinks_Click;
            var btnSetting = activity.FindViewById<LinearLayout>(Resource.MainActionBar.btnSetting);
            btnSetting.Click += btnSetting_Click;

            SetFocusLayout(btnListlinks);
        }


        LinearLayout lastFocusLinearLayout = null;
        void SetFocusLayout(object sender)
        {
            CurrentActivity.RunOnUI(() =>
            {
                var layout = sender as LinearLayout;
                layout.Enabled = false;
                if (lastFocusLinearLayout != null && lastFocusLinearLayout != layout)
                    lastFocusLinearLayout.Enabled = true;
                lastFocusLinearLayout = layout;
            });
        }


        void btnAllMenu_Click(object sender, EventArgs e)
        {
            if (MainActivity.IsLoading)
                return;
            LinearLayout btnAllMenu = sender as LinearLayout;
            CurrentActivity.RegisterForContextMenu(btnAllMenu);
            CurrentActivity.OpenContextMenu(btnAllMenu);
            CurrentActivity.UnregisterForContextMenu(btnAllMenu);
        }

        void btnSetting_Click(object sender, EventArgs e)
        {
            if (MainActivity.IsLoading)
                return;
            SetFocusLayout(sender);
            if (CurrentActivity.appSettingsView != null)
            {
                CurrentActivity.appSettingsView.InitializeView();
            }
            else
            {
                var actionToolBoxLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.bottomToolboxMenuLayout);
                var mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.contentLayout);
                var actionTopToolBoxLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.topToolboxMenuLayout);
                CurrentActivity.appSettingsView = new ApplicationSettingsView(CurrentActivity, mainLayout, actionToolBoxLayout, actionTopToolBoxLayout);
            }
        }


        void btnListlinks_Click(object sender, EventArgs e)
        {
            if (MainActivity.IsLoading)
                return;
            SetFocusLayout(sender);
            MainActivity.linksVM.InitializeView();
        }

        void btnAlert_Click(object sender, EventArgs e)
        {
            if (MainActivity.IsLoading)
                return;
            SetFocusLayout(sender);
            if (CurrentActivity.noticesManager != null)
            {
                CurrentActivity.noticesManager.InitializeView();
            }
            else
            {
                var actionToolBoxLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.bottomToolboxMenuLayout);
                var mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.contentLayout);
                var actionTopToolBoxLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.topToolboxMenuLayout);
                CurrentActivity.noticesManager = new NoticesManager(CurrentActivity, Resource.Layout.NoticInfoView, mainLayout, actionToolBoxLayout, actionTopToolBoxLayout);
            }
        }

        Agrin.BaseViewModels.Link.AddLinksBaseViewModel addLinksBaseViewModel = new BaseViewModels.Link.AddLinksBaseViewModel();
        void btnAddLink_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainActivity.IsLoading)
                    return;
                AddLinks addLinks = new AddLinks(CurrentActivity, () =>
                {
                    //lastFocusLinearLayout.Enabled = true;
                    //lastFocusLinearLayout = null;
                });
            }
            catch (Exception ex)
            {

            }

        }


        public void ClickMenuItem(Models.MenuItem menuItem, Download.Web.LinkInfo SelectedLinkInfoFromMenu, MainActivity mainActivity)
        {
            try
            {
                if (menuItem.Mode == Models.MenuItemModeEnum.Learning)
                {
                    ViewsUtility.ShowPageDialog("http://framesoft.ir/Learning/AndroidPro", "AboutLearning_Language", null, CurrentActivity);
                }
                else if (menuItem.Mode == Models.MenuItemModeEnum.About)
                {
                    var mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.contentLayout);
                    var viewObj = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.AboutView, mainLayout, false);
                    if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
                    {
                        ViewsUtility.SetRightToLeftLayout(viewObj, new List<int>() { Resource.AboutView.ReverceLinearLayout1, Resource.AboutView.ReverceLinearLayout2, Resource.AboutView.ReverceLinearLayout3 });
                    }

                    ViewsUtility.SetTextLanguage(CurrentActivity, viewObj, new List<int>() { Resource.AboutView.TxtProgrammer, Resource.AboutView.TxtProgrammerTitle, Resource.AboutView.TxtSite, Resource.AboutView.TxtSiteTitle, Resource.AboutView.TxtVersionTitle });
                    var textVersion = viewObj.FindViewById<TextView>(Resource.AboutView.TxtVersion);
                    textVersion.Text = Agrin.Download.Data.Settings.ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion;

                    ViewsUtility.ShowCustomResultDialog(CurrentActivity, "About_Language", viewObj, null, isCancel: false);
                }
                else if (menuItem.Mode == Models.MenuItemModeEnum.TaskInfoesManagement)
                {
                    ShowTaskManager();
                }
                else if (menuItem.Mode == Models.MenuItemModeEnum.AgrinBrowser)
                {
                    ShowBrowser();
                }
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "ClickMenuItem " + (menuItem == null ? "null" : menuItem.Mode.ToString()));
            }
        }

        public void ShowTaskManager()
        {
            if (lastFocusLinearLayout != null)
                lastFocusLinearLayout.Enabled = true;
            if (CurrentActivity.taskInfoesManager != null)
            {
                CurrentActivity.taskInfoesManager.InitializeView();
            }
            else
            {
                var actionToolBoxLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.bottomToolboxMenuLayout);
                var mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.contentLayout);
                var actionTopToolBoxLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.topToolboxMenuLayout);
                CurrentActivity.taskInfoesManager = new TaskInfoesManager(CurrentActivity, Resource.Layout.TaskInfoItem, mainLayout, actionToolBoxLayout, actionTopToolBoxLayout);
            }
        }

        public void ShowBrowser()
        {
            if (lastFocusLinearLayout != null)
                lastFocusLinearLayout.Enabled = true;
            if (CurrentActivity.agrinBrowser != null)
            {
                CurrentActivity.agrinBrowser.InitializeView();
            }
            else
            {
                var actionToolBoxLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.bottomToolboxMenuLayout);
                var mainLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.contentLayout);
                var actionTopToolBoxLayout = CurrentActivity.FindViewById<LinearLayout>(Resource.MainActionBar.topToolboxMenuLayout);
                CurrentActivity.agrinBrowser = new AgrinBrowser(CurrentActivity, Resource.Layout.TaskInfoItem, mainLayout, actionToolBoxLayout, actionTopToolBoxLayout);
            }
        }

        public void Dispose()
        {
            addLinksBaseViewModel = null;
        }
    }
}