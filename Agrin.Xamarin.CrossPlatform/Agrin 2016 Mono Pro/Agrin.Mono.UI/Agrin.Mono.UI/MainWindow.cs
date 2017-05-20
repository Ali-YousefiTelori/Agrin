using System;
using Gtk;

namespace Agrin.Mono.UI
{
	public partial class MainWindow : Gtk.Window
	{
		public static MainWindow This{ get; set; }

		public MainWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			This = this;
			DeleteEvent += delegate {
				Application.Quit ();
			};
			this.Build ();
			menubar1.Direction = TextDirection.Rtl;
			Menu filemenu = new Menu();
			MenuItem file = new MenuItem("فایل");
			file.Submenu = filemenu;
			MenuItem contactUS = new MenuItem("تماس با ما");
			filemenu.Append(contactUS);
			MenuItem aboutUS = new MenuItem("درباره ی ما");
			filemenu.Append(aboutUS);
			MenuItem openSite = new MenuItem("ورود به سایت");
			filemenu.Append(openSite);
			menubar1.Append (file);
			menubar1.ShowAll ();

			aboutUS.Activated += BtnAbout_Clicked;
			openSite.Activated += BtnOpenSite_Clicked;
			contactUS.Activated += BtnReport_Clicked;
		}

		protected void BtnAbout_Clicked (object sender, EventArgs e)
		{
			using (AboutDialog window = new AboutDialog ()) {
				window.Parent = this;
				window.Title = "درباره ی ما";
				//window.VBox.BorderWidth = 50;
				//addlinks.comboBox.MainControlWindow = window;
				window.WindowPosition = WindowPosition.CenterAlways;
				window.ShowAll ();
				window.Run ();
				window.Destroy ();
			}
		}

		protected void BtnOpenSite_Clicked (object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://framesoft.ir");
		}

		protected void BtnReport_Clicked (object sender, EventArgs e)
		{
			ReportWindow window = new ReportWindow ();
			window.Parent = this;
			window.Title ="تماس با ما";
			//window.VBox.BorderWidth = 50;
			//addlinks.comboBox.MainControlWindow = window;
			window.WindowPosition = WindowPosition.CenterAlways;
			window.ShowAll ();
			window.Show ();
		}
	}
}

