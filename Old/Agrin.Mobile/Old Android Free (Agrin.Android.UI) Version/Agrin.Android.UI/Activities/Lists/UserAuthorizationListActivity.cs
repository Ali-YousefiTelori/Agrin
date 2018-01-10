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
using Agrin.Download.Data.Settings;

namespace Agrin.MonoAndroid.UI.Activities.Lists
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class UserAuthorizationListActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);
            this.Title = "UserAuthorizationListTitle_Language";
            // Create your application here
            SetContentView(Resource.Layout.UserAuthorizationList);

            if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.UserAuthorizationList.LinearLayoutReverce1 });
            }

            ViewUtility.SetTextLanguage(this, new List<int>() { Resource.UserAuthorizationList.btnDelete, Resource.UserAuthorizationList.btnAddUserAuthorization });

            var vm = new UserAuthorizationListViewModel(this, Resource.Layout.GroupListItem, ApplicationSetting.Current.UserAccountsSetting.Items);
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            ActivitesManager.ToolbarActive(null);
            base.Finish();
        }
    }
}