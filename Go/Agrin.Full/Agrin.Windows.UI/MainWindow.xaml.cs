using Agrin.BaseViewModels.Models;
using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Helper.ComponentModel;
using Agrin.Log;
using Agrin.OS.Management;
using Agrin.ViewModels.HardWare;
using Agrin.ViewModels.Helper.ComponentModel;
using Agrin.ViewModels.Windows;
using Agrin.Windows.UI.ViewModels.Lists;
using Agrin.Windows.UI.Views.WindowLayouts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Agrin.Windows.UI
{
    public class IconElementGenerator
    {
        Stream _defaultIcon = null;

        public Stream DefaultIcon
        {
            get
            {
                _defaultIcon.Seek(0, SeekOrigin.Begin);
                return _defaultIcon;
            }
            set
            {
                _defaultIcon = value;
            }
        }


        Dictionary<Icon, LinkStatusInfo> savedIcons = new Dictionary<Icon, LinkStatusInfo>();

        public void GenerateNewIcon(FrameworkElement controlToIcon, LinkStatusInfo status, int width, int height)
        {
            controlToIcon.Measure(new System.Windows.Size(width, height));
            controlToIcon.Arrange(new Rect(0, 0, width, height));
            controlToIcon.UpdateLayout();
            var target = new RenderTargetBitmap((int)width, (int)height, 96d, 96d, PixelFormats.Default);
            target.Render(controlToIcon);
            target.Freeze();
            var encoder = new PngBitmapEncoder();

            var outputFrame = BitmapFrame.Create(target);
            encoder.Frames.Add(outputFrame);
            var memoryIcon = new System.IO.MemoryStream();
            encoder.Save(memoryIcon);
            memoryIcon.Seek(0, System.IO.SeekOrigin.Begin);

            var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(memoryIcon);
            var icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
            savedIcons.Add(icon, status.Clone());

        }

        public Icon GetNewIcon(FrameworkElement controlToIcon, LinkStatusInfo status, int width, int height)
        {
            try
            {
                //foreach (var item in savedIcons)
                //{
                //    if (item.Value.IsComplete == status.IsComplete && item.Value.IsConnecting == status.IsConnecting && item.Value.IsError == status.IsError
                //        && item.Value.TotalMaximumProgress == status.TotalMaximumProgress && item.Value.TotalProgressValue == status.TotalProgressValue &&
                //        item.Value.TotalSpeed == status.TotalSpeed)
                //    {
                //        //item.Key.Seek(0, SeekOrigin.Begin);
                //        return item.Key;
                //    }
                //}
                var item = (from x in savedIcons where x.Value.IsError == status.IsError && x.Value.TotalProgressValue == status.TotalProgressValue select x.Key).FirstOrDefault();
                if (item != null)
                    return item;
                //BitmapImage bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.StreamSource = memoryIcon;
                //bitmap.EndInit();
                Agrin.Log.AutoLogger.LogText("GetNewIcon item is null : " + savedIcons.Count + " " + status.IsError + " " + status.TotalProgressValue);
                return null;
            }
            catch (Exception ex)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                //oneError = true;
                Agrin.Log.AutoLogger.LogError(ex, "UI Notify");
            }
            return null;
        }
        //public static System.Drawing.Icon ConvertoToIcon(System.Drawing.Bitmap bmp)
        //{
        //    System.IntPtr icH = bmp.GetHicon();
        //    var toReturn = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(icH).Clone();
        //    //IconDisposer.DisposeNow(icH);
        //    return toReturn;
        //}

        //System.Drawing.Icon _defaultIcon = null;

        //public System.Drawing.Icon DefaultIcon
        //{
        //    get { return _defaultIcon; }
        //    set { _defaultIcon = value; }
        //}


        //Dictionary<System.Drawing.Icon, LinkStatusInfo> savedIcons = new Dictionary<System.Drawing.Icon, LinkStatusInfo>();

        //public System.Drawing.Icon GenerateNewIcon(FrameworkElement controlToIcon, LinkStatusInfo status)
        //{
        //    try
        //    {
        //        foreach (var item in savedIcons)
        //        {
        //            if (item.Value.IsComplete == status.IsComplete && item.Value.IsConnecting == status.IsConnecting && item.Value.IsError == status.IsError
        //                && item.Value.TotalMaximumProgress == status.TotalMaximumProgress && item.Value.TotalProgressValue == status.TotalProgressValue &&
        //                item.Value.TotalSpeed == status.TotalSpeed)
        //            {
        //                return item.Key;
        //            }
        //        }
        //        controlToIcon.Measure(new System.Windows.Size(16, 16));
        //        controlToIcon.Arrange(new Rect(0, 0, 16, 16));
        //        controlToIcon.UpdateLayout();
        //        var target = new RenderTargetBitmap((int)16, (int)16, 96, 96, PixelFormats.Default);
        //        target.Render(controlToIcon);

        //        var encoder = new PngBitmapEncoder();

        //        var outputFrame = BitmapFrame.Create(target);
        //        encoder.Frames.Add(outputFrame);
        //        var memoryIcon = new System.IO.MemoryStream();
        //        encoder.Save(memoryIcon);
        //        memoryIcon.Seek(0, System.IO.SeekOrigin.Begin);

        //        var bitmap = System.Drawing.Bitmap.FromStream(memoryIcon);

        //        var icon = ConvertoToIcon((System.Drawing.Bitmap)bitmap);
        //        savedIcons.Add(icon, status.Clone());
        //        return icon;
        //    }
        //    catch
        //    {
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //        GC.Collect();
        //        //oneError = true;
        //        //Agrin.Log.AutoLogger.LogError(e, "UI Notify");
        //    }
        //    return null;
        //}
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> LinksMustAdd { get; set; }
        public static MainWindow This { get; set; }
        static Window balloonWindow = null;
        Thread CurrentThread { get; set; }
        public MainWindow()
        {
            ApplicationHelperBase.Current = new ApplicationHelper();
            WindowsControllerBase.Current = new WindowsController();
            DriveHelper.Current = new DriveHelper();

            CurrentThread = Thread.CurrentThread;
            This = this;
            TaskbarProgress.MainWindow = this;
            ApplicationHelperBase.AddThreadDispather(this.Dispatcher);
            ApplicationHelperBase.RefreshCommandAction = () =>
            {
                if (Thread.CurrentThread != CurrentThread)
                {
                    var dispatcher = System.Windows.Threading.Dispatcher.FromThread(Thread.CurrentThread);
                    ApplicationHelperBase.EnterDispatcherThreadActionBegin(() =>
                    {
                        CommandManager.InvalidateRequerySuggested();
                    }, dispatcher);
                }
                else
                    CommandManager.InvalidateRequerySuggested();
            };
            ApplicationTaskManager.Current = new ApplicationTaskManager((info) =>
            {

            }, (info) =>
            {

            });

            ApplicationTaskManager.Current.UserCompleteAction = () =>
            {
                ApplicationTaskManager.Current.DeActiveTask(ApplicationTaskManager.Current.TaskInfoes.FirstOrDefault());
            };

            Agrin.ViewModels.ApplicationLoader.LoadApplicationData(this.Dispatcher, Assembly.GetExecutingAssembly().GetName().Version);
            ExitCommand = new RelayCommand(() =>
            {
                handleCloding = false;
                Exit();
            });

            Agrin.Windows.UI.ViewModels.Popups.CompleteLinksBalloonViewModel.CreateBalloon((w) => balloonWindow = w);

            InitializeComponent();
            busyControl = (BusyMessageBox)mainGrid.Children[2];
            busyApplicationMessageControl = (BusyMessageBox)busyControl.ContentChild;
            //var guid = Agrin.Download.Data.Settings.ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid;
            //Agrin.About.SendMessage.SendFeedBack(new About.UserMessage() { GUID = Agrin.Download.Data.Settings.ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid, message = "سلام ع53245245لی یوسفی هستم", LastUserMessageID = 4 });
            Loaded += MainWindow_Loaded;
            //notify.Visibility = System.Windows.Visibility.Collapsed;
            notify.LeftClickCommand = new RelayCommand(notify_Click);
            var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Agrin.Windows.UI;component/Project1.ico")).Stream;
            //var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Agrin.Windows.UI;component/Resources/Images/AgrinNotifyIcon.png")).Stream;
            //BitmapImage bitmapIcon = new BitmapImage();
            //bitmapIcon.BeginInit();
            //bitmapIcon.StreamSource = iconStream;
            //bitmapIcon.EndInit();
            //notify.

            iconElementGenerator.DefaultIcon = iconStream;// Agrin.NotifyIcon.TaskbarNotification.Util.GetIconStreamFromSize(iconStream, 256, 256);
            notify.IconStream = iconElementGenerator.DefaultIcon;
            notify.ToolTipText = "Agrin Download Manager";

            GenerateAllIcons();

            Agrin.ViewModels.Toolbox.StatusBarViewModel.ProgressChanged = (status) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ChangeStateProgress(status);
                }), System.Windows.Threading.DispatcherPriority.Normal);
            };
            if (LinksMustAdd != null && LinksMustAdd.Count > 0)
                LinksViewModel.AddLinkListItem(LinksMustAdd, LinksMustAdd.First());

            Agrin.Download.Engine.TimeDownloadEngine.UpdatedAction = (update) =>
            {
                updateDownloadURL = update.DownloadUri;
                ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                {
                    titleStack.Visibility = System.Windows.Visibility.Collapsed;
                    updateStack.Visibility = System.Windows.Visibility.Visible;
                });
            };

            Agrin.Download.Engine.TimeDownloadEngine.GetLastMessageAction = (update) =>
            {
                ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                {
                    if (!busyApplicationMessageControl.IsBusy)
                    {
                        busyApplicationMessageControl.Message = update.Message;
                        busyApplicationMessageControl.Command = new RelayCommand(() =>
                        {
                            ApplicationSetting.Current.LastApplicationMessageID = update.LastApplicationMessageID;
                            busyApplicationMessageControl.IsBusy = false;
                            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                        });
                        busyApplicationMessageControl.IsBusy = true;
                    }
                });
                //var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);

                //var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
                //var mainActivity = new Intent(currentActivity, typeof(MainActivity));
                //mainActivity.SetFlags(ActivityFlags.NewTask);
                //mainActivity.SetFlags(ActivityFlags.ClearTop);
                //mainActivity.PutExtra("AgrinApplicationMessage", Newtonsoft.Json.JsonConvert.SerializeObject(update));
                //var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
                //notification.SetLatestEventInfo(currentActivity, ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageTitle_Language"), ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageMessage_Language"), pendingIntent);
                //notification.Flags = NotificationFlags.AutoCancel;

                //nMgr.Notify(MaxNotifyID + 2, notification);
            };

            Agrin.Download.Engine.TimeDownloadEngine.GetUserMessageAction = (user) =>
            {
                ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                {
                    if (!busyApplicationMessageControl.IsBusy)
                    {
                        busyApplicationMessageControl.Message = user.Message;
                        busyApplicationMessageControl.Command = new RelayCommand(() =>
                        {
                            ApplicationSetting.Current.LastUserMessageID = user.LastUserMessageID;
                            busyApplicationMessageControl.IsBusy = false;
                            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                        });
                        busyApplicationMessageControl.IsBusy = true;
                    }
                });
            };
            //try
            //{
            //    Agrin.Download.Helper.LinkHelper.ExtractLinkReport("D:\\ReportLink.agn");
            //}
            //catch
            //{

            //}
            Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();

            Agrin.Windows.UI.Views.WindowLayouts.Asuda.BasketReceiverWindow.ShowBasket();
            //var report = Agrin.IO.Helper.SerializeStream.OpenSerializeStream("g:\\ReportLink.agn") as Agrin.Download.Helper.LinkInfoReport;
            //var linkInfo = DeSerializeData.LinkInfoSerializeToLinkInfoData(report.LinkInfoData);
            //linkInfo.PathInfo.Id = Agrin.Download.Manager.ApplicationLinkInfoManager.Current.FindNewId();
            //linkInfo.PathInfo.ConnectionsSavedAddress = null;
            //foreach (var item in linkInfo.Connections)
            //{
            //    item.SaveFileName = null;
            //}


            //Agrin.Download.Manager.ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, null, false);
            //foreach (var item in linkInfo.Connections)
            //{
            //    item.SaveFileName = null;
            //    string fn = System.IO.Path.GetFileName(item.SaveFileName);
            //    var getLen = (from x in report.Files where System.IO.Path.GetFileName(x.FileName) == fn select x).FirstOrDefault();
            //    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(item.SaveFileName));
            //    System.IO.File.Create(item.SaveFileName).Dispose();
            //    using (var file = System.IO.File.OpenWrite(item.SaveFileName))
            //    {
            //        file.SetLength(getLen.Lenght);
            //    }
            //}
            //linkInfo.SaveThisLink(true);

            //Test();
            //TetBallon();
            //ApplicationLinkInfoManager.Current.ChangeSaveDataPath(@"C:\Users\Ali Visual Studio\AppData\Roaming", (ex, ex2) =>
            //{

            //}, (s, s2, i, i2) =>
            //{

            //});
            //System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
            //{
            //    int i = 0;
            //    bool isComplete = false;
            //    while (true)
            //    {
            //        Dispatcher.Invoke(new Action(() =>
            //        {
            //            ChangeStateProgress(new LinkStatusInfo() { TotalMaximumProgress = 100, TotalProgressValue = i, TotalSpeed = i, IsComplete = isComplete });
            //        }), System.Windows.Threading.DispatcherPriority.Normal);
            //        Thread.Sleep(1);
            //        i++;
            //        if (i > 100)
            //        {
            //            isComplete = !isComplete;
            //            i = 0;
            //        }
            //    }
            //});
            //task.Start();
        }

        //void TetBallon()
        //{
        //    System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
        //    {
        //        while(true)
        //        {
        //            System.Threading.Thread.Sleep(10000);
        //            ApplicationBalloonManager.Current.ShowBalloonAction(BalloonMode.Show);
        //            System.Threading.Thread.Sleep(10000);
        //            ApplicationBalloonManager.Current.ShowBalloonAction(BalloonMode.Changed);
        //            System.Threading.Thread.Sleep(10000);
        //            ApplicationBalloonManager.Current.ShowBalloonAction(BalloonMode.Hide);
        //        }
        //    });
        //    task.Start();
        //}
        //void Test()
        //{
        //    System.Threading.Thread th = new System.Threading.Thread(() =>
        //    {
        //        while (true)
        //        {
        //            for (int i = 0; i < 100; i++)
        //            {
        //                Agrin.Download.Engine.TimeDownloadEngine.TotalItemsChanged(10, 100, i, false, false, false);
        //                System.Threading.Thread.Sleep(100);
        //            }
        //            Agrin.Download.Engine.TimeDownloadEngine.TotalItemsChanged(10, 100, 100, false, false, true);

        //            System.Threading.Thread.Sleep(1000);
        //        }
        //    });
        //    th.Start();
        //}


        string updateDownloadURL = "";

        System.Windows.WindowState _lastState = WindowState.Normal;
        bool _IsNotify = false;

        public bool IsNotify
        {
            get { return _IsNotify; }
            set
            {
                _IsNotify = value;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (!value)
                    {
                        try
                        {
                            //Render();
                            if (_lastState == System.Windows.WindowState.Minimized)
                                _lastState = System.Windows.WindowState.Normal;
                            this.WindowState = _lastState;
                            this.Show();
                            this.Focus();
                            this.Activate();
                            ANotifyPropertyChanged.StartNotifyChanging();
                            notify.IconStream = iconElementGenerator.DefaultIcon;
                            notify.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        catch (Exception ee)
                        {
                            AutoLogger.LogError(ee, "notify_Click");
                        }
                    }
                    else
                    {
                        try
                        {
                            notify.IconStream = iconElementGenerator.DefaultIcon;
                            notify.Visibility = System.Windows.Visibility.Visible;
                            this.Hide();
                        }
                        catch (Exception ee)
                        {
                            AutoLogger.LogError(ee, "notify_Click 2 ");
                        }
                        ANotifyPropertyChanged.StopNotifyChanging();
                    }
                    _lastState = this.WindowState;
                }), System.Windows.Threading.DispatcherPriority.Render);
            }
        }

        IconElementGenerator iconElementGenerator = new IconElementGenerator();
        BrushConverter conv = new BrushConverter();
        bool setedOneTime = true;
        Agrin.Windows.UI.Views.Controls.ProgressIconRender ren = new Views.Controls.ProgressIconRender() { Width = 16, Height = 16 };

        public void GenerateAllIcons()
        {
            LinkStatusInfo status = new LinkStatusInfo();
            status.IsError = true;
            status.TotalMaximumProgress = 100;
            for (int i = 0; i <= 100; i++)
            {
                status.TotalProgressValue = i;
                if (status.IsError)
                {
                    ren.mainProgress.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    ren.mainProgress.Foreground = (System.Windows.Media.Brush)conv.ConvertFromString("#FF0072B4");
                }

                if (status.TotalMaximumProgress != 0)
                {
                    ren.mainProgress.Value = i;
                }
                iconElementGenerator.GenerateNewIcon(ren, status, 16, 16);
            }
            status.IsError = false;
            for (int i = 0; i <= 100; i++)
            {
                status.TotalProgressValue = i;
                if (status.IsError)
                {
                    ren.mainProgress.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    ren.mainProgress.Foreground = (System.Windows.Media.Brush)conv.ConvertFromString("#FF0072B4");
                }

                if (status.TotalMaximumProgress != 0)
                {
                    ren.mainProgress.Value = i;
                }
                iconElementGenerator.GenerateNewIcon(ren, status, 16, 16);
            }
        }

        public void ChangeStateProgress(LinkStatusInfo status)
        {
            try
            {
                if (notify.Visibility == Visibility.Collapsed || status.IsComplete || ScreenManager.IsScreensaverRunning() || ScreenManager.IsWorkstationLocked())
                {
                    if (!setedOneTime)
                    {
                        notify.IconStream = null;
                        notify.IconStream = iconElementGenerator.DefaultIcon;
                        setedOneTime = true;
                    }
                    return;
                }
                setedOneTime = false;
                var max = status.TotalMaximumProgress;
                var value = status.TotalProgressValue;
                status.TotalMaximumProgress = 100;
                status.TotalProgressValue = (int)(100.0 * (max == 0 ? 0 : (value / max)));
                notify.Icon = iconElementGenerator.GetNewIcon(ren, status, 16, 16);
            }
            catch (Exception ex)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                //oneError = true;
                Agrin.Log.AutoLogger.LogError(ex, "ChangeStateProgress");
            }
        }



        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Activate();
        }


        void notify_Click()
        {
            IsNotify = false;
        }

        BusyMessageBox busyControl { get; set; }
        BusyMessageBox busyApplicationMessageControl { get; set; }

        bool handleCloding = true;

        public RelayCommand ExitCommand { get; set; }

        public void Exit()
        {
            Hide();

            foreach (var item in ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToList())
            {
                ApplicationLinkInfoManager.Current.StopLinkInfo(item);
            }
            try
            {
                SerializeData.SaveLinkInfoesToFileNoThread();
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, "Exit Save");
            }
            SerializeData.CloseApplicationWaitForSavingAllComplete();
            //Microsoft.Shell.SingleInstance<App>.Cleanup();
            Agrin.Network.ProxyMonitor.Stop();
            Process.GetCurrentProcess().Kill();
        }
        //void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{

        //    System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
        //    {
        //        while (true)
        //        {
        //            Dispatcher.Invoke(new Action(() =>
        //                {
        //                    System.Threading.Thread.Sleep(2000);
        //                    IntPtr capturingHandle = GetCapture();

        //                    for (int i = 0; i < Application.Current.Windows.Count; i++)
        //                    {
        //                        if (new System.Windows.Interop.WindowInteropHelper(
        //                                                    Application.Current.Windows[i]
        //                                                   ).Handle == capturingHandle)
        //                        {
        //                            Mouse.Capture(
        //                                          Application.Current.Windows[i],
        //                                          CaptureMode.Element
        //                                         );
        //                            Application.Current.Windows[i].ReleaseMouseCapture();

        //                            break;
        //                        }
        //                    }
        //                }), System.Windows.Threading.DispatcherPriority.Input);
        //            System.Threading.Thread.Sleep(10000);
        //        }
        //    });
        //    task.Start();
        //}
        //[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        //public static extern IntPtr GetCapture();

        private void mainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mainWindow_TouchDown(object sender, TouchEventArgs e)
        {
            DragMove();
        }

        //public void Render()
        //{
        //    System.Windows.Interop.HwndSource hwndSource = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
        //    System.Windows.Interop.HwndTarget hwndTarget = hwndSource.CompositionTarget;
        //    hwndTarget.RenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
        //}

        private void mainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Minimized)
                _lastState = this.WindowState;
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                if (!Agrin.OS.Management.OSSystemInfo.IsWindowsXPAndLower())
                {
                    mainGrid.Margin = new Thickness(7);
                }
                else
                    mainGrid.Margin = new Thickness();
            }
            else
            {
                mainGrid.Margin = new Thickness();
            }

            if (this.WindowState == System.Windows.WindowState.Minimized)
                this.Clip = new LineGeometry();
            else
                this.Clip = null;

            if (WindowState == System.Windows.WindowState.Normal || WindowState == System.Windows.WindowState.Maximized)
                ANotifyPropertyChanged.StartNotifyChanging();
            else if (WindowState == System.Windows.WindowState.Minimized)
                ANotifyPropertyChanged.StopNotifyChanging();
            //Render();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
            //Render();
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = handleCloding;
            busyControl.IsBusy = true;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnNotify_Click(object sender, RoutedEventArgs e)
        {
            IsNotify = true;
        }

        private void updateStack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LinksViewModel.AddLinkListItem(new List<string>() { updateDownloadURL }, updateDownloadURL);
        }
    }
}
