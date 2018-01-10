using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Agrin.About
{
    public class SendMessage
    {
#if (DEBUG)
        public bool SendMessages(string name, string mail, string title, string message, string[] attachFiles)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("ali.visual.studio.messenger", "H!A@M#I$S%H^E&B*A(H)A<R>?\"3{7}7_3+2.0-8=");
                StringBuilder body = new StringBuilder();

                body.Append(@"<html><style>
.hl {font-size: 12pt;font-family: Tahoma; font-weight: bold;text-decoration:none;color:green}
.hl2 {font-size: 10pt;font-family: Tahoma; text-decoration:none;color:black}
</style>
<body><div class=""page"" dir=""rtl"">");
                message = message.Replace("\r\n", "<br/>");
                body.Append(@"<p><span class=""hl"">" + "نام: " + @"</span><span class=""hl2"" dir=""rtl"">" + name + "</span></p>");
                body.Append(@"<p><span class=""hl"">" + "ایمیل: " + @"</span><span class=""hl2"" dir=""ltr"">" + mail + "</span></p>");
                body.Append(@"<p><span class=""hl"">" + "تیتر: " + @"</span><span class=""hl2"" dir=""rtl"">" + title + "</span></p>");
                body.Append("<br/>");
                body.Append("---------------------------------------------------------------------------------------------------");
                body.Append("<br/>");
                body.Append(@"<p><span class=""hl"">" + "متن: " + @"</span><span class=""hl2"" dir=""rtl"">" + message + "</span></p>");
                body.AppendLine(@"</div></body></html>");
                MailMessage msg = new MailMessage("ali.visual.studio.messenger@gmail.com", "ali.visual.studio@gmail.com", "Agrin Nazarat", body.ToString());
                msg.IsBodyHtml = true;
                if (attachFiles != null)
                    foreach (var item in attachFiles)
                    {
                        msg.Attachments.Add(new Attachment(item));
                    }
                msg.BodyEncoding = Encoding.UTF8;
                client.Send(msg);
                return true;
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "SendMessages");
                return false;
            }
        }

#endif

        public static bool SendFeedBack(UserMessage message)
        {
            try
            {
                string uri = "http://framesoft.ir/Reporter/FeedBackMessage";
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Headers.Add("Guid", message.GUID);
                    client.Headers.Add("Name", message.Name);
                    client.Headers.Add("Email", message.Email);
                    client.Headers.Add("message", Convert.ToBase64String(Encoding.UTF8.GetBytes(message.message)));
                    client.Headers.Add("LastUserMessageID", message.LastUserMessageID.ToString());
                    string jsonString = client.DownloadString(uri);
                    if (jsonString == "OK")
                        return true;
                    else
                    {
                        return false;
                    }
                }

            }
            catch
            {
                return false;
            }
        }
    }

    public class UserMessage
    {
        public string GUID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string message { get; set; }
        public int LastUserMessageID { get; set; }
    }
}
