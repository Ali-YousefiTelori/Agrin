using Agrin.Data.Mapping;
using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Helper.ComponentModel;
using Agrin.UI.ViewModels.Downloads;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            This = this;
            //Window win = new Window() { SizeToContent = System.Windows.SizeToContent.WidthAndHeight };
            //win.Content = new Agrin.UI.Views.UserControls.DateTime.TimePicker() { };
            //win.ShowDialog();
            //try
            //{
            //    var sttt = new System.Net.WebClient().DownloadString("http://abzar.temkade.com/tools/php/stat/visitor.php?site=framesoft.ir&font=0F4F86&backg=FFFFFF");
            //}
            //catch
            //{
            //string avali=  Agrin.IO.Strings.Decodings.FullDecodeString("http://goo.com/ali.txt");
            //}

            //string ali = String.Format("{0:00.00%}", 1.0);
            //CheckTowFileBytes("E:\\dms.rar", "D:\\DMail Sender.rar");
            //Exception e = Agrin.Download.Data.DeSerializeData.LoadDataInFile();
            ApplicationHelper.DispatcherThread = this.Dispatcher;

            Agrin.IO.Helper.MPath.InitializePath();
            ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
            ApplicationGroupManager.Current = new ApplicationGroupManager();
            ApplicationNotificationManager.Current = new ApplicationNotificationManager();
            ApplicationBalloonManager.Current = new ApplicationBalloonManager();

            DeSerializeData.LoadApplicationData();
            Agrin.UI.ViewModels.Popups.BalloonViewModel.CreateBalloon();
            InitializeComponent();
            notify.Click += notify_Click;
            System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Agrin.UI;component/Project1.ico")).Stream;
            notify.Icon = new System.Drawing.Icon(iconStream);

            downloadManager.linksListData.CurrentToolbox = mainTopToolBox;
            //if (!Agrin.OS.Management.OSSystemInfo.IsWindowsVistaAndLower())
            //{
            //    MessageBox.Show("");
            //    mainlayout.Margin = new Thickness(0);
            //}

            //ser app setting
            ApplicationSetting.Current.ApplicationOSSetting.Application = "Agrin WPF Windows";
            if (string.IsNullOrEmpty(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid))
            {
                ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid = Guid.NewGuid().ToString();
                SerializeData.SaveApplicationSettingToFile();
            }
            ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ApplicationSetting.Current.ApplicationOSSetting.OSName = Agrin.OS.Management.OSSystemInfo.GetSystemInformation();
            ApplicationSetting.Current.ApplicationOSSetting.OSVersion = Environment.OSVersion.VersionString;

            Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();
            IsShowToolbar = true;
            Agrin.UI.ViewModels.Toolbox.ToolbarViewModel.This.PinCommand.Execute();
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            txtVersion.Text = "نسخه آزمایشی " + version.ToString() + " تیر 1394";
        }

        void notify_Click(object sender, EventArgs e)
        {
            GC.Collect(5, GCCollectionMode.Forced);
            GC.WaitForFullGCComplete();
            GC.Collect(5, GCCollectionMode.Forced);
            this.WindowState = System.Windows.WindowState.Maximized;
            this.Show();
            this.Focus();
            this.Activate();
            notify.Visible = false;
            ANotifyPropertyChanged.StartNotifyChanging();
        }

        System.Windows.Forms.NotifyIcon notify = new System.Windows.Forms.NotifyIcon();

        private static MainWindow _this;
        public static MainWindow This
        {
            get { return _this; }
            set { _this = value; }
        }


        public static readonly DependencyProperty IsShowToolbarProperty = DependencyProperty.Register("IsShowToolbar", typeof(bool), typeof(MainWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsShowGroupProperty = DependencyProperty.Register("IsShowGroup", typeof(bool), typeof(MainWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsShowPageProperty = DependencyProperty.Register("IsShowPage", typeof(bool), typeof(MainWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsShowMessegeProperty = DependencyProperty.Register("IsShowMessege", typeof(bool), typeof(MainWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsShowToolbar
        {
            get
            {
                return (bool)this.GetValue(IsShowToolbarProperty);
            }
            set
            {
                this.SetValue(IsShowToolbarProperty, value);
            }
        }
        public bool IsShowGroup
        {
            get
            {
                return (bool)this.GetValue(IsShowGroupProperty);
            }
            set
            {
                this.SetValue(IsShowGroupProperty, value);
            }
        }
        public bool IsShowPage
        {
            get
            {
                return (bool)this.GetValue(IsShowPageProperty);
            }
            set
            {
                this.SetValue(IsShowPageProperty, value);
            }
        }
        public bool IsShowMessege
        {
            get
            {
                return (bool)this.GetValue(IsShowMessegeProperty);
            }
            set
            {
                this.SetValue(IsShowMessegeProperty, value);
                if (value)
                    IsShowToolbar = IsShowGroup = false;
                else
                {
                    if (IsToolbarPin)
                        IsShowToolbar = true;
                    //if (IsGroupPin)
                    //    IsShowGroup = true;
                }
            }
        }

        public bool IsToolbarPin { get; set; }
        //public bool IsGroupPin { get; set; }

        void CheckTowFileBytes(string fileName1, string fileName2)
        {
            using (System.IO.FileStream fileStream1 = new System.IO.FileStream(fileName1, System.IO.FileMode.Open))
            {
                using (System.IO.FileStream fileStream2 = new System.IO.FileStream(fileName2, System.IO.FileMode.Open))
                {
                    long errorBytes = 0;
                    while (fileStream1.Position != fileStream1.Length)
                    {
                        byte[] readBytes = new byte[1024 * 1024];
                        byte[] readBytes1 = new byte[1024 * 1024];
                        int readCount = 0;
                        readCount = fileStream1.Read(readBytes, 0, readBytes.Length);
                        //if (readCount != fileStream2.Read(readBytes1, 0, readBytes.Length))
                        //{
                        //    Debugger.Break();
                        //}

                        for (int i = 0; i < readCount; i++)
                        {
                            long pos = fileStream1.Position - i;
                            byte v1 = readBytes[i], v2 = readBytes1[i];
                            if (v1 != v2)
                            {
                                errorBytes++;
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
        }

        //Download.Web.LinkInfo info = new Download.Web.LinkInfo("http://localhost:16494/Default.aspx");
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (info.DownloadingProperty.State == Download.Web.ConnectionState.Downloading || info.DownloadingProperty.State == Download.Web.ConnectionState.Connecting)
            //    info.Pause();
            //else
            //    info.Play();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //Point mainPoint;
        private void Toolbar_MouseMove(object sender, MouseEventArgs e)
        {
            //&& toolbarGrid.Margin.Left == -110
            if (!IsToolbarPin && !IsShowMessege)//&& !LinkInfoesDownloadingManagerViewModel.This.IsShowLinkInfoDownloading
            {
                //Point pos = Mouse.GetPosition(downloadManager);
                //if (pos.X < 20)
                //{
                //var point = toolbarGrid.PointToScreen(new Point());
                //if (pos.Y > 0 && pos.Y < downloadManager.ActualHeight - 35)
                //{
                IsShowToolbar = true;
                //}
                // }
            }
        }

        //private void Group_MouseMove(object sender, MouseEventArgs e)
        //{
        //    //&& groupGrid.Margin.Right == -254
        //    if (!IsGroupPin && !IsShowMessege)
        //    {
        //        //Point pos = Mouse.GetPosition(downloadManager);
        //        //if (pos.X > this.ActualWidth - 20)
        //        //{
        //        // var point = groupGrid.PointToScreen(new Point());
        //        //if (pos.Y > 0 && pos.Y < downloadManager.ActualHeight - 35)
        //        IsShowGroup = true;
        //        //}
        //    }
        //}

        private void Toolbar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsToolbarPin)
                IsShowToolbar = false;
            //if (!IsGroupPin)
            //    IsShowGroup = false;
        }

        private void mainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                mainMenu.Margin = new Thickness(5, 0, 5, 0);
                mainNotify.Margin = new Thickness(0, 0, 0, 0);
                titleGrid.Margin = new Thickness(2, 2, 0, 0);
            }
            else
            {
                titleGrid.Margin = new Thickness();
                mainMenu.Margin = new Thickness(-2);
                mainNotify.Margin = new Thickness(7, -2, 0, -2);
            }
            if (WindowState == System.Windows.WindowState.Normal || WindowState == System.Windows.WindowState.Maximized)
                ANotifyPropertyChanged.StartNotifyChanging();
            else if (WindowState == System.Windows.WindowState.Minimized)
                ANotifyPropertyChanged.StopNotifyChanging();
        }

        public void Exit()
        {
            Hide();
            foreach (var item in ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToList())
            {
                item.Dispose();
            }
            try
            {
                SerializeData.SaveLinkInfoesToFileNoThread();
            }
            catch
            {

            }
            Process.GetCurrentProcess().Kill();
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Exit();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Agrin.UI.ViewModels.Pages.PagesManagerViewModel.SetIndex(3);
            MainWindow.This.IsShowPage = true;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Maximized;
            this.Activate();
        }

        private void notifyButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            notify.Visible = true;
            ANotifyPropertyChanged.StopNotifyChanging();
        }

        //private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (WindowState == System.Windows.WindowState.Maximized)
        //        WindowState = System.Windows.WindowState.Normal;
        //    else
        //    {
        //        WindowState = System.Windows.WindowState.Maximized;
        //    }

        //}






    }
}
