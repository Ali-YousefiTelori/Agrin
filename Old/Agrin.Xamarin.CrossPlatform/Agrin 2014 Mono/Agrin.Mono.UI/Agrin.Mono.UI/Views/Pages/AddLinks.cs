using System;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.IO.Helper;
using System.Collections.Generic;
using System.ComponentModel;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class AddLinks : Gtk.Bin,INotifyPropertyChanged
	{
		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public Action DialogResult;
		//public static Gdk.Pixbuf GetIcon(string filename, int size) 
		//{
		//	string mimeType = Gnome.Vfs.Mime.TypeFromName(filename); 
		//
		//	Gnome.IconLookupResultFlags result; 
		//	string iconName = Gnome.Icon.Lookup(IconTheme.Default, null, null, 
		//	                                    null, null, mimeType, Gnome.IconLookupFlags.None, out result); 

		//	if (IconTheme.Default.HasIcon(iconName)) 
		//	{ 
		//		return IconTheme.Default.LoadIcon(iconName, size, 
		//		                                  IconLookupFlags.UseBuiltin); 
		//	} 
		//	return null; 
		//} 
		Guid bindingGuid=Guid.NewGuid();
		public AddLinks ()
		{

			this.Build ();
			//label1.ModifyFont (Pango.FontDescription.FromString ("arial"));
			combobox3.Direction = entry3.Direction = Gtk.TextDirection.Rtl;
			BindingHelper.AddBindingOneWay (this,entry1, "Text", this, "UriAddress");
			BindingHelper.AddBindingTwoWay (this, this, "SaveToPath", entry3, "Text");
			BindingHelper.AddActionPropertyChanged (() => CanAddLink (), this, new List<string> (){ "UriAddress","SaveToPath" },bindingGuid);
			//comboBox = agrincombobox1;
			CanAddLink ();
			combobox3.Changed += (sender, e) => {
				SelectedGroup=ApplicationGroupManager.Current.FindGroupByName(combobox3.ActiveText);
			};
			foreach (var item in ApplicationGroupManager.Current.GroupInfoes) {
				combobox3.AppendText (item.Name);
			}
		}

		//public	AgrinComboBox comboBox;
		string _UriAddress;
		public string UriAddress
		{
			get { return _UriAddress; }
			set
			{
				SelectedGroup = SelectedGroup;
				_UriAddress = value;
				OnPropertyChanged ("UriAddress");
				//entry2.Text = value;
				//if (CanAddLink())
				//	SelectedGroup = ApplicationGroupManager.Current.FindGroupByFileName(UriAddress);
			}
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
				_SaveToPath = value;
				OnPropertyChanged ("SaveToPath");
				if (SelectedGroup == null)
					ApplicationGroupManager.Current.NoGroup.SavePath = value;
			}
		}

		protected void OnButton16Clicked (object sender, EventArgs e)
		{
			//Gtk.FileChooserDialog dialog = new Gtk.FileChooserDialog ("ذخیره فایل", MainWindow.This,Gtk.FileChooserAction.Save,"انصراف",Gtk.ResponseType.Cancel,"انتخاب",Gtk.ResponseType.Accept);
			Gtk.FileChooserDialog dialog = new Gtk.FileChooserDialog ("محل ذخیره", MainWindow.This,Gtk.FileChooserAction.SelectFolder,"انصراف",Gtk.ResponseType.Cancel,"انتخاب",Gtk.ResponseType.Accept);
			//dialog.CurrentName = System.IO.Path.GetFileName (UriAddress);
			var result = dialog.Run ();
			if (result == -3)
			{
				SaveToPath = dialog.CurrentFolder;
			}
			dialog.Destroy ();
		}

		bool CanAddLink()
		{
			Uri uri = null;
			try {
				btnAdd.Sensitive = btnPlay.Sensitive = Uri.TryCreate (UriAddress, UriKind.Absolute, out uri) && System.IO.Directory.Exists (System.IO.Path.GetDirectoryName (SaveToPath));
			} catch {
				btnAdd.Sensitive = btnPlay.Sensitive = false;
			}
			return btnAdd.Sensitive;
		}

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			LinksList.This.AddLinkInfo (UriAddress, SelectedGroup, false);
			DialogResult ();
		}

		protected void OnBtnPlayClicked (object sender, EventArgs e)
		{
			LinksList.This.AddLinkInfo (UriAddress, SelectedGroup, true);
			DialogResult ();
		}

		public override void Dispose()
		{
			BindingHelper.DisposeBindingOneWay (this);
			BindingHelper.DisposeBindingAction (bindingGuid);
			base.Dispose();
		}
	}
}

