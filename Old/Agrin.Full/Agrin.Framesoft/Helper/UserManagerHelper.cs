using Agrin.LinkExtractor;
using System.Collections.Generic;
using YoutubeExtractor;

namespace Agrin.Framesoft.Helper
{
    public class ResponseData<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public static class UserManagerHelper
    {
        public static bool IsLogin { get; set; }
       
        public static string domain = "agrindownloadmanager.ir";
        //public static string domain = "localhost:4786";
        public static ResponseData<string> RegisterUser(UserInfoData user)
        {
            return DataSerializationHelper.SendRequestData<string>("http://" + domain + "/UserManager/RegisterUser", user);
        }

        public static ResponseData<UserInfoData> LoginUser(UserInfoData user)
        {
            return DataSerializationHelper.SendRequestData<UserInfoData>("http://" + domain + "/UserManager/LoginUser", user);
        }

        public static ResponseData<UserInfoData> GetUserInfo(UserInfoData user)
        {
            return DataSerializationHelper.SendRequestData<UserInfoData>("http://" + domain + "/UserManager/GetUserInfo", user);
        }

        public static ResponseData<UserFileInfoData> UploadOneLink(UserFileInfoData file)
        {
            return DataSerializationHelper.SendRequestData<UserFileInfoData>("http://" + domain + "/UserManager/UploadOneLink", file);
        }

        public static ResponseData<UserFileInfoData> GetOneFileStatus(UserFileInfoData file)
        {
            return DataSerializationHelper.SendRequestData<UserFileInfoData>("http://" + domain + "/UserManager/GetOneFileStatus", file);
        }

        public static ResponseData<List<UserFileInfoData>> GetListOfFiles(UserInfoData user)
        {
            return DataSerializationHelper.SendRequestData<List<UserFileInfoData>>("http://" + domain + "/UserManager/GetListOfFiles", user);
        }

        public static ResponseData<string> BuyStorageFromUser(UserPurchaseData user)
        {
            return DataSerializationHelper.SendRequestData<string>("http://" + domain + "/UserManager/BuyStorageFromUser", user);
        }

        public static ResponseData<List<VideoInfo>> GetYoutubeVideoList(UserFileInfoData user)
        {
            return DataSerializationHelper.SendRequestData<List<VideoInfo>>("http://" + domain + "/UserManager/GetYoutubeVideoList", user);
        }

        public static ResponseData<UserFileInfoData> DownloadYoutubeLink(UserFileInfoData user)
        {
            return DataSerializationHelper.SendRequestData<UserFileInfoData>("http://" + domain + "/UserManager/DownloadYoutubeLink", user);
        }

        public static ResponseData<string> SetCompleteUserFiles(List<UserFileInfoData> files)
        {
            return DataSerializationHelper.SendRequestData<string>("http://" + domain + "/UserManager/SetCompleteUserFiles", files);
        }

        public static string GetServerResponseMessageValue(string message, string okMessage)
        {
            string language = "_Language";
            string retValue = "";
            switch (message)
            {
                case "OK":
                    {
                        return okMessage;
                    }
                case "Null":
                    {
                        retValue = "PleaseFillAllItems";
                        break;
                    }
                case "UserName":
                    {
                        retValue = "UserNameExist";
                        break;
                    }
                case "Email":
                    {
                        retValue = "EmailExist";
                        break;
                    }
                case "No":
                    {
                        retValue = "LoginUserPassError";
                        break;
                    }
                case "NoStorage":
                    {
                        retValue = "NoStorage";
                        break;
                    }
                case "StorageBackContactMe":
                    {
                        retValue = "StorageBackContactMe";
                        break;
                    }
                case "NoStorageFileSize":
                    {
                        retValue = "NoStorageFileSize";
                        break;
                    }
                case "NoFileSize":
                    {
                        retValue = "NoFileSize";
                        break;
                    }
                case "CannotDownload":
                    {
                        retValue = "CannotDownload";
                        break;
                    }
                case "FileNotFound":
                    {
                        retValue = "FileNotFound";
                        break;
                    }
                case "Repeat":
                    {
                        return okMessage;
                    }
                case "InvalidUserPurchase":
                    {
                        retValue = "InvalidUserPurchase";
                        break;
                    }
                default:
                    {
                        return message;
                    }
            }

            return retValue + language;
        }
    }
}
