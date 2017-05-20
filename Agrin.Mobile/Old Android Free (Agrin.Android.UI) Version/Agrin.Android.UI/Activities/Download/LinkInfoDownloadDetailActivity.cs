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
using Agrin.MonoAndroid.UI.ViewModels.Download;
using Agrin.Download.Web;
using Agrin.Log;

namespace Agrin.MonoAndroid.UI.Activities.Download
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class LinkInfoDownloadDetailActivity : Activity
    {
        public static LinkInfo CurrentLinkInfo { get; set; }
        LinkInfoDownloadDetailViewModel vm = null;
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                ActivitesManager.AddActivity(this);
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.LinkInfoDownloadDetail);
                this.Title = "LinkDetail_Language";
                if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
                {
                    ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.LinkInfoDownloadDetail.LinearLayoutReverce1, Resource.LinkInfoDownloadDetail.LinearLayoutReverce2, Resource.LinkInfoDownloadDetail.LinearLayoutReverce3, Resource.LinkInfoDownloadDetail.LinearLayoutReverce4, Resource.LinkInfoDownloadDetail.LinearLayoutReverce5, Resource.LinkInfoDownloadDetail.LinearLayoutReverce6, Resource.LinkInfoDownloadDetail.LinearLayoutReverce7 });
                    ViewUtility.SetRightToLeftViews(this, new List<int>() { Resource.LinkInfoDownloadDetail.txtDownloadedSizeTitle, Resource.LinkInfoDownloadDetail.txtResumableTitle, Resource.LinkInfoDownloadDetail.txtSizeTitle, Resource.LinkInfoDownloadDetail.txtSpeedTitle, Resource.LinkInfoDownloadDetail.txtStatusTitle, Resource.LinkInfoDownloadDetail.txtTimeRemainingTitle, Resource.LinkInfoDownloadDetail.txtDownloadedSize, Resource.LinkInfoDownloadDetail.txtResumable, Resource.LinkInfoDownloadDetail.txtSize, Resource.LinkInfoDownloadDetail.txtSpeed, Resource.LinkInfoDownloadDetail.txtStatus, Resource.LinkInfoDownloadDetail.txtTimeRemainig, Resource.LinkInfoDownloadDetail.mainLayout });
                    ViewUtility.SetRightToLeftViews(this, new List<int>() { Resource.LinkInfoDownloadDetail.LinearLayoutRightToLeft1, Resource.LinkInfoDownloadDetail.LinearLayoutRightToLeft2, Resource.LinkInfoDownloadDetail.LinearLayoutRightToLeft3, Resource.LinkInfoDownloadDetail.LinearLayoutRightToLeft4, Resource.LinkInfoDownloadDetail.LinearLayoutRightToLeft5, Resource.LinkInfoDownloadDetail.LinearLayoutRightToLeft7 });
                }

                ViewUtility.SetTextLanguage(this, new List<int>() { Resource.LinkInfoDownloadDetail.txtDownloadedSizeTitle, Resource.LinkInfoDownloadDetail.txtResumableTitle, Resource.LinkInfoDownloadDetail.txtSizeTitle, Resource.LinkInfoDownloadDetail.txtSpeedTitle, Resource.LinkInfoDownloadDetail.txtStatusTitle, Resource.LinkInfoDownloadDetail.txtTimeRemainingTitle, Resource.LinkInfoDownloadDetail.btnPlay, Resource.LinkInfoDownloadDetail.btnReConnect, Resource.LinkInfoDownloadDetail.btnStop, Resource.LinkInfoDownloadDetail.txtPerSecound });

                vm = new LinkInfoDownloadDetailViewModel(this, Resource.Layout.ConnectionInfoListItem, CurrentLinkInfo);
                // Create your application here
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, "TTTTTTTT 11 ",true);
            }
           
        }

        public override void Finish()
        {
            try
            {
                ActivitesManager.RemoveActivity(this);
                vm.DisposeAll();
                if (LinksListDataViewModel.This != null)
                    LinksListDataViewModel.This.ValidateAllButtons();
                base.Finish();
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, "TTTTTTTT 00 ", true);
            }
        }
    }
}