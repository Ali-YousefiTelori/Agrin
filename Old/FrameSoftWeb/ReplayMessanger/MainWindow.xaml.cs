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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReplayMessanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<int> replayToIDs = new List<int>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                replayToIDs.Clear();
                txtUserMesages.Text = "";
                txtReplay.Text = "";
                string uri = "http://framesoft.ir/reporter/GetLastNoAswerMessage";

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    string jsonString = client.DownloadString(uri);
                    if (jsonString == "No Message Found")
                    {
                        MessageBox.Show("No Message Found");
                        return;
                    }

                    var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserMessage>>(jsonString);
                    if (msg.Count > 0)
                    {
                        txtUserMesages.Text = "نرم افزار: " + client.ResponseHeaders["AppName"] + System.Environment.NewLine;
                        txtUserMesages.Text += "نسخه: " + client.ResponseHeaders["AppVersion"] + System.Environment.NewLine;

                        foreach (var item in msg)
                        {
                            lastGuidIDMessage = item.GuidId;
                            replayToIDs.Add(item.ID);
                            txtUserMesages.Text += "پیغام: " + item.Message + System.Environment.NewLine;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = "http://framesoft.ir/reporter/ReplayToUserMessage";
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    if (replayToIDs.Count == 0)
                    {
                        MessageBox.Show("No Replay Id Found");
                        return;
                    }
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.Headers.Add("messageIDs", Newtonsoft.Json.JsonConvert.SerializeObject(replayToIDs.ToArray()));
                    client.Headers.Add("message", Convert.ToBase64String(Encoding.UTF8.GetBytes(txtReplay.Text)));
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

        int lastGuidIDMessage = -1;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                replayToIDs.Clear();
                txtUserMesages.Text = "";
                txtReplay.Text = "";
                string uri = "http://framesoft.ir/reporter/GetLastNoAswerMessage";

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.Headers.Add("returnAll", "true");
                    string jsonString = client.DownloadString(uri);
                    if (jsonString == "No Message Found")
                    {
                        MessageBox.Show("No Message Found");
                        return;
                    }
                    var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserMessage>>(jsonString);
                    if (msg.Count > 0)
                    {
                        txtUserMesages.Text = "نرم افزار: " + client.ResponseHeaders["AppName"] + System.Environment.NewLine;
                        txtUserMesages.Text += "نسخه: " + client.ResponseHeaders["AppVersion"] + System.Environment.NewLine;
                        txtUserMesages.Text += "سیستم عامل: " + client.ResponseHeaders["OSName"] + System.Environment.NewLine;
                        txtUserMesages.Text += "نسخه ی سیستم عامل: " + client.ResponseHeaders["OSVersion"] + System.Environment.NewLine;
                        foreach (var item in msg)
                        {
                            lastGuidIDMessage = item.GuidId;
                            replayToIDs.Add(item.ID);
                            txtUserMesages.Text += "پیغام: " + item.Message + System.Environment.NewLine;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = "http://framesoft.ir/reporter/SkeepMessage";
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    if (replayToIDs.Count == 0)
                    {
                        MessageBox.Show("No Replay Id Found");
                        return;
                    }
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.Headers.Add("messageIDs", Newtonsoft.Json.JsonConvert.SerializeObject(replayToIDs.ToArray()));
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("OK?", "Black List?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    var uri = "http://framesoft.ir/reporter/SendToBlackList";
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        if (lastGuidIDMessage == -1)
                        {
                            MessageBox.Show("Not Send -1");
                            return;
                        }
                        client.Encoding = System.Text.Encoding.UTF8;
                        client.Headers.Add("GuidID", lastGuidIDMessage.ToString());
                        string jsonString = client.DownloadString(uri);
                        MessageBox.Show(jsonString);
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }

        List<int> replayNewToIDs = new List<int>();
        int newLastGuidIDMessage = -1;
        private void btn_newGetLastMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                replayNewToIDs.Clear();
                txtNewUserMesages.Text = "";
                txtNewReplay.Text = "";
                string uri = "http://framesoft.ir/reporter/GetNoReplayMessagesForAdmin";

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Headers.Add("getFullMessage", "false");
                    client.Encoding = System.Text.Encoding.UTF8;
                    string jsonString = client.DownloadString(uri);
                    if (jsonString == "No Message Found")
                    {
                        MessageBox.Show("No Message Found");
                        return;
                    }

                    var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserMessageInfoData>>(jsonString);
                    if (msg.Count > 0)
                    {
                        txtNewUserMesages.Text = "نرم افزار: " + client.ResponseHeaders["AppName"] + System.Environment.NewLine;
                        txtNewUserMesages.Text += "نسخه: " + client.ResponseHeaders["AppVersion"] + System.Environment.NewLine;
                        txtNewUserMesages.Text += "سیستم عامل: " + client.ResponseHeaders["OSName"] + System.Environment.NewLine;
                        txtNewUserMesages.Text += "نسخه ی سیستم عامل: " + client.ResponseHeaders["OSVersion"] + System.Environment.NewLine;

                        foreach (var item in msg)
                        {
                            //newLastGuidIDMessage = item.GuidId;
                            replayNewToIDs.Add(item.MessageID);
                            txtNewUserMesages.Text += "پیغام: " + item.Message + System.Environment.NewLine;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btn_newGetAllAnswer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtNewUserMesages.Text = "";
                txtNewReplay.Text = "";
                replayNewToIDs.Clear();
                string uri = "http://framesoft.ir/reporter/GetNoReplayMessagesForAdmin";

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Headers.Add("getFullMessage", "true");
                    client.Encoding = System.Text.Encoding.UTF8;
                    string jsonString = client.DownloadString(uri);
                    if (jsonString == "No Message Found")
                    {
                        MessageBox.Show("No Message Found");
                        return;
                    }

                    var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserMessageInfoData>>(jsonString);
                    if (msg.Count > 0)
                    {
                        txtNewUserMesages.Text = "نرم افزار: " + client.ResponseHeaders["AppName"] + System.Environment.NewLine;
                        txtNewUserMesages.Text += "نسخه: " + client.ResponseHeaders["AppVersion"] + System.Environment.NewLine;
                        txtNewUserMesages.Text += "سیستم عامل: " + client.ResponseHeaders["OSName"] + System.Environment.NewLine;
                        txtNewUserMesages.Text += "نسخه ی سیستم عامل: " + client.ResponseHeaders["OSVersion"] + System.Environment.NewLine;

                        foreach (var item in msg)
                        {
                            //newLastGuidIDMessage = item.GuidId;
                            replayNewToIDs.Add(item.MessageID);
                            txtNewUserMesages.Text += "پیغام: " + item.Message + System.Environment.NewLine;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btn_newReplay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (replayNewToIDs.Count == 0)
                {
                    MessageBox.Show("No ID For Replay");
                    return;
                }
                else if (string.IsNullOrEmpty(txtNewReplay.Text))
                {
                    MessageBox.Show("MessageIsEmpty");
                    return;
                }
                string uri = "http://framesoft.ir/reporter/SendReplayForMessagesForAdmin";

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    var msg = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(txtNewReplay.Text));
                    client.Headers.Add("messageIDs", Newtonsoft.Json.JsonConvert.SerializeObject(replayNewToIDs.ToArray()));
                    client.Headers.Add("message", msg);

                    var link = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(txtLink.Text));
                    client.Headers.Add("UserLinkAddress", link);

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

        private void btn_SendForAll_Click(object sender, RoutedEventArgs e)
        {
            SendPublicMessageWindow win = new SendPublicMessageWindow();
            win.Show();
        }

        private void btn_newSkeep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = "http://framesoft.ir/reporter/SkeepReplayMessages";
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    if (replayNewToIDs.Count == 0)
                    {
                        MessageBox.Show("No Replay Id Found");
                        return;
                    }
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.Headers.Add("messageIDs", Newtonsoft.Json.JsonConvert.SerializeObject(replayNewToIDs.ToArray()));
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
