using System;
using Gtk;
using Agrin.Download.Web;
using System.Collections.Generic;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class AgrinToolbar : Gtk.Bin
	{
		public static AgrinToolbar CurrentToolbar{ get; set; }

		public AgrinToolbar ()
		{
			CurrentToolbar = this;
			this.Build ();
			toolbar.ToolbarStyle = ToolbarStyle.Icons;
			//Initialize ();
		}

		//		public void Initialize ()
		//		{
		//			Button btnAddLink = new Button ();
		//			//Button btnPlayLinks;
		//			//Button btnPauseLink;
		//			//Button btnRefreshLink;
		//			//Button btnTrash;
		//			btnAddLink.WidthRequest = 36;
		//			var pb=Gdk.PixbufLoader.LoadFromResource ("Agrin.Mono.UI.icons.add.png").Pixbuf;
		//			Image img = new Image ();
		//			var srcWidth = 24;
		//			var srcHeight = 24;
		//			int resultWidth, resultHeight;
		//			ScaleRatio (srcWidth, srcHeight, 24, 24, out resultWidth, out resultHeight);
		//			img.Pixbuf = pb.ScaleSimple (resultWidth, resultHeight, Gdk.InterpType.Bilinear);
		//			//btnAddLink.b
		//			//img.Pixbuf=img;//new Gdk.Pixbuf (pb, 0, 0, 24, 24);
		//			btnAddLink.Image = img;
		//
		//			hbox1.Add (btnAddLink);
		//			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [btnAddLink]));
		//			w1.Position = 0;
		//			w1.Expand = false;
		//			w1.Fill = false;
		//
		//
		//
		//		}
		//
		//		private static void ScaleRatio(int srcWidth, int srcHeight, int destWidth, int destHeight, out int resultWidth, out int resultHeight)
		//		{
		//			var widthRatio = (float)destWidth / srcWidth;
		//			var heigthRatio = (float)destHeight / srcHeight;
		//
		//			var ratio = Math.Min(widthRatio, heigthRatio);
		//			resultHeight = (int)(srcHeight * ratio);
		//			resultWidth = (int)(srcWidth * ratio);
		//		}

		public void RefreshButtons ()
		{
			btnPlay.Sensitive = LinksList.LinksBVM.CanPlayLinks ();
			btnPause.Sensitive = LinksList.LinksBVM.CanStopLinks ();
			btnRefresh.Sensitive = LinksList.LinksBVM.CanReconnectSelectedLinks ();
			btnTrash.Sensitive = LinksList.LinksBVM.CanDeleteLinks ();
		}

		protected void OnBtnTrashActivated (object sender, EventArgs e)
		{
			DeleteSelectedItems ();
		}

		public static void DeleteSelectedItems ()
		{
			Gtk.MessageDialog m = new Gtk.MessageDialog (MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, "آیا میخواهید لینک های انتخاب شده را حذف کنید؟");
			Gtk.ResponseType result = (Gtk.ResponseType)m.Run ();
			m.Destroy ();
			if (result == Gtk.ResponseType.Yes) {
				try {
					var list= LinksList.This.GetSelectionsIters ().ToArray();
					LinksList.LinksBVM.DeleteLinks ();
					foreach (var item in list) {
						var iter = item;
						LinkInfo value = (LinkInfo)LinksList.store.GetValue (iter, 0);
						BindingHelper.DisposeObject (value);
						bool remove = LinksList.store.Remove (ref iter);
					}

					AgrinToolbar.CurrentToolbar.RefreshButtons ();
				} catch (Exception ex) {
					Agrin.Log.AutoLogger.LogError (ex, "mono DeleteSelectedItems");
				}
			}
		}

		protected void OnBtnRefreshActivated (object sender, EventArgs e)
		{
			LinksList.LinksBVM.ReconnectSelectedLinks ();
		}


		protected void OnBtnPauseActivated (object sender, EventArgs e)
		{
			LinksList.LinksBVM.StopLinks ();
		}


		protected void OnBtnPlayActivated (object sender, EventArgs e)
		{
			LinksList.LinksBVM.PlayLinks ();
		}

		protected void OnBtnAddActivated (object sender, EventArgs e)
		{
			using (Gtk.Dialog window = new Gtk.Dialog ("درج لینک", MainWindow.This, Gtk.DialogFlags.Modal, new object[]{ })) {

				window.WidthRequest = 400;
				window.VBox.BorderWidth = 50;
				using (AddLinks addlinks = new AddLinks ()) {
					addlinks.DialogResult = () => {
						window.Destroy ();
					};
					window.VBox.Add (addlinks);
					//addlinks.comboBox.MainControlWindow = window;
					window.ShowAll ();
					addlinks.Settings.FontName = "segoe ui";
					window.Run ();
					window.Destroy ();
				}
			}
		}
	}
}

