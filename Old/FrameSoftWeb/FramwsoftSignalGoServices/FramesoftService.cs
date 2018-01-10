using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameSoft.AmarGiri.DataBase.Contexts;
using FrameSoft.AmarGiri.DataBase.Heper;
using FrameSoft.AmarGiri.DataBase.Models;
using System.Threading.Tasks;
using System.Threading;
using Agrin.Framesoft.Messages;
using FrameSoft.Agrin.DataBase.Models;
using FrameSoft.Agrin.DataBase.Helper;
using SignalGo.Shared.Log;
using System.IO;

namespace FramwsoftSignalGoServices
{
    public static class AmarGiriUpdater
    {
        public static List<AmarLocalInfo> MemoryInsertedAmar = new List<AmarLocalInfo>();
        public static void InsertAmar(string ipAddress, string applicationName, string applicationVersion, string OSName
            , string OSVersion, Guid applicationGuid)
        {
            Task task = new Task(() =>
            {
                Thread.Sleep(100);
                lock (lockOBJ)
                {
                    foreach (var item in MemoryInsertedAmar.ToArray())
                    {
                        if (item.ApplicationGuid == applicationGuid)
                        {
                            MemoryInsertedAmar.Remove(item);
                            break;
                        }
                    }

                    MemoryInsertedAmar.Add(new AmarLocalInfo() { ApplicationGuid = applicationGuid, ApplicationName = applicationName, ApplicationVersion = applicationVersion, IPAddress = ipAddress, OSName = OSName, OSVersion = OSVersion });
                }
            });
            task.Start();

            //var ipID = AmarContext.GetAndUpdateIPIDByIP(ipAddress);
            //var guidID = AmarContext.AddUpdateApplicationDetails(ipID, applicationName, applicationVersion,
            //     OSName, OSVersion, applicationGuid);
            //AmarContext.AddAmar(guidID, ipID);
        }

        public static string LastError = "";
        public static string CountAdded = "";
        public static int AddCount = 0;
        public static int ErrorCount = 0;
        static object lockOBJ = new object();
        static bool IsStarted = false;
        static AmarGiriUpdater()
        {
            if (IsStarted)
                return;
            IsStarted = true;
            LastError = "";
            var ctx = new AmarContext();
            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        AmarLocalInfo item = null;
                        lock (lockOBJ)
                            item = MemoryInsertedAmar.FirstOrDefault();
                        if (item != null)
                        {
                            var guidID = AmarContext.AddUpdateApplicationDetails(ctx, item.ApplicationName, item.ApplicationVersion, item.OSName, item.OSVersion, item.ApplicationGuid);
                            AmarContext.AddAmar(guidID, ctx);
                            ctx.SaveChanges();
                            //var ipID = AmarContext.GetAndUpdateIPIDByIP(item.IPAddress);

                            lock (lockOBJ)
                                MemoryInsertedAmar.Remove(item);
                            System.Threading.Thread.Sleep(50);
                            AddCount++;
                        }
                        else
                            System.Threading.Thread.Sleep(1000);
                        //lock (lockOBJ)
                        //{
                        //    LastError = "Play For Add " + items.Count;
                        //    AmarContext.GetAndUpdateIPIDByIP(items);
                        //    AmarContext.AddUpdateApplicationDetails(items);
                        //    AmarContext.AddAmar(items);
                        //    LastError = "OK";
                        //}
                    }
                    catch (Exception e)
                    {
                        ErrorCount++;
                        LastError = e.Message + " newLine " + e.StackTrace;
                        System.Threading.Thread.Sleep(1000);
                    }
                    CountAdded = "Not Add Count: " + MemoryInsertedAmar.Count + "Add Count: " + AddCount + " Errors: " + ErrorCount;
                }
            });
            task.Start();
        }

        //public static AmarInfo GetFullAmarInfo()
        //{
        //    var amar = AmarDataBaseHelper.GetFullAmarInfo();
        //    var on = AmarLocalHelper.GetFullAmarInfo(MemoryInsertedAmar.ToList());
        //    amar.OnlineApplicationCount = on.OnlineApplicationCount;
        //    amar.OnlineCount = on.OnlineCount;
        //    return amar;
        //}
    }
    public class FramesoftService : IFramesoftService
    {
        public MessageContract AddApplicationErrorLogReport(ApplicationErrorReport error)
        {
            AgrinDataBaseHelper.AddApplicationErrorLogReport(error);
            return MessageContract.Success();
        }

        public MessageContract<Exception> AddErrorLog(Exception error)
        {
            return AmarContext.AddErrorLog(error).Success();
        }

        public MessageContract AddPublicMessage(string message, string title, LimitMessageEnum limit)
        {
            AgrinDataBaseHelper.AddPublicMessage(message, title, limit);
            return MessageContract.Success();
        }

        public MessageContract AddUserMesage(UserMessage userMessage)
        {
            AgrinDataBaseHelper.AddUserMesage(userMessage);
            return MessageContract.Success();
        }

        public MessageContract BuyStorageData(UserPurchase buy)
        {
            AgrinDataBaseHelper.BuyStorageData(buy);
            return MessageContract.Success();
        }

        public MessageContract<bool> CheckMarkAnswerMessage(int[] messageIDs)
        {
            return AgrinDataBaseHelper.CheckMarkAnswerMessage(messageIDs).Success();
        }

        public MessageContract<int> CreateLeechFile(int userID, long size, string path, string link, int formatCode, string fileName)
        {
            return AgrinDataBaseHelper.CreateLeechFile(userID, size, path, link, formatCode, fileName).Success();
        }

        public MessageContract<bool> ExistEmail(string email)
        {
            return AgrinDataBaseHelper.ExistEmail(email).Success();
        }

        public MessageContract<bool> ExistPurchaseByToken(int userID, string token)
        {
            return AgrinDataBaseHelper.ExistPurchaseByToken(userID, token).Success();
        }

        public MessageContract<bool> ExistUserName(string userName)
        {
            return AgrinDataBaseHelper.ExistUserName(userName).Success();
        }

        public MessageContract<int> GetAndUpdateGuidIDByGuid(Guid guid)
        {
            return AmarContext.GetAndUpdateGuidIDByGuid(guid).Success();
        }

        public MessageContract<string> GetApplicationNameByGuidId(GuidDetailsTable detail)
        {
            return AmarContext.GetApplicationNameByGuidId(detail).Success();
        }

        public MessageContract<string> GetApplicationVersionByGuidId(GuidDetailsTable detail)
        {
            return AmarContext.GetApplicationVersionByGuidId(detail).Success();
        }

        public MessageContract<List<UserFileInfo>> GetCanDownloadLeechFileInfoes()
        {
            return AgrinDataBaseHelper.GetCanDownloadLeechFileInfoes().Success();
        }

        public MessageContract<AmarInfo> GetFullAmarInfo()
        {
            return AmarDataBaseHelper.GetFullAmarInfo().Success();
        }

        public MessageContract<GuidDetailsTable> GetGuidDetails(int guidID)
        {
            return AmarContext.GetGuidDetails(guidID).Success();
        }

        public MessageContract<int> GetGuidIdByMessageID(int messageID)
        {
            return AgrinDataBaseHelper.GetGuidIdByMessageID(messageID).Success();
        }

        public MessageContract<List<UserMessage>> GetLastNoAswerMessage(bool returnAll)
        {
            return AgrinDataBaseHelper.GetLastNoAswerMessage(returnAll).Success();
        }

        public MessageContract<UserFileInfo> GetLeechFileInfo(int userID, int fileID)
        {
            return AgrinDataBaseHelper.GetLeechFileInfo(userID, fileID).Success();
        }

        public MessageContract<UserFileInfo> GetLeechFileInfoByGUID(int userID, string guid)
        {
            return AgrinDataBaseHelper.GetLeechFileInfoByGUID(userID, guid).Success();
        }

        public MessageContract<List<UserFileInfo>> GetListLeechFileInfoes(int userID)
        {
            return AgrinDataBaseHelper.GetListLeechFileInfoes(userID).Success();
        }

        public MessageContract<List<PublicMessageInfo>> GetMessagesByDataTime(DateTime dt, LimitMessageEnum limit)
        {
            return AgrinDataBaseHelper.GetMessagesByDataTime(dt, limit).Success();
        }

        public MessageContract<List<UserMessage>> GetNoAswerMessageById(int messageID, bool returnAll)
        {
            return AgrinDataBaseHelper.GetNoAswerMessageById(messageID, returnAll).Success();
        }

        public MessageContract<List<UserMessageReceiveInfo>> GetNoReplayMessages(bool getFull)
        {
            return AgrinDataBaseHelper.GetNoReplayMessages(getFull).Success();
        }

        public MessageContract<string> GetOSNameByGuidId(GuidDetailsTable detail)
        {
            return AmarContext.GetOSNameByGuidId(detail).Success();
        }

        public MessageContract<string> GetOSVersionByGuidId(GuidDetailsTable detail)
        {
            return AmarContext.GetOSVersionByGuidId(detail).Success();
        }

        public MessageContract<List<UserMessageReplayInfo>> GetUserMessageReplaysByDataTime(DateTime dt, int userGuidID)
        {
            return AgrinDataBaseHelper.GetUserMessageReplaysByDataTime(dt, userGuidID).Success();
        }

        public MessageContract<UserInfo> GetUserPropertiesInfo(int userID)
        {
            return AgrinDataBaseHelper.GetUserPropertiesInfo(userID).Success();
        }

        public MessageContract<UserMessage> GetUserReplay(int guidID, int lastUserMessageID)
        {
            return AgrinDataBaseHelper.GetUserReplay(guidID, lastUserMessageID).Success();
        }

        public MessageContract<AmarInfo> GetWindowsAmarInfo()
        {
            return AmarDataBaseHelper.GetWindowsAmarInfo().Success();
        }

        public MessageContract InsertAmar(string ipAddress, string applicationName, string applicationVersion, string OSName, string OSVersion, Guid applicationGuid)
        {
            AmarGiriUpdater.InsertAmar(ipAddress, applicationName, applicationVersion, OSName, OSVersion, applicationGuid);
            return MessageContract.Success();
        }

        public MessageContract<bool> IsBlackListGUID(Guid guid)
        {
            return AmarContext.IsBlackListGUID(guid).Success();
        }

        static object locker = new object();
        public MessageContract LogError(string log)
        {
            try
            {
                lock (locker)
                {
                    var fileName = System.IO.Path.Combine(AutoLogger.ApplicationDirectory, "UserErrorLogs");
                    fileName = System.IO.Path.Combine(fileName, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
                    if (!Directory.Exists(fileName))
                        Directory.CreateDirectory(fileName);
                    fileName = System.IO.Path.Combine(fileName, $"ErrorLogs-{DateTime.Now.Hour}.txt");
                    File.AppendAllText(fileName, log + Environment.NewLine,Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "LogError");
            }
            return MessageContract.Success();
        }

        public MessageContract<int> LoginUser(string userName, string password)
        {
            return AgrinDataBaseHelper.LoginUser(userName, password).Success();
        }

        public MessageContract RegisterUser(UserInfo user)
        {
            AgrinDataBaseHelper.RegisterUser(user);
            return MessageContract.Success();
        }

        public MessageContract<bool> ReplayToUserMessage(int[] messageIDs, string message)
        {
            return AgrinDataBaseHelper.ReplayToUserMessage(messageIDs, message).Success();
        }

        public MessageContract<bool> SendReplayForMessages(int[] ids, string msg, int userID, string url)
        {
            return AgrinDataBaseHelper.SendReplayForMessages(ids, msg, userID, url).Success();
        }

        public MessageContract<bool> SendToBlackList(int guidID)
        {
            return AmarContext.SendToBlackList(guidID).Success();
        }

        public MessageContract SendUserFeedbackMessage(string msg, int userID)
        {
            AgrinDataBaseHelper.SendUserFeedbackMessage(msg, userID);
            return MessageContract.Success();
        }

        public MessageContract<bool> SkeepReplayMessages(int[] ids)
        {
            return AgrinDataBaseHelper.SkeepReplayMessages(ids).Success();
        }

        public MessageContract SubUserStorageSize(int userID, long size)
        {
            AgrinDataBaseHelper.SubUserStorageSize(userID, size);
            return MessageContract.Success();
        }

        public MessageContract SumUserStorageSize(int userID, long size)
        {
            AgrinDataBaseHelper.SumUserStorageSize(userID, size);
            return MessageContract.Success();
        }

        public MessageContract UpdateLeechFile(UserFileInfo fileInfo)
        {
            AgrinDataBaseHelper.UpdateLeechFile(fileInfo);
            return MessageContract.Success();
        }
    }
}
