using Agrin.BaseViewModels.Link;
using Agrin.IO.Strings;
using Agrin.Log;
using Agrin.ViewModels.Link;
using Agrin.Windows.UI.Views.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agrin.Windows.UI.ViewModels.Popups
{
    public static class ClipboardLinksBalloonViewModel
    {
        static Window balloonWindow;
        static double width = 500, height = 300;
        static AddLinks addLinks = null;
        static AddLinksViewModel vm = null;
        public static void Initialize()
        {
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                balloonWindow = new Window() { Background = System.Windows.Media.Brushes.Transparent, Topmost = true, SizeToContent = SizeToContent.WidthAndHeight, MaxWidth = width, MaxHeight = height, Width = width, Height = height, ShowInTaskbar = false, WindowStyle = WindowStyle.None, ResizeMode = ResizeMode.NoResize, AllowsTransparency = true, Title = "RapidbazPlus Clipboard" };
                balloonWindow.Closing += (s, e) =>
                {
                    e.Cancel = true;
                };

                System.Windows.Threading.Dispatcher.Run();
            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            while (true)
            {
                var dis = System.Windows.Threading.Dispatcher.FromThread(thread);
                if (dis != null)
                {
                    dis.Invoke(new Action(() =>
                        {
                            try
                            {
                                AddLinksBaseViewModel.CurrentDispatcherMustSet = balloonWindow.Dispatcher;
                                addLinks = new AddLinks();
                                balloonWindow.Content = addLinks;
                                vm = addLinks.DataContext as AddLinksViewModel;
                                vm.IsBackAction = false;
                                vm.IgnoreStopChanged = true;
                                AddLinksBaseViewModel.BackItemClipboardClick = () =>
                                {
                                    Hide();
                                };
                                vm.BackCommand = new Agrin.ViewModels.Helper.ComponentModel.RelayCommand(() =>
                                {
                                    Hide();
                                    vm.BackItem();
                                });
                            }
                            catch (Exception ex)
                            {
                                AutoLogger.LogError(ex, "clipboard Initialize");
                            }
                            
                        }));
                    break;
                }
                System.Threading.Thread.Sleep(500);
            }
        }

        static object lockOBJ = new object();
        public static void Show(string clipboard)
        {
            balloonWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (lockOBJ)
                {
                    try
                    {
                        var links = HtmlPage.ExtractLinksFromHtmlTwo(clipboard); ;
                        if (links.Count != 0)
                        {
                            bool lowCount = links.Count() <= 1;
                            if (lowCount && string.IsNullOrEmpty(vm.UriAddress) && vm.GroupLinks.Count == 0)
                                vm.UriAddress = links.FirstOrDefault();
                            else
                            {
                                if (vm.GroupLinks.Count == 0 && !string.IsNullOrEmpty(vm.UriAddress))
                                    vm.GroupLinks.Add(vm.UriAddress);
                                foreach (var item in vm.GroupLinks)
                                {
                                    if (links.Contains(item))
                                        links.Remove(item);
                                }
                                vm.GroupLinks.AddRange(links);
                                vm.AddGroupLinks();
                            }
                            if (!balloonWindow.IsVisible)
                            {
                                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                                balloonWindow.Left = desktopWorkingArea.Right - balloonWindow.Width;
                                balloonWindow.Top = desktopWorkingArea.Bottom - balloonWindow.Height;
                                balloonWindow.Show();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, "clipboard Show");
                    }
                }
            }));
        }

        public static void Hide()
        {
            balloonWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (lockOBJ)
                {
                    balloonWindow.Hide();
                }
            }));
        }
    }
}