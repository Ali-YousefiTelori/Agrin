using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Agrin.Download.Manager;
using Agrin.Download.Web;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class AddGroup : Gtk.Bin,INotifyPropertyChanged
	{
		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
		public Action DialogResult;
		Guid bindingGuid=Guid.NewGuid();
		public AddGroup ()
		{
			this.Build ();
			entry1.Direction = Gtk.TextDirection.Rtl;
			BindingHelper.AddBindingOneWay (this,entry1, "Text", this, "GroupName");
			BindingHelper.AddBindingTwoWay (this, this, "SaveToPath", entry3, "Text");
			BindingHelper.AddActionPropertyChanged (() => CanAddGroup (), this, new List<string> (){ "GroupName","SaveToPath" },bindingGuid);

			//comboBox = agrincombobox1;
			CanAddGroup ();
		}

		string _SaveToPath;
		public string SaveToPath
		{
			get { return _SaveToPath; }
			set { _SaveToPath = value; OnPropertyChanged("SaveToPath"); }
		}

		string _GroupName;
		public string GroupName
		{
			get { return _GroupName; }
			set
			{
				_GroupName = value;
				OnPropertyChanged("GroupName");
			}
		}

		bool CanAddGroup()
		{
			try {
				btnAdd.Sensitive = !string.IsNullOrWhiteSpace (GroupName) && System.IO.Directory.Exists (System.IO.Path.GetDirectoryName (SaveToPath));
			} catch {
				btnAdd.Sensitive = false;
			}
			return btnAdd.Sensitive;
		}

		protected void OnButton16Clicked (object sender, EventArgs e)
		{
			Gtk.FileChooserDialog dialog = new Gtk.FileChooserDialog ("انتخاب مسیر", MainWindow.This,Gtk.FileChooserAction.SelectFolder,"انصراف",Gtk.ResponseType.Cancel,"انتخاب",Gtk.ResponseType.Accept);
			dialog.SetCurrentFolderUri( Agrin.IO.Helper.MPath.DownloadsPath);
			//dialog.SelectFilename (CanAddLink () ? System.IO.Path.GetFileName (UriAddress) : "");
			//dialog.SelectUri (SaveToPath);
			var result = dialog.Run ();
			if (result == -3)
			{
				SaveToPath = dialog.Filename;
			}
			dialog.Destroy ();
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			List<string> extentions = new List<string>();
			foreach (var item in entry2.Text.ToLower().Trim().Split(new char[] { ',' }))
			{
				if (!extentions.Contains(item))
					extentions.Add(item.Trim());
			}
			GroupInfo group = new GroupInfo () { Name = GroupName, SavePath = SaveToPath, Extentions = extentions };
			GroupsList.This.AddGroupInfo (group);
			DialogResult ();
		}

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public override void Dispose()
		{
			BindingHelper.DisposeBindingOneWay(this);
			BindingHelper.DisposeBindingAction (bindingGuid);

			base.Dispose();
		}
	}
}

