using System;
using System.Threading;
using System.Reflection;
using System.Resources;

namespace Agrin.Mono.UI
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ReportBugs : Gtk.Bin
	{
		public ReportBugs()
		{
			this.Build();
		}

		protected void OnBtnSendClicked(object sender, EventArgs e)
		{

			txtName.Sensitive = txt_Text.Sensitive = txt_subject.Sensitive = txt_email.Sensitive = btnSend.Sensitive = false;
			lbl_message.LabelProp = "در حال ارسال...";
			Thread thread = new Thread(() =>
			{
			bool isSend = false;
			try
			{

				System.IO.Stream stream =(System.IO.Stream)Assembly.GetExecutingAssembly().GetManifestResourceStream("Agrin.Mono.UI.Resources.Agrin.About.dll");
				stream.Seek(0,System.IO.SeekOrigin.Begin);
				byte[] bytes = new byte[stream.Length];
				stream.Read(bytes,0,bytes.Length);
				Assembly assembly = Assembly.Load(bytes);
				var msg = Activator.CreateInstance(assembly.GetType("Agrin.About.SendMessage"));
				MethodInfo method = msg.GetType().GetMethods()[0];
				isSend = (bool)method.Invoke(msg, new object[] { txtName.Text, txt_email.Text, txt_subject.Text, txt_Text.Text, null });
				//Agrin.About.SendMessage a = new About.SendMessage();
				//a.SendMessages(Name, Mail, Title, Content);
			}
			catch (Exception c)
			{
				Gtk.MessageDialog dlg = new Gtk.MessageDialog(MainWindow.This, Gtk.DialogFlags.Modal, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, c.Message);
				dlg.Run();
			}
			finally
			{
				txtName.Sensitive = txt_Text.Sensitive = txt_subject.Sensitive = txt_email.Sensitive = btnSend.Sensitive = true;
				if (isSend)
				{
					lbl_message.LabelProp = "با تشکر از شما پیغام شما با موفقیت ارسال شد در صورت لزوم به ایمیل شما پاسخ داده خواهد شد.";
					txtName.Text = txt_email.Text = txt_subject.Text = txt_Text.Text = "";
				}
				else
				{
					lbl_message.LabelProp = "خطا در ارسال پیغام شما رخ داده است.";
				}
			}
			});
			thread.Start();
		}
	}
}

