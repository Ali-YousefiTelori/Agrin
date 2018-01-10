
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
    public class AddGroupActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.AddGroup);
            this.Title = "AddGroup_Language";
            if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.AddGroups.LinearLayoutReverce1 });
                ViewUtility.SetRightToLeftViews(this, new List<int>() { Resource.AddGroups.txt_AddressTitle, Resource.AddGroups.txt_ExtentionsTitle, Resource.AddGroups.txt_NameTitle, Resource.AddGroups.txt_Name, Resource.AddGroups.LinearLayoutRightToLeft });
            }

            ViewUtility.SetTextLanguage(this, new List<int>() { Resource.AddGroups.txt_AddressTitle, Resource.AddGroups.txt_ExtentionsTitle, Resource.AddGroups.txt_NameTitle, Resource.AddGroups.btnAdd, Resource.AddGroups.btnCancel, Resource.AddGroups.btnBrowse });

            var vm = new AddGroupViewModel(this);
            vm.Initialize();
            // Create your application here
        }
        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            ActivitesManager.GroupManagerListActive(null);
            base.Finish();
        }
    }
}

