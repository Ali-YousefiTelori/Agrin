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
using System.ComponentModel;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.IO.Helper;
using System.Reflection;
using Agrin.Download.Data.Settings;
using Agrin.Download.Web.Link;
using Agrin.MonoAndroid.UI.Activities;
using Agrin.MonoAndroid.UI.ViewModels.Lists;

namespace Agrin.MonoAndroid.UI
{
    public class AddLinkViewModel : IBaseViewModel, INotifyPropertyChanged
    {
        public Activity CurrentActivity { get; set; }

        public AddLinkViewModel(Activity activity)
        {
            CurrentActivity = activity;
        }
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        Button btnAdd, btnPlay, btnCancel, btnUploadToServer = null;
        Spinner spinner = null;
        EditText txt_Address = null;
        public void Initialize()
        {
            btnAdd = CurrentActivity.FindViewById<Button>(Resource.AddLinks.btnAdd);
            btnPlay = CurrentActivity.FindViewById<Button>(Resource.AddLinks.btnPlay);
            btnAdd.Click += btnAddClick;
            btnPlay.Click += btnPlayClick;
            Button button = CurrentActivity.FindViewById<Button>(Resource.AddLinks.btnCancel);
            btnCancel = button;
            button.Click += btnCancelClick;

            Button btnAddUserAuthorization = CurrentActivity.FindViewById<Button>(Resource.AddLinks.btnAddUserAuthorization);
            btnAddUserAuthorization.Click += btnAddUserAuthorization_Click;

            btnUploadToServer = CurrentActivity.FindViewById<Button>(Resource.AddLinks.btnUploadToServer);
            btnUploadToServer.Click += btnUploadToServer_Click;
            btnUploadToServer.Visibility = (!string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.UserName) && !string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.Password)) ? ViewStates.Visible : ViewStates.Gone;

            button = CurrentActivity.FindViewById<Button>(Resource.AddLinks.btnBrowse);
            button.Click += btnBrowse_Click;

            var btn_Extract = CurrentActivity.FindViewById<Button>(Resource.AddLinks.btn_Extract);
            btn_Extract.Click += btnExtractClick;

            spinner = CurrentActivity.FindViewById<Spinner>(Resource.AddLinks.cboGroups);

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);

            var list = ApplicationGroupManager.Current.GroupInfoes.ToList();
            list.Insert(0, ApplicationGroupManager.Current.NoGroup);
            List<string> items = new List<string>();
            foreach (var item in list)
            {
                items.Add(item.Name);
            }
            var adapter = new ArrayAdapter<String>(CurrentActivity, Android.Resource.Layout.SimpleSpinnerItem, items);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //			var adapter = new CustomAdapter<GroupInfo> (Resource.ComboBoxItem.txtDisplayText,list , (pos) => {
            //				return pos;
            //			}, (position, convertView, parent,adp,_templateResourceId) => {
            //				View view = convertView;
            //				GroupInfo item = adp [position];
            //				if (view == null || !(view is LinearLayout))
            //					view = CurrentActivity.LayoutInflater.Inflate (Resource.ComboBoxItem.txtDisplayText, parent, false);
            //				TextView txtDisplayText = view.FindViewById<TextView> (Resource.ComboBoxItem.txtDisplayText);
            //				txtDisplayText.Text = item.Name;
            //				return view;
            //			});

            spinner.Adapter = adapter;
            txt_Address = CurrentActivity.FindViewById<EditText>(Resource.AddLinks.txt_Address);
            EditText txt_SavePath = CurrentActivity.FindViewById<EditText>(Resource.AddLinks.txt_SavePath);

            BindingHelper.AddBindingOneWay(this, txt_Address, "Text", this, "UriAddress");
            BindingHelper.AddBindingTwoWay(this, this, "SaveToPath", txt_SavePath, "Text");
            BindingHelper.AddActionPropertyChanged((x) => CanAddLink(), this, new List<string>() {
				"UriAddress",
				"SaveToPath"
			});
            //#if(DEBUG)
            //            //txt_Address.Text = "http://s12.p30download.com/users/212/software/network-internet/download-manager/Internet.Download.Manager.v6.21.Build.15_p30download.com.rar";
            //#else
            //#endif
            txt_Address.Text = AddYoutubeLinkViewModel.CurrentSelectedURL;

            if (!string.IsNullOrEmpty(AddYoutubeLinkViewModel.CurrentSelectedURL))
                btnExtractClick(null, null);
            AddYoutubeLinkViewModel.CurrentSelectedURL = "";
            if (string.IsNullOrEmpty(txt_Address.Text))
            {
                Android.Text.ClipboardManager clipboardManager = (Android.Text.ClipboardManager)CurrentActivity.GetSystemService(Context.ClipboardService);
                string text = clipboardManager.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    txt_Address.Text = Agrin.IO.Strings.HtmlPage.ExtractFirstLinkFromHtml(text, 1);
                }
            }
            CanAddLink();

            //            button.Click += (sender, e) =>
            //            {
            //                //var clipboard = (Android.Text.ClipboardManager)MainActivity.This.GetSystemService(MainActivity.This.ClipboardService);
            //
            //                //if (clipboard.HasText)
            //                //	txt_Address.Text = clipboard.Text;
            //            };
        }

        void btnUploadToServer_Click(object sender, EventArgs e)
        {
            ViewUtility.ShowQuestionMessageDialog(CurrentActivity, "QuestionServerFileUpload_Language", "SendFile_Language", () =>
                {
                    Agrin.MonoAndroid.UI.ViewModels.Toolbox.VIPToolbarViewModel.UploadLinkToServer(CurrentActivity, () =>
                    {
                        FramesoftLinksListDataViewModel.MustRefresh = true;
                        ActivitesManager.VIPLinksActive(CurrentActivity);
                    }, txt_Address.Text);
                });
        }

        void btnAddUserAuthorization_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this.CurrentActivity);
            builder.SetTitle(ViewUtility.FindTextLanguage(CurrentActivity, "AddUserAuthorization_Language"));
            LinearLayout layout = new LinearLayout(CurrentActivity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            EditText txtUserName = new EditText(this.CurrentActivity);
            txtUserName.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "UserName_Language");

            EditText txtPassword = new EditText(this.CurrentActivity) { InputType = Android.Text.InputTypes.TextVariationPassword };
            txtPassword.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "Password_Language");
            if (CurrentNetworkCredentialInfo != null)
            {
                txtUserName.Text = CurrentNetworkCredentialInfo.UserName;
                txtPassword.Text = CurrentNetworkCredentialInfo.Password;
            }
            layout.AddView(txtUserName);
            layout.AddView(txtPassword);
            builder.SetView(layout);
            // Set up the buttons
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "OK_Language"), (dialog, which) =>
            {
                if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    if (CurrentNetworkCredentialInfo != null)
                    {
                        CurrentNetworkCredentialInfo = null;
                        Toast.MakeText(this.CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "UserAuthorizationDeleted_Language"), ToastLength.Short).Show();
                    }
                    return;
                }
                var user = new NetworkCredentialInfo();
                user.UserName = txtUserName.Text;
                user.Password = txtPassword.Text;
                CurrentNetworkCredentialInfo = user;
                //m_Text = input.Text;
                builder.Dispose();
            });
            builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "Cancel_Language"), (dialog, which) =>
            {
                builder.Dispose();
            });

            builder.Show();

        }

        void btnBrowse_Click(object sender, EventArgs e)
        {
            ActivitesManager.FolderBrowserDialogActive(CurrentActivity, SaveToPath, (path) =>
            {
                SaveToPath = path;
            });
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position == 0)
                SelectedGroup = ApplicationGroupManager.Current.NoGroup;
            else
                SelectedGroup = ApplicationGroupManager.Current.GroupInfoes[e.Position - 1];
            //			string toast = string.Format ("The planet is {0}", spinner.GetItemAtPosition (e.Position));
            //			Toast.MakeText (this, toast, ToastLength.Long).Show ();
        }

        string _UriAddress = "";

        public string UriAddress
        {
            get { return _UriAddress; }
            set
            {

                _UriAddress = value.Trim();
                SelectedGroup = Agrin.Download.Manager.ApplicationGroupManager.Current.FindGroupByFileName(string.IsNullOrEmpty(AddYoutubeLinkViewModel.FileName) ? value : AddYoutubeLinkViewModel.FileName);
                spinner.SetSelection(ApplicationGroupManager.Current.GroupInfoes.IndexOf(SelectedGroup) + 1);
                CurrentNetworkCredentialInfo = AppUserAccountsSetting.FindFromAddress(_UriAddress);
                if (value.ToLower().Contains(Framesoft.Helper.UserManagerHelper.domain))
                    btnUploadToServer.Visibility = ViewStates.Gone;
                else
                    btnUploadToServer.Visibility = (!string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.UserName) && !string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.Password)) ? ViewStates.Visible : ViewStates.Gone;
                OnPropertyChanged("UriAddress");
                //entry2.Text = value;
                //if (CanAddLink())
                //	SelectedGroup = ApplicationGroupManager.Current.FindGroupByFileName(UriAddress);
            }
        }

        private NetworkCredentialInfo _CurrentNetworkCredentialInfo;
        public NetworkCredentialInfo CurrentNetworkCredentialInfo
        {
            get { return _CurrentNetworkCredentialInfo; }
            set { _CurrentNetworkCredentialInfo = value; }
        }

        GroupInfo _SelectedGroup;

        public GroupInfo SelectedGroup
        {
            get { return _SelectedGroup; }
            set
            {
                if (_SelectedGroup != null && value == null)
                    return;
                var group = _SelectedGroup == null ? ApplicationGroupManager.Current.NoGroup : _SelectedGroup;
                bool changedAddress = MPath.EqualPath(group.SavePath, SaveToPath) || !System.IO.Path.IsPathRooted(SaveToPath);
                _SelectedGroup = value;
                if (changedAddress)
                    SaveToPath = _SelectedGroup == null ? group.SavePath : _SelectedGroup.SavePath;
            }
        }

        bool _addedLink = false;

        bool CanAddLink()
        {
            if (_addedLink)
                btnCancel.Enabled = false;
            Uri uri = null;
            try
            {
                btnUploadToServer.Enabled = btnAdd.Enabled = btnPlay.Enabled = !_addedLink && Uri.TryCreate(UriAddress, UriKind.Absolute, out uri) && System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(SaveToPath));
            }
            catch
            {
                btnUploadToServer.Enabled = btnAdd.Enabled = btnPlay.Enabled = false;
            }
            return btnAdd.Enabled;
        }

        public List<GroupInfo> Groups
        {
            get
            {
                return ApplicationGroupManager.Current.GroupInfoes.ToList();
            }
        }

        string _SaveToPath;

        public string SaveToPath
        {
            get { return _SaveToPath; }
            set
            {
                if (value == _SaveToPath)
                    return;
                _SaveToPath = value;
                OnPropertyChanged("SaveToPath");
                //if (SelectedGroup == null)
                //    ApplicationGroupManager.Current.NoGroup.SavePath = value;
            }
        }
        //		void ShowLinksListData ()
        //		{
        //			LinksListDataViewModel linksListdata = new LinksListDataViewModel (MainActivity.This, Resource.Layout.LinksListItem, ApplicationLinkInfoManager.Current.LinkInfoes.ToList ());
        //			linksListdata.Initialize ();
        //		}



        void btnExtractClick(object sender, EventArgs e)
        {
            var list = Agrin.IO.Strings.HtmlPage.ExtractLinksFromHtml(UriAddress);
            if (list.Count > 1)
            {
                (CurrentActivity as AddLinkActivity).ShowListDialog(list.ToArray(), (value) =>
                {
                    txt_Address.Text = value;
                });
            }
            else
                txt_Address.Text = list.FirstOrDefault();
        }

        bool mustSave = false;
        LinkInfo AddLink()
        {
            if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(UriAddress) && AddYoutubeLinkViewModel.CurrentSelectedFormatCode == -1)
            {
                Toast.MakeText(CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "NoVideoFormatFound_Language"), ToastLength.Long).Show();
                return null;
            }
            _addedLink = true;
            CanAddLink();
            LinkInfo linkInfo = new LinkInfo(UriAddress);
            linkInfo.Management.NetworkUserPass = CurrentNetworkCredentialInfo;
            if (!string.IsNullOrEmpty(AddYoutubeLinkViewModel.FileName))
            {
                linkInfo.PathInfo.UserFileName = AddYoutubeLinkViewModel.FileName;
                AddYoutubeLinkViewModel.FileName = "";
            }
            if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(UriAddress) && AddYoutubeLinkViewModel.CurrentSelectedFormatCode != -1)
            {
                mustSave = true;
                linkInfo.Management.SharingSettings.Add(AddYoutubeLinkViewModel.CurrentSelectedFormatCode);
                AddYoutubeLinkViewModel.CurrentSelectedFormatCode = -1;
            }
            if (!string.IsNullOrEmpty(SaveToPath))
            {
                if (SelectedGroup != null)
                {
                    if (!Agrin.IO.Helper.MPath.EqualPath(SelectedGroup.SavePath, SaveToPath))
                    {
                        linkInfo.PathInfo.UserSavePath = SaveToPath;
                    }
                }
                else
                    linkInfo.PathInfo.UserSavePath = SaveToPath;
            }
            return linkInfo;
        }
        void btnAddClick(object sender, EventArgs e)
        {
            var linkInfo = AddLink();
            if (linkInfo == null)
                return;
            ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, SelectedGroup, false);
            if (mustSave)
            {
                linkInfo.SaveThisLink();
                mustSave = false;
            }

            ActivitesManager.DownloadListActive(CurrentActivity);
        }
        void btnPlayClick(object sender, EventArgs e)
        {
            var linkInfo = AddLink();
            if (linkInfo == null)
                return;
            ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, SelectedGroup, true);

            if (mustSave)
            {
                linkInfo.SaveThisLink();
                mustSave = false;
            }
            ActivitesManager.DownloadListActive(CurrentActivity);
        }

        void btnCancelClick(object sender, EventArgs e)
        {
            ActivitesManager.ToolbarActive(CurrentActivity);
            AddYoutubeLinkViewModel.FileName = "";
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

