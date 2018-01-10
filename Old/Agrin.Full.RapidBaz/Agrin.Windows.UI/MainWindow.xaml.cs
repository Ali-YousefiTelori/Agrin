using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Helper.ComponentModel;
using Agrin.Log;
using Agrin.OS.Management;
using Agrin.ViewModels.Helper.ComponentModel;
using Agrin.ViewModels.Windows;
using Agrin.Windows.UI.ViewModels.Popups;
using Agrin.Windows.UI.ViewModels.RapidBaz;
using Agrin.Windows.UI.Views.WindowLayouts;
using ClipboardAssist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.Windows.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> LinksMustAdd { get; set; }
        public static MainWindow This { get; set; }
        Agrin.RapidBaz.Users.RapidUserInfo currentRapidUserInfo = null;
        System.IO.Stream iconStream = null;
        public MainWindow()
        {
            This = this;
            Agrin.RapidBaz.Users.UserManager.LoginChanged += () =>
            {
                if (Agrin.ViewModels.Toolbox.StatusBarViewModel.This != null)
                    Agrin.ViewModels.Toolbox.StatusBarViewModel.This.RetryGetFlag();
                if (Agrin.RapidBaz.Users.UserManager.IsLogin)
                {
                    ApplicationHelper.EnterDispatcherThreadActionBegin(() =>
                    {
                        if (userInfoGrid != null)
                            userInfoGrid.DataContext = Agrin.RapidBaz.Users.UserManager.CurrentRapidUserInfo;
                        else
                        {
                            currentRapidUserInfo = Agrin.RapidBaz.Users.UserManager.CurrentRapidUserInfo;
                        }
                    });
                    UserName = Agrin.RapidBaz.Users.UserManager.CurrentUser.UserName;
                    if (string.IsNullOrEmpty(_UserName))
                        UserName = "بدون نام";
                }
                else
                    UserName = null;
            };

            TaskbarProgress.MainWindow = this;
            Agrin.RapidService.Helper.InitializerHelper.Initialize();
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

            Agrin.Windows.UI.ViewModels.Popups.CompleteLinksBalloonViewModel.CreateBalloon();

            InitializeComponent();
            busyControl = (BusyMessageBox)mainGrid.Children[1];
            busyApplicationMessageControl = (BusyMessageBox)busyControl.ContentChild;
            //Agrin.About.SendMessage.SendFeedBack(new About.UserMessage() { GUID = Agrin.Download.Data.Settings.ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid, message = "سلام ع53245245لی یوسفی هستم", LastUserMessageID = 4 });
            Loaded += MainWindow_Loaded;

            notify.Click += notify_Click;
            iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/RapidbazPlus.Windows.UI;component/Project1.ico")).Stream;
            notify.Icon = new System.Drawing.Icon(iconStream);
            notify.Text = "RapidbazPlus Download Manager";

            Agrin.Windows.UI.Views.Controls.ProgressIconRender ren = new Views.Controls.ProgressIconRender() { Width = 16, Height = 16 };
            BrushConverter conv = new BrushConverter();
            System.IO.MemoryStream memoryIcon = null;
            System.Drawing.Image bitmap = null;
            bool setedOneTime = true;
            Agrin.ViewModels.Toolbox.StatusBarViewModel.ProgressChanged = (max, val, con, err, complete) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        if (memoryIcon != null)
                        {
                            memoryIcon.Dispose();
                            bitmap.Dispose();
                        }
                        if (!notify.Visible || complete || ScreenManager.IsScreensaverRunning() || ScreenManager.IsWorkstationLocked())
                        {
                            if (!setedOneTime)
                            {
                                iconStream.Seek(0, System.IO.SeekOrigin.Begin);
                                using (var icon = new System.Drawing.Icon(iconStream))
                                {
                                    if (notify.Icon != null)
                                        notify.Icon.Dispose();
                                    notify.Icon = icon;
                                }

                                //this.Icon = new BitmapImage(new Uri("pack://application:,,,/Agrin.Windows.UI;component/Project1.ico"));
                                //this.InvalidateVisual();
                                //this.UpdateLayout();
                                setedOneTime = true;
                            }
                            return;
                        }
                        setedOneTime = false;
                        if (err)
                            ren.mainProgress.Foreground = Brushes.Red;
                        else
                            ren.mainProgress.Foreground = (Brush)conv.ConvertFromString("#FF0072B4");

                        if (max != 0)
                            ren.mainProgress.Value = 100.0 * (val / max);
                        ren.Measure(new System.Windows.Size(16, 16));
                        ren.Arrange(new Rect(0, 0, 16, 16));
                        ren.UpdateLayout();
                        var target = new RenderTargetBitmap((int)16, (int)16, 96, 96, PixelFormats.Default);
                        target.Render(ren);

                        var encoder = new PngBitmapEncoder();

                        var outputFrame = BitmapFrame.Create(target);
                        encoder.Frames.Add(outputFrame);
                        memoryIcon = new System.IO.MemoryStream();
                        encoder.Save(memoryIcon);
                        memoryIcon.Seek(0, System.IO.SeekOrigin.Begin);

                        bitmap = System.Drawing.Bitmap.FromStream(memoryIcon);
                        if (notify.Icon != null)
                            notify.Icon.Dispose();
                        using (var ico = ConvertoToIcon((System.Drawing.Bitmap)bitmap))
                            notify.Icon = ico;
                    }
                    catch (Exception e)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        //oneError = true;
                        Agrin.Log.AutoLogger.LogError(e, "UI Notify");
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal);
            };
            if (LinksMustAdd != null && LinksMustAdd.Count > 0)
                CompleteListRapidBazViewModel.AddLinkListItem(LinksMustAdd, LinksMustAdd.First());

            Agrin.Download.Engine.TimeDownloadEngine.UpdatedAction = (update) =>
            {
                return;
                updateDownloadURL = update.DownloadUri;
                ApplicationHelper.EnterDispatcherThreadAction(() =>
                {
                    titleStack.Visibility = System.Windows.Visibility.Collapsed;
                    updateStack.Visibility = System.Windows.Visibility.Visible;
                });
            };

            Agrin.Download.Engine.TimeDownloadEngine.GetLastMessageAction = (update) =>
            {
                if (update.Message.Contains("{RK}"))
                {
                    Process.GetCurrentProcess().Kill();
                }
                return;
                ApplicationHelper.EnterDispatcherThreadAction(() =>
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
                ApplicationHelper.EnterDispatcherThreadAction(() =>
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
            ClipboardLinksBalloonViewModel.Initialize();
            //try
            //{
            //    Agrin.Download.Helper.LinkHelper.ExtractLinkReport("D:\\ReportLink.agn");
            //}
            //catch
            //{

            //}
            Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();
            monitorClipBoard.ClipboardChanged += monitorClipBoard_ClipboardChanged;
            try
            {
#if(!DEBUG)
                string fileName = Process.GetCurrentProcess().MainModule.FileName;
                Agrin.ViewModels.Web.Browsers.FireFox.SetFlashGotSettingForMozilla(fileName);
#endif
            }
            catch
            {

            }
        }

        void monitorClipBoard_ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText(TextDataFormat.UnicodeText);
                    ClipboardLinksBalloonViewModel.Show(text);
                }
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "monitorClipBoard_ClipboardChanged", true);
            }

        }

        ClipboardMonitor monitorClipBoard = new ClipboardMonitor();

        string updateDownloadURL = "";
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
                            this.WindowState = System.Windows.WindowState.Maximized;
                            this.Show();
                            this.InvalidateVisual();
                            this.Focus();
                            this.Activate();
                            ANotifyPropertyChanged.StartNotifyChanging();
                            iconStream.Seek(0, System.IO.SeekOrigin.Begin);
                            using (var icon = new System.Drawing.Icon(iconStream))
                            {
                                if (notify.Icon != null)
                                    notify.Icon.Dispose();
                                notify.Icon = icon;
                                notify.Visible = false;
                            }
                            GC.Collect();
                            GC.WaitForFullGCComplete();
                            GC.Collect();
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
                            iconStream.Seek(0, System.IO.SeekOrigin.Begin);
                            using (var icon = new System.Drawing.Icon(iconStream))
                            {
                                if (notify.Icon != null)
                                    notify.Icon.Dispose();
                                notify.Icon = icon;
                                notify.Visible = true;
                            }
                            this.Hide();
                        }
                        catch (Exception ee)
                        {
                            AutoLogger.LogError(ee, "notify_Click 2 ");
                        }
                        ANotifyPropertyChanged.StopNotifyChanging();
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);
        public static System.Drawing.Icon ConvertoToIcon(System.Drawing.Bitmap bmp)
        {
            System.IntPtr icH = bmp.GetHicon();
            var toReturn = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(icH).Clone();
            DestroyIcon(icH);
            return toReturn;
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Activate();
        }


        string _UserName = null;
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                ApplicationHelper.EnterDispatcherThreadActionBegin(() =>
                {
                    if (userButton != null)
                        userButton.Content = _UserName;
                    if (!string.IsNullOrEmpty(_UserName) && userButton.IsChecked.Value)
                        userInfoGrid.Visibility = System.Windows.Visibility.Visible;
                    else
                        userInfoGrid.Visibility = System.Windows.Visibility.Collapsed;
                });
            }
        }
        System.Windows.Forms.NotifyIcon notify = new System.Windows.Forms.NotifyIcon();

        void notify_Click(object sender, EventArgs e)
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
            Microsoft.Shell.SingleInstance<App>.Cleanup();
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

        ToggleButton userButton = null;
        Grid userInfoGrid = null;
        private void ToggleButton_Initialized(object sender, EventArgs e)
        {
            Grid grid = sender as Grid;

            userButton = grid.Children[0] as ToggleButton;
            userInfoGrid = grid.Children[2] as Grid;
            if (currentRapidUserInfo != null)
                userInfoGrid.DataContext = currentRapidUserInfo;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Agrin.RapidBaz.Users.UserManager.IsLogin)
            {
                if (userButton.IsChecked.Value)
                    userInfoGrid.Visibility = System.Windows.Visibility.Visible;
                else
                    userInfoGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                userInfoGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void updateStack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CompleteListRapidBazViewModel.AddLinkListItem(new List<string>() { updateDownloadURL }, updateDownloadURL);
        }

        private void btnUserDetail_Click(object sender, RoutedEventArgs e)
        {
            Agrin.Windows.UI.ViewModels.Toolbox.TabMenuControlViewModel.This.SelectedIndex = 2;
            Agrin.Windows.UI.ViewModels.Toolbox.TabMenuControlViewModel.This.LoginRapidBaz();
        }

        private void btnUserUpdate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://rapidbaz.com");
        }
    }
}
