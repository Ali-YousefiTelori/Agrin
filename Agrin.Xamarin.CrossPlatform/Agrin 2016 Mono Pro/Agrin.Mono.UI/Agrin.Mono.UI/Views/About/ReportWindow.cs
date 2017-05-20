using System;
using Agrin.BaseViewModels.Help;

namespace Agrin.Mono.UI
{
	public partial class ReportWindow : Gtk.Window
	{
		public ReportWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			BindingHelper.BindOneWay (this, lbl_message, "LabelProp", feedBackBaseViewModel, "BusyMessage");
			BindingHelper.BindTwoway (this, txt_Text, "Text", feedBackBaseViewModel, "Message");
			BindingHelper.BindAction (this, feedBackBaseViewModel, new System.Collections.Generic.List<string> () {
				"IsBusy",
				"Message",
				"MessageBoxMessage"
			},
				(name) => {
					this.RunOnUI (() => {
						if (!string.IsNullOrEmpty (feedBackBaseViewModel.MessageBoxMessage))
							lbl_message.Text = feedBackBaseViewModel.MessageBoxMessage;
						RefreshButtons ();
					});
				});
			RefreshButtons ();
		}

		public void RefreshButtons ()
		{
			btnSend.Sensitive = feedBackBaseViewModel.CanSendMessage () && !feedBackBaseViewModel.IsBusy && !feedBackBaseViewModel.IsMessageBoxBusy; 
		}

		FeedBackBaseViewModel feedBackBaseViewModel = new FeedBackBaseViewModel ();

		protected void OnBtnSendClicked (object sender, EventArgs e)
		{
			feedBackBaseViewModel.SendMessage ();
		}
	}
}

