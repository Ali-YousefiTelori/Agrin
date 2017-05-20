using Agrin.Framesoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft.Helper
{
    public static class FeedBackHelper
    {
        public static LimitMessageEnum DefaultLimitMessage = LimitMessageEnum.All;
        static string domain = UserManagerHelper.domain;
        public static bool SendFeedBack(Guid guid, string message, string userName, string email)
        {
            //ارسال فیدبک
            string uri = "http://" + domain + "/reporter/FeedBackUserMessage";

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers.Add("Name", userName);
                client.Headers.Add("Email", email);
                client.Headers.Add("message", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message)));
                client.Headers.Add("Guid", guid.ToString());

                client.Encoding = System.Text.Encoding.UTF8;
                string jsonString = client.DownloadString(uri);
                if (jsonString == "OK")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///دریافت پاسخ های از طرف مدیر 
        /// </summary>
        /// <param name="lastDateTime"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static List<UserMessageInfoData> GetUserMessageReplays(DateTime lastDateTime, Guid guid)
        {
           

            string uri = "http://" + domain + "/reporter/GetUserMessageReplays";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers.Add("LastDateTime", lastDateTime.Ticks.ToString());
                //client.Headers.Add("LastDateTime", DateTime.Parse("2015-12-12T17:18:24.77").Ticks.ToString());
                client.Headers.Add("Guid", guid.ToString());

                client.Encoding = System.Text.Encoding.UTF8;
                string jsonString = client.DownloadString(uri);
                if (jsonString == "Not Found")
                {
                    //no replay
                    return null;
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserMessageInfoData>>(jsonString);
            }
        }

        public static List<PublicMessageInfoReceiveData> GetLastPublicMessages(DateTime lastDateTime)
        {
            ///دریافت اخرین پیام های عمومی

            string uri = "http://" + domain + "/reporter/GetLastPublicMessages";

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers.Add("LastDateTime", lastDateTime.Ticks.ToString());
                //client.Headers.Add("LastDateTime", DateTime.Parse("2015-12-12T17:18:24.77").Ticks.ToString());
                client.Headers.Add("LimitMessage", ((int)DefaultLimitMessage).ToString());

                client.Encoding = System.Text.Encoding.UTF8;
                string jsonString = client.DownloadString(uri);
                if (jsonString == "Not Found")
                {
                    //no replay
                    return null;
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<PublicMessageInfoReceiveData>>(jsonString);
            }
        }
    }
}
