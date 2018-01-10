using System;
using System.ComponentModel;
using Agrin.Download.Web;
using Agrin.Download.Manager;
using System.Collections.Generic;
using Gtk;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class AddLinks : Gtk.Bin
	{
		Agrin.BaseViewModels.Link.AddLinksBaseViewModel AddLinksBVM{ get; set; }

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public System.Action DialogResult;

		public AddLinks ()
		{
			AddLinksBVM = new Agrin.BaseViewModels.Link.AddLinksBaseViewModel ();
			this.Build ();
			//label1.ModifyFont (Pango.FontDescription.FromString ("arial"));
			combobox3.Direction = entry3.Direction = Gtk.TextDirection.Rtl;

			BindingHelper.BindTwoway (this, entry1, "Text", AddLinksBVM, "UriAddress");
			BindingHelper.BindTwoway (this, AddLinksBVM, "SaveToPath", entry3, "Text");
			BindingHelper.BindAction (this, AddLinksBVM, new List<string> (){ "UriAddress", "SaveToPath" }, (name) => {
				if (AddLinksBVM.SelectedGroup == null || combobox3.ActiveText != AddLinksBVM.SelectedGroup.Name) {
					var index = AddLinksBVM.Groups.IndexOf (AddLinksBVM.SelectedGroup);
					combobox3.Active = index;
				}
				RefreshButtons ();
			});

			combobox3.Changed += (sender, e) => {
				AddLinksBVM.SelectedGroup = ApplicationGroupManager.Current.FindGroupByName (combobox3.ActiveText);
			};
			foreach (var item in AddLinksBVM.Groups) {
				combobox3.AppendText (item.Name);
			}
			InitializeButtonImages ();
			RefreshButtons ();
		}

		public void InitializeButtonImages ()
		{
			SetButtonImage (btnAdd, 20, 20, "Agrin.Mono.UI.icons.add.png");
			SetButtonImage (btnPlay, 20, 20, "Agrin.Mono.UI.icons.play.png");
			SetButtonImage (btnsave, 20, 20, "Agrin.Mono.UI.icons.save.png");
		}

		public void RefreshButtons ()
		{
			btnAdd.Sensitive = AddLinksBVM.CanAddLink ();
			btnPlay.Sensitive = AddLinksBVM.CanAddLink ();
		}

		public void SetButtonImage (Button btn, int width, int height, string imageSource)
		{
			var pb = Gdk.PixbufLoader.LoadFromResource (imageSource).Pixbuf;
			Image img = new Image ();
			var srcWidth = width;
			var srcHeight = height;
			int resultWidth, resultHeight;
			ScaleRatio (srcWidth, srcHeight, width, height, out resultWidth, out resultHeight);
			img.Pixbuf = pb.ScaleSimple (resultWidth, resultHeight, Gdk.InterpType.Bilinear);
			//btnAddLink.b
			//img.Pixbuf=img;//new Gdk.Pixbuf (pb, 0, 0, 24, 24);
			btn.Image = img;
		}

		private static void ScaleRatio (int srcWidth, int srcHeight, int destWidth, int destHeight, out int resultWidth, out int resultHeight)
		{
			var widthRatio = (float)destWidth / srcWidth;
			var heigthRatio = (float)destHeight / srcHeight;
		
			var ratio = Math.Min (widthRatio, heigthRatio);
			resultHeight = (int)(srcHeight * ratio);
			resultWidth = (int)(srcWidth * ratio);
		}

		protected void OnButton16Clicked (object sender, EventArgs e)
		{
			//Gtk.FileChooserDialog dialog = new Gtk.FileChooserDialog ("ذخیره فایل", MainWindow.This,Gtk.FileChooserAction.Save,"انصراف",Gtk.ResponseType.Cancel,"انتخاب",Gtk.ResponseType.Accept);
			Gtk.FileChooserDialog dialog = new Gtk.FileChooserDialog ("محل ذخیره", MainWindow.This, Gtk.FileChooserAction.SelectFolder, "انصراف", Gtk.ResponseType.Cancel, "انتخاب", Gtk.ResponseType.Accept);
			//dialog.CurrentName = System.IO.Path.GetFileName (UriAddress);
			var result = dialog.Run ();
			if (result == -3) {
				AddLinksBVM.SaveToPath = dialog.CurrentFolder;
			}
			dialog.Destroy ();
		}

		public void OnPropertyChanged (string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			AddLinksBVM.AddLink ();
			DialogResult ();
			foreach (var linkInfo in AddLinksBVM.AddedLinks) {
				LinksList.This.AddBinding (LinksList.store.InsertWithValues (0, linkInfo), linkInfo);
			}
			LinksList.This.ScrollToZero ();
		}

		protected void OnBtnPlayClicked (object sender, EventArgs e)
		{
			AddLinksBVM.AddLinkAndPlay ();
			DialogResult ();
			foreach (var linkInfo in AddLinksBVM.AddedLinks) {
				LinksList.This.AddBinding (LinksList.store.InsertWithValues (0, linkInfo), linkInfo);
			}
			LinksList.This.ScrollToZero ();
		}

		public override void Dispose ()
		{
			BindingHelper.DisposeObject (this);
			base.Dispose ();
		}
	}
}

