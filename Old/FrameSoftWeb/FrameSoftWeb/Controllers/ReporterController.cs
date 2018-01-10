using Agrin.Framesoft.Messages;
using FrameSoft.Agrin.DataBase.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers
{
    public class ReporterController : Controller
    {
        //
        // GET: /Reporter/

        public ActionResult FeedBackMessage()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("Guid") || !allkeys.Contains("Name") || !allkeys.Contains("Email") || !allkeys.Contains("message") || !allkeys.Contains("LastUserMessageID"))
                {
                    return Content("Not Found");
                }
                var headers = this.Request.Headers;


                if (string.IsNullOrEmpty(headers["message"]) || string.IsNullOrEmpty(headers["Guid"]))
                    return Content("OK!");

                Guid guid = new Guid(headers["Guid"]);
                if (Engine.Amar.FramesoftServiceProvider.IsBlackListGUID(guid))
                    return Content("Black List");
                var msg = new UserMessage();
                msg.Name = headers["Name"];
                msg.Email = headers["Email"];
                msg.Message = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(headers["message"]));
                msg.LastUserMessageID = int.Parse(headers["LastUserMessageID"]);
                msg.GuidId = Engine.Amar.FramesoftServiceProvider.GetAndUpdateGuidIDByGuid(guid);
                Engine.Amar.FramesoftServiceProvider.AddUserMesage(msg);
                return Content("OK");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/FeedBackMessage");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult ReplayToUserMessage()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("messageIDs") || !allkeys.Contains("message"))
                {
                    return Content("Not Found");
                }
                var headers = this.Request.Headers;
                //var jarray = Newtonsoft.Json.Linq.JArray.Parse(;
                int[] messageID = (int[])Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(headers["messageIDs"]);
                string message = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(headers["message"]));
                if (Engine.Amar.FramesoftServiceProvider.ReplayToUserMessage(messageID, message))
                    return Content("Success");
                return Content("Error");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/ReplayToUserMessage");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult SendToBlackList()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("GuidID"))
                {
                    return Content("Not Found");
                }
                var headers = this.Request.Headers;
                int guidID = int.Parse(headers["GuidID"]);
                if (Engine.Amar.FramesoftServiceProvider.SendToBlackList(guidID))
                    return Content("Success");
                return Content("Exist");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/SkeepMessage");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult SkeepMessage()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("messageIDs"))
                {
                    return Content("Not Found");
                }
                var headers = this.Request.Headers;
                //var jarray = Newtonsoft.Json.Linq.JArray.Parse(;
                int[] messageID = (int[])Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(headers["messageIDs"]);
                if (Engine.Amar.FramesoftServiceProvider.CheckMarkAnswerMessage(messageID))
                    return Content("Success");
                return Content("Not Found answer Message");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/SkeepMessage");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult GetUserMessage()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("Guid") || !allkeys.Contains("LastUserMessageID"))
                {
                    return Content("Not Found");
                }

                var headers = this.Request.Headers;
                Guid guid = new Guid(headers["Guid"]);
                int guidID = Engine.Amar.FramesoftServiceProvider.GetAndUpdateGuidIDByGuid(guid);
                int lastmessageID = int.Parse(headers["LastUserMessageID"]);
                var replay = Engine.Amar.FramesoftServiceProvider.GetUserReplay(guidID, lastmessageID);
                if (replay == null)
                    return Content("Not Found");
                var info = new MessageInformation() { GUID = headers["Guid"], LastUserMessageID = replay.LastUserMessageID, Message = replay.Message };
                if (allkeys.Contains("AppVersion"))
                    return Content(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(info))));
                else
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(info));
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/GetUserMessage");
#endif

            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult GetLastNoAswerMessage()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                var headers = this.Request.Headers;
                int manualID = -1;
                if (allkeys.Contains("manualID"))
                {
                    manualID = int.Parse(headers["manualID"]);
                }

                bool returnAll = false;
                if (allkeys.Contains("returnAll"))
                {
                    returnAll = bool.Parse(headers["returnAll"]);
                }

                var items = manualID == -1 ? Engine.Amar.FramesoftServiceProvider.GetLastNoAswerMessage(returnAll) : Engine.Amar.FramesoftServiceProvider.GetNoAswerMessageById(manualID, returnAll);
                if (items == null || items.Count == 0)
                    return Content("No Message Found");
                var detail = Engine.Amar.FramesoftServiceProvider.GetGuidDetails(items.FirstOrDefault().GuidId);
                HttpContext.Response.AddHeader("AppVersion", Engine.Amar.FramesoftServiceProvider.GetApplicationVersionByGuidId(detail) ?? "null");
                HttpContext.Response.AddHeader("AppName", Engine.Amar.FramesoftServiceProvider.GetApplicationNameByGuidId(detail) ?? "null");
                HttpContext.Response.AddHeader("OSVersion", Engine.Amar.FramesoftServiceProvider.GetOSVersionByGuidId(detail) ?? "null");
                HttpContext.Response.AddHeader("OSName", Engine.Amar.FramesoftServiceProvider.GetOSNameByGuidId(detail) ?? "null");
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(items));
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/GetLastNoAswerMessage");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message + " stack" + e.StackTrace);
            }
        }

        /// <summary>
        /// برای نسخ ی جدید
        /// </summary>
        /// <returns>دریافت پیغام های پاسخ داده نشده</returns>
        public ActionResult GetNoReplayMessagesForAdmin()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("getFullMessage"))
                {
                    return Content("Not Found");
                }

                var headers = this.Request.Headers;
                bool getFull = bool.Parse(headers["getFullMessage"]);

                var replays = Engine.Amar.FramesoftServiceProvider.GetNoReplayMessages(getFull);
                if (replays == null || replays.Count == 0)
                    return Content("No Message Found");
                var item = replays.First();
                var detail = Engine.Amar.FramesoftServiceProvider.GetGuidDetails(item.GuidID);

                HttpContext.Response.AddHeader("AppVersion", Engine.Amar.FramesoftServiceProvider.GetApplicationVersionByGuidId(detail) ?? "not set value!");
                HttpContext.Response.AddHeader("AppName", Engine.Amar.FramesoftServiceProvider.GetApplicationNameByGuidId(detail) ?? "not set value!");
                HttpContext.Response.AddHeader("OSVersion", Engine.Amar.FramesoftServiceProvider.GetOSVersionByGuidId(detail) ?? "not set value!");
                HttpContext.Response.AddHeader("OSName", Engine.Amar.FramesoftServiceProvider.GetOSNameByGuidId(detail) ?? "not set value!");

                List<UserMessageInfoData> items = new List<UserMessageInfoData>();
                foreach (var replay in replays)
                {
                    items.Add(new UserMessageInfoData() { Message = replay.Message, MessageDateTime = replay.MessageDateTime, MessageID = replay.ID });
                }
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(items));
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/GetNoReplayMessagesForAdmin");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message + " s:" + e.StackTrace);
            }
        }

        /// <summary>
        /// نسخه ی جدید
        /// </summary>
        /// <returns>ارسال پاسخ به کاربر</returns>
        public ActionResult SendReplayForMessagesForAdmin()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("messageIDs") || !allkeys.Contains("message") || !allkeys.Contains("UserLinkAddress"))
                {
                    return Content("Not Found");
                }
                var headers = this.Request.Headers;
                //var jarray = Newtonsoft.Json.Linq.JArray.Parse(;
                int[] messageIDs = (int[])Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(headers["messageIDs"]);
                string message = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(headers["message"]));
                string link = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(headers["UserLinkAddress"]));

                int guidID = Engine.Amar.FramesoftServiceProvider.GetGuidIdByMessageID(messageIDs[0]);

                if (Engine.Amar.FramesoftServiceProvider.SendReplayForMessages(messageIDs, message, guidID, link))
                    return Content("Success");
                return Content("Error");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/SendReplayForMessagesForAdmin");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
        /// <summary>
        /// برای نسخه ی جدید
        /// </summary>
        /// <returns>چشم پوشی کردن چند سوال</returns>
        public ActionResult SkeepReplayMessages()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("messageIDs"))
                {
                    return Content("Not Found");
                }
                var headers = this.Request.Headers;
                //var jarray = Newtonsoft.Json.Linq.JArray.Parse(;
                int[] messageIDs = (int[])Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(headers["messageIDs"]);

                if (Engine.Amar.FramesoftServiceProvider.SkeepReplayMessages(messageIDs))
                    return Content("Success");
                return Content("Error");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/SendReplayForMessagesForAdmin");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        /// <summary>
        /// برای نسخه ی جدید
        /// </summary>
        /// <returns>دریافت پاسخ هایی که مدیر به کاربر داده این برای یوزر کاربرد دارد</returns>
        public ActionResult GetUserMessageReplays()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("Guid") || !allkeys.Contains("LastDateTime"))
                {
                    return Content("Not Found");
                }

                var headers = this.Request.Headers;
                Guid guid = new Guid(headers["Guid"]);
                int guidID = Engine.Amar.FramesoftServiceProvider.GetAndUpdateGuidIDByGuid(guid);
                long lastDateTimeTicks = long.Parse(headers["LastDateTime"]);
                var replays = Engine.Amar.FramesoftServiceProvider.GetUserMessageReplaysByDataTime(new DateTime(lastDateTimeTicks), guidID);
                if (replays == null || replays.Count == 0)
                    return Content("Not Found");
                List<UserMessageInfoData> items = new List<UserMessageInfoData>();
                foreach (var item in replays)
                {
                    items.Add(new UserMessageInfoData() { Message = item.Message, MessageDateTime = item.MessageDateTime, Link = item.Link, MessageID = item.ID });
                }
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(items));
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/GetUserMessageReplays");
#endif

            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }


        /// <summary>
        /// برای نسخه ی جدید
        /// </summary>
        /// <returns>ارسال سوال از طرف کاربر</returns>
        public ActionResult FeedBackUserMessage()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("Guid") || !allkeys.Contains("Name") || !allkeys.Contains("Email") || !allkeys.Contains("message"))
                {
                    return Content("Not Found");
                }

                var headers = this.Request.Headers;

                if (string.IsNullOrEmpty(headers["message"]) || string.IsNullOrEmpty(headers["Guid"]))
                    return Content("OK!");

                Guid guid = new Guid(headers["Guid"]);
                if (Engine.Amar.FramesoftServiceProvider.IsBlackListGUID(guid))
                    return Content("Black List");
                int id = Engine.Amar.FramesoftServiceProvider.GetAndUpdateGuidIDByGuid(guid);
                Engine.Amar.FramesoftServiceProvider.SendUserFeedbackMessage(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(headers["message"])), id);
                return Content("OK");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/FeedBackUserMessage");
#endif
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        /// <summary>
        /// برای نسخه ی جدید
        /// </summary>
        /// <returns>دریافت آخرین پیغام های عمومی</returns>
        public ActionResult GetLastPublicMessages()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("LastDateTime") || !allkeys.Contains("LimitMessage"))
                {
                    return Content("Not Found");
                }

                var headers = this.Request.Headers;
                var limit = (LimitMessageEnum)int.Parse(headers["LimitMessage"]);
                long lastDateTimeTicks = long.Parse(headers["LastDateTime"]);
                var replays = Engine.Amar.FramesoftServiceProvider.GetMessagesByDataTime(new DateTime(lastDateTimeTicks), limit);
                if (replays == null || replays.Count == 0)
                    return Content("Not Found");
                List<PublicMessageInfoReceiveData> items = new List<PublicMessageInfoReceiveData>();
                foreach (var item in replays)
                {
                    items.Add(new PublicMessageInfoReceiveData() { Message = item.Message, MessageDateTime = item.MessageDateTime, Title = item.Title, Link = item.Link, MessageID = item.ID });
                }
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(items));
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/GetUserMessageReplays");
#endif

            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }


        /// <summary>
        /// برای نسخه ی جدید
        /// </summary>
        /// <returns>ارسال یک پیام عمومی</returns>
        public ActionResult SendPublicMessages()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("message") || !allkeys.Contains("LimitMessage") || !allkeys.Contains("title"))
                {
                    return Content("Not Found");
                }

                var headers = this.Request.Headers;
                var message = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(headers["message"]));
                var title = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(headers["title"]));
                var limit = (LimitMessageEnum)int.Parse(headers["LimitMessage"]);

                Engine.Amar.FramesoftServiceProvider.AddPublicMessage(message, title, limit);
                return Content("Success");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/GetUserMessageReplays");
#endif

            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
        static volatile string LastError = "";
        public ActionResult ReportApplicationError()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var headers = this.Request.Headers;
                var version = headers["AppVersion"];
                if (version != "2.2.3")
                    return Content("nok");

                var guid = headers["Guid"];
                if (!Guid.TryParse(guid,out Guid g))
                    return Content("");
                var guidId = Engine.Amar.FramesoftServiceProvider.GetAndUpdateGuidIDByGuid(g);
                var detail = Engine.Amar.FramesoftServiceProvider.GetGuidDetails(guidId);
                var osVersion = Engine.Amar.FramesoftServiceProvider.GetOSVersionByGuidId(detail);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("New Start=>");
                builder.AppendLine(osVersion);
                builder.AppendLine(version);
                builder.AppendLine(headers["ErrorMessage"]);
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    builder.AppendLine(reader.ReadToEnd());
                }
                Engine.Amar.FramesoftServiceProvider.LogError(builder.ToString().Replace("  ", "").Trim());
                Engine.Amar.FramesoftServiceProvider.AddApplicationErrorLogReport(new ApplicationErrorReport() { ErrorLog = headers["ErrorMessage"] });
                return Content("OK");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Reporter/ReportApplicationError");
#endif

            }
            catch (Exception e)
            {
                LastError = "**** Server Error ***" + e.ToString() + Environment.NewLine + " Headers:" + System.Environment.NewLine + this.Request.Headers.ToString();
                return Content(e.Message);
            }
        }

        public ActionResult GetLastError()
        {
            return Content(LastError);
        }
    }
    public class MessageInformation
    {
        public string GUID { get; set; }
        public string Message { get; set; }
        public int LastUserMessageID { get; set; }
    }
}
