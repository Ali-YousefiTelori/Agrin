using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Agrin.ViewModels.ControlHelpers
{
    public class PopupAutoLocationHelper
    {
        Popup _popup = null;
        public void Initialize(Window window, Popup popup)
        {
            _popup = popup;
            window.LocationChanged += This_LocationChanged;
            window.SizeChanged += This_LocationChanged;
            popup.Loaded += PopupAutoLocation_Loaded;
            popup.LayoutUpdated += This_LocationChanged;
        }

        void PopupAutoLocation_Loaded(object sender, RoutedEventArgs e)
        {
            if (_popup.PlacementTarget == null)
                return;
            ((FrameworkElement)_popup.PlacementTarget).SizeChanged += This_LocationChanged;
        }

        void This_LocationChanged(object sender, EventArgs e)
        {
            _popup.HorizontalOffset += 0.1;
            _popup.HorizontalOffset -= 0.1;
        }
    }
}
