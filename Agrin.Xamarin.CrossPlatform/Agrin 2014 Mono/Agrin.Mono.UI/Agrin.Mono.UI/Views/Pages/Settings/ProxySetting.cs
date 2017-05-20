using System;
using Gtk;
using Agrin.Download.Web.Link;
using System.Collections.Generic;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ProxySetting : Gtk.Bin
	{
		public static ProxySetting This;
		public TreeStore store;
		public ProxySetting()
		{
			This = this;
			this.Build();
			chk_address.Direction = chk_username.Direction = Gtk.TextDirection.Rtl;

			store = new TreeStore(typeof(ProxyInfo));

			//for (int i=0; i < 5; i++)
			//{
			//TreeIter iter = store.AppendValues ("Demo " , 1);
			//}
			tv.Model = store;
			tv.HeadersVisible = true;
			tv.AppendColumn("آدرس", new CellRendererText(), RenderAddress);
			tv.AppendColumn("پورت", new CellRendererText(), RenderPort);
			tv.AppendColumn("نام کاربری", new CellRendererText(), RenderUserName);
			CellRendererToggle render = new CellRendererToggle();
			render.Activatable=true;
			render.Toggled += checkedToggleButton;
			tv.AppendColumn("استفاده", render, RendeUse);
			CellRendererButton btn = new CellRendererButton(global::Stetic.IconLoader.LoadIcon (tv, "gtk-close", global::Gtk.IconSize.SmallToolbar));
			btn.Clicked += ClickedDeleteTreeIter;
			tv.AppendColumn("حذف", btn, RenderDelete);
			InitControl();
		}

		void checkedToggleButton(object sender,ToggledArgs arg)
		{
			TreeIter iter;
			if (store.GetIter(out iter, new TreePath(arg.Path)))
			{
				//Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, arg.Path);
				//m.Run();
				ProxyInfo old = (ProxyInfo)store.GetValue(iter, 0);
				old.IsSelected = !old.IsSelected;
			}
		}

		void ClickedDeleteTreeIter(object sender,EventArgs e)
		{
			TreeIter iter;

			if (store.GetIter(out iter, tv.Selection.GetSelectedRows()[0]))
			{
				//Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, arg.Path);
				//m.Run();
				ProxyInfo old = (ProxyInfo)store.GetValue(iter, 0);
				store.Remove(ref iter);
				Items.Remove(old);
				if (!LinkInfoSetting.This.IsApplicationSetting)
					LinkAddressesSetting.This.ResetProxyList();
			}
		}

		public void RenderAddress(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ProxyInfo value = (ProxyInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.ServerAddress;
		}

		public void RenderPort(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ProxyInfo value = (ProxyInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.Port.ToString();
		}

		public void RenderUserName(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ProxyInfo value = (ProxyInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.UserName;
		}

		public void RendeUse(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ProxyInfo value = (ProxyInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererToggle).Active = value.IsSelected;
		}
		public void RenderDelete(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			//ProxyInfo value = (ProxyInfo)model.GetValue(iter, 0);
			//(cell as Gtk.CellRendererSpin).Text = value.FullAddress;
		}

		List<ProxyInfo> _Items = new List<ProxyInfo>();

		public List<ProxyInfo> Items
		{
			get
			{
				return _Items;
			}
			set
			{
				_Items = value;
			}
		}

		public void AddRanges(List<ProxyInfo> items)
		{
			foreach (var item in items)
			{
				Items.Add(item);
				store.AppendValues(item);
			}
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			if (!chk_address.Active)
			{
				ProxyInfo info = new ProxyInfo() { Id = 1, ServerAddress = "بدون پروکسی", Port = -1, IsUserPass = false, UserName = "", Password = "", IsSelected = true };
				Items.Add(info);
				store.AppendValues(info);
			}
			else
			{
				ProxyInfo info = new ProxyInfo() { Id = GetNewId(), ServerAddress = txt_Address.Text, Port = (int)txtPort.Value, IsUserPass = chk_username.Active, UserName = txt_userName.Text, Password = txt_password.Text, IsSelected = true };
				Items.Add(info);
				store.AppendValues(info);
				txt_userName.Text = txt_password.Text = txt_Address.Text = "";
			}
			if (!LinkInfoSetting.This.IsApplicationSetting)
				LinkAddressesSetting.This.ResetProxyList();
			InitControl();
		}

		int GetNewId()
		{
			int i = 0;
			foreach (var item in Items)
			{
				if (item.Id > i)
					i = item.Id;
			}
			i++;
			return i <= 1 ? 2 : i;
		}

		protected void OnBtnSysProxyClicked (object sender, EventArgs e)
		{
			var proxy = System.Net.WebProxy.GetDefaultProxy();
			if (proxy.Address == null)
				return;
			txt_Address.Text = proxy.Address.ToString().Replace("http:", "").Trim(new char[] { '/', '\\' });
			txt_Address.Text = txt_Address.Text.Replace(":" + proxy.Address.Port, "");
			txtPort.Value = proxy.Address.Port;
			chk_username.Active = proxy.UseDefaultCredentials;
			if (chk_username.Active)
			{
				txt_userName.Text = ((System.Net.NetworkCredential)proxy.Credentials).UserName;
				txt_password.Text = ((System.Net.NetworkCredential)proxy.Credentials).Password;
			}
			InitControl();
		}

		protected void OnChkAddressToggled (object sender, EventArgs e)
		{
			InitControl();
		}

		protected void OnChkUsernameToggled (object sender, EventArgs e)
		{
			InitControl();
		}

		protected void OnTxtAddressChanged (object sender, EventArgs e)
		{
			InitControl();
		}

		protected void OnTxtUserNameChanged (object sender, EventArgs e)
		{
			InitControl();
		}

		protected void OnTxtPasswordChanged (object sender, EventArgs e)
		{
			InitControl();
		}

		public void InitControl()
		{
			if (!chk_address.Active)
			{
				btnAdd.Sensitive = true;
				btn_sysProxy.Sensitive= txt_Address.Sensitive = chk_username.Sensitive = txtPort.Sensitive = txt_userName.Sensitive = txt_password.Sensitive=false;
				return;
			}

			btn_sysProxy.Sensitive = txt_Address.Sensitive = chk_username.Sensitive = txtPort.Sensitive = true;
			txt_userName.Sensitive = txt_password.Sensitive = chk_username.Active;
			bool can = String.IsNullOrWhiteSpace(txt_Address.Text) || txtPort.Value < 0;
			if (can)
				btnAdd.Sensitive = false;
			else if (chk_username.Active)
				btnAdd.Sensitive = !String.IsNullOrEmpty(txt_userName.Text) && !String.IsNullOrEmpty(txt_password.Text);
			else
				btnAdd.Sensitive = true;
		}
	}
}

