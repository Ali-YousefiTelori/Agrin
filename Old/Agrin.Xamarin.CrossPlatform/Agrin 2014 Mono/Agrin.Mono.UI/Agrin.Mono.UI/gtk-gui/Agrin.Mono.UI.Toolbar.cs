
// This file has been generated by the GUI designer. Do not modify.
namespace Agrin.Mono.UI
{
	public partial class Toolbar
	{
		private global::Gtk.VBox vbox1;

		private global::Gtk.Button button1;

		private global::Gtk.Button button6;

		private global::Gtk.Button btnPlay;

		private global::Gtk.Button btnStop;

		private global::Gtk.Button btn_Setting;

		private global::Gtk.Button btnDelete;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget Agrin.Mono.UI.Toolbar
			global::Stetic.BinContainer.Attach(this);
			this.Name = "Agrin.Mono.UI.Toolbar";
			// Container child Agrin.Mono.UI.Toolbar.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.button1 = new global::Gtk.Button();
			this.button1.CanFocus = true;
			this.button1.Name = "button1";
			this.button1.UseUnderline = true;
			global::Gtk.Image w1 = new global::Gtk.Image();
			w1.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-add", global::Gtk.IconSize.LargeToolbar);
			this.button1.Image = w1;
			this.vbox1.Add(this.button1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.button1]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.button6 = new global::Gtk.Button();
			this.button6.CanFocus = true;
			this.button6.Name = "button6";
			this.button6.UseUnderline = true;
			global::Gtk.Image w3 = new global::Gtk.Image();
			w3.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-dnd-multiple", global::Gtk.IconSize.LargeToolbar);
			this.button6.Image = w3;
			this.vbox1.Add(this.button6);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.button6]));
			w4.Position = 1;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.btnPlay = new global::Gtk.Button();
			this.btnPlay.CanFocus = true;
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.UseUnderline = true;
			global::Gtk.Image w5 = new global::Gtk.Image();
			w5.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-media-play", global::Gtk.IconSize.LargeToolbar);
			this.btnPlay.Image = w5;
			this.vbox1.Add(this.btnPlay);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.btnPlay]));
			w6.Position = 2;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.btnStop = new global::Gtk.Button();
			this.btnStop.CanFocus = true;
			this.btnStop.Name = "btnStop";
			this.btnStop.UseUnderline = true;
			global::Gtk.Image w7 = new global::Gtk.Image();
			w7.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-media-stop", global::Gtk.IconSize.LargeToolbar);
			this.btnStop.Image = w7;
			this.vbox1.Add(this.btnStop);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.btnStop]));
			w8.Position = 3;
			w8.Expand = false;
			w8.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.btn_Setting = new global::Gtk.Button();
			this.btn_Setting.CanFocus = true;
			this.btn_Setting.Name = "btn_Setting";
			this.btn_Setting.UseUnderline = true;
			global::Gtk.Image w9 = new global::Gtk.Image();
			w9.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-execute", global::Gtk.IconSize.LargeToolbar);
			this.btn_Setting.Image = w9;
			this.vbox1.Add(this.btn_Setting);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.btn_Setting]));
			w10.Position = 4;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.btnDelete = new global::Gtk.Button();
			this.btnDelete.CanFocus = true;
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.UseUnderline = true;
			global::Gtk.Image w11 = new global::Gtk.Image();
			w11.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-delete", global::Gtk.IconSize.LargeToolbar);
			this.btnDelete.Image = w11;
			this.vbox1.Add(this.btnDelete);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.btnDelete]));
			w12.Position = 5;
			w12.Expand = false;
			w12.Fill = false;
			this.Add(this.vbox1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
			this.button1.Clicked += new global::System.EventHandler(this.OnButton1Clicked);
			this.button6.Clicked += new global::System.EventHandler(this.OnButton6Clicked);
			this.btnPlay.Clicked += new global::System.EventHandler(this.OnBtnPlayClicked);
			this.btnStop.Clicked += new global::System.EventHandler(this.OnBtnStopClicked);
			this.btn_Setting.Clicked += new global::System.EventHandler(this.OnBtnSettingClicked);
			this.btnDelete.Clicked += new global::System.EventHandler(this.OnBtnDeleteClicked);
		}
	}
}
