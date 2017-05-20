using Agrin.BaseViewModels.Link;
using Agrin.Download.Manager;
using Agrin.Helpers;
using Agrin.ViewModels.Dialogs;
using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Agrin.Views.Link
{
    public class AddLinks : AddLinksBaseViewModel
    {
        public Activity CurrentActivity { get; set; }

        Button btnCancel = null;
        EditText txt_Address = null;
        LinearLayout mainLayout = null;
        bool _isBrowse = false;
        public AddLinks(Activity currentActivity, Action disposeAction, string extras = null, bool isBrowse = false)
        {
            _isBrowse = isBrowse;
            CurrentActivity = currentActivity;
            var viewObj = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.AddLinks, null);

            if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewsUtility.SetRightToLeftLayout(viewObj, new List<int>() { Resource.AddLinks.LinearLayoutReverce1, Resource.AddLinks.LinearLayoutReverce2 });
                ViewsUtility.SetRightToLeftViews(viewObj, new List<int>() { Resource.AddLinks.txtAddressTitle, Resource.AddLinks.txtGroupNameTitle, Resource.AddLinks.txtSavePathTitle, Resource.AddLinks.LinearLayoutRightToLeft, Resource.AddLinks.LinearLayoutBeforeQualitySelectorRightToLeft, Resource.AddLinks.LinearLayoutAfterQualitySelectorRightToLeft });
            }

            ViewsUtility.SetTextLanguage(CurrentActivity, viewObj, new List<int>() { Resource.AddLinks.txtAddressTitle, Resource.AddLinks.txtGroupNameTitle, Resource.AddLinks.txtSavePathTitle, Resource.AddLinks.btn_Extract, Resource.AddLinks.btnAdd, Resource.AddLinks.btnCancel, Resource.AddLinks.btnPlay, Resource.AddLinks.btnBrowse, Resource.AddLinks.btnAddUserAuthorization, Resource.AddLinks.btnUploadToServer, Resource.AddLinks.btnRefreshQuelity, Resource.AddLinks.btnSelectQuality, Resource.AddLinks.txtQualityTitle });

            btnCancel = viewObj.FindViewById<Button>(Resource.AddLinks.btnCancel);
            mainLayout = ViewsUtility.ShowControlDialog(CurrentActivity, viewObj, "AddLink_Language", () =>
            {
                BindingHelper.DisposeObject(this);
                disposeAction();
            }, btnCancel).Layout;


            txt_Address = viewObj.FindViewById<EditText>(Resource.AddLinks.txt_Address);
            var txt_SavePath = viewObj.FindViewById<EditText>(Resource.AddLinks.txt_SavePath);

            var btnAdd = viewObj.FindViewById<Button>(Resource.AddLinks.btnAdd);
            var btnPlay = viewObj.FindViewById<Button>(Resource.AddLinks.btnPlay);
            var btn_Extract = viewObj.FindViewById<Button>(Resource.AddLinks.btn_Extract);
            var btnBrowse = viewObj.FindViewById<Button>(Resource.AddLinks.btnBrowse);
            var btnAddUserAuthorization = viewObj.FindViewById<Button>(Resource.AddLinks.btnAddUserAuthorization);
            var btnSelectQuality = viewObj.FindViewById<Button>(Resource.AddLinks.btnSelectQuality);
            var btnRefreshQuelity = viewObj.FindViewById<Button>(Resource.AddLinks.btnRefreshQuelity);

            var list = ApplicationGroupManager.Current.GroupInfoes.ToList();
            list.Insert(0, ApplicationGroupManager.Current.NoGroup);
            List<string> items = new List<string>();
            foreach (var item in list)
            {
                items.Add(item.Name);
            }
            var spinner = viewObj.FindViewById<Spinner>(Resource.AddLinks.cboGroups);
            var adapter = new ArrayAdapter<String>(CurrentActivity, Android.Resource.Layout.SimpleSpinnerItem, items);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            btnAdd.Click += btnAdd_Click;
            btnPlay.Click += btnPlay_Click;
            btn_Extract.Click += btnExtractClick;
            btnBrowse.Click += btnBrowse_Click;
            btnAddUserAuthorization.Click += btnAddUserAuthorization_Click;
            btnSelectQuality.Click += btnSelectQuality_Click;
            btnRefreshQuelity.Click += btnRefreshQuelity_Click;
            btnSelectQuality.Enabled = false;

            var spinnerQuality = viewObj.FindViewById<Spinner>(Resource.AddLinks.cboQualities);

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            spinnerQuality.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerQuality_ItemSelected);


            BindingHelper.BindTwoway(this, txt_Address, "Text", this, "UriAddress");
            BindingHelper.BindTwoway(this, txt_SavePath, "Text", this, "SaveToPath");
            BindingHelper.BindAction(this, this, new List<string>() { "SelectedGroup" }, (i) =>
            {
                spinner.SetSelection(ApplicationGroupManager.Current.GroupInfoes.IndexOf(SelectedGroup) + 1);
            });

            BindingHelper.BindAction(this, this, new List<string>() { "IsVideoSharing" }, (property) =>
            {
                CurrentActivity.RunOnUI(() =>
                {
                    CheckVideSharing(viewObj);
                });
            });
            BindingHelper.BindAction(this, this, new List<string>() { "SharingLinksSelectedIndex" }, (property) =>
            {
                CurrentActivity.RunOnUI(() =>
                {
                    if (SharingLinksSelectedIndex >= 0)
                        btnSelectQuality.Enabled = true;
                    else
                        btnSelectQuality.Enabled = false;
                });
            });
            BindingHelper.BindAction(this, this, new List<string>() { "IsBusy" }, (property) =>
            {
                try
                {
                    CurrentActivity.RunOnUI(() =>
                    {
                        ProgressBar progressBar = viewObj.FindViewById<ProgressBar>(Resource.AddLinks.busyProgressBar);
                        var linearLayoutRightToLeft = viewObj.FindViewById<LinearLayout>(Resource.AddLinks.LinearLayoutRightToLeft);
                        var linearLayoutBusy = viewObj.FindViewById<LinearLayout>(Resource.AddLinks.LinearLayoutBusy);
                        if (IsBusy)
                        {
                            var animation = Xamarin.NineOldAndroids.Animations.ObjectAnimator.OfInt(progressBar, "progress", 0, 500); // see this max value coming back here, we animale towards that value
                            animation.SetDuration(50000); //in milliseconds
                            animation.SetInterpolator(new Android.Views.Animations.DecelerateInterpolator());
                            animation.Start();
                            linearLayoutRightToLeft.Visibility = ViewStates.Gone;
                            linearLayoutBusy.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            progressBar.ClearAnimation();
                            linearLayoutRightToLeft.Visibility = ViewStates.Visible;
                            linearLayoutBusy.Visibility = ViewStates.Gone;

                            CheckVideSharing(viewObj);

                            List<string> qualities = new List<string>();
                            foreach (var quality in SharingLinks)
                            {
                                qualities.Add(quality.Text);
                            }

                            var newAdapter = new ArrayAdapter<String>(CurrentActivity, Android.Resource.Layout.SimpleSpinnerItem, qualities);
                            newAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                            spinnerQuality.Adapter = newAdapter;
                            if (qualities.Count > 0)
                            {
                                spinnerQuality.SetSelection(0);
                            }
                            else
                            {
                                btnSelectQuality.Enabled = false;
                            }
                        }
                    });

                }
                catch
                {

                }
            });

            txt_Address.Text = extras;

            if (!string.IsNullOrEmpty(extras))
                btnExtractClick(null, null);

            if (string.IsNullOrEmpty(txt_Address.Text))
            {
                Android.Text.ClipboardManager clipboardManager = (Android.Text.ClipboardManager)InitializeApplication.CurrentContext.GetSystemService(Android.Content.Context.ClipboardService);
                txt_Address.Text = clipboardManager.Text;
                if (!string.IsNullOrEmpty(txt_Address.Text))
                    btnExtractClick(null, null);
            }

            Action initializeCommands = () =>
            {
                btnPlay.Enabled = btnAdd.Enabled = CanAddLink();
            };

            PropertyChanged += (s, e) =>
            {
                initializeCommands();
            };

            initializeCommands();
        }

        private void spinnerQuality_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SharingLinksSelectedIndex = e.Position;
        }

        void btnRefreshQuelity_Click(object sender, EventArgs e)
        {
            RefreshSharingLinks();
        }

        void btnSelectQuality_Click(object sender, EventArgs e)
        {
            IsVideoSharing = false;
        }

        void CheckVideSharing(View viewObj)
        {
            var linearLayoutBeforeQualitySelectorRightToLeft = viewObj.FindViewById<LinearLayout>(Resource.AddLinks.LinearLayoutBeforeQualitySelectorRightToLeft);
            var linearLayoutAfterQualitySelectorRightToLeft = viewObj.FindViewById<LinearLayout>(Resource.AddLinks.LinearLayoutAfterQualitySelectorRightToLeft);
            if (IsVideoSharing)
            {
                linearLayoutBeforeQualitySelectorRightToLeft.Visibility = ViewStates.Visible;
                linearLayoutAfterQualitySelectorRightToLeft.Visibility = ViewStates.Gone;
            }
            else
            {
                linearLayoutBeforeQualitySelectorRightToLeft.Visibility = ViewStates.Gone;
                linearLayoutAfterQualitySelectorRightToLeft.Visibility = ViewStates.Visible;
            }
        }

        void btnAddUserAuthorization_Click(object sender, EventArgs e)
        {
            LinearLayout layout = new LinearLayout(CurrentActivity);
            layout.Orientation = Orientation.Vertical;
            //LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            //layoutParams.SetMargins(5, 5, 5, 5);
            //layout.LayoutParameters = layoutParams;

            EditText txtUserName = new EditText(new ContextThemeWrapper(CurrentActivity, Resource.Style.editText));
            txtUserName.Hint = ViewsUtility.FindTextLanguage(CurrentActivity, "UserName_Language");

            EditText txtPassword = new EditText(new ContextThemeWrapper(CurrentActivity, Resource.Style.editText)) { InputType = Android.Text.InputTypes.TextVariationPassword };
            txtPassword.Hint = ViewsUtility.FindTextLanguage(CurrentActivity, "Password_Language");

            txtUserName.Text = UserName;
            txtPassword.Text = Password;

            layout.AddView(txtUserName);
            layout.AddView(txtPassword);

            ViewsUtility.ShowCustomResultDialog(CurrentActivity, "AddUserAuthorization_Language", layout, () =>
            {
                UserName = txtUserName.Text;
                Password = txtPassword.Text;
            });
        }

        void btnBrowse_Click(object sender, EventArgs e)
        {
            ViewsUtility.ShowFolderBrowseInLayout(CurrentActivity, mainLayout, SaveToPath, (path, isSecurityPath) =>
            {
                if (isSecurityPath)
                    SecurityPath = path;
                else
                {
                    SaveToPath = path;
                    SecurityPath = "";
                }
            }, isBrowse: _isBrowse);
        }

        void btnPlay_Click(object sender, EventArgs e)
        {
            AddLinkAndPlay();
            btnCancel.PerformClick();
        }


        void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AddLink();
                btnCancel.PerformClick();
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex);
            }
        }

        void btnExtractClick(object sender, EventArgs e)
        {
            var list = Agrin.IO.Strings.HtmlPage.ExtractLinksFromHtml(txt_Address.Text);
            //if (list.Count > 1)
            //{
            //    (CurrentActivity as AddYoutubeLinkActivity).ShowListDialog(list.ToArray(), (value) =>
            //    {
            //        txt_Address.Text = value;
            //    });
            //}
            //else
            txt_Address.Text = list.FirstOrDefault();
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position == 0)
                SelectedGroup = ApplicationGroupManager.Current.NoGroup;
            else
                SelectedGroup = ApplicationGroupManager.Current.GroupInfoes[e.Position - 1];
        }
    }
}