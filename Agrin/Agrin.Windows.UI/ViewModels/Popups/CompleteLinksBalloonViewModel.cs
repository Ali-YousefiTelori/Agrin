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
        static CompleteLinksBalloon _completeLinksBalloon { get; set; }
        public static void CreateBalloon(Action<Window> initedWindow)
        {
            System.Threading.Thread thread = null;
            thread = new System.Threading.Thread(() =>
            {
                try
                {
                    _completeLinksBalloon = new CompleteLinksBalloon();

                    This = (CompleteLinksBalloonViewModel)_completeLinksBalloon.DataContext;
                    This.CurrentContentElement = _completeLinksBalloon;
                    var window = This.Initialize();
                    initedWindow(window);
                    _completeLinksBalloon.mainGrid.Margin = new Thickness(380, 0, -380, 0);
                    _completeLinksBalloon.mainGrid.Visibility = Visibility.Collapsed;
                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception ex)
                {
                    Agrin.Log.AutoLogger.LogError(ex, "CreateBalloon Dispatcher");
                }

            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
