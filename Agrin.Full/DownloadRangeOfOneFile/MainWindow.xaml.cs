using System;
using System.Collections.Generic;
using System.IO;
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

namespace DownloadRangeOfOneFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                //string address = "http://framesoft.ir/download/downloadonefile/Agrin.Android-Aligned.rar";
                //long range = 6372968;
                //long size = 6374730;


                //var _request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(address);
                //_request.Timeout = 60000;
                //_request.AllowAutoRedirect = true;
                //_request.ServicePoint.ConnectionLimit = int.MaxValue;

                //_request.KeepAlive = true;
                //_request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                //_request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                //_request.AddRange(range);

                //long startBytes = 0;
                //int packSize = 1024 * 10; //read in block，every block 10K bytes
                //var _response = _request.GetResponse();
                //Stream myFile = _response.GetResponseStream();
                //BinaryReader br = new BinaryReader(myFile);

                ////int sleep = (int)Math.Ceiling(1000.0 * packSize / speed);//the number of millisecond
                ////string lastUpdateTiemStr = System.IO.File.GetLastWriteTimeUtc(FileDownloadName).ToString("r");
                ////string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + lastUpdateTiemStr;

                ////validate whether the file is too large
                //if (_response.ContentLength <= 0)
                //{
                //    //context.HttpContext.Response.StatusCode = 404;
                //    return;
                //}

                //long fileLength = size - range;

                //try
                //{
                //    //send data
                //    int maxCount = (int)Math.Ceiling((fileLength - startBytes + 0.0) / packSize);//download in block
                //    for (int i = 0; i < maxCount; i++)
                //    {
                //        var read = br.ReadBytes(packSize);
                //    }
                //}
                //catch (Exception ex)
                //{
                    
                //}
                //finally
                //{
                //    br.Close();
                //    myFile.Close();
                //}

                if (responseStreamNew == null)
                {
                    _address = "";
                    var _request = (HttpWebRequest)WebRequest.Create("http://framesoft.ir/download/DownloadEndOfOneFileStreamIfCannotRead");
                    _request.Headers.Add("address", "http://agrindownloadmanager.ir/download/downloadonefile/Agrin.Android-Aligned.rar");
                    _request.Headers.Add("RNG", 6372001.ToString());
                    _request.Headers.Add("Size", 6374730.ToString());
                    var _response = (HttpWebResponse)_request.GetResponse();
                    responseStreamNew = _response.GetResponseStream();
                    StreamReader reader = new StreamReader(_response.GetResponseStream());
                    var test = reader.ReadToEnd();
                    responseStreamNew.ReadTimeout = 1000 * 5;
                }
                byte[] bytes = new byte[1024];
                var readCount = responseStreamNew.Read(bytes, 0, 1024);
            }
            catch (WebException ex)
            {
                var text = ex.Response.Headers["ex1"];
                var text2 = ex.Response.Headers["ex2"];
            }
            catch
            {

            }
        }
        int getBug = 0;
        string _address;
        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            Action<Action> runOnUI = (run) =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                        {
                            run();
                        }));
                };
            string address = txtAddress.Text;
            _address = address;
            long range = long.Parse(txtStart.Text);
            long lenght = long.Parse(txtEnd.Text);
            Task task = new Task(() =>
            {
                try
                {
                    var _request = (HttpWebRequest)WebRequest.Create(address);
                    //_request.Timeout = 60000;
                    //_request.AllowAutoRedirect = true;
                    //_request.ServicePoint.ConnectionLimit = ParentLinkWebRequest.Parent.DownloadingProperty.ConnectionCount + 1;
                    //_request.ServicePoint.ConnectionLimit = int.MaxValue;

                    //_request.KeepAlive = true;
                    //_request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                    //_request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                    _request.AddRange(range);
                    var size = lenght - range;
                    MemoryStream stream = null;
                    using (var _response = (HttpWebResponse)_request.GetResponse())
                    {
                        var _responseStream = _response.GetResponseStream();
                        _responseStream.ReadTimeout = 1000 * 5;
                        _size = _response.ContentLength;

                        stream = new MemoryStream();

                        while (stream.Length < size)
                        {
                            int count = 1024 * 1024 * 100;
                            byte[] bytes = new byte[count];
                            int readCount = 0;
                            runOnUI(() =>
                            {
                                txtdownloaded.Text = "Reading...";
                            });
                            try
                            {
                                _range = stream.Length;
                                ReadBytes(out readCount, _responseStream, ref bytes, count);
                            }
                            catch//(Exception exx)
                            {

                            }
                            stream.Write(bytes, 0, readCount);
                            runOnUI(() =>
                            {
                                progressMain.Maximum = size;
                                progressMain.Value = stream.Length;
                                txtdownloaded.Text = stream.Length.ToString();
                            });
                        }
                    }
                    _request.Abort();
                    runOnUI(() =>
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        File.WriteAllBytes("D:\\test end of range.rar", stream.ToArray());
                        MessageBox.Show("Compelete");
                    });
                }
                catch (Exception ex)
                {
                    runOnUI(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                }
            });
            task.Start();
        }

        long _range = 0;
        long _size = 0;
        bool mustReadISP = false;
        Stream responseStreamNew = null;
        public void ReadBytes(out int readCount, Stream _responseStream, ref byte[] bytes, int count)
        {
            if (mustReadISP)
            {
                if (responseStreamNew == null)
                {
                    var _request = (HttpWebRequest)WebRequest.Create("http://framesoft.ir/download/DownloadEndOfOneFileStreamIfCannotRead");
                    _request.Headers.Add("address", _address);
                    _request.Headers.Add("RNG", _range.ToString());
                    _request.Headers.Add("Size", _size.ToString());
                    var _response = (HttpWebResponse)_request.GetResponse();
                    responseStreamNew = _response.GetResponseStream();
                    responseStreamNew.ReadTimeout = 1000 * 5;
                }
                readCount = responseStreamNew.Read(bytes, 0, count);
            }
            else
            {
                readCount = _responseStream.Read(bytes, 0, count);
                int iii = readCount;
                if (readCount == 0)
                    getBug++;
                if (getBug >= 5)
                    mustReadISP = true;
            }
        }

        private void btnGetSize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _request = (HttpWebRequest)WebRequest.Create(txtAddress.Text);
                _request.Timeout = 60000;
                _request.AllowAutoRedirect = true;
                //_request.ServicePoint.ConnectionLimit = ParentLinkWebRequest.Parent.DownloadingProperty.ConnectionCount + 1;
                _request.ServicePoint.ConnectionLimit = int.MaxValue;

                _request.KeepAlive = true;
                _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

                using (var _response = (HttpWebResponse)_request.GetResponse())
                {
                    txtEnd.Text = _response.ContentLength.ToString();
                }
                _request.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnForce_Click(object sender, RoutedEventArgs e)
        {
            mustReadISP = true;
        }
    }
}
