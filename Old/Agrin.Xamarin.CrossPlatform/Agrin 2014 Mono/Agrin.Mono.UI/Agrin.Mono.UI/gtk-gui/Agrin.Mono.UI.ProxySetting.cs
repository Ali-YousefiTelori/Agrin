
// This file has been generated by the GUI designer. Do not modify.
namespace Agrin.Mono.UI
{
	public partial class ProxySetting
	{
		private global::Gtk.VBox vbox7;

		private global::Gtk.HBox hbox10;

		private global::Gtk.SpinButton txtPort;

		private global::Gtk.Label label11;

		private global::Gtk.Entry txt_Address;

		private global::Gtk.CheckButton chk_address;

		private global::Gtk.HBox hbox11;

		private global::Gtk.Button btn_sysProxy;

		private global::Gtk.Label label13;

		private global::Gtk.Entry txt_userName;

		private global::Gtk.CheckButton chk_username;

		private global::Gtk.HBox hbox12;

		private global::Gtk.Button btnAdd;

		private global::Gtk.Label label14;

		private global::Gtk.Entry txt_password;

		private global::Gtk.Label label12;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TreeView tv;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget Agrin.Mono.UI.ProxySetting
			global::Stetic.BinContainer.Attach(this);
			this.Name = "Agrin.Mono.UI.ProxySetting";
			// Container child Agrin.Mono.UI.ProxySetting.Gtk.Container+ContainerChild
			this.vbox7 = new global::Gtk.VBox();
			this.vbox7.Name = "vbox7";
			this.vbox7.Spacing = 6;
			// Container child vbox7.Gtk.Box+BoxChild
			this.hbox10 = new global::Gtk.HBox();
			this.hbox10.Name = "hbox10";
			this.hbox10.Spacing = 6;
			// Container child hbox10.Gtk.Box+BoxChild
			this.txtPort = new global::Gtk.SpinButton(0D, 9999D, 1D);
			this.txtPort.WidthRequest = 120;
			this.txtPort.CanFocus = true;
			this.txtPort.Name = "txtPort";
			this.txtPort.Adjustment.PageIncrement = 10D;
			this.txtPort.ClimbRate = 1D;
			this.txtPort.Numeric = true;
			this.hbox10.Add(this.txtPort);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox10[this.txtPort]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox10.Gtk.Box+BoxChild
			this.label11 = new global::Gtk.Label();
			this.label11.WidthRequest = 50;
			this.label11.Name = "label11";
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString("پورت:");
			this.hbox10.Add(this.label11);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox10[this.label11]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox10.Gtk.Box+BoxChild
			this.txt_Address = new global::Gtk.Entry();
			this.txt_Address.CanFocus = true;
			this.txt_Address.Name = "txt_Address";
			this.txt_Address.IsEditable = true;
			this.txt_Address.InvisibleChar = '●';
			this.hbox10.Add(this.txt_Address);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox10[this.txt_Address]));
			w3.Position = 2;
			// Container child hbox10.Gtk.Box+BoxChild
			this.chk_address = new global::Gtk.CheckButton();
			this.chk_address.WidthRequest = 120;
			this.chk_address.CanFocus = true;
			this.chk_address.Name = "chk_address";
			this.chk_address.Label = global::Mono.Unix.Catalog.GetString("آدرس پروکسی:");
			this.chk_address.Active = true;
			this.chk_address.DrawIndicator = true;
			this.chk_address.UseUnderline = true;
			this.hbox10.Add(this.chk_address);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox10[this.chk_address]));
			w4.Position = 3;
			w4.Expand = false;
			this.vbox7.Add(this.hbox10);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox7[this.hbox10]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.hbox11 = new global::Gtk.HBox();
			this.hbox11.Name = "hbox11";
			this.hbox11.Spacing = 6;
			// Container child hbox11.Gtk.Box+BoxChild
			this.btn_sysProxy = new global::Gtk.Button();
			this.btn_sysProxy.WidthRequest = 120;
			this.btn_sysProxy.CanFocus = true;
			this.btn_sysProxy.Name = "btn_sysProxy";
			this.btn_sysProxy.UseUnderline = true;
			this.btn_sysProxy.Label = global::Mono.Unix.Catalog.GetString("پروکسی سیستم");
			this.hbox11.Add(this.btn_sysProxy);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox11[this.btn_sysProxy]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox11.Gtk.Box+BoxChild
			this.label13 = new global::Gtk.Label();
			this.label13.WidthRequest = 50;
			this.label13.Name = "label13";
			this.hbox11.Add(this.label13);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox11[this.label13]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox11.Gtk.Box+BoxChild
			this.txt_userName = new global::Gtk.Entry();
			this.txt_userName.CanFocus = true;
			this.txt_userName.Name = "txt_userName";
			this.txt_userName.IsEditable = true;
			this.txt_userName.InvisibleChar = '●';
			this.hbox11.Add(this.txt_userName);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox11[this.txt_userName]));
			w8.Position = 2;
			// Container child hbox11.Gtk.Box+BoxChild
			this.chk_username = new global::Gtk.CheckButton();
			this.chk_username.WidthRequest = 120;
			this.chk_username.CanFocus = true;
			this.chk_username.Name = "chk_username";
			this.chk_username.Label = global::Mono.Unix.Catalog.GetString("نام کاربری:");
			this.chk_username.DrawIndicator = true;
			this.chk_username.UseUnderline = true;
			this.hbox11.Add(this.chk_username);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox11[this.chk_username]));
			w9.Position = 3;
			w9.Expand = false;
			w9.Fill = false;
			this.vbox7.Add(this.hbox11);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox7[this.hbox11]));
			w10.Position = 1;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.hbox12 = new global::Gtk.HBox();
			this.hbox12.Name = "hbox12";
			this.hbox12.Spacing = 6;
			// Container child hbox12.Gtk.Box+BoxChild
			this.btnAdd = new global::Gtk.Button();
			this.btnAdd.WidthRequest = 120;
			this.btnAdd.CanFocus = true;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.UseUnderline = true;
			global::Gtk.Image w11 = new global::Gtk.Image();
			w11.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-add", global::Gtk.IconSize.SmallToolbar);
			this.btnAdd.Image = w11;
			this.hbox12.Add(this.btnAdd);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox12[this.btnAdd]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child hbox12.Gtk.Box+BoxChild
			this.label14 = new global::Gtk.Label();
			this.label14.WidthRequest = 50;
			this.label14.Name = "label14";
			this.hbox12.Add(this.label14);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox12[this.label14]));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			// Container child hbox12.Gtk.Box+BoxChild
			this.txt_password = new global::Gtk.Entry();
			this.txt_password.CanFocus = true;
			this.txt_password.Name = "txt_password";
			this.txt_password.IsEditable = true;
			this.txt_password.InvisibleChar = '●';
			this.hbox12.Add(this.txt_password);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox12[this.txt_password]));
			w14.Position = 2;
			// Container child hbox12.Gtk.Box+BoxChild
			this.label12 = new global::Gtk.Label();
			this.label12.WidthRequest = 120;
			this.label12.Name = "label12";
			this.label12.Xalign = 1F;
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString("رمز عبور:");
			this.hbox12.Add(this.label12);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox12[this.label12]));
			w15.Position = 3;
			w15.Expand = false;
			w15.Fill = false;
			this.vbox7.Add(this.hbox12);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox7[this.hbox12]));
			w16.Position = 2;
			w16.Expand = false;
			w16.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.tv = new global::Gtk.TreeView();
			this.tv.CanFocus = true;
			this.tv.Name = "tv";
			this.GtkScrolledWindow.Add(this.tv);
			this.vbox7.Add(this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox7[this.GtkScrolledWindow]));
			w18.Position = 3;
			this.Add(this.vbox7);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
			this.txt_Address.Changed += new global::System.EventHandler(this.OnTxtAddressChanged);
			this.chk_address.Toggled += new global::System.EventHandler(this.OnChkAddressToggled);
			this.btn_sysProxy.Clicked += new global::System.EventHandler(this.OnBtnSysProxyClicked);
			this.txt_userName.Changed += new global::System.EventHandler(this.OnTxtUserNameChanged);
			this.chk_username.Toggled += new global::System.EventHandler(this.OnChkUsernameToggled);
			this.btnAdd.Clicked += new global::System.EventHandler(this.OnBtnAddClicked);
			this.txt_password.Changed += new global::System.EventHandler(this.OnTxtPasswordChanged);
		}
	}
}
