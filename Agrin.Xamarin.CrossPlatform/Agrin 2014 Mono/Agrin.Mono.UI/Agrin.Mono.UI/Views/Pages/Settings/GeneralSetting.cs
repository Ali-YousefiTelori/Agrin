using System;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class GeneralSetting : Gtk.Bin
	{
		public static GeneralSetting This;
		public GeneralSetting()
		{
			This = this;
			this.Build();
			lbl_Size.HeightRequest = txt_FileName.HeightRequest;
		}

		public Gtk.Entry Txt_FileName
		{
			get
			{
				return txt_FileName;
			}
		}

		public Gtk.Entry Txt_Description
		{
			get
			{
				return txt_Description;
			}
		}

		public Gtk.Entry Txt_SavePath
		{
			get
			{
				return txt_SavePath;
			}
		}

		public Gtk.Label Lbl_Size
		{
			get
			{
				return lbl_Size;
			}
		}

		protected void OnBtnPathClicked (object sender, EventArgs e)
		{
			//Gtk.FileChooserDialog dialog = new Gtk.FileChooserDialog ("ذخیره فایل", MainWindow.This,Gtk.FileChooserAction.Save,"انصراف",Gtk.ResponseType.Cancel,"انتخاب",Gtk.ResponseType.Accept);
			Gtk.FileChooserDialog dialog = new Gtk.FileChooserDialog ("محل ذخیره", MainWindow.This,Gtk.FileChooserAction.SelectFolder,"انصراف",Gtk.ResponseType.Cancel,"انتخاب",Gtk.ResponseType.Accept);
			//dialog.CurrentName = System.IO.Path.GetFileName (UriAddress);
			var result = dialog.Run ();
			if (result == -3)
			{
				txt_SavePath.Text = dialog.CurrentFolder;
			}
			dialog.Destroy ();
		}
	}
}

