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
using Agrin.MonoAndroid.UI.Models;

namespace Agrin.MonoAndroid.UI.Activities.Lists
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class FolderBrowserDialogActivity : Activity
    {
        public static string CurrentPath { get; set; }
        FolderBrowserDialogViewModel vm = null;
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FolderBrowserDialog);
            this.Title = "BrowseFolder_Language";
            if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.FolderBrowserDialog.LinearLayoutReverce1, Resource.FolderBrowserDialog.LinearLayoutReverce2 });
            }
            ViewUtility.SetTextLanguage(this, new List<int>() { Resource.FolderBrowserDialog.btnBack, Resource.FolderBrowserDialog.btnCancel, Resource.FolderBrowserDialog.btnGo, Resource.FolderBrowserDialog.btnNewFolder, Resource.FolderBrowserDialog.btnSelect });

            List<FolderInfo> folders = new List<FolderInfo>();
            int id = 0;
            string currentFolder = "";
            try
            {
                foreach (var item in System.IO.Directory.GetDirectories(CurrentPath))
                {
                    folders.Add(new FolderInfo() { Name = System.IO.Path.GetFileName(item), Address = item, Id = id });
                    id++;
                }
                currentFolder = CurrentPath;
            }
            catch (Exception e)
            {
                currentFolder = "/";
                foreach (var item in System.IO.Directory.GetDirectories("/"))
                {
                    folders.Add(new FolderInfo() { Name = System.IO.Path.GetFileName(item), Address = item, Id = id });
                    id++;
                }
            }

            vm = new FolderBrowserDialogViewModel(this, Resource.Layout.FolderInfoItem, folders);
            vm.CurrentFolder = currentFolder;
            // Create your application here
        }
        public override void OnBackPressed()
        {
            vm.BackFolder();
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            base.Finish();
        }
    }
}