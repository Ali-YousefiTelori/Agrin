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
using Agrin.MonoAndroid.UI.ViewModels.Lists;
using Agrin.Framesoft;
using Agrin.Download.Data.Settings;

namespace Agrin.MonoAndroid.UI.Activities.Lists
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class FramesoftLinksListDataActivity : Activity
    {
        FramesoftLinksListDataViewModel linksListdata;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                ActivitesManager.AddActivity(this);
                base.OnCreate(bundle);
                menuItems = new string[] { ViewUtility.FindTextLanguage(this, "Download_Language"), ViewUtility.FindTextLanguage(this, "DeleteFromServer_Language") };
                // Create your application here
                SetContentView(Resource.Layout.FramesoftLinksListData);
                this.Title = "ServerLinks_Language";
                ViewUtility.SetTextLanguage(this, new List<int>() { Resource.FramesoftLinksListData.btnRefersh });


                linksListdata = new FramesoftLinksListDataViewModel(this, Resource.Layout.LinksListItem, new List<UserFileInfoData>());
                RegisterForContextMenu(linksListdata._listView);
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "FATALLLLLLLL EEEEEEE:", true);
            }
        }
        string[] menuItems;
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id == Resource.FramesoftLinksListData.mainListView)
            {
                var info = (AdapterView.AdapterContextMenuInfo)menuInfo;
                var listItemName = linksListdata[info.Position];
                menu.SetHeaderTitle(listItemName.FileName);

                for (var i = 0; i < menuItems.Length; i++)
                {
                    menu.Add(Menu.None, i, i, menuItems[i]);
                }
            }
        }

        public static void CheckForAddFrameSoftUserAuth()
        {
            bool exist = false, changed = false;
            string address = "*" + Framesoft.Helper.UserManagerHelper.domain + "*";
            foreach (var item in ApplicationSetting.Current.UserAccountsSetting.Items)
            {
                if (item.ServerAddress == address)
                {
                    exist = true;
                    item.IsUsed = true;
                    if (item.UserName != ApplicationSetting.Current.FramesoftSetting.UserName || item.Password != ApplicationSetting.Current.FramesoftSetting.Password)
                    {
                        changed = true;
                    }
                    item.UserName = ApplicationSetting.Current.FramesoftSetting.UserName;
                    item.Password = ApplicationSetting.Current.FramesoftSetting.Password;
                    item.ServerAddress = address;
                    break;
                }
            }
            if (!exist)
            {
                ApplicationSetting.Current.UserAccountsSetting.Items.Add(new Agrin.Download.Web.Link.NetworkCredentialInfo() { IsUsed = true, ServerAddress = address, UserName = ApplicationSetting.Current.FramesoftSetting.UserName, Password = ApplicationSetting.Current.FramesoftSetting.Password });
                Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            }
            if (changed)
            {
                Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            var menuItemIndex = item.ItemId;
            var menuItemName = menuItems[menuItemIndex];
            var listItemName = linksListdata[info.Position];
            if (menuItemIndex == 0)
            {
                if (listItemName.Status == 5)
                {
                    CheckForAddFrameSoftUserAuth();
                    AddYoutubeLinkViewModel.CurrentSelectedURL = "http://" + Framesoft.Helper.UserManagerHelper.domain + "/UserManager/DownloadOneFileForUser/" + listItemName.FileGuid;
                    ActivitesManager.AddNewLinkActive(this);
                }
                else
                {
                    ViewUtility.ShowMessageDialog(this, "FileNotComplete_Language", "SendFile_Language");
                }
            }
            else if (menuItemIndex == 1)
            {
                ViewUtility.ShowQuestionMessageDialog(this, "QuestionDeleteFilesError_Language", "DeleteFilesFromServer_Language", () =>
                {
                    linksListdata.RemoveLink(listItemName);
                });
            }
            return true;
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            linksListdata.DisposeAll();
            ActivitesManager.VIPToolbarActive(null);
            base.Finish();
        }
    }
}