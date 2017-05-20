using System;
using Gtk;
using Agrin.Download.Web;
using Agrin.Download.Manager;
using System.Collections.Generic;
using System.Linq;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class GroupsList : Gtk.Bin
	{
		public static GroupsList This;
		TreeStore store;

		public GroupsList ()
		{
			this.Build ();
			hbox1.Direction = tv.Direction = TextDirection.Rtl;
			store = new TreeStore (typeof(GroupInfo));
			tv.Model = store;
			tv.HeadersVisible = true;
			tv.AppendColumn ("نام گروه", new CellRendererText (), RenderName);
			btnRemove.Sensitive = GetSelectionsIters ().Count > 0;
			tv.Selection.Changed += (sender, e) => {
				btnRemove.Sensitive = GetSelectionsIters ().Count > 0;
			};
			This = this;
		}

		public List<Gtk.TreeIter> GetSelectionsIters ()
		{
			var list = tv.Selection.GetSelectedRows ().ToList ();
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

		public void RenderName (TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			GroupInfo value = (GroupInfo)model.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = value.Name;
		}

		public void LoadgroupInfoesData ()
		{
			foreach (var item in ApplicationGroupManager.Current.GroupInfoes) {
				store.AppendValues (item);
			}
		}

		public void AddGroupInfo (GroupInfo groupInfo)
		{
			ApplicationGroupManager.Current.AddGroupInfo (groupInfo);
			//TreeIter iter = store.AppendValues (50);
			store.AppendValues (groupInfo);
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			Toolbar.This.OnButton6Clicked (null, null);
		}

		protected void OnBtnRemoveClicked (object sender, EventArgs e)
		{
			List<GroupInfo> infoes = new List<GroupInfo> ();
			foreach (var item in GetSelectionsIters()) {
				var iter = item;
				GroupInfo value = (GroupInfo)store.GetValue (iter, 0);
				bool remove =	store.Remove (ref iter);
				ApplicationGroupManager.Current.DeleteGroupInfo (value);
			}
			btnRemove.Sensitive = GetSelectionsIters ().Count > 0;
		}
	}
}

