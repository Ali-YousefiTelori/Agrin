using System;
using Agrin.Download.Data.Settings;
using Agrin.Download.Web;
using System.Linq;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkInfoSetting : Gtk.Bin
	{
		public static LinkInfoSetting This;

		public Action CloseButton;
		public LinkInfoSetting()
		{
			This = this;
			this.Build();
			notebook1.Direction = Gtk.TextDirection.Rtl;

		}

		bool _IsApplicationSetting;

		public bool IsApplicationSetting
		{
			get
			{
				return _IsApplicationSetting;
			}
			set
			{
				_IsApplicationSetting = value;
			}
		}

		LinkInfo _CurrentLinkInfo;
		public LinkInfo CurrentLinkInfo
		{
			get { return _CurrentLinkInfo; }
			set { _CurrentLinkInfo = value; }
		}

		private void SaveSetting()
		{
			if (CurrentLinkInfo == null)
				UIToAppSetting();
			else
				UIToLinkInfoSetting(CurrentLinkInfo);
		}

		public void AppSettingToUI()
		{
			IsApplicationSetting = true;
			CurrentLinkInfo = null;
			hbox13.Visible = true;
			this.notebook1.RemovePage(0);
			this.notebook1.RemovePage(3);
			//CanSave = true;
			LinkInfoDownloadSetting.This.CboSystemEnd.Active = ApplicationSetting.Current.LinkInfoDownloadSetting.EndDownloadSelectedIndex;
			LinkInfoDownloadSetting.This.ChkEndDownload.Active = ApplicationSetting.Current.LinkInfoDownloadSetting.IsEndDownloaded;
			LinkInfoDownloadSetting.This.ChkExtreme.Active = ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme;
			LinkInfoDownloadSetting.This.TxtErrorCount.Value = ApplicationSetting.Current.LinkInfoDownloadSetting.TryException;


			ProxySetting.This.AddRanges(ApplicationSetting.Current.ProxySetting.Items);
			//ProxySetting.This.IsNotNullProxy = true;

			SpeedSetting.This.TxtBufferSize.Value = ApplicationSetting.Current.SpeedSetting.BufferSize / 1024;
			SpeedSetting.This.TxtConnectionCount.Value = ApplicationSetting.Current.SpeedSetting.ConnectionCount;
			SpeedSetting.This.TxtSpeed.Value = ApplicationSetting.Current.SpeedSetting.SpeedSize / 1024;
			SpeedSetting.This.ChkIsLimit.Active = ApplicationSetting.Current.SpeedSetting.IsLimit;


			UserAccountsSetting.This.AddRange(ApplicationSetting.Current.UserAccountsSetting.Items);

			chkAllLink.Active = ApplicationSetting.Current.IsSettingForAllLinks;
			chkNewLink.Active = ApplicationSetting.Current.IsSettingForNewLinks;
		}

		public void UIToAppSetting()
		{
			ApplicationSetting.Current.LinkInfoDownloadSetting.EndDownloadSelectedIndex = LinkInfoDownloadSetting.This.CboSystemEnd.Active;
			ApplicationSetting.Current.LinkInfoDownloadSetting.IsEndDownloaded = LinkInfoDownloadSetting.This.ChkEndDownload.Active;
			ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme = LinkInfoDownloadSetting.This.ChkExtreme.Active;
			ApplicationSetting.Current.LinkInfoDownloadSetting.TryException = (int)LinkInfoDownloadSetting.This.TxtErrorCount.Value;

			ApplicationSetting.Current.ProxySetting.Items = ProxySetting.This.Items.ToList();

			ApplicationSetting.Current.SpeedSetting.BufferSize = (int)SpeedSetting.This.TxtBufferSize.Value * 1024;
			ApplicationSetting.Current.SpeedSetting.ConnectionCount = (int)SpeedSetting.This.TxtConnectionCount.Value;
			ApplicationSetting.Current.SpeedSetting.SpeedSize = (int)SpeedSetting.This.TxtSpeed.Value * 1024;
			ApplicationSetting.Current.SpeedSetting.IsLimit = SpeedSetting.This.ChkIsLimit.Active;

			ApplicationSetting.Current.UserAccountsSetting.Items = UserAccountsSetting.This.Items.ToList();

			ApplicationSetting.Current.IsSettingForAllLinks = chkAllLink.Active;
			ApplicationSetting.Current.IsSettingForNewLinks = chkNewLink.Active;

			Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
		}

		public void LinkInfoSettingToUI(LinkInfo linkInfo)
		{
			IsApplicationSetting = false;
			CurrentLinkInfo = linkInfo;
			vbox10.Remove(hbox13);
			//ViewElement.stackAllSetting.Visibility = Visibility.Collapsed;
			//CanSave = true;
			LinkInfoDownloadSetting.This.CboSystemEnd.Active = (int)linkInfo.Management.EndDownloadSystemMode;
			LinkInfoDownloadSetting.This.ChkEndDownload.Active = linkInfo.Management.IsEndDownload;
			LinkInfoDownloadSetting.This.ChkExtreme.Active = linkInfo.Management.IsTryExtreme;
			LinkInfoDownloadSetting.This.TxtErrorCount.Value = linkInfo.Management.TryAginCount;


			ProxySetting.This.AddRanges(linkInfo.Management.MultiProxy);

			SpeedSetting.This.TxtBufferSize.Value = linkInfo.Management.ReadBuffer / 1024;
			SpeedSetting.This.TxtConnectionCount.Value = linkInfo.DownloadingProperty.ConnectionCount;
			SpeedSetting.This.TxtSpeed.Value = linkInfo.Management.LimitPerSecound / 1024;
			SpeedSetting.This.ChkIsLimit.Active = linkInfo.Management.IsLimit;

			UserAccountsSetting.This.Items.Clear();
			if (linkInfo.Management.NetworkUserPass != null)
			{
				UserAccountsSetting.This.Txt_UserName.Text = linkInfo.Management.NetworkUserPass.UserName;
				UserAccountsSetting.This.TxtPassword.Text = linkInfo.Management.NetworkUserPass.Password;
			}


			GeneralSetting.This.Txt_FileName.Text = linkInfo.PathInfo.FileName;
			//GeneralSetting.This.FileType = linkInfo.PathInfo.FileType;
			GeneralSetting.This.Txt_Description.Text = linkInfo.Management.Description;
			GeneralSetting.This.Txt_SavePath.Text = linkInfo.PathInfo.SavePath;
			GeneralSetting.This.Lbl_Size.Text =  Agrin.Helper.Converters.MonoConverters.GetSizeStringEnum(linkInfo.DownloadingProperty.Size);

			//LinkAddressesSetting.This.UriAddress = "";
			//LinkAddressesSetting.This.IsEnabled = true;
			//LinkAddressesSetting.This.CurrentChecker = new LinkCheckerHelper();
			//LinkAddressesSetting.This.Messege = ApplicationHelper.GetAppResource("CheckLinksHelp_Language");
			//LinkAddressesSetting.This.Items.Clear();
			//List<MultiLinkAddress> items = LinkAddressesSetting.This.ToAMuliLinkAddress(linkInfo.Management.MultiLinks);
			LinkAddressesSetting.This.AddRange(linkInfo.Management.MultiLinks);
			LinkAddressesSetting.This.ResetProxyList();

		}
		public void UIToLinkInfoSetting(LinkInfo linkInfo)
		{
			//ViewElement.stackAllSetting.Visibility = Visibility.Collapsed;
			//CanSave = true;

			linkInfo.Management.EndDownloadSystemMode = (Download.Web.Link.CompleteDownloadSystemMode)LinkInfoDownloadSetting.This.CboSystemEnd.Active;
			linkInfo.Management.IsEndDownload = LinkInfoDownloadSetting.This.ChkEndDownload.Active;
			linkInfo.Management.IsTryExtreme = LinkInfoDownloadSetting.This.ChkExtreme.Active;
			linkInfo.Management.TryAginCount = (int)LinkInfoDownloadSetting.This.TxtErrorCount.Value;
			linkInfo.Management.IsApplicationSetting = false;

			linkInfo.Management.MultiProxy = ProxySetting.This.Items.ToList();

			linkInfo.Management.ReadBuffer = (int)SpeedSetting.This.TxtBufferSize.Value * 1024;
			linkInfo.DownloadingProperty.ConnectionCount = (int)SpeedSetting.This.TxtConnectionCount.Value;
			linkInfo.Management.LimitPerSecound = (int)SpeedSetting.This.TxtSpeed.Value * 1024;
			linkInfo.Management.IsLimit = SpeedSetting.This.ChkIsLimit.Active;

			if (!String.IsNullOrEmpty(UserAccountsSetting.This.Txt_UserName.Text) && !String.IsNullOrEmpty(UserAccountsSetting.This.TxtPassword.Text))
			{
				if (linkInfo.Management.NetworkUserPass == null)
					linkInfo.Management.NetworkUserPass = new Download.Web.Link.NetworkCredentialInfo();
				linkInfo.Management.NetworkUserPass.UserName = UserAccountsSetting.This.Txt_UserName.Text;
				linkInfo.Management.NetworkUserPass.Password = UserAccountsSetting.This.TxtPassword.Text;
			}
			else if (String.IsNullOrEmpty(UserAccountsSetting.This.Txt_UserName.Text) && String.IsNullOrEmpty(UserAccountsSetting.This.TxtPassword.Text))
			{
				linkInfo.Management.NetworkUserPass = null;
			}


			if (linkInfo.PathInfo.AddressFileName != GeneralSetting.This.Txt_FileName.Text)
				linkInfo.PathInfo.UserFileName = GeneralSetting.This.Txt_FileName.Text;
			linkInfo.Management.Description = GeneralSetting.This.Txt_Description.Text;
			if (linkInfo.PathInfo.AppSavePath != GeneralSetting.This.Txt_SavePath.Text)
				linkInfo.PathInfo.UserSavePath = GeneralSetting.This.Txt_SavePath.Text;

			//LinkAddressesSetting.This.ProxyToIds();
			linkInfo.Management.MultiLinks = LinkAddressesSetting.This.Items;

			linkInfo.SaveThisLink();

		}

		protected void OnBtnSaveClicked (object sender, EventArgs e)
		{
			if (IsApplicationSetting)
				UIToAppSetting();
			else
				UIToLinkInfoSetting(CurrentLinkInfo);
			CloseButton();
		}

		protected void OnButton1Clicked (object sender, EventArgs e)
		{
			CloseButton();
		}
	}
}

