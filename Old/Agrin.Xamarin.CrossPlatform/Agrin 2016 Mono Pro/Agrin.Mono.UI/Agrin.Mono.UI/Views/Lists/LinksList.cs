using System;
using Gtk;
using Agrin.Download.Web;
using System.Collections.Generic;
using System.Linq;
using Agrin.Log;
using Agrin.Download.Manager;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class LinksList : Gtk.Bin
	{
		public static TreeStore store;

		public static LinksBaseViewModel LinksBVM { get; set; }

		public static LinksList This{ get; set; }

		public LinksList ()
		{
			This = this;
			this.Build ();
			store = new TreeStore (typeof(LinkInfo));

			//for (int i=0; i < 5; i++)
			//{
			//TreeIter iter = store.AppendValues ("Demo " , 1);
			//}aas
			treeview.Selection.Mode = SelectionMode.Multiple;
			treeview.Model = store;
			treeview.Selection.Changed += (sender, e) => {
				AgrinToolbar.CurrentToolbar.RefreshButtons ();
			};
			treeview.HeadersVisible = true;
			treeview.AppendColumn ("", new CellRendererPixbuf (), RenderPix);
			treeview.AppendColumn ("نام فایل", new CellRendererText (), RenderFileName);
			treeview.AppendColumn ("حجم", new CellRendererText (), RenderSize);
			treeview.AppendColumn ("وضعیت", new CellRendererText (), RenderState);
			treeview.AppendColumn ("پیشرفت", new CellRendererProgress () { Width = 150 }, RenderProgress);
			treeview.AppendColumn ("آخرین تاریخ دریافت", new CellRendererText (), RenderDate);
			treeview.AppendColumn ("محل ذخیره", new CellRendererText (), RenderAddress);
			LoadLinkInfoesData ();
			LinksBVM = new LinksBaseViewModel (() => {
				List<LinkInfo> items = new List<LinkInfo> ();
				foreach (var item in GetSelectionsIters()) {
					var iter = item;
					LinkInfo value = (LinkInfo)store.GetValue (iter, 0);
					items.Add (value);
				}
				return items;
			});
			AgrinToolbar.CurrentToolbar.RefreshButtons ();
			MenuInitialize ();
			treeview.ButtonReleaseEvent += OnTreeviewButtonReleaseEvent;
		}

		public void ScrollToZero ()
		{
			var scroll = GtkScrolledWindow.VScrollbar as VScrollbar;
			if (scroll != null)
				scroll.Adjustment.Changed += ScrollChangedMustGotoZero;
		}

		public void ScrollChangedMustGotoZero (object sender, EventArgs e)
		{
			Adjustment adjust = sender as Adjustment;
			adjust.Value = 0;
			adjust.Changed -= ScrollChangedMustGotoZero;
		}

		MenuItem openFileMNU, openLocationMNU, sendGroupMNU, DeleteMNU, SettingMNU;
		Menu contextMenu = new Menu ();

		void MenuInitialize ()
		{
			//contextMenu.Direction = TextDirection.Rtl;

			openFileMNU = new MenuItem ("باز کردن");
			//openFileMNU.Direction = TextDirection.Rtl;
			openFileMNU.Activated += (sender, e) => {
				var iter = GetSelectionsIters () [0];
				LinkInfo value = (LinkInfo)store.GetValue (iter, 0);
				System.Diagnostics.Process.Start (value.PathInfo.FullAddressFileName);
			};
			contextMenu.Add (openFileMNU);
			openLocationMNU = new MenuItem ("بازکردن محل ذخیره");
			openLocationMNU.Activated += (sender, e) => {
				var iter = GetSelectionsIters () [0];
				LinkInfo value = (LinkInfo)store.GetValue (iter, 0);
				//Environment.OpenFileBrowser(value.PathInfo.FullAddressFileName);
				System.Diagnostics.Process.Start ("explorer.exe", "/select, " + value.PathInfo.FullAddressFileName);
			};
			contextMenu.Add (openLocationMNU);
			sendGroupMNU = new MenuItem ("ارسال به گروه");
			contextMenu.Add (sendGroupMNU);
			DeleteMNU = new MenuItem ("حذف");

			DeleteMNU.Activated += (sender, e) => {
				AgrinToolbar.DeleteSelectedItems ();
			};
			contextMenu.Add (DeleteMNU);

//			SettingMNU = new MenuItem("تنظیمات");
//			SettingMNU.Activated += (sender,e) =>
//			{
//				var iter = GetSelectionsIters()[0];
//				LinkInfo value = (LinkInfo)store.GetValue(iter, 0);
//				using (Gtk.Dialog window = new Gtk.Dialog ("تنظیمات", MainWindow.This, Gtk.DialogFlags.Modal, new object[]{ }))
//				{
//					window.WidthRequest = 600;
//					window.HeightRequest = 400;
//					window.VBox.BorderWidth = 50;
//					using (LinkInfoSetting linkInfoSetting = new LinkInfoSetting())
//					{
//						linkInfoSetting.LinkInfoSettingToUI(value);
//						linkInfoSetting.CloseButton = () =>
//						{
//							linkInfoSetting.Dispose();
//							window.Destroy();
//						};
//						linkInfoSetting.Direction = Gtk.TextDirection.Rtl;
//						//linkInfoSetting.DialogResult = () => {
//						//	window.Destroy();
//						//};
//						window.VBox.Add(linkInfoSetting);
//						//addlinks.comboBox.MainControlWindow = window;
//						window.ShowAll();
//						linkInfoSetting.Settings.FontName = "segoe ui";
//						window.Run();
//						window.Destroy();
//						//BindingHelper.DisposeBindingOneWay(addlinks);
//					}
//				}
//			};
//			contextMenu.Add(SettingMNU);
			treeview.Add (contextMenu);
			contextMenu.ShowAll ();
		}

		Dictionary<Widget,GroupInfo> sendGroupMNUItems = new  Dictionary<Widget, GroupInfo> ();

		void MenuItemsValidate ()
		{
			bool enableOpen = false;
			foreach (var item in GetSelectionsIters()) {
				var iter = item;
				LinkInfo value = (LinkInfo)store.GetValue (iter, 0);
				if (value.DownloadingProperty.State == ConnectionState.Complete && System.IO.File.Exists (value.PathInfo.FullAddressFileName)) {
					enableOpen = true;
					break;
				}
			}
			openFileMNU.Sensitive = openLocationMNU.Sensitive = enableOpen;
			List<GroupInfo> forechedItems = new List<GroupInfo> ();
			Menu subMenus = new Menu ();
			sendGroupMNUItems.Clear ();
			foreach (var item in ApplicationGroupManager.Current.GroupInfoes) {
				var menuItem = new MenuItem (item.Name);
				menuItem.Activated += SendToGroup;
				sendGroupMNUItems.Add (menuItem, item);

				subMenus.Add (menuItem);
				forechedItems.Add (item);
			}
			sendGroupMNU.Submenu = subMenus;
			sendGroupMNU.ShowAll ();
		}

		void SendToGroup (object sender, EventArgs e)
		{
			GroupInfo groupInfo = sendGroupMNUItems [(Widget)sender];
			foreach (var item in GetSelectionsIters()) {
				var iter = item;
				LinkInfo value = (LinkInfo)store.GetValue (iter, 0);
				Agrin.Download.Manager.ApplicationGroupManager.Current.SetGroupByLinkInfo (value, groupInfo);
				var pth = treeview.Model.GetPath (iter);
				store.EmitRowChanged (pth, iter);
			}

			Agrin.Download.Data.SerializeData.SaveLinkInfoesToFile ();
		}

		public List<Gtk.TreeIter> GetSelectionsIters ()
		{
			var list = treeview.Selection.GetSelectedRows ().ToList ();
			List<Gtk.TreeIter> iters = new List<Gtk.TreeIter> ();
			foreach (var item in list) {
				Gtk.TreeIter iter = Gtk.TreeIter.Zero;
				bool geted = store.GetIter (out iter, item);
				if (geted) {
					iters.Add (iter);
				}
			}
			return iters;
		}

		public void LoadLinkInfoesData ()
		{
			foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes) {
				AddBinding (store.AppendValues (item), item);
			}
		}

		public void AddBinding (TreeIter iter, LinkInfo linkInfo)
		{
			BindingHelper.BindAction (linkInfo, linkInfo, new List<string> () { "FileIcon" },
				(name) => {
					linkInfo.RunOnUI (() => {
						var pth = treeview.Model.GetPath (iter);
						store.EmitRowChanged (pth, iter);
					});
				});
			BindingHelper.BindAction (linkInfo, linkInfo.DownloadingProperty, new List<string> () {
				"DownloadedSize",
				"Size",
				"State"
			},
				(name) => {
					linkInfo.RunOnUI (() => {
						var add = linkInfo.PathInfo.FileName;
						var pth = treeview.Model.GetPath (iter);
						store.EmitRowChanged (pth, iter);
						AgrinToolbar.CurrentToolbar.RefreshButtons ();
					});
				});
			BindingHelper.BindAction (linkInfo, linkInfo.PathInfo, new List<string> () {
				"FileName",
				"CurrentGroupInfo",
				"SavePath"
			},
				(name) => {
					linkInfo.RunOnUI (() => {
						var pth = treeview.Model.GetPath (iter);
						LinkInfo value = (LinkInfo)store.GetValue (iter, 0);
						store.EmitRowChanged (pth, iter);
					});
				});
		}

		public void RenderPix (TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			try {
				LinkInfo value = (LinkInfo)model.GetValue (iter, 0);
				if (value.FileIcon != null)
					(cell as Gtk.CellRendererPixbuf).Pixbuf = new Gdk.Pixbuf (value.FileIcon, 24, 24);// new Gdk.Pixbuf(GetFileIconName (value.PathInfo.FileName), 32, 32);
				else {
					(cell as Gtk.CellRendererPixbuf).Pixbuf = new Gdk.Pixbuf (Gdk.PixbufLoader.LoadFromResource ("Agrin.Mono.UI.icons.AgrinIcon.png").Pixbuf, 0, 0, 24, 24);
				}

			} catch (Exception ex) {
				AutoLogger.LogError (ex, "RenderPix");
			}

		}

		public void RenderFileName (TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = value.PathInfo.FileName;
		}

		public void RenderSize (TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum (value.DownloadingProperty.Size);
		}

		public void RenderState (TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = value.DownloadingProperty.State.ToString ();
		}

		public void RenderProgress (TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue (iter, 0);
			double val = ((double)value.DownloadingProperty.DownloadedSize / value.DownloadingProperty.Size);
			(cell as Gtk.CellRendererProgress).Value = int.Parse (((int)(val * 100.0)).ToString ());
			(cell as Gtk.CellRendererProgress).Text = value.DownloadingProperty.Size < 0 ? "نامشخص" : String.Format ("{0:00.00%}", val);
		}

		public void RenderDate (TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = value.DownloadingProperty.DateLastDownload.ToShortDateString ();
		}

		public void RenderAddress (TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = value.PathInfo.SavePath;
		}

		protected void OnTreeviewButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			switch (args.Event.Button) {
			//case 1: /*left button*/
			//Console.WriteLine( "Left Mouse button released" );
			//break;
			//case 2: /*middle button*/
			//break;
			case 3: /*right button */
				{
					if (treeview.Selection.CountSelectedRows () > 0) {
						MenuItemsValidate ();
						contextMenu.Popup ();
					}
					break;
				}

			}
		}
	}
}

