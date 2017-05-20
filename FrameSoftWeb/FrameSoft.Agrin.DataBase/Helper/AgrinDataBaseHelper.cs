using Agrin.Framesoft.Messages;
using FrameSoft.Agrin.DataBase.Contexts;
using FrameSoft.Agrin.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Helper
{
    public static class AgrinDataBaseHelper
    {
        public static void RegisterUser(UserInfo user)
        {
            using (var ctx = new AgrinContext())
            {
                user.RegisterDateTime = DateTime.Now;
                ctx.Users.Add(user);
                ctx.SaveChanges();
            }
        }

        public static int LoginUser(string userName, string password)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.Users where u.UserName == userName && u.Password == password select u);
                if (items.FirstOrDefault() == null)
                    return -1;
                return items.FirstOrDefault().ID;
            }
        }

        public static bool ExistUserName(string userName)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.Users where u.UserName == userName select u);
                if (items.FirstOrDefault() == null)
                    return false;
                return true;
            }
        }

        public static bool ExistEmail(string email)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.Users where u.Email == email select u);
                if (items.FirstOrDefault() == null)
                    return false;
                return true;
            }
        }

        public static UserInfo GetUserPropertiesInfo(int userID)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.Users where u.ID == userID select u);
                return items.FirstOrDefault();
            }
        }

        public static void SubUserStorageSize(int userID, long size)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.Users where u.ID == userID select u);
                items.FirstOrDefault().UserSize -= size;
                ctx.SaveChanges();
            }
        }

        public static void SumUserStorageSize(int userID, long size)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.Users where u.ID == userID select u);
                items.FirstOrDefault().UserSize += size;
                ctx.SaveChanges();
            }
        }

        public static int CreateLeechFile(int userID, long size, string path, string link, int formatCode, string fileName)
        {
            using (var ctx = new AgrinContext())
            {
                var info = new UserFileInfo() { CreatedDateTime = DateTime.Now, DiskPath = path, FileGuid = Guid.NewGuid(), FileName = fileName, Link = link, Size = size, UserID = userID, FormatCode = formatCode };
                ctx.LeechFiles.Add(info);
                ctx.SaveChanges();
                return info.ID;
            }
        }

        public static void BuyStorageData(UserPurchase buy)
        {
            using (var ctx = new AgrinContext())
            {
                ctx.UserPurchases.Add(buy);
                ctx.SaveChanges();
            }
        }

        public static bool ExistPurchaseByToken(int userID, string token)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.UserPurchases where u.UserID == userID && u.PurchaseToken == token select u);
                var item = items.FirstOrDefault();
                if (item == null)
                    return false;
                else
                    return true;
            }
        }

        public static void UpdateLeechFile(UserFileInfo fileInfo)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.LeechFiles where u.ID == fileInfo.ID select u);
                var item = items.FirstOrDefault();
                item.Status = fileInfo.Status;
                item.IsError = fileInfo.IsError;
                item.IsComplete = fileInfo.IsComplete;
                item.IsUserDownloadedThis = fileInfo.IsUserDownloadedThis;
                item.IsDeletedByApplication = fileInfo.IsDeletedByApplication;
                ctx.SaveChanges();
            }
        }

        public static UserFileInfo GetLeechFileInfo(int userID, int fileID)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.LeechFiles where u.UserID == userID && u.ID == fileID select u);
                return items.FirstOrDefault();
            }
        }

        public static List<UserFileInfo> GetCanDownloadLeechFileInfoes()
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.LeechFiles where u.Status != 5 && !u.IsDeletedByApplication && !u.IsUserDownloadedThis && !u.IsComplete select u);
                return items.ToList();
            }
        }

        public static UserFileInfo GetLeechFileInfoByGUID(int userID, string guid)
        {
            using (var ctx = new AgrinContext())
            {
                Guid guidM = Guid.Parse(guid);
                var items = (from u in ctx.LeechFiles where u.UserID == userID && u.FileGuid == guidM select u);
                return items.FirstOrDefault();
            }
        }

        public static List<UserFileInfo> GetListLeechFileInfoes(int userID)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from u in ctx.LeechFiles where u.UserID == userID && !u.IsUserDownloadedThis && !u.IsDeletedByApplication select u);
                return items.ToList();
            }
        }

        public static UserMessage GetUserReplay(int guidID, int lastUserMessageID)
        {
            using (var ctx = new AgrinContext())
            {
                var find = from x in ctx.UserMessages where x.GuidId == guidID && x.IsReplay && x.LastUserMessageID > lastUserMessageID select x;
                return find.FirstOrDefault();
            }
        }

        public static void AddUserMesage(UserMessage userMessage)
        {
            using (var ctx = new AgrinContext())
            {
                userMessage.InsertDateTime = DateTime.Now;
                ctx.UserMessages.Add(userMessage);
                ctx.SaveChanges();
            }
        }

        public static void AddApplicationErrorLogReport(ApplicationErrorReport error)
        {
            using (var ctx = new AgrinContext())
            {
                ctx.ApplicationErrorReports.Add(error);
                ctx.SaveChanges();
            }
        }

        public static bool ReplayToUserMessage(int[] messageIDs, string message)
        {
            using (var ctx = new AgrinContext())
            {
                var find = (from x in ctx.UserMessages where messageIDs.Contains(x.ID) select x).ToList();
                if (find.Count == 0)
                    return false;

                var lastid = find.Max(x => x.LastUserMessageID) + 1;
                foreach (var item in find)
                {
                    item.Answered = true;
                }
                ctx.UserMessages.Add(new UserMessage() { GuidId = find.First().GuidId, InsertDateTime = DateTime.Now, IsReplay = true, LastUserMessageID = lastid, Message = message });
                ctx.SaveChanges();
                return true;
            }
        }

        public static bool CheckMarkAnswerMessage(int[] messageIDs)
        {
            using (var ctx = new AgrinContext())
            {
                var find = (from x in ctx.UserMessages where messageIDs.Contains(x.ID) select x).ToList();
                if (find.Count == 0)
                    return false;

                foreach (var item in find)
                {
                    item.Answered = true;
                }
                ctx.SaveChanges();
                return true;
            }
        }

        public static List<UserMessage> GetLastNoAswerMessage(bool returnAll)
        {
            using (var ctx = new AgrinContext())
            {
                var find = (from x in ctx.UserMessages where !x.IsReplay && !x.Answered select x).FirstOrDefault();
                if (find != null)
                {
                    var allReplayfoUser = (from x in ctx.UserMessages where x.GuidId == find.GuidId && ((!x.Answered && !x.IsReplay) || returnAll) select x);
                    return allReplayfoUser.ToList();
                }
                return null;
            }
        }

        public static List<UserMessage> GetNoAswerMessageById(int messageID, bool returnAll)
        {
            using (var ctx = new AgrinContext())
            {
                var find = (from x in ctx.UserMessages where !x.IsReplay && !x.Answered && x.ID == messageID select x).FirstOrDefault();
                if (find != null)
                {
                    var allReplayfoUser = (from x in ctx.UserMessages where x.GuidId == find.GuidId && ((!x.Answered && !x.IsReplay) || returnAll) select x);
                    return allReplayfoUser.ToList();
                }
                return null;
            }
        }

        /// <summary>
        /// اضافه کردن یک پیغام عمومی به تمام کاربران
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static void AddPublicMessage(string message, string title, LimitMessageEnum limit)
        {
            using (var ctx = new AgrinContext())
            {
                ctx.PublicMessages.Add(new PublicMessageInfo() { LimitMessageMode = limit, Message = message, MessageDateTime = DateTime.Now, Title = title });
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// وقتی یوزر میخواد آخرین پیغام های عمومی رو دریافت کنه
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<PublicMessageInfo> GetMessagesByDataTime(DateTime dt, LimitMessageEnum limit)
        {
            using (var ctx = new AgrinContext())
            {
                var find = (from x in ctx.PublicMessages where x.MessageDateTime > dt && (limit == LimitMessageEnum.All || x.LimitMessageMode == LimitMessageEnum.All || x.LimitMessageMode == limit) select x);
                if (find == null)
                    return null;
                find = find.OrderByDescending<PublicMessageInfo, DateTime>(x => x.MessageDateTime).Take(5);///((Math.Max(0, find.Count() - 5));
                return find.ToList();
            }
        }

        /// <summary>
        /// وقتی یوزر میخواد پاسخ هایی که مدیر بهش داده رو دریافت کنه
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="userGuidID"></param>
        /// <returns></returns>
        public static List<UserMessageReplayInfo> GetUserMessageReplaysByDataTime(DateTime dt, int userGuidID)
        {
            using (var ctx = new AgrinContext())
            {
                var find = (from x in ctx.UserMessageReplays where x.MessageDateTime > dt && x.GuidID == userGuidID select x);
                if (find == null)
                    return null;
                return find.ToList();
            }
        }

        /// <summary>
        /// پیغامی که باید پاسخ داده بشه از طرف مدیر
        /// </summary>
        /// <returns></returns>
        public static List<UserMessageReceiveInfo> GetNoReplayMessages(bool getFull)
        {
            using (var ctx = new AgrinContext())
            {
                var find = (from x in ctx.UserReceiveMessages where !x.IsReplay select x);
                if (find == null)
                    return null;
                var first = find.FirstOrDefault();
                if (first == null)
                    return null;
                if (getFull)
                {
                    var items = (from x in ctx.UserReceiveMessages where x.GuidID == first.GuidID select x);
                    return items.ToList();
                }
                return (from x in find where x.GuidID == first.GuidID select x).ToList();
            }
        }

        /// <summary>
        /// ارسال پاسخ به پیغام های کاربر
        /// </summary>
        /// <returns></returns>
        public static bool SendReplayForMessages(int[] ids, string msg, int userID, string url)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from x in ctx.UserReceiveMessages where !x.IsReplay && ids.Contains(x.ID) select x);
                foreach (var item in items)
                {
                    item.IsReplay = true;
                }
                ctx.UserMessageReplays.Add(new UserMessageReplayInfo() { GuidID = userID, Message = msg, MessageDateTime = DateTime.Now, Link = string.IsNullOrEmpty(url) ? null : url });
                ctx.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// چشم پوشی کردن پیغام ها
        /// </summary>
        /// <returns></returns>
        public static bool SkeepReplayMessages(int[] ids)
        {
            using (var ctx = new AgrinContext())
            {
                var items = (from x in ctx.UserReceiveMessages where !x.IsReplay && ids.Contains(x.ID) select x);
                foreach (var item in items)
                {
                    item.IsReplay = true;
                }
                ctx.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// ارسال سوال از طرف کاربر
        /// </summary>
        /// <returns></returns>
        public static void SendUserFeedbackMessage(string msg, int userID)
        {
            using (var ctx = new AgrinContext())
            {
                ctx.UserReceiveMessages.Add(new UserMessageReceiveInfo() { IsReplay = false, GuidID = userID, Message = msg, MessageDateTime = DateTime.Now });
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// دریافت آی دی کاربر بر اساس ای دی پیغام
        /// </summary>
        /// <returns></returns>
        public static int GetGuidIdByMessageID(int messageID)
        {
            using (var ctx = new AgrinContext())
            {
                var find = (from x in ctx.UserReceiveMessages where x.ID == messageID select x);
                return find.FirstOrDefault().GuidID;
            }
        }
    }
}
