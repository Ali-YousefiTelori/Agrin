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
    public class AddLinkActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.AddLink);
            this.Title = "AddLink_Language";
            if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.AddLinks.LinearLayoutReverce1, Resource.AddLinks.LinearLayoutReverce2 });
                ViewUtility.SetRightToLeftViews(this, new List<int>() { Resource.AddLinks.txtAddressTitle, Resource.AddLinks.txtGroupNameTitle, Resource.AddLinks.txtSavePathTitle, Resource.AddLinks.LinearLayoutRightToLeft });
            }

            ViewUtility.SetTextLanguage(this, new List<int>() { Resource.AddLinks.txtAddressTitle, Resource.AddLinks.txtGroupNameTitle, Resource.AddLinks.txtSavePathTitle, Resource.AddLinks.btn_Extract, Resource.AddLinks.btnAdd, Resource.AddLinks.btnCancel, Resource.AddLinks.btnPlay, Resource.AddLinks.btnBrowse, Resource.AddLinks.btnAddUserAuthorization, Resource.AddLinks.btnUploadToServer });

            AddLinkViewModel addLinks = new AddLinkViewModel(this);
            addLinks.Initialize();
            // Create your application here
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            ActivitesManager.ToolbarActive(null);
            AddYoutubeLinkViewModel.CurrentSelectedFormatCode = -1;
            AddYoutubeLinkViewModel.FileName = ""; 
            base.Finish();
        }

        public void ShowListDialog(string[] items, Action<string> okAction)
        {
            _okAction = okAction;
            _items = items;
            ShowDialog(MultiChoiceDialog);
        }

        private const int MultiChoiceDialog = 3;
        string[] _items = null;
        Action<string> _okAction;

        protected override Dialog OnCreateDialog(int id, Bundle args)
        {
            switch (id)
            {
                case MultiChoiceDialog:
                    {
                        var builder = new AlertDialog.Builder(this);
                        //builder.SetIcon (Resource.Drawable.Icon);
                        builder.SetTitle(ViewUtility.FindTextLanguage(this, "Selectyourlink_Language"));
                        builder.SetSingleChoiceItems(_items, 0, MultiListClicked);

                        builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), OkClicked);
                        builder.SetNegativeButton(ViewUtility.FindTextLanguage(this, "Cancel_Language"), CancelClicked);

                        return builder.Create();
                    }
            }

            return base.OnCreateDialog(id, args);
        }

        private void OkClicked(object sender, DialogClickEventArgs args)
        {
            _okAction(_items[_selected]);
            this.RemoveDialog(MultiChoiceDialog);
        }

        private void CancelClicked(object sender, DialogClickEventArgs args)
        {
            this.RemoveDialog(MultiChoiceDialog);
        }

        int _selected = 0;

        private void MultiListClicked(object sender, DialogClickEventArgs args)
        {
            _selected = args.Which;
        }
    }
}

