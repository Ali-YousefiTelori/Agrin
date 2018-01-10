using System;
using Gtk;
using Agrin.Download.Web.Link;
using System.Collections.Generic;
using System.Linq;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class UserAccountsSetting : Gtk.Bin
	{
		public static UserAccountsSetting This;
		public TreeStore store;
		public UserAccountsSetting()
		{
			This = this;
			this.Build();
			store = new TreeStore(typeof(NetworkCredentialInfo));
			tv.Model = store;
			tv.HeadersVisible = true;
			tv.AppendColumn("آدرس", new CellRendererText(), RenderAddress);
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

		public Gtk.Entry Txt_UserName
		{
			get
			{
				return txt_UserName;
			}
		}

		public Gtk.Entry TxtPassword
		{
			get
			{
				return txtPassword;
			}
		}

		bool _IsApplicationSetting = true;

		public bool IsApplicationSetting
		{
			get { return _IsApplicationSetting; }
			set 
			{
				_IsApplicationSetting = value; 
				InitControl();
			}
		}

		public void InitControl()
		{
			btnAdd.Sensitive = !(String.IsNullOrEmpty(txtAddress.Text) || String.IsNullOrEmpty(txt_UserName.Text) || String.IsNullOrEmpty(txtPassword.Text));
			if (IsApplicationSetting)
			{
				tv.Visible= hbox20.Visible = true;
			}
			else
			{
				tv.Visible= hbox20.Visible = false;
			}
		}

		public List<NetworkCredentialInfo> Items = new List<NetworkCredentialInfo>();

		public void AddRange(List<NetworkCredentialInfo> items)
		{
			foreach (var item in items)
			{
				Items.Add(item);
				store.AppendValues(item);
			}
		}
		void ClickedDeleteTreeIter(object sender,EventArgs e)
		{
			TreeIter iter;

			if (store.GetIter(out iter, tv.Selection.GetSelectedRows()[0]))
			{
				//Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, arg.Path);
				//m.Run();
				NetworkCredentialInfo old = (NetworkCredentialInfo)store.GetValue(iter, 0);
				store.Remove(ref iter);
				Items.Remove(old);
			}
		}

		void checkedToggleButton(object sender,ToggledArgs arg)
		{
			TreeIter iter;
			if (store.GetIter(out iter, new TreePath(arg.Path)))
			{
				//Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, arg.Path);
				//m.Run();
				NetworkCredentialInfo old = (NetworkCredentialInfo)store.GetValue(iter, 0);
				old.IsSelected = !old.IsSelected;
			}
		}

		public void RenderAddress(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			NetworkCredentialInfo value = (NetworkCredentialInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.ServerAddress;
		}

		public void RenderUserName(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			NetworkCredentialInfo value = (NetworkCredentialInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.UserName;
		}

		public void RendeUse(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			NetworkCredentialInfo value = (NetworkCredentialInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererToggle).Active = value.IsSelected;
		}
		public void RenderDelete(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			//ProxyInfo value = (ProxyInfo)model.GetValue(iter, 0);
			//(cell as Gtk.CellRendererSpin).Text = value.FullAddress;
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			var info=new NetworkCredentialInfo() { IsSelected = true, Password = txtPassword.Text, ServerAddress = txtAddress.Text, UserName = txt_UserName.Text };
			Items.Add(info);
			txtPassword.Text = txt_UserName.Text = txtAddress.Text = "";
			store.AppendValues(info);
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
	}
}

