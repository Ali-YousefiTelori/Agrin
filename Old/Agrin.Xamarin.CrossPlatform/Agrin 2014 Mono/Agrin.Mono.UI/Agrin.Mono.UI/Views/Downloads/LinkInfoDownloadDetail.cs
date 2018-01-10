using System;
using Gtk;
using Agrin.Download.Web.Creator;
using Agrin.Download.Web;
using System.Collections.Generic;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkInfoDownloadDetail : Gtk.Bin
	{
		public TreeStore store;

		public LinkInfoDownloadDetail()
		{
			this.Build();
			this.Direction = Gtk.TextDirection.Rtl;
			This = this;
			store = new TreeStore(typeof(ConnectionInfo));

			tv.Model = store;
			tv.HeadersVisible = true;
			tv.AppendColumn("کلید", new CellRendererText(), RenderID);
			tv.AppendColumn("وضعیت", new CellRendererText(), RenderState);
			tv.AppendColumn("حجم", new CellRendererText(), RenderSize);
			tv.AppendColumn("دریافت شده", new CellRendererText(), RenderDownloaded);
			tv.AppendColumn("پیشرفت", new CellRendererProgress() { Width = 150 }, RenderProgress);
			//tv.AppendColumn("شروع", new CellRenderer(), RenderStart);
			//tv.AppendColumn("ایست", new CellRendererText(), RenderStop);

			//tv.ButtonReleaseEvent += OnTvButtonReleaseEvent;
			this.Add(tv);
			this.ShowAll();
		}

		public static LinkInfoDownloadDetail This;

		public void RenderID(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ConnectionInfo value = (ConnectionInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.ConnectionId.ToString();
		}

		public void RenderState(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ConnectionInfo value = (ConnectionInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = value.State.ToString();
		}

		public void RenderSize(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ConnectionInfo value = (ConnectionInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum(value.Length);
		}

		public void RenderDownloaded(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ConnectionInfo value = (ConnectionInfo)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum(value.DownloadedSize);
			if (CurrentLinkInfo != null)
				lbl_downloaded.Text = Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum(CurrentLinkInfo.DownloadingProperty.DownloadedSize);
		}

		public void RenderProgress(TreeViewColumn col, CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			ConnectionInfo value = (ConnectionInfo)model.GetValue(iter, 0);
			double val = ((double)value.DownloadedSize / value.Length);
			(cell as Gtk.CellRendererProgress).Value = int.Parse(((int)(val * 100.0)).ToString ());
			(cell as Gtk.CellRendererProgress).Text = value.Length < 0 ? "نامشخص" : String.Format("{0:00.00%}", val);
		}

		LinkInfo _CurrentLinkInfo;

		public LinkInfo CurrentLinkInfo
		{
			get
			{
				return _CurrentLinkInfo;
			}
			set
			{
				_CurrentLinkInfo = value;
			}
		}

		System.Collections.DictionaryEntry bindingItems;
		object lockOBJ = new object();

		public void SetLinkInfo(LinkInfo info)
		{
			lock (lockOBJ)
			{
				if (CurrentLinkInfo != null)
				{
					store.Clear();//lbl_tavagh
					BindingHelper.DisposeBindingAction((Guid)bindingItems.Key);
					foreach (var item in (List<Guid>)bindingItems.Value)
					{
						BindingHelper.DisposeBindingAction((Guid)item);
					}
					CurrentLinkInfo.Connections.ChangedCollection = null;
				}
				if (info == null)
				{
					CurrentLinkInfo = null;
					return;
				}
				bindingItems = new System.Collections.DictionaryEntry();
				lbl_address.Text = info.PathInfo.Address;
				bindingItems.Key = Guid.NewGuid();
				bool setting = false;
				BindingHelper.AddActionPropertyChanged((str) =>{
				if (str == "ResumeCapability")
					lbl_tavaghof.LabelProp=info.DownloadingProperty.ResumeCapability.ToString();
				else if (str == "SpeedByteDownloaded")
					lbl_Speed.LabelProp=Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum(info.DownloadingProperty.SpeedByteDownloaded);
			}, info.DownloadingProperty, new List<string>() { "ResumeCapability", "SpeedByteDownloaded" }, (Guid)bindingItems.Key);

				//BindingHelper.AddBindingOneWayConverter(this, info.DownloadingProperty, "DownloadedSize",lbl_downloaded, "LabelProp",(value)=>
				//{
				//	return (string) Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum(double.Parse(value.ToString()));
				//});
			

				//BindingHelper.AddActionPropertyChanged(() =>{
				//	lock(lockobj)
				//	lbl_downloaded.Text=Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum(info.DownloadingProperty.DownloadedSize);
				//}, info.DownloadingProperty, new List<string>() { "DownloadedSize" }, (Guid)bindingItems.Key);
				List<Guid> items = new List<Guid>();
				List<ConnectionInfo> addedItems = new List<ConnectionInfo>();
				foreach (var item in info.Connections)
				{
					Guid guid = Guid.NewGuid();
					AddBinding(store.AppendValues(item), item, guid);
					items.Add(guid);
					addedItems.Add(item);
				}
				bindingItems.Value = items;
				CurrentLinkInfo = info;
				info.Connections.ChangedCollection = () =>
				{
					lock (lockOBJ)
					{
						if (CurrentLinkInfo == info)
						{
							foreach (var item in info.Connections)
							{
								if (!addedItems.Contains(item))
								{
									Guid guid = Guid.NewGuid();
									AddBinding(store.AppendValues(item), item, guid);
									items.Add(guid);
									addedItems.Add(item);
								}
							}
						}
					}
				};

			}
		}

		void AddBinding(TreeIter iter, ConnectionInfo connectionInfo, Guid guid)
		{

			BindingHelper.AddActionPropertyChanged((name) =>
			{
				var pth = tv.Model.GetPath (iter);
				store.EmitRowChanged (pth, iter);
			}, connectionInfo, new List<string>() { "StartPosition", "EndPosition", "DownloadedSize", "Lenght", "State" }, guid);

		}

		protected void OnBtnCancelClicked(object sender, EventArgs e)
		{
			Agrin.Download.Manager.ApplicationLinkInfoManager.Current.StopLinkInfo(CurrentLinkInfo);
			Agrin.Download.Manager.ApplicationLinkInfoManager.Current.DisposeLinkInfo(CurrentLinkInfo);
			Agrin.Download.Manager.ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Remove(CurrentLinkInfo);
			Toolbar.This.InitToolbarSelectionItems();
			SetLinkInfo(null);
		}
	}
}

