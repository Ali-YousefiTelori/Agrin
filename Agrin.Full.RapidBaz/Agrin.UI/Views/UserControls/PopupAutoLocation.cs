using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Agrin.UI.Views.UserControls
{
    public class PopupAutoLocation:Popup
    {
        public PopupAutoLocation()
        {
            MainWindow.This.LocationChanged += This_LocationChanged;
            MainWindow.This.SizeChanged += This_LocationChanged;
            this.Loaded += PopupAutoLocation_Loaded;
            this.LayoutUpdated += This_LocationChanged;
        }

        void PopupAutoLocation_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)this.PlacementTarget).SizeChanged += This_LocationChanged;
        }

        void This_LocationChanged(object sender, EventArgs e)
        {
            this.HorizontalOffset += 0.1;
            this.HorizontalOffset -= 0.1;
        }
    }
}
