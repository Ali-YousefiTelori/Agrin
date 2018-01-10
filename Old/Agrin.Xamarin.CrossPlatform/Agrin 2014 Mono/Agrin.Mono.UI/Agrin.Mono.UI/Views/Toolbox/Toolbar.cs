using System;
using System.Linq;
using System.Collections.Generic;
using Agrin.Download.Web;
using Agrin.Download.Manager;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class Toolbar : Gtk.Bin
	{
		public Toolbar()
		{
			this.Build();
			LinksList.SelectionChanged = () => {
				InitToolbarSelectionItems();
			};
			This = this;
		}

		public static Toolbar This;

		protected void OnButton1Clicked(object sender, EventArgs e)
		{
			using (Gtk.Dialog window = new Gtk.Dialog ("درج لینک", MainWindow.This, Gtk.DialogFlags.Modal, new object[]{ }))
			{

				window.WidthRequest = 400;
				window.VBox.BorderWidth = 50;
				using (AddLinks addlinks = new AddLinks())
				{
					addlinks.DialogResult = () => {
						window.Destroy();
					};
					window.VBox.Add(addlinks);
					//addlinks.comboBox.MainControlWindow = window;
					window.ShowAll();
					addlinks.Settings.FontName = "segoe ui";
					window.Run();
					window.Destroy();
					BindingHelper.DisposeBindingOneWay(addlinks);
				}
			}
		}

		public void OnButton6Clicked(object sender, EventArgs e)
		{
			using (Gtk.Dialog window = new Gtk.Dialog ("درج گروه", MainWindow.This, Gtk.DialogFlags.Modal, new object[]{ }))
			{
				window.WidthRequest = 300;
				window.VBox.BorderWidth = 50;
				using (AddGroup addGroup = new AddGroup())
				{
					addGroup.DialogResult = () => {
						window.Destroy();
					};
					window.VBox.Add(addGroup);
					//addlinks.comboBox.MainControlWindow = window;
					window.ShowAll();
					window.Run();
					window.Destroy();
					BindingHelper.DisposeBindingOneWay(addGroup);
				}
			}
		}

		public void InitToolbarSelectionItems(bool setLinkInfo=true)
		{
			var list = LinksList.This.GetSelectionsIters();
			if (list.Count == 0)
			{
				btnPlay.Sensitive = btnStop.Sensitive = btnDelete.Sensitive = false;
				LinkInfoDownloadDetail.This.Visible = false;
				LinkInfoDownloadDetail.This.SetLinkInfo(null);
			}
			else
			{
				int canPlay = 0, canStop = 0, canDelete = 0;
				LinkInfo first = null;
				foreach (var item in list)
				{
					var iter = item;
					LinkInfo value = (LinkInfo)LinksList.This.store.GetValue(iter, 0);
					if (first == null)
						first = value;
					if (value.CanPlay)
						canPlay++;
					if (value.CanStop)
						canStop++;
					if (value.CanDelete)
						canDelete++;
				}
				if (setLinkInfo)
				{
				if (first != null && ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Contains(first))
				{
					if (LinkInfoDownloadDetail.This.CurrentLinkInfo != first)
					{
						LinkInfoDownloadDetail.This.SetLinkInfo(first);
						LinkInfoDownloadDetail.This.Visible = true;
					}
				}
				else
				{
					LinkInfoDownloadDetail.This.Visible = false;
					LinkInfoDownloadDetail.This.SetLinkInfo(null);
				}
				}
				btnPlay.Sensitive = canPlay > 0;
				btnStop.Sensitive = canStop > 0;
				btnDelete.Sensitive = canDelete > 0;

			}
		}

		public void OnBtnDeleteClicked(object sender, EventArgs e)
		{
			List<LinkInfo> infoes = new List<LinkInfo>();
			var selections = LinksList.This.GetSelectionsIters();
			if (selections.Count == 0)
				return;
			Gtk.MessageDialog m = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, false, "آیا میخواهید لینک های انتخاب شده را حذف کنید؟");
			Gtk.ResponseType result = (Gtk.ResponseType)m.Run();
			m.Destroy();
			if (result == Gtk.ResponseType.Yes)
			{
				try
				{

				foreach (var item in selections)
				{
					var iter = item;
					LinkInfo value = (LinkInfo)LinksList.This.store.GetValue(iter, 0);
						value.Dispose();
					bool remove = LinksList.This.store.Remove(ref iter);
					infoes.Add(value);
					LinksList.This.RemoveBinding(value);
				}
				}
				catch(Exception ee)
				{
				}

				ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(infoes);

				InitToolbarSelectionItems();
			}
		}

		protected void OnBtnPlayClicked(object sender, EventArgs e)
		{
			foreach (var item in LinksList.This.GetSelectionsIters())
			{
				var iter = item;
				LinkInfo value = (LinkInfo)LinksList.This.store.GetValue(iter, 0);
				if (value.CanPlay)
					ApplicationLinkInfoManager.Current.PlayLinkInfo(value);
			}
			InitToolbarSelectionItems();
		}

		protected void OnBtnStopClicked(object sender, EventArgs e)
		{
			foreach (var item in LinksList.This.GetSelectionsIters())
			{
				var iter = item;
				LinkInfo value = (LinkInfo)LinksList.This.store.GetValue(iter, 0);
				if (value.CanStop)
				{
					ApplicationLinkInfoManager.Current.StopLinkInfo(value);
					ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Remove(value);
				}
			}
			InitToolbarSelectionItems();
		}

		protected void OnBtnSettingClicked (object sender, EventArgs e)
		{
			using (Gtk.Dialog window = new Gtk.Dialog ("تنظیمات", MainWindow.This, Gtk.DialogFlags.Modal, new object[]{ }))
			{

				window.WidthRequest = 600;
				window.HeightRequest = 400;
				window.VBox.BorderWidth = 50;
				using (LinkInfoSetting linkInfoSetting = new LinkInfoSetting())
				{

					linkInfoSetting.AppSettingToUI();
					linkInfoSetting.Direction = Gtk.TextDirection.Rtl;
					linkInfoSetting.CloseButton=()=>
					{
						linkInfoSetting.Dispose();
						window.Destroy();
					};
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
		}
	}
}

