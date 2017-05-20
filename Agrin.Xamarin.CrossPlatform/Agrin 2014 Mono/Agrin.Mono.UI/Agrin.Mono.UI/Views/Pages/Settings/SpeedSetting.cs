using System;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SpeedSetting : Gtk.Bin
	{
		public static SpeedSetting This;
		public SpeedSetting()
		{
			This = this;
			this.Build();
			InitControl();
		}

		public Gtk.CheckButton ChkIsLimit
		{
			get
			{
				return chkIsLimit;
			}
		}

		public Gtk.SpinButton TxtSpeed
		{
			get
			{
				return txtSpeed;
			}
		}

		public Gtk.SpinButton TxtBufferSize
		{
			get
			{
				return txtBufferSize;
			}
		}

		public Gtk.SpinButton TxtConnectionCount
		{
			get
			{
				return txtConnectionCount;
			}
		}

		public void InitControl()
		{
			txtSpeed.Sensitive = chkIsLimit.Active;
		}

		protected void OnHbox18SizeAllocated (object o, Gtk.SizeAllocatedArgs args)
		{
			lblText.SetSizeRequest(args.Allocation.Width, args.Allocation.Height);
		}

		protected void OnChkIsLimitToggled (object sender, EventArgs e)
		{
			InitControl();
		}
	}
}

