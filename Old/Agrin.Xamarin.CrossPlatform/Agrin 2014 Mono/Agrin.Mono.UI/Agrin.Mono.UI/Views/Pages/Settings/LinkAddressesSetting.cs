using System;
using Gtk;
using Agrin.Download.Web.Link;
using System.Collections.Generic;
using Agrin.Download.Web;
using System.Linq;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkAddressesSetting : Gtk.Bin
	{
		public static LinkAddressesSetting This;
		public TreeStore store;
		public TreeStore comboStore;

		public LinkAddressesSetting()
		{
			This = this;
			this.Build();
			store = new TreeStore(typeof(MultiLinkAddress));
			comboStore = new TreeStore(typeof(string));
			tv.Model = store;
			tv.HeadersVisible = true;
			tv.AppendColumn("آدرس", new CellRendererText(), RenderAddress);
			CellRendererCombo combo = new CellRendererCombo();
			combo.Editable = true;
			combo.HasEntry = false;
			combo.Model = comboStore;
			combo.Width = 150;
			combo.Edited += EditedComboItem;
			TreeViewColumn comboColumn = tv.AppendColumn("پروکسی", combo, RenderProxy);
			comboColumn.PackStart(combo, true);

			CellRendererToggle render = new CellRendererToggle();
			render.Activatable = true;
			render.Toggled += checkedToggleButton;
			tv.AppendColumn("استفاده", render, RendeUse);
			CellRendererButton btn = new CellRendererButton(global::Stetic.IconLoader.LoadIcon (tv, "gtk-close", global::Gtk.IconSize.SmallToolbar));
			btn.Clicked += ClickedDeleteTreeIter;
			tv.AppendColumn("حذف", btn, RenderDelete);
			btn = new CellRendererButton(global::Stetic.IconLoader.LoadIcon (tv, "gtk-copy", global::Gtk.IconSize.SmallToolbar));
			btn.Clicked += ClickedCopyTreeIter;
			tv.AppendColumn("کپی", btn, RenderCopy);
			ResetProxyList();
			InitControl();
		}

		List<ProxyInfo> proxies = new List<ProxyInfo>();

		public void ResetProxyList()
		{
			comboStore.Clear();
			proxies.Clear();
			var info = new ProxyInfo() { ServerAddress = "اتوماتیک", Port = -1 };
			proxies.Add(info);
			TreeIter iter = comboStore.AppendValues(info.Id +"*"+info.FullAddress);
			comboStore.EmitRowChanged(comboStore.GetPath(iter), iter);
			foreach (var item in ProxySetting.This.Items)
			{
				iter = comboStore.AppendValues(item.Id+"*"+item.FullAddress);
				proxies.Add(item);
				comboStore.EmitRowChanged(comboStore.GetPath(iter), iter);
			}
			foreach (var item in tv.Selection.GetSelectedRows())
			{
				if (store.GetIter(out iter, item))
					store.EmitRowChanged(item, iter);
			}
		}

		public List<MultiLinkAddress> Items = new List<MultiLinkAddress>();

		public void AddRange(List<MultiLinkAddress> items)
		{
			foreach (var item in items)
			{
				Items.Add(item);
				store.AppendValues(item);
			}
		}

		void EditedComboItem(object sender, EditedArgs args)
		{
			TreeIter iter;
			if (comboStore.GetIter(out iter, new TreePath(args.Path)))
			{
				//Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false,args.NewText.Split(new char[]{'\\'})[0].ToString() );
				//m.Run();
				int id = int.Parse(args.NewText.Split(new char[]{'*'})[0]);
				foreach (var item in proxies)
				{
					if (item.Id == id)
					{
						store.GetIter(out iter, tv.Selection.GetSelectedRows()[0]);
						MultiLinkAddress old = (MultiLinkAddress)store.GetValue(iter, 0);
						old.ProxyID = id;
						break;
					}
				}
				//MultiLinkAddress old = (MultiLinkAddress)store.GetValue(iter, 0);
				//old.ProxyID = proxies[((CellRendererCombo)sender).TextColumn].Id;
			}
		}

		void checkedToggleButton(object sender, ToggledArgs arg)
		{
			TreeIter iter;
			if (store.GetIter(out iter, new TreePath(arg.Path)))
			{
				//Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, arg.Path);
				//m.Run();
				MultiLinkAddress old = (MultiLinkAddress)store.GetValue(iter, 0);
				old.IsSelected = !old.IsSelected;
			}
		}

		void ClickedDeleteTreeIter(object sender, EventArgs e)
		{
			TreeIter iter;

			if (store.GetIter(out iter, tv.Selection.GetSelectedRows()[0]))
			{
				//Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, arg.Path);
				//m.Run();
				MultiLinkAddress old = (MultiLinkAddress)store.GetValue(iter, 0);
				bool remove = store.Remove(ref iter);
				Items.Remove(old);
				InitControl();
			}
		}

		void ClickedCopyTreeIter(object sender, EventArgs e)
		{
			TreeIter iter;

			if (store.GetIter(out iter, tv.Selection.GetSelectedRows()[0]))
			{
				//Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, arg.Path);
				//m.Run();
				MultiLinkAddress old = (MultiLinkAddress)store.GetValue(iter, 0);
				//System.Windows.Forms.Clipboard.SetText(old.Address);
			}
		}

		public void RenderAddress(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			MultiLinkAddress value = (MultiLinkAddress)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.Address;
		}

		public void RenderProxy(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			MultiLinkAddress value = (MultiLinkAddress)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererCombo).TextColumn = 0;
			foreach (var item in proxies)
			{
				if (item.Id == value.ProxyID)
				{
					//string portText = item.Id > 1 ? ":" + item.Port : "";
					(cell as Gtk.CellRendererCombo).Text = item.Id + "*" + item.FullAddress;
					return;
				}
			}
			value.ProxyID = 0;
		}

		public void RendeUse(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			MultiLinkAddress value = (MultiLinkAddress)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererToggle).Active = value.IsSelected;
		}

		public void RenderDelete(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			(cell as CellRendererButton).Sensitive = Items.Count > 1;
		}

		public void RenderCopy(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			//ProxyInfo value = (ProxyInfo)model.GetValue(iter, 0);
			//(cell as Gtk.CellRendererSpin).Text = value.FullAddress;
		}

		bool _isEnabled = true;

		public bool IsEnabled
		{
			get { return _isEnabled; }
			set { _isEnabled = value;}
		}

		public void InitControl()
		{
			Uri uri = null;
			bool canCreate = IsEnabled && Uri.TryCreate(txt_address.Text, UriKind.Absolute, out uri);
			btnAdd.Sensitive = canCreate;
			int count = 0;
			foreach (var item in Items)
			{
				if (item.IsSelected)
					count++;
			}

			btnCheck.Sensitive = IsEnabled && count >= 2;
		}

		protected void OnBtnAddClicked(object sender, EventArgs e)
		{
			var info = new MultiLinkAddress() { Address = txt_address.Text, IsSelected = true };
			Items.Add(info);
			store.AppendValues(info);
			txt_address.Text = "";
			InitControl();
		}

		protected void OnBtnCheckClicked(object sender, EventArgs e)
		{
			CheckLinkInfo();
		}

		protected void OnTxtAddressChanged(object sender, EventArgs e)
		{
			InitControl();
		}

		LinkCheckerHelper _currentChecker = new LinkCheckerHelper();

		public LinkCheckerHelper CurrentChecker
		{
			get { return _currentChecker; }
			set { _currentChecker = value; }
		}

		private void CheckLinkInfo()
		{
			IsEnabled = false;
			InitControl();
			lblMessage.Text = "در حال بازنگری لینک ها...";
			CurrentChecker.CompleteCheck = (mode, checker) =>
			{
				if (checker == CurrentChecker)
				{
					IsEnabled = true;
					InitControl();
					switch (mode)
					{
						case LinkaddressCheckMode.Exception:
							{
								lblMessage.Text = "خطا در بازنگری فایل ها رخ داده است.";
								break;
							}
						case LinkaddressCheckMode.True:
							{
								lblMessage.Text = "حجم فایل ها برابر است. شما میتوانید فایل را دانلود کنید";
								break;
							}
						case LinkaddressCheckMode.False:
							{
								lblMessage.Text = "فایل ها هماهنگ نیستند";
								break;
							}
						case LinkaddressCheckMode.UnknownFileSize:
							{
								lblMessage.Text = "حجم فایل نامشخص است.";
								break;
							}
					}
				}
			};
			CurrentChecker.Check(Items, proxies.ToList());
		}

		protected void OnHbox1SizeAllocated (object o, SizeAllocatedArgs args)
		{
			lblMessage.SetSizeRequest(args.Allocation.Width, args.Allocation.Height);
		}
	}

	public class LinkCheckerHelper
	{
		public Action<LinkaddressCheckMode, LinkCheckerHelper> CompleteCheck;

		public void Check(List<MultiLinkAddress> items, List<ProxyInfo> proxies)
		{
			Agrin.Helper.ComponentModel.AsyncActions.Action(() =>
			{
				var mode = LinkChecker.CheckMultiLinkInfo(items, proxies);
				if (CompleteCheck != null)
					CompleteCheck(mode, this);
			});
		}
	}
}

