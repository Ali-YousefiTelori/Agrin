using System;
using Gtk;

namespace Agrin.Mono.UI
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			//AppDomain.CurrentDomain.FirstChanceException += error1;
			GLib.ExceptionManager.UnhandledException += eeeeeee;

			Application.Init();
			try 
			{
				MainWindow win = new MainWindow();
				win.Show();
			}
			catch(Exception e)
			{
				Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, "aaaaa");
				m.Run();
			}


			Application.Run();
		}

		static void eeeeeee(GLib.UnhandledExceptionArgs e)
		{
			e.ExitApplication=false;
			Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, "unhanled");
			m.Run();
		}
	}
}
