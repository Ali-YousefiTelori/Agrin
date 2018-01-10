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

namespace Agrin.MonoAndroid.UI
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class GroupListActivity : Activity
    {
        GroupListDataViewModel groupListData;
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);
            // Create your application here
            SetContentView(Resource.Layout.GroupListData);
            this.Title = "GroupList_Language";
            if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.GroupListData.LinearLayoutReverce1 });
            }

            ViewUtility.SetTextLanguage(this, new List<int>() { Resource.GroupListData.btnDelete, Resource.GroupListData.btnAddGroup });

            groupListData = new GroupListDataViewModel(this, Resource.Layout.GroupListItem, Agrin.Download.Manager.ApplicationGroupManager.Current.GroupInfoes.ToList());
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            groupListData.DisposeAll();
            ActivitesManager.ToolbarActive(null);
            base.Finish();
        }
    }
}

