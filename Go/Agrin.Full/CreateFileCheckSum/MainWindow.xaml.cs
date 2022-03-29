using Agrin.Download.Engine.Repairer;
using Agrin.IO.File;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CreateFileCheckSum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                //System.IO.File.OpenWrite(@"C:\Users\Ali Visual Studio\AppData\Local\Xamarin\Android.Support.v4\22.2.0\android_m2repository_r15 - fix.zip").SetLength(95620673);
                //string[] items = System.IO.File.ReadAllLines(@"D:\aaa.txt");
                //for (int i = 0; i < items.Length; i++)
                //{
                //    items[i] = "{\"" + items[i].Replace("	", "\",\"") + "\"},";
                //}
                //System.IO.File.WriteAllLines(@"D:\aaa.txt", items);
            }
            catch
            {

            }
            InitializeComponent();

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Files|*.*";
            if (dialog.ShowDialog().Value)
            {
                btnStart.IsEnabled = false;
                Task task = new Task(() =>
                {
                    FileCheckSum.ProgressAction = (value, lenght) =>
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    mainProgress.Maximum = lenght;
                                    mainProgress.Value = value;
                                }));
                        };
                    var chekcSum = FileCheckSum.GetFileCheckSum(dialog.FileName);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (chekcSum == null)
                        {
                            MessageBox.Show("Error");
                        }
                        else
                        {
                        newSave:
                            SaveFileDialog save = new SaveFileDialog();
                            save.Filter = "CheckSum File|*.CheckSum";
                            if (save.ShowDialog().Value)
                            {
                                try
                                {
                                    FileCheckSum.SaveToFile(chekcSum, save.FileName);
                                }
                                catch (Exception eee)
                                {
                                    MessageBox.Show(eee.Message);
                                    goto newSave;
                                }
                            }
                        }
                        btnStart.IsEnabled = true;
                    }));
                });
                task.Start();
            }
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            var data = FileCheckSum.GetErrorsFromTwoCheckSum(@"D:\Film\fixed automatic.CheckSum", @"D:\Film\true.CheckSum");
            //byte[] bytes1 = null, bytes2 = null;
            //using (System.IO.FileStream stream = new System.IO.FileStream(@"D:\Film\Dragon.Blade.2015.720p.Farsi.Dubbed(TinyMoviez).Fixed.mkv", System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
            //{
            //    stream.Seek(177377181, System.IO.SeekOrigin.Begin);
            //    bytes1 = FileCheckSum.GetBytesPerBuffer(stream, 1024 * 1024 * 5);
            //}

            //using (System.IO.FileStream stream = new System.IO.FileStream(@"D:\Film\Error Download Dragon.Blade.2015.720p.Farsi.Dubbed(TinyMoviez).Fixed.mkv", System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
            //{
            //    stream.Seek(177377181, System.IO.SeekOrigin.Begin);
            //    bytes2 = FileCheckSum.GetBytesPerBuffer(stream, 1024 * 1024 * 5);
            //}
            //for (int i = 0; i < bytes1.Length; i++)
            //{
            //    if (bytes1[i] != bytes2[i])
            //    {

            //    }
            //}

            //var obj = (Agrin.Download.Data.Serializition.LinkInfoSerialize)Agrin.IO.Helper.SerializeStream.OpenSerializeStream(@"D:\BaseProjects\Agrin Download Manager\Agrin.Full\Agrin.Windows.UI\bin\Debug\Ali Visual Studio\BKCL\25.agn");
            //foreach (var chk in data)
            //{
            //    foreach (var item in obj.DownloadingProperty.DownloadRangePositions)
            //    {
            //        var val = chk.EndPosition;

            //        //if (item > val - (1024 * 1024 * 1) && item < val + (1024 * 1024 * 1))
            //        //{
            //        //    break;
            //        //}
            //        //val = chk.EndPosition;
            //        if (item > val - (1024 * 1024 * 1) && item < val + (1024 * 1024 * 1))
            //        {
            //            break;
            //        }
            //    }
            //}

        }

        private void btnBytes_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Files|*.*";
            if (dialog.ShowDialog().Value)
            {
                for (int i = 0; i < 1024 * 1024 * 5; i += 50)
                {
                    var bytes = FileCheckSum.GetListOfBytes(dialog.FileName, 127926270 + (i), 50);
                    bool f1 = false, f2 = false, f3 = false, f4 = false;
                    int ii = 0;
                    foreach (var item in bytes)
                    {
                        if (item == 231)
                            f1 = true;
                        else if (item == 168 && f1)
                            f2 = true;
                        else if (item == 200 && f2)
                            f3 = true;
                        else if (item == 120 && f3)
                            f4 = true;
                        else if (item == 130 && f4)
                            f4 = true;
                        else
                        {
                            f1 = false;
                            f2 = false;
                            f3 = false;
                            f4 = false;
                        }
                        ii++;
                    }
                }
                //mainListBox.ItemsSource = FileCheckSum.GetListOfBytes(dialog.FileName, 133169150 + (1024 * 500), 50);
            }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            btnDownload.IsEnabled = false;
            Task task = new Task(() =>
            {
                FileCheckSum.ProgressAction = (value, lenght) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mainProgress.Maximum = lenght;
                        mainProgress.Value = value;
                    }));
                };
                var data = FileCheckSum.GetErrorsFromTwoCheckSum(@"D:\Error.CheckSum", @"D:\true.CheckSum");
                var ok = LinkRepairer.DownloadCheckSums(@"D:\Film\", data, "http://www.frameapp.ir/Download/DownloadOneFile/android_m2repository_r15.zip", null);
                if (ok)
                {
                    if (LinkRepairer.StartFileRepair(@"C:\Users\Ali Visual Studio\AppData\Local\Xamarin\Android.Support.v4\22.2.0\android_m2repository_r15 - fix.zip", data))
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show("Save OK");
                            btnDownload.IsEnabled = true;
                        }));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show("Save Error");
                            btnDownload.IsEnabled = true;
                        }));
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Download Error");
                        btnDownload.IsEnabled = true;
                    }));
                }

            });
            task.Start();
        }
        System.Threading.Thread taska = null;
        private void btnGetPositions_Click(object sender, RoutedEventArgs e)
        {
            //btnGetPositions.IsEnabled = false;
            LinkRepairer.LinkRepairerProcessAction = (v, l, s) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    mainProgress.Maximum = l;
                    mainProgress.Value = v;
                }));
            };
            if (taska != null)
            {
                taska.Abort();
                return;
            }
            taska = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    HttpWebRequest aaa = (HttpWebRequest)HttpWebRequest.Create("https://cafebazaar.ir/developers/docs/iab/faq/?l=fa");
                    aaa.GetResponse().GetResponseStream();
                }
                //LinkRepairer.RepairFile(obj, @"D:\Projects\monodroid-samples-master must Fix.zip", @"D:\Projects\monodroid-samples-master must Fix.zip");

                //var cons = LinkRepairer.FindConnectionProblems(obj, @"D:\Film\Error Download Dragon.Blade.2015.720p.Farsi.Dubbed(TinyMoviez).Fixed.mkv");

            });
            taska.Start();
        }
    }
}
