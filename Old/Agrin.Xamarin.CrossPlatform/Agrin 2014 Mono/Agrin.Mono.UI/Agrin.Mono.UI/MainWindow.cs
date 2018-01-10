using System;
using Gtk;
using Agrin.Download.Manager;

public partial class MainWindow: Gtk.Window
{
	public MainWindow() : base (Gtk.WindowType.Toplevel)
	{
		Build();
		this.Direction = hbox3.Direction = TextDirection.Rtl;
		This = this;
		ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
		ApplicationGroupManager.Current = new ApplicationGroupManager();
		Agrin.Download.Data.DeSerializeData.LoadApplicationData();
		Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();
		linkslist1.LoadLinkInfoesData();
		groupslist1.LoadgroupInfoesData();
		//this.ModifyFont (new Pango.FontDescription(){Family="arial"});
		//menubar1.ModifyFont (new Pango.FontDescription(){Family="arial"});
		//for (int i = 0; i < 5; i++) {
		//	linkslist1.AddLinkInfo ("http://google.com/"+System.IO.Path.GetRandomFileName(), null, false);
		//}
		//Gtk.fon
		toolbar1.InitToolbarSelectionItems();
		//menubar1.Settings.FontName = "arial";
		///var ff=Gdk.Font.Load(Agrin.IO.Helper.MPath.CurrentAppDirectory + "\\Fonts\\Tehran.ttf");
		//this.Settings.FontName =ff.Display.Name;
		this.Settings.FontName = "segoe ui";
		//this.Settings.FontName =use embed resource font;
	}

	public static Gtk.Window This;

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		foreach (var item in ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToList())
		{
			item.Dispose();
		}
		try
		{
			Agrin.Download.Data.SerializeData.SaveLinkInfoesToFileNoThread();
		}
		catch
		{

		}
		Application.Quit();
		a.RetVal = true;
		System.Diagnostics.Process.GetCurrentProcess().Kill();
	}

	protected void OnBtnAboutClicked(object sender, EventArgs e)
	{
		using (Agrin.Mono.UI.AboutDialog window = new Agrin.Mono.UI.AboutDialog())
		{
			window.Parent = this;
			window.HeightRequest = 150;
			//window.VBox.BorderWidth = 50;
			//addlinks.comboBox.MainControlWindow = window;
			window.WindowPosition = WindowPosition.CenterAlways;
			window.ShowAll();

			window.Run();
			window.Destroy();
		}

	}

	protected void OnBtnReportBugClicked(object sender, EventArgs e)
	{
		using (Gtk.Dialog window = new Gtk.Dialog ("گزارش باگ و اشکال،پیشنهاد،انتقاد", MainWindow.This, Gtk.DialogFlags.Modal, new object[]{ }))
		{
			Agrin.Mono.UI.ReportBugs bugs = new Agrin.Mono.UI.ReportBugs();
			window.VBox.Add(bugs);
			//addlinks.comboBox.MainControlWindow = window;
			window.ShowAll();
			window.Run();
			window.Destroy();
		}
	}
}
