using Agrin.Windows.UI.Views.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agrin.Windows.UI.ViewModels.Popups
{
    public class CompleteLinksBalloonViewModel : Agrin.ViewModels.Popups.CompleteLinksBalloonViewModel
    {
        public static CompleteLinksBalloonViewModel This { get; set; }
        public static void CreateBalloon()
        {
            var b = new CompleteLinksBalloon();
            This = (CompleteLinksBalloonViewModel)b.DataContext;
            This.CurrentContentElement = b;
            This.Initialize();
            b.mainGrid.Margin = new Thickness(380, 0, -380, 0);
            b.mainGrid.Visibility = Visibility.Collapsed;
        }
    }
}
