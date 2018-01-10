using System;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkInfoDownloadSetting : Gtk.Bin
	{
		public static LinkInfoDownloadSetting This;
		public LinkInfoDownloadSetting()
		{
			This = this;
			this.Build();
			chkExtreme.Direction = chkEndDownload.Direction = Gtk.TextDirection.Rtl;
			InitControl();
		}

		public Gtk.CheckButton ChkEndDownload
		{
			get
			{
				return chkEndDownload;
			}
		}

		public Gtk.CheckButton ChkExtreme
		{
			get
			{
				return chkExtreme;
			}
		}

		public Gtk.SpinButton TxtErrorCount
		{
			get
			{
				return txtErrorCount;
			}
		}

		public Gtk.ComboBox CboSystemEnd
		{
			get
			{
				return cboSystemEnd;
			}
		}

		public void InitControl()
		{
			cboSystemEnd.Sensitive = chkEndDownload.Active;
			txtErrorCount.Sensitive = !chkExtreme.Active;
		}

		protected void OnChkEndDownloadToggled (object sender, EventArgs e)
		{
			InitControl();
		}

		protected void OnChkExtremeToggled (object sender, EventArgs e)
		{
			InitControl();
		}
	}
}

