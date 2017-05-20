using System;

namespace Agrin.Mono.UI
{
	public partial class AboutDialog : Gtk.Dialog
	{
		public AboutDialog ()
		{
			this.Build ();
		}

		protected void OnLabel9ButtonReleaseEvent (object o, Gtk.ButtonReleaseEventArgs args)
		{
			System.Diagnostics.Process.Start("http://framesoft.ir");
		}
	}
}

