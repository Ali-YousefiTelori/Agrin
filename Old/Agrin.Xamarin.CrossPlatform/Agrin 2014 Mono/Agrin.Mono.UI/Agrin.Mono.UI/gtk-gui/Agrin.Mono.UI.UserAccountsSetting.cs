
// This file has been generated by the GUI designer. Do not modify.
namespace Agrin.Mono.UI
{
	public partial class UserAccountsSetting
	{
		private global::Gtk.VBox vbox9;

		private global::Gtk.HBox hbox20;

		private global::Gtk.Label lbl_nullAddress;

		private global::Gtk.Entry txtAddress;

		private global::Gtk.Label lbl_address;

		private global::Gtk.HBox hbox21;

		private global::Gtk.Label label25;

		private global::Gtk.Entry txt_UserName;

		private global::Gtk.Label label22;

		private global::Gtk.HBox hbox22;

		private global::Gtk.Button btnAdd;

		private global::Gtk.Entry txtPassword;

		private global::Gtk.Label label23;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TreeView tv;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget Agrin.Mono.UI.UserAccountsSetting
			global::Stetic.BinContainer.Attach(this);
			this.Name = "Agrin.Mono.UI.UserAccountsSetting";
			// Container child Agrin.Mono.UI.UserAccountsSetting.Gtk.Container+ContainerChild
			this.vbox9 = new global::Gtk.VBox();
			this.vbox9.Name = "vbox9";
			this.vbox9.Spacing = 6;
			// Container child vbox9.Gtk.Box+BoxChild
			this.hbox20 = new global::Gtk.HBox();
			this.hbox20.Name = "hbox20";
			this.hbox20.Spacing = 6;
			// Container child hbox20.Gtk.Box+BoxChild
			this.lbl_nullAddress = new global::Gtk.Label();
			this.lbl_nullAddress.WidthRequest = 30;
			this.lbl_nullAddress.Name = "lbl_nullAddress";
			this.hbox20.Add(this.lbl_nullAddress);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox20[this.lbl_nullAddress]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox20.Gtk.Box+BoxChild
			this.txtAddress = new global::Gtk.Entry();
			this.txtAddress.CanFocus = true;
			this.txtAddress.Name = "txtAddress";
			this.txtAddress.IsEditable = true;
			this.txtAddress.InvisibleChar = '●';
			this.hbox20.Add(this.txtAddress);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox20[this.txtAddress]));
			w2.Position = 1;
			// Container child hbox20.Gtk.Box+BoxChild
			this.lbl_address = new global::Gtk.Label();
			this.lbl_address.WidthRequest = 80;
			this.lbl_address.Name = "lbl_address";
			this.lbl_address.Xalign = 1F;
			this.lbl_address.LabelProp = global::Mono.Unix.Catalog.GetString("آدرس:");
			this.hbox20.Add(this.lbl_address);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox20[this.lbl_address]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			this.vbox9.Add(this.hbox20);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox9[this.hbox20]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox9.Gtk.Box+BoxChild
			this.hbox21 = new global::Gtk.HBox();
			this.hbox21.Name = "hbox21";
			this.hbox21.Spacing = 6;
			// Container child hbox21.Gtk.Box+BoxChild
			this.label25 = new global::Gtk.Label();
			this.label25.WidthRequest = 30;
			this.label25.Name = "label25";
			this.hbox21.Add(this.label25);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox21[this.label25]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox21.Gtk.Box+BoxChild
			this.txt_UserName = new global::Gtk.Entry();
			this.txt_UserName.CanFocus = true;
			this.txt_UserName.Name = "txt_UserName";
			this.txt_UserName.IsEditable = true;
			this.txt_UserName.InvisibleChar = '●';
			this.hbox21.Add(this.txt_UserName);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox21[this.txt_UserName]));
			w6.Position = 1;
			// Container child hbox21.Gtk.Box+BoxChild
			this.label22 = new global::Gtk.Label();
			this.label22.WidthRequest = 80;
			this.label22.Name = "label22";
			this.label22.Xalign = 1F;
			this.label22.LabelProp = global::Mono.Unix.Catalog.GetString("نام کاربری:");
			this.hbox21.Add(this.label22);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox21[this.label22]));
			w7.Position = 2;
			w7.Expand = false;
			w7.Fill = false;
			this.vbox9.Add(this.hbox21);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox9[this.hbox21]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			// Container child vbox9.Gtk.Box+BoxChild
			this.hbox22 = new global::Gtk.HBox();
			this.hbox22.Name = "hbox22";
			this.hbox22.Spacing = 6;
			// Container child hbox22.Gtk.Box+BoxChild
			this.btnAdd = new global::Gtk.Button();
			this.btnAdd.WidthRequest = 30;
			this.btnAdd.CanFocus = true;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.UseUnderline = true;
			global::Gtk.Image w9 = new global::Gtk.Image();
			w9.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-add", global::Gtk.IconSize.SmallToolbar);
			this.btnAdd.Image = w9;
			this.hbox22.Add(this.btnAdd);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox22[this.btnAdd]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child hbox22.Gtk.Box+BoxChild
			this.txtPassword = new global::Gtk.Entry();
			this.txtPassword.CanFocus = true;
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.IsEditable = true;
			this.txtPassword.InvisibleChar = '●';
			this.hbox22.Add(this.txtPassword);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox22[this.txtPassword]));
			w11.Position = 1;
			// Container child hbox22.Gtk.Box+BoxChild
			this.label23 = new global::Gtk.Label();
			this.label23.WidthRequest = 80;
			this.label23.Name = "label23";
			this.label23.Xalign = 1F;
			this.label23.LabelProp = global::Mono.Unix.Catalog.GetString("رمز عبور:");
			this.hbox22.Add(this.label23);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox22[this.label23]));
			w12.Position = 2;
			w12.Expand = false;
			w12.Fill = false;
			this.vbox9.Add(this.hbox22);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox9[this.hbox22]));
			w13.Position = 2;
			w13.Expand = false;
			w13.Fill = false;
			// Container child vbox9.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.tv = new global::Gtk.TreeView();
			this.tv.CanFocus = true;
			this.tv.Name = "tv";
			this.GtkScrolledWindow.Add(this.tv);
			this.vbox9.Add(this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox9[this.GtkScrolledWindow]));
			w15.Position = 3;
			this.Add(this.vbox9);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
			this.txtAddress.Changed += new global::System.EventHandler(this.OnTxtAddressChanged);
			this.txt_UserName.Changed += new global::System.EventHandler(this.OnTxtUserNameChanged);
			this.btnAdd.Clicked += new global::System.EventHandler(this.OnBtnAddClicked);
			this.txtPassword.Changed += new global::System.EventHandler(this.OnTxtPasswordChanged);
		}
	}
}
