using System;
using Gtk;

namespace Agrin.Mono.UI
{
	public class CellRendererButton : CellRenderer
	{
		Gdk.Pixbuf pixbuf;

		public CellRendererButton (Gdk.Pixbuf pixbuf)
		{
			this.pixbuf = pixbuf;
			base.Mode = CellRendererMode.Editable;
		}

		protected virtual void OnClicked (System.EventArgs e)
		{
			EventHandler handler = this.Clicked;
			if (handler != null)
				handler (this, e);
		}

		public event EventHandler Clicked;

		public override void GetSize (Widget widget, ref Gdk.Rectangle cell_area,
		                              out int x_offset, out int y_offset,
		                              out int width, out int height)
		{
			x_offset = 0;
			y_offset = 0;
			width = 2;
			width += pixbuf.Width;
			height = pixbuf.Width + 2;
		}

		public override CellEditable StartEditing (Gdk.Event evnt, Widget widget,
		                                           string path, Gdk.Rectangle background_area,
		                                           Gdk.Rectangle cell_area, CellRendererState flags)
		{
			try {
				if (evnt.Type == Gdk.EventType.ButtonPress && base.Sensitive)
					OnClicked (EventArgs.Empty);
			} catch (Exception ex) {
				GLib.ExceptionManager.RaiseUnhandledException (ex, false);
			}
			return base.StartEditing (evnt, widget, path, background_area, cell_area, flags);
		}

		protected override void Render (Gdk.Drawable window, Widget widget,
		                                Gdk.Rectangle background_area, Gdk.Rectangle cell_area,
		                                Gdk.Rectangle expose_area, CellRendererState flags)
		{
			if (!Visible)
				return;
			int x = cell_area.X + 1;
			int y = cell_area.Y + 1;
			window.DrawPixbuf (widget.Style.BaseGC (StateType.Normal),
			                   pixbuf, 0, 0, x, y, pixbuf.Width, pixbuf.Height,
			                   Gdk.RgbDither.None, 0, 0);
		}
	}
}

