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
using Agrin.MonoAndroid.UI.ViewModels.Toolbox;

namespace Agrin.MonoAndroid.UI.Activities.Toolbox
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class VIPToolbarActivity : Activity
    {
        VIPToolbarViewModel toolbar = null;
        //public static ToolbarActivity This;
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            //This = this;
            base.OnCreate(bundle);
            // Create your application here
            SetContentView(Resource.Layout.VIPToolbar);
            this.Title = "VIPAccount_Language";

            if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.VIPToolbar.ReverceLinearLayout1, Resource.VIPToolbar.ReverceLinearLayout2 });
            }

            ViewUtility.SetTextLanguage(this, new List<int>() { Resource.VIPToolbar.btn_BuyStorage, Resource.VIPToolbar.btn_Downloads, Resource.VIPToolbar.btn_Login, Resource.VIPToolbar.btn_Logout, Resource.VIPToolbar.btn_Register, Resource.VIPToolbar.btn_UserPermission, Resource.VIPToolbar.txtSizeTitle, Resource.VIPToolbar.txtUserNameTitle });
            //, Resource.Toolbars.btn_QueueManager

            toolbar = new VIPToolbarViewModel(this);
            toolbar.Initialize();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            // Ask the open service connection's billing handler to process this request
            toolbar.buyProductHelper._serviceConnection.BillingHandler.HandleActivityResult(requestCode, resultCode, data);
            if (toolbar.buyProductHelper.ChangedItems != null)
                toolbar.buyProductHelper.ChangedItems(requestCode, resultCode);
        }

        public override void Finish()
        {
            toolbar.buyProductHelper.Disconnect();
            ActivitesManager.RemoveActivity(this);
            ActivitesManager.ToolbarActive(null);
            base.Finish();
        }
    }
}