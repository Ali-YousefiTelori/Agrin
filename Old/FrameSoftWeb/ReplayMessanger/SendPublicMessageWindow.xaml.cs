using Agrin.Framesoft.Messages;
using FrameSoft.Agrin.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReplayMessanger
{
    public class LimitEnumInfo
    {
        public LimitMessageEnum Limit { get; set; }
        public string Text { get; set; }
    }

    /// <summary>
    /// Interaction logic for SendPublicMessageWindow.xaml
    /// </summary>
    public partial class SendPublicMessageWindow : Window
    {
        public SendPublicMessageWindow()
        {
            InitializeComponent();
            List<LimitEnumInfo> items = new List<LimitEnumInfo>();
            items.Add(new LimitEnumInfo() { Limit = LimitMessageEnum.All, Text= "همه" });
            items.Add(new LimitEnumInfo() { Limit = LimitMessageEnum.Android, Text = "اندروید" });
            items.Add(new LimitEnumInfo() { Limit = LimitMessageEnum.Windows, Text = "ویندوز" });
            items.Add(new LimitEnumInfo() { Limit = LimitMessageEnum.WindowsPhone, Text = "ویندوز فون" }); 
            items.Add(new LimitEnumInfo() { Limit = LimitMessageEnum.IOS, Text = "گوشی اپل" });
            items.Add(new LimitEnumInfo() { Limit = LimitMessageEnum.Linux, Text = "لینوکس" });
            items.Add(new LimitEnumInfo() { Limit = LimitMessageEnum.Mac, Text = "مک" });
            cboLimit.ItemsSource = items;
            cboLimit.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
             try
            {
                string uri = "http://framesoft.ir/reporter/SendPublicMessages";

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    var msg = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(txtMessage.Text));
                    var title = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(txtTitle.Text));
                    client.Headers.Add("LimitMessage", ((int)((LimitEnumInfo)cboLimit.SelectedItem).Limit).ToString());
                    client.Headers.Add("title", title);
                    client.Headers.Add("message", msg);

                    client.Encoding = System.Text.Encoding.UTF8;
                    string jsonString = client.DownloadString(uri);
                    if (jsonString == "Success")
                    {
                        MessageBox.Show("Success");
                    }
                    else
                    {
                        MessageBox.Show(jsonString);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
    }
}
