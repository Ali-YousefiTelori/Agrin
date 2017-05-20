using System;
using System.ComponentModel;
using Android.App;
using Android.Widget;
using System.Collections.Generic;
using Agrin.Download.Manager;
using Agrin.Download.Web;

namespace Agrin.MonoAndroid.UI
{
	public class AddGroupViewModel:IBaseViewModel, INotifyPropertyChanged
	{
		public	Activity CurrentActivity{ get; set; }

		public AddGroupViewModel (Activity activity)
		{
			CurrentActivity = activity;
		}

		bool _addedGroup = false;

		bool CanAddLink ()
		{
			btnAdd.Enabled = !_addedGroup && !string.IsNullOrWhiteSpace (GroupName);
			return btnAdd.Enabled;
		}

		Button btnAdd = null ;
		public void Initialize ()
		{
		    btnAdd = CurrentActivity.FindViewById<Button> (Resource.AddGroups.btnAdd);
			btnAdd.Click += btnAddClick;

			var btnCancel = CurrentActivity.FindViewById<Button> (Resource.AddGroups.btnCancel);
			btnCancel.Click += btnCancelClick;

            var btnBrowse = CurrentActivity.FindViewById<Button>(Resource.AddGroups.btnBrowse);
            btnBrowse.Click += btnBrowse_Click;

			var txtName = CurrentActivity.FindViewById<EditText> (Resource.AddGroups.txt_Name);
			var txtSave = CurrentActivity.FindViewById<EditText> (Resource.AddGroups.txt_SavePath);
			var txtExtentions = CurrentActivity.FindViewById<EditText> (Resource.AddGroups.txt_Extentions);

			BindingHelper.AddBindingOneWay (this, txtName, "Text", this, "GroupName");
			BindingHelper.AddBindingTwoWay (this, txtSave, "Text", this, "SaveAddress");
			BindingHelper.AddBindingOneWay (this, txtExtentions, "Text", this, "Extentions");
			BindingHelper.AddActionPropertyChanged ((x) => CanAddLink (), this, new List<string> () {
				"GroupName",
				"SaveAddress"
			});

			GroupName = "";
			CanAddLink ();
		}

        void btnBrowse_Click(object sender, EventArgs e)
        {
            ActivitesManager.FolderBrowserDialogActive(CurrentActivity, SaveAddress, (path) =>
            {
                SaveAddress = path;
            });
        }

		string _groupName;

		public string GroupName {
			get {
				return _groupName;
			}
			set {
				_groupName = value;
				OnPropertyChanged ("GroupName");
				SaveAddress = String.IsNullOrWhiteSpace (value) ? Agrin.IO.Helper.MPath.DownloadsPath : System.IO.Path.Combine (Agrin.IO.Helper.MPath.DownloadsPath, value);
			}
		}

		string _SaveAddress;

		public string SaveAddress {
			get {
				return _SaveAddress;
			}
			set {
				_SaveAddress = value;
				OnPropertyChanged ("SaveAddress");
			}
		}

		string _Extentions = "";

		public string Extentions {
			get {
				return _Extentions;
			}
			set {
				_Extentions = value;
				OnPropertyChanged ("Extentions");
			}
		}

		void Clear()
		{
			GroupName = "";
			Extentions = "";
			btnCancelClick (null, null);
		}

		void AddGroup()
		{
			List<string> extentions = new List<string>();
			foreach (var item in Extentions.ToLower().Trim().Split(new char[] { ',' }))
			{
				if (!extentions.Contains(item))
					extentions.Add(item.Trim());
			}
			ApplicationGroupManager.Current.AddGroupInfo(new GroupInfo() { Name = GroupName, UserSavePath = SaveAddress, Extentions = extentions });
			Clear();
		}
		
		void btnAddClick (object sender, EventArgs e)
		{
			AddGroup ();
		}

		void btnCancelClick (object sender, EventArgs e)
		{
			ActivitesManager.GroupManagerListActive (CurrentActivity);
		}

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public void OnPropertyChanged (string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}
	}
}