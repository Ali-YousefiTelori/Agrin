using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agrin.UI.Helper.Views
{
    public static class FocusBehaviour
    {
        public static bool GetForceFocus(DependencyObject d)
        {
            return (bool)d.GetValue(FocusBehaviour.ForceFocusProperty);
        }

        public static void SetForceFocus(DependencyObject d, bool val)
        {
            d.SetValue(FocusBehaviour.ForceFocusProperty, val);
        }

        public static readonly DependencyProperty ForceFocusProperty =
            DependencyProperty.RegisterAttached("ForceFocus",
                typeof(bool),
                typeof(FocusBehaviour),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if ((bool)e.NewValue)
                        {
                            if (d is UIElement)
                            {
                                ((UIElement)d).Focus();
                            }
                        }
                    }));
    }
}
