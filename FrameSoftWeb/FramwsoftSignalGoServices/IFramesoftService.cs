using FrameSoft.AmarGiri.DataBase.Contexts;
using FrameSoft.AmarGiri.DataBase.Heper;
using FrameSoft.AmarGiri.DataBase.Models;
using FrameSoft.Agrin.DataBase.Models;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FramwsoftSignalGoServices
{
    [ServiceContract("FramesoftService")]
    public interface IFramesoftService
    {
        MessageContract<Exception> AddErrorLog(Exception error);
        MessageContract<AmarInfo> GetFullAmarInfo();
        MessageContract<AmarInfo> GetWindowsAmarInfo();
        MessageContract<bool> IsBlackListGUID(Guid guid);
        MessageContract<int> GetAndUpdateGuidIDByGuid(Guid guid);
        MessageContract<bool> SendToBlackList(int guidID);
        MessageContract<GuidDetailsTable> GetGuidDetails(int guidID);
        MessageContract<string> GetApplicationVersionByGuidId(GuidDetailsTable detail);
        MessageContract<string> GetApplicationNameByGuidId(GuidDetailsTable detail);
        MessageContract<string> GetOSVersionByGuidId(GuidDetailsTable detail);
        MessageContract<string> GetOSNameByGuidId(GuidDetailsTable detail);
        MessageContract InsertAmar(string ipAddress, string applicationName, string applicationVersion, string OSName, string OSVersion, Guid applicationGuid);

        MessageContract AddUserMesage(UserMessage userMessage);
        MessageContract<bool> CheckMarkAnswerMessage(int[] messageIDs);
        MessageContract<bool> ReplayToUserMessage(int[] messageIDs, string message);
        MessageContract<UserMessage> GetUserReplay(int guidID, int lastUserMessageID);
        MessageContract<List<UserMessage>> GetLastNoAswerMessage(bool returnAll);
        MessageContract<List<UserMessage>> GetNoAswerMessageById(int messageID, bool returnAll);
        MessageContract<List<UserMessageReceiveInfo>> GetNoReplayMessages(bool getFull);
        MessageContract<int> GetGuidIdByMessageID(int messageID);
        MessageContract<bool> SendReplayForMessages(int[] ids, string msg, int userID, string url);
        MessageContract<bool> SkeepReplayMessages(int[] ids);
        MessageContract SendUserFeedbackMessage(string msg, int userID);
        MessageContract<List<PublicMessageInfo>> GetMessagesByDataTime(DateTime dt, Agrin.Framesoft.Messages.LimitMessageEnum limit);
        MessageContract AddPublicMessage(string message, string title, Agrin.Framesoft.Messages.LimitMessageEnum limit);
        MessageContract AddApplicationErrorLogReport(ApplicationErrorReport error);

        MessageContract<bool> ExistUserName(string userName);
        MessageContract RegisterUser(UserInfo user);
        MessageContract<bool> ExistEmail(string email);
        MessageContract<int> LoginUser(string userName, string password);
        MessageContract<UserInfo> GetUserPropertiesInfo(int userID);
        MessageContract<UserFileInfo> GetLeechFileInfoByGUID(int userID, string guid);
        MessageContract SubUserStorageSize(int userID, long size);
        MessageContract UpdateLeechFile(UserFileInfo fileInfo);
        MessageContract<List<UserFileInfo>> GetListLeechFileInfoes(int userID);
        MessageContract<bool> ExistPurchaseByToken(int userID, string token);
        MessageContract BuyStorageData(UserPurchase buy);
        MessageContract SumUserStorageSize(int userID, long size);
        MessageContract<List<UserMessageReplayInfo>> GetUserMessageReplaysByDataTime(DateTime dt, int userGuidID);

        MessageContract<UserFileInfo> GetLeechFileInfo(int userID, int fileID);
        MessageContract<List<UserFileInfo>> GetCanDownloadLeechFileInfoes();
        MessageContract<int> CreateLeechFile(int userID, long size, string path, string link, int formatCode, string fileName);


    }
}
