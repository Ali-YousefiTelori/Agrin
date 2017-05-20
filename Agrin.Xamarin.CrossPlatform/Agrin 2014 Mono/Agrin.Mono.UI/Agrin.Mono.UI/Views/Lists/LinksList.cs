using System;
using Agrin.Download.Web;
using Agrin.Download.Manager;
using Gtk;
using System.Collections.Generic;
using System.Linq;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class LinksList : Gtk.Bin
	{
		public static System.Action SelectionChanged;
		public TreeStore store;

		public LinksList()
		{
			this.Build();
			This = this;
			store = new TreeStore(typeof(LinkInfo));

			//for (int i=0; i < 5; i++)
			//{
			//TreeIter iter = store.AppendValues ("Demo " , 1);
			//}
			tv.Selection.Mode = SelectionMode.Multiple;
			tv.Model = store;
			tv.Selection.Changed += (sender, e) => {
				if (SelectionChanged != null)
					SelectionChanged();
			};
			tv.HeadersVisible = true;
			tv.AppendColumn("", new CellRendererPixbuf(), RenderPix);
			tv.AppendColumn("نام فایل", new CellRendererText(), RenderFileName);
			tv.AppendColumn("حجم", new CellRendererText(), RenderSize);
			tv.AppendColumn("وضعیت", new CellRendererText(), RenderState);
			tv.AppendColumn("پیشرفت", new CellRendererProgress() { Width = 150 }, RenderProgress);
			tv.AppendColumn("آخرین تاریخ دریافت", new CellRendererText(), RenderDate);
			tv.AppendColumn("محل ذخیره", new CellRendererText(), RenderAddress);
			MenuInitialize();
			tv.ButtonReleaseEvent += OnTvButtonReleaseEvent;
			//this.Add(tv);
			//this.ShowAll();

		}

		MenuItem openFileMNU, openLocationMNU, sendGroupMNU, DeleteMNU, SettingMNU;

		void MenuInitialize()
		{
			//contextMenu.Direction = TextDirection.Rtl;

			openFileMNU = new MenuItem("باز کردن");
			//openFileMNU.Direction = TextDirection.Rtl;
			openFileMNU.Activated += (sender,e) =>
			{
				var iter = GetSelectionsIters()[0];
				LinkInfo value = (LinkInfo)LinksList.This.store.GetValue(iter, 0);
				System.Diagnostics.Process.Start(value.PathInfo.FullAddressFileName);
			};
			contextMenu.Add(openFileMNU);
			openLocationMNU = new MenuItem("بازکردن محل ذخیره");
			openLocationMNU.Activated += (sender,e) =>
			{
				var iter = GetSelectionsIters()[0];
				LinkInfo value = (LinkInfo)LinksList.This.store.GetValue(iter, 0);
				//Environment.OpenFileBrowser(value.PathInfo.FullAddressFileName);
				System.Diagnostics.Process.Start("explorer.exe", "/select, " + value.PathInfo.FullAddressFileName);
			};
			contextMenu.Add(openLocationMNU);
			sendGroupMNU = new MenuItem("ارسال به گروه");
			contextMenu.Add(sendGroupMNU);
			DeleteMNU = new MenuItem("حذف");
		    
			DeleteMNU.Activated += (sender,e) =>
			{
				Toolbar.This.OnBtnDeleteClicked(null, null);
			};
			contextMenu.Add(DeleteMNU);

			SettingMNU = new MenuItem("تنظیمات");
			SettingMNU.Activated += (sender,e) =>
			{
				var iter = GetSelectionsIters()[0];
				LinkInfo value = (LinkInfo)store.GetValue(iter, 0);
				using (Gtk.Dialog window = new Gtk.Dialog ("تنظیمات", MainWindow.This, Gtk.DialogFlags.Modal, new object[]{ }))
				{
					window.WidthRequest = 600;
					window.HeightRequest = 400;
					window.VBox.BorderWidth = 50;
					using (LinkInfoSetting linkInfoSetting = new LinkInfoSetting())
					{
						linkInfoSetting.LinkInfoSettingToUI(value);
						linkInfoSetting.CloseButton = () =>
						{
							linkInfoSetting.Dispose();
							window.Destroy();
						};
						linkInfoSetting.Direction = Gtk.TextDirection.Rtl;
						//linkInfoSetting.DialogResult = () => {
						//	window.Destroy();
						//};
						window.VBox.Add(linkInfoSetting);
						//addlinks.comboBox.MainControlWindow = window;
						window.ShowAll();
						linkInfoSetting.Settings.FontName = "segoe ui";
						window.Run();
						window.Destroy();
						//BindingHelper.DisposeBindingOneWay(addlinks);
					}
				}
			};
			contextMenu.Add(SettingMNU);
			tv.Add(contextMenu);
			contextMenu.ShowAll();
		}

		Dictionary<Widget,GroupInfo> sendGroupMNUItems = new  Dictionary<Widget, GroupInfo>();

		void MenuItemsValidate()
		{
			bool enableOpen = false;
			foreach (var item in GetSelectionsIters())
			{
				var iter = item;
				LinkInfo value = (LinkInfo)LinksList.This.store.GetValue(iter, 0);
				if (value.DownloadingProperty.State == ConnectionState.Complete && System.IO.File.Exists(value.PathInfo.FullAddressFileName))
				{
					enableOpen = true;
					break;
				}
			}
			openFileMNU.Sensitive = openLocationMNU.Sensitive = enableOpen;
			List<GroupInfo> forechedItems = new List<GroupInfo>();
			Menu subMenus = new Menu();
			sendGroupMNUItems.Clear();
			foreach (var item in ApplicationGroupManager.Current.GroupInfoes)
			{
				var menuItem = new MenuItem(item.Name);
				menuItem.Activated += SendToGroup;
				sendGroupMNUItems.Add(menuItem, item);

				subMenus.Add(menuItem);
				forechedItems.Add(item);
			}
			sendGroupMNU.Submenu = subMenus;
			sendGroupMNU.ShowAll();
		}

		void SendToGroup(object sender, EventArgs e)
		{
			GroupInfo groupInfo = sendGroupMNUItems[(Widget)sender];
			foreach (var item in GetSelectionsIters())
			{
				var iter = item;
				LinkInfo value = (LinkInfo)LinksList.This.store.GetValue(iter, 0);
				value.PathInfo.UserGroupInfo = groupInfo;
				var pth = tv.Model.GetPath(iter);
				store.EmitRowChanged(pth, iter);
			}

			Agrin.Download.Data.SerializeData.SaveLinkInfoesToFile();
		}

		Menu contextMenu = new Menu();

		public TreeView TreeView
		{
			get
			{
				return this.tv;
			}
		}

		public List<Gtk.TreeIter> GetSelectionsIters()
		{
			var list = tv.Selection.GetSelectedRows().ToList();
			List<Gtk.TreeIter> iters = new List<Gtk.TreeIter>();
			foreach (var item in list)
			{
				Gtk.TreeIter iter = Gtk.TreeIter.Zero;
				bool geted = store.GetIter(out iter, item);
				if (geted)
				{
					iters.Add(iter);
				}
			}
			return iters;
		}

		public void RenderPix(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue(iter, 0);

			(cell as Gtk.CellRendererPixbuf).Pixbuf = new Gdk.Pixbuf(GetFileIconName (value.PathInfo.FileName), 32, 32);
		}

		public void RenderFileName(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.PathInfo.FileName;
		}

		public void RenderSize(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum(value.DownloadingProperty.Size);
		}

		public void RenderState(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.DownloadingProperty.State.ToString();
		}

		public void RenderProgress(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue(iter, 0);
			double val = ((double)value.DownloadingProperty.DownloadedSize / value.DownloadingProperty.Size);
			(cell as Gtk.CellRendererProgress).Value = int.Parse(((int)(val * 100.0)).ToString ());
			(cell as Gtk.CellRendererProgress).Text = value.DownloadingProperty.Size < 0 ? "نامشخص" : String.Format("{0:00.00%}", val);
		}

		public void RenderDate(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.DownloadingProperty.DateLastDownload.ToShortDateString();
		}

		public void RenderAddress(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			LinkInfo value = (LinkInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.PathInfo.SavePath;
		}

		public static LinksList This;
		object lockobj = new object();

		public string GetFileIconName(string fileName)
		{
			try
			{
				lock (lockobj)
				{
					string address = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, fileName);
					string extention = System.IO.Path.GetExtension(address);
					string iconAddress = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "SaveIcons");
					string fileIcon = System.IO.Path.Combine(iconAddress, extention + ".png");
					if (System.IO.File.Exists(fileIcon))
						return fileIcon;
					System.IO.Directory.CreateDirectory(iconAddress);
					System.IO.File.Create(address).Dispose();
					var icon = System.Drawing.Icon.ExtractAssociatedIcon(address).Handle;
					var bitmap = System.Drawing.Bitmap.FromHicon(icon);
					bitmap.Save(fileIcon, System.Drawing.Imaging.ImageFormat.Png);
					System.IO.File.Delete(address);
					return fileIcon;
				}
			}
			catch
			{
				return "";
			}

		}

		public void LoadLinkInfoesData()
		{
			foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes)
			{
				AddBinding(store.AppendValues(item), item);
			}
		}

		Dictionary<LinkInfo,Guid> bindingItems = new Dictionary<LinkInfo, Guid>();

		void AddBinding(TreeIter iter, LinkInfo linkInfo)
		{
			Guid guid = Guid.NewGuid();
			bindingItems.Add(linkInfo, guid);
			bool changing = false;
			BindingHelper.AddActionPropertyChanged((name) =>
			                                       {
				if (name == "State")
					Toolbar.This.InitToolbarSelectionItems(false);
				if (!changing)
				{
					changing=true;
					var pth = tv.Model.GetPath (iter);
					store.EmitRowChanged (pth, iter);
					changing=false;
				}
			}, linkInfo.DownloadingProperty, new List<string>() { "DownloadedSize", "Size", "State" }, guid);
			BindingHelper.AddActionPropertyChanged(() =>
			                                       {
				var pth = tv.Model.GetPath (iter);
				store.EmitRowChanged (pth, iter);
			}, linkInfo.PathInfo, new List<string>() { "FileName" }, guid);
		}

		public void RemoveBinding(LinkInfo linkInfo)
		{
			BindingHelper.DisposeBindingAction(bindingItems[linkInfo]);
			bindingItems.Remove(linkInfo);
		}

		public void AddLinkInfo(string uriAddress, GroupInfo groupInfo, bool isPlay)
		{
			LinkInfo linkInfo = new LinkInfo(uriAddress);
			ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, groupInfo, isPlay);
			//TreeIter iter = store.AppendValues (50);
			AddBinding(store.AppendValues(linkInfo), linkInfo);
		}

		protected void OnButton1Clicked(object sender, EventArgs e)
		{

			//RenderProgress (TreeViewItemProperties.Columns [0], TreeViewItemProperties.Renders[0], tv.Model, TreeViewItemProperties.Values[0]);
			//TreeViewItemProperties.Columns [0].CellSetCellData (tv.Model, TreeViewItemProperties.Values [0], false, false);
			//tv.ShowAll ();
			//TreeViewItemProperties.Renders [0].StartEditing (null, tv, "", null, null, CellRendererState.Selected);
			//
		}

		protected void OnTvButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
		{
			switch (args.Event.Button)
			{
			//case 1: /*left button*/
			//Console.WriteLine( "Left Mouse button released" );
			//break;
			//case 2: /*middle button*/
			//break;
				case 3: /*right button */
					{
						if (tv.Selection.CountSelectedRows() > 0)
						{
							MenuItemsValidate();
							contextMenu.Popup();
						}
						break;
					}

			}

		}
	}
}

