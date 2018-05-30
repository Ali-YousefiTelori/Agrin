using SignalGo.Shared.DataTypes;
using System.Threading.Tasks;
using SignalGo.Shared.Models;
using System;
using System.Collections.Generic;
using SignalGo.Shared.Http;
using Agrin.Server.Models;
using System.Linq;
using System.Text;
using AgrinMainServer.ServerServices;
using AgrinMainServer.HttpServices;
using AgrinMainServer.ClientServices;

namespace AgrinMainServer.ServerServices
{
    [ServiceContract("FileManager",ServiceType.ServerService, InstanceType.SingleInstance)]
    public interface IFileManager
    {
        Agrin.Server.Models.MessageContract<long> CreateEmptyFile();
        Task<Agrin.Server.Models.MessageContract<long>> CreateEmptyFileAsync();
        Agrin.Server.Models.MessageContract<long> CreateEmptyFile(int userId);
        Task<Agrin.Server.Models.MessageContract<long>> CreateEmptyFileAsync(int userId);
        Agrin.Server.Models.MessageContract RoamStorageFileComplete(int userId, long fileId, long fileSize);
        Task<Agrin.Server.Models.MessageContract> RoamStorageFileCompleteAsync(int userId, long fileId, long fileSize);
    }
    [ServiceContract("PostService",ServiceType.ServerService, InstanceType.SingleInstance)]
    public interface IPostService
    {
        Agrin.Server.Models.MessageContract<System.Collections.Generic.List<Agrin.Server.DataBase.Models.PostInfo>> GetListOfPost(int index, int length);
        Task<Agrin.Server.Models.MessageContract<System.Collections.Generic.List<Agrin.Server.DataBase.Models.PostInfo>>> GetListOfPostAsync(int index, int length);
        Agrin.Server.Models.MessageContract<System.Collections.Generic.List<Agrin.Server.DataBase.Models.PostCategoryInfo>> FilterPostCategories(Agrin.Server.Models.Filters.FilterBaseInfo filterBaseInfo);
        Task<Agrin.Server.Models.MessageContract<System.Collections.Generic.List<Agrin.Server.DataBase.Models.PostCategoryInfo>>> FilterPostCategoriesAsync(Agrin.Server.Models.Filters.FilterBaseInfo filterBaseInfo);
        Agrin.Server.Models.MessageContract<System.Collections.Generic.List<Agrin.Server.DataBase.Models.PostCategoryInfo>> FilterVirtualPostCategories(Agrin.Server.Models.Filters.FilterBaseInfo filterBaseInfo);
        Task<Agrin.Server.Models.MessageContract<System.Collections.Generic.List<Agrin.Server.DataBase.Models.PostCategoryInfo>>> FilterVirtualPostCategoriesAsync(Agrin.Server.Models.Filters.FilterBaseInfo filterBaseInfo);
        Agrin.Server.Models.MessageContract<System.Collections.Generic.List<Agrin.Server.DataBase.Models.PostInfo>> FilterPosts(Agrin.Server.Models.Filters.FilterPostInfo filterPostInfo);
        Task<Agrin.Server.Models.MessageContract<System.Collections.Generic.List<Agrin.Server.DataBase.Models.PostInfo>>> FilterPostsAsync(Agrin.Server.Models.Filters.FilterPostInfo filterPostInfo);
    }
    [ServiceContract("AuthenticationService",ServiceType.ServerService, InstanceType.SingleInstance)]
    public interface IAuthenticationService
    {
        Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo> Login(System.Guid firstKey, System.Guid secondKey);
        Task<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>> LoginAsync(System.Guid firstKey, System.Guid secondKey);
    }
}

namespace AgrinMainServer.StreamServices
{
    [ServiceContract("PostStorageManager",ServiceType.StreamService, InstanceType.SingleInstance)]
    public interface IPostStorageManager
    {
        SignalGo.Shared.Models.StreamInfo<System.DateTime> DownloadPostImage(int postUserId, int postId, string fileName);
        Task<SignalGo.Shared.Models.StreamInfo<System.DateTime>> DownloadPostImageAsync(int postUserId, int postId, string fileName);
    }
}

namespace AgrinMainServer.OneWayServices
{
    [ServiceContract("StorageAuthentication",ServiceType.OneWayService, InstanceType.SingleInstance)]
    public class StorageAuthenticationService
    {
        public static StorageAuthenticationService Current { get; set; }
        string _signalGoServerAddress = "";
        int _signalGoPortNumber = 0;
        public StorageAuthenticationService(string signalGoServerAddress, int signalGoPortNumber)
        {
            _signalGoServerAddress = signalGoServerAddress;
            _signalGoPortNumber = signalGoPortNumber;
        }
         public Agrin.Server.Models.MessageContract CheckAccessUserToFileUpload(System.Guid firstKey, System.Guid secondKey, long directFileId)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "CheckAccessUserToFileUpload", firstKey, secondKey, directFileId);
        }
         public Task<Agrin.Server.Models.MessageContract> CheckAccessUserToFileUploadAsync(System.Guid firstKey, System.Guid secondKey, long directFileId)
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "CheckAccessUserToFileUpload", firstKey, secondKey, directFileId));
        }
         public Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo> GetUserByTelegramUserId(int telegramUserId)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "GetUserByTelegramUserId", telegramUserId);
        }
         public Task<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>> GetUserByTelegramUserIdAsync(int telegramUserId)
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "GetUserByTelegramUserId", telegramUserId));
        }
         public Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo> GetUserByUserName(string userName)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "GetUserByUserName", userName);
        }
         public Task<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>> GetUserByUserNameAsync(string userName)
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "GetUserByUserName", userName));
        }
         public Agrin.Server.Models.MessageContract ChangeUserTelegramId(int userId, int telegramUserId)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "ChangeUserTelegramId", userId, telegramUserId);
        }
         public Task<Agrin.Server.Models.MessageContract> ChangeUserTelegramIdAsync(int userId, int telegramUserId)
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "ChangeUserTelegramId", userId, telegramUserId));
        }
         public Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo> AddUser(Agrin.Server.DataBase.Models.UserInfo userInfo)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "AddUser", userInfo);
        }
         public Task<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>> AddUserAsync(Agrin.Server.DataBase.Models.UserInfo userInfo)
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<Agrin.Server.DataBase.Models.UserInfo>>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "AddUser", userInfo));
        }
         public Agrin.Server.Models.MessageContract GiftCradit(System.Guid key, System.Guid value, int amount, int toUserId)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "GiftCradit", key, value, amount, toUserId);
        }
         public Task<Agrin.Server.Models.MessageContract> GiftCraditAsync(System.Guid key, System.Guid value, int amount, int toUserId)
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "StorageAuthentication", "GiftCradit", key, value, amount, toUserId));
        }
    }
    [ServiceContract("FileManager",ServiceType.OneWayService, InstanceType.SingleInstance)]
    public class FileManager
    {
        public static FileManager Current { get; set; }
        string _signalGoServerAddress = "";
        int _signalGoPortNumber = 0;
        public FileManager(string signalGoServerAddress, int signalGoPortNumber)
        {
            _signalGoServerAddress = signalGoServerAddress;
            _signalGoPortNumber = signalGoPortNumber;
        }
         public Agrin.Server.Models.MessageContract<long> CreateEmptyFile()
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<long>>(_signalGoServerAddress, _signalGoPortNumber, "FileManager", "CreateEmptyFile");
        }
         public Task<Agrin.Server.Models.MessageContract<long>> CreateEmptyFileAsync()
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract<long>>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<long>>(_signalGoServerAddress, _signalGoPortNumber, "FileManager", "CreateEmptyFile"));
        }
         public Agrin.Server.Models.MessageContract<long> CreateEmptyFile(int userId)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<long>>(_signalGoServerAddress, _signalGoPortNumber, "FileManager", "CreateEmptyFile", userId);
        }
         public Task<Agrin.Server.Models.MessageContract<long>> CreateEmptyFileAsync(int userId)
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract<long>>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract<long>>(_signalGoServerAddress, _signalGoPortNumber, "FileManager", "CreateEmptyFile", userId));
        }
         public Agrin.Server.Models.MessageContract RoamStorageFileComplete(int userId, long fileId, long fileSize)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "FileManager", "RoamStorageFileComplete", userId, fileId, fileSize);
        }
         public Task<Agrin.Server.Models.MessageContract> RoamStorageFileCompleteAsync(int userId, long fileId, long fileSize)
        {
                return System.Threading.Tasks.Task<Agrin.Server.Models.MessageContract>.Factory.StartNew(() => SignalGo.Client.ClientProvider.SendOneWayMethod<Agrin.Server.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "FileManager", "RoamStorageFileComplete", userId, fileId, fileSize));
        }
    }
}

namespace AgrinMainServer.HttpServices
{
    public class IndexController
    {
        string OpenPage(string address, string name, System.Collections.Generic.List<string> values)
        {
                throw new NotSupportedException();
        }
        Task<string> OpenPageAsync(string address, string name, System.Collections.Generic.List<string> values)
        {
                return System.Threading.Tasks.Task<string>.Factory.StartNew(() => throw new NotSupportedException());
        }
    }
    public class DownloadController
    {
        SignalGo.Shared.Http.ActionResult AgrinDownloadManagerProLastVersion()
        {
                throw new NotSupportedException();
        }
        Task<SignalGo.Shared.Http.ActionResult> AgrinDownloadManagerProLastVersionAsync()
        {
                return System.Threading.Tasks.Task<SignalGo.Shared.Http.ActionResult>.Factory.StartNew(() => throw new NotSupportedException());
        }
        SignalGo.Shared.Http.ActionResult AgrinDownloadManagerWindowsLastVersion()
        {
                throw new NotSupportedException();
        }
        Task<SignalGo.Shared.Http.ActionResult> AgrinDownloadManagerWindowsLastVersionAsync()
        {
                return System.Threading.Tasks.Task<SignalGo.Shared.Http.ActionResult>.Factory.StartNew(() => throw new NotSupportedException());
        }
        SignalGo.Shared.Http.ActionResult DownloadFile(string file)
        {
                throw new NotSupportedException();
        }
        Task<SignalGo.Shared.Http.ActionResult> DownloadFileAsync(string file)
        {
                return System.Threading.Tasks.Task<SignalGo.Shared.Http.ActionResult>.Factory.StartNew(() => throw new NotSupportedException());
        }
    }
}

namespace Agrin.Server.Models
{
    public class MessageContract<T> : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private T _Data;
        public T Data
        {
                get
                {
                        return _Data;
                }
                set
                {
                        _Data = value;
                        OnPropertyChanged(nameof(Data));
                }
        }

        private string _Message;
        public string Message
        {
                get
                {
                        return _Message;
                }
                set
                {
                        _Message = value;
                        OnPropertyChanged(nameof(Message));
                }
        }

        private Agrin.Server.Models.MessageType _Error;
        public Agrin.Server.Models.MessageType Error
        {
                get
                {
                        return _Error;
                }
                set
                {
                        _Error = value;
                        OnPropertyChanged(nameof(Error));
                }
        }

        private bool _IsSuccess;
        public bool IsSuccess
        {
                get
                {
                        return _IsSuccess;
                }
                set
                {
                        _IsSuccess = value;
                        OnPropertyChanged(nameof(IsSuccess));
                }
        }


    }

    public class MessageContract : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private object _Data;
        public object Data
        {
                get
                {
                        return _Data;
                }
                set
                {
                        _Data = value;
                        OnPropertyChanged(nameof(Data));
                }
        }

        private string _Message;
        public string Message
        {
                get
                {
                        return _Message;
                }
                set
                {
                        _Message = value;
                        OnPropertyChanged(nameof(Message));
                }
        }

        private Agrin.Server.Models.MessageType _Error;
        public Agrin.Server.Models.MessageType Error
        {
                get
                {
                        return _Error;
                }
                set
                {
                        _Error = value;
                        OnPropertyChanged(nameof(Error));
                }
        }

        private bool _IsSuccess;
        public bool IsSuccess
        {
                get
                {
                        return _IsSuccess;
                }
                set
                {
                        _IsSuccess = value;
                        OnPropertyChanged(nameof(IsSuccess));
                }
        }

        public static implicit operator MessageContract(MessageType errorMessage)
        {
            if (errorMessage == MessageType.Success)
                return new MessageContract() { IsSuccess = true };
            return new MessageContract() { Error = errorMessage, IsSuccess = false };
        }

        public static implicit operator MessageContract(bool value)
        {
            return new MessageContract() { IsSuccess = value };
        }


    }

}

namespace Agrin.Server.DataBase.Models
{
    public class PostInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private int _CategoryId;
        public int CategoryId
        {
                get
                {
                        return _CategoryId;
                }
                set
                {
                        _CategoryId = value;
                        OnPropertyChanged(nameof(CategoryId));
                }
        }

        private int _UserId;
        public int UserId
        {
                get
                {
                        return _UserId;
                }
                set
                {
                        _UserId = value;
                        OnPropertyChanged(nameof(UserId));
                }
        }

        private Agrin.Server.DataBase.Models.PostType _PostType;
        public Agrin.Server.DataBase.Models.PostType PostType
        {
                get
                {
                        return _PostType;
                }
                set
                {
                        _PostType = value;
                        OnPropertyChanged(nameof(PostType));
                }
        }

        private int? _PostVideoId;
        public int? PostVideoId
        {
                get
                {
                        return _PostVideoId;
                }
                set
                {
                        _PostVideoId = value;
                        OnPropertyChanged(nameof(PostVideoId));
                }
        }

        private int? _PostMusicId;
        public int? PostMusicId
        {
                get
                {
                        return _PostMusicId;
                }
                set
                {
                        _PostMusicId = value;
                        OnPropertyChanged(nameof(PostMusicId));
                }
        }

        private long _ViewCount;
        public long ViewCount
        {
                get
                {
                        return _ViewCount;
                }
                set
                {
                        _ViewCount = value;
                        OnPropertyChanged(nameof(ViewCount));
                }
        }

        private System.DateTime _CreatedDateTime;
        public System.DateTime CreatedDateTime
        {
                get
                {
                        return _CreatedDateTime;
                }
                set
                {
                        _CreatedDateTime = value;
                        OnPropertyChanged(nameof(CreatedDateTime));
                }
        }

        private System.DateTime _LastUpdateDateTime;
        public System.DateTime LastUpdateDateTime
        {
                get
                {
                        return _LastUpdateDateTime;
                }
                set
                {
                        _LastUpdateDateTime = value;
                        OnPropertyChanged(nameof(LastUpdateDateTime));
                }
        }

        private System.DateTime _LastUpdateFileVersionDateTime;
        public System.DateTime LastUpdateFileVersionDateTime
        {
                get
                {
                        return _LastUpdateFileVersionDateTime;
                }
                set
                {
                        _LastUpdateFileVersionDateTime = value;
                        OnPropertyChanged(nameof(LastUpdateFileVersionDateTime));
                }
        }

        private Agrin.Server.DataBase.Models.PostVideoInfo _PostVideoInfo;
        public Agrin.Server.DataBase.Models.PostVideoInfo PostVideoInfo
        {
                get
                {
                        return _PostVideoInfo;
                }
                set
                {
                        _PostVideoInfo = value;
                        OnPropertyChanged(nameof(PostVideoInfo));
                }
        }

        private Agrin.Server.DataBase.Models.PostSoundInfo _PostMusicInfo;
        public Agrin.Server.DataBase.Models.PostSoundInfo PostMusicInfo
        {
                get
                {
                        return _PostMusicInfo;
                }
                set
                {
                        _PostMusicInfo = value;
                        OnPropertyChanged(nameof(PostMusicInfo));
                }
        }

        private Agrin.Server.DataBase.Models.PostCategoryInfo _PostCategoryInfo;
        public Agrin.Server.DataBase.Models.PostCategoryInfo PostCategoryInfo
        {
                get
                {
                        return _PostCategoryInfo;
                }
                set
                {
                        _PostCategoryInfo = value;
                        OnPropertyChanged(nameof(PostCategoryInfo));
                }
        }

        private Agrin.Server.DataBase.Models.UserInfo _UserInfo;
        public Agrin.Server.DataBase.Models.UserInfo UserInfo
        {
                get
                {
                        return _UserInfo;
                }
                set
                {
                        _UserInfo = value;
                        OnPropertyChanged(nameof(UserInfo));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.PostFileInfo> _FileInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.PostFileInfo> FileInfoes
        {
                get
                {
                        return _FileInfoes;
                }
                set
                {
                        _FileInfoes = value;
                        OnPropertyChanged(nameof(FileInfoes));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.PostTagRelationInfo> _PostTagRelationInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.PostTagRelationInfo> PostTagRelationInfoes
        {
                get
                {
                        return _PostTagRelationInfoes;
                }
                set
                {
                        _PostTagRelationInfoes = value;
                        OnPropertyChanged(nameof(PostTagRelationInfoes));
                }
        }


    }

    public class PostCategoryInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private string _Title;
        public string Title
        {
                get
                {
                        return _Title;
                }
                set
                {
                        _Title = value;
                        OnPropertyChanged(nameof(Title));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.PostCategoryTagRelationInfo> _PostCategoryTagRelationInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.PostCategoryTagRelationInfo> PostCategoryTagRelationInfoes
        {
                get
                {
                        return _PostCategoryTagRelationInfoes;
                }
                set
                {
                        _PostCategoryTagRelationInfoes = value;
                        OnPropertyChanged(nameof(PostCategoryTagRelationInfoes));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.PostInfo> _Posts;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.PostInfo> Posts
        {
                get
                {
                        return _Posts;
                }
                set
                {
                        _Posts = value;
                        OnPropertyChanged(nameof(Posts));
                }
        }


    }

    public class UserInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private string _UserName;
        public string UserName
        {
                get
                {
                        return _UserName;
                }
                set
                {
                        _UserName = value;
                        OnPropertyChanged(nameof(UserName));
                }
        }

        private string _Name;
        public string Name
        {
                get
                {
                        return _Name;
                }
                set
                {
                        _Name = value;
                        OnPropertyChanged(nameof(Name));
                }
        }

        private string _Family;
        public string Family
        {
                get
                {
                        return _Family;
                }
                set
                {
                        _Family = value;
                        OnPropertyChanged(nameof(Family));
                }
        }

        private string _DisplayName;
        public string DisplayName
        {
                get
                {
                        return _DisplayName;
                }
                set
                {
                        _DisplayName = value;
                        OnPropertyChanged(nameof(DisplayName));
                }
        }

        private Agrin.Server.DataBase.Models.UserStatus _Status;
        public Agrin.Server.DataBase.Models.UserStatus Status
        {
                get
                {
                        return _Status;
                }
                set
                {
                        _Status = value;
                        OnPropertyChanged(nameof(Status));
                }
        }

        private long _StaticUploadSize;
        public long StaticUploadSize
        {
                get
                {
                        return _StaticUploadSize;
                }
                set
                {
                        _StaticUploadSize = value;
                        OnPropertyChanged(nameof(StaticUploadSize));
                }
        }

        private long _RoamUploadSize;
        public long RoamUploadSize
        {
                get
                {
                        return _RoamUploadSize;
                }
                set
                {
                        _RoamUploadSize = value;
                        OnPropertyChanged(nameof(RoamUploadSize));
                }
        }

        private long _Credit;
        public long Credit
        {
                get
                {
                        return _Credit;
                }
                set
                {
                        _Credit = value;
                        OnPropertyChanged(nameof(Credit));
                }
        }

        private int? _TelegramUserId;
        public int? TelegramUserId
        {
                get
                {
                        return _TelegramUserId;
                }
                set
                {
                        _TelegramUserId = value;
                        OnPropertyChanged(nameof(TelegramUserId));
                }
        }

        private System.DateTime _CreatedDateTime;
        public System.DateTime CreatedDateTime
        {
                get
                {
                        return _CreatedDateTime;
                }
                set
                {
                        _CreatedDateTime = value;
                        OnPropertyChanged(nameof(CreatedDateTime));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.PostInfo> _PostInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.PostInfo> PostInfoes
        {
                get
                {
                        return _PostInfoes;
                }
                set
                {
                        _PostInfoes = value;
                        OnPropertyChanged(nameof(PostInfoes));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.UserCreditInfo> _FromUserCreditInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.UserCreditInfo> FromUserCreditInfoes
        {
                get
                {
                        return _FromUserCreditInfoes;
                }
                set
                {
                        _FromUserCreditInfoes = value;
                        OnPropertyChanged(nameof(FromUserCreditInfoes));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.UserCreditInfo> _ToUserCreditInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.UserCreditInfo> ToUserCreditInfoes
        {
                get
                {
                        return _ToUserCreditInfoes;
                }
                set
                {
                        _ToUserCreditInfoes = value;
                        OnPropertyChanged(nameof(ToUserCreditInfoes));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.DirectFileToUserRelationInfo> _DirectFileToUserRelationInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.DirectFileToUserRelationInfo> DirectFileToUserRelationInfoes
        {
                get
                {
                        return _DirectFileToUserRelationInfoes;
                }
                set
                {
                        _DirectFileToUserRelationInfoes = value;
                        OnPropertyChanged(nameof(DirectFileToUserRelationInfoes));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.DirectFolderToUserRelationInfo> _DirectFolderToUserRelationInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.DirectFolderToUserRelationInfo> DirectFolderToUserRelationInfoes
        {
                get
                {
                        return _DirectFolderToUserRelationInfoes;
                }
                set
                {
                        _DirectFolderToUserRelationInfoes = value;
                        OnPropertyChanged(nameof(DirectFolderToUserRelationInfoes));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.UserSessionInfo> _UserSessionInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.UserSessionInfo> UserSessionInfoes
        {
                get
                {
                        return _UserSessionInfoes;
                }
                set
                {
                        _UserSessionInfoes = value;
                        OnPropertyChanged(nameof(UserSessionInfoes));
                }
        }


    }

    public class PostVideoInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private string _Title;
        public string Title
        {
                get
                {
                        return _Title;
                }
                set
                {
                        _Title = value;
                        OnPropertyChanged(nameof(Title));
                }
        }


    }

    public class PostSoundInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private string _Title;
        public string Title
        {
                get
                {
                        return _Title;
                }
                set
                {
                        _Title = value;
                        OnPropertyChanged(nameof(Title));
                }
        }


    }

    public class PostFileInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private string _ServerAddress;
        public string ServerAddress
        {
                get
                {
                        return _ServerAddress;
                }
                set
                {
                        _ServerAddress = value;
                        OnPropertyChanged(nameof(ServerAddress));
                }
        }

        private string _Version;
        public string Version
        {
                get
                {
                        return _Version;
                }
                set
                {
                        _Version = value;
                        OnPropertyChanged(nameof(Version));
                }
        }

        private int? _VersionNumber;
        public int? VersionNumber
        {
                get
                {
                        return _VersionNumber;
                }
                set
                {
                        _VersionNumber = value;
                        OnPropertyChanged(nameof(VersionNumber));
                }
        }

        private Agrin.Server.DataBase.Models.FileType _Type;
        public Agrin.Server.DataBase.Models.FileType Type
        {
                get
                {
                        return _Type;
                }
                set
                {
                        _Type = value;
                        OnPropertyChanged(nameof(Type));
                }
        }

        private Agrin.Server.DataBase.Models.OperationSystemType _OperationSystemSupports;
        public Agrin.Server.DataBase.Models.OperationSystemType OperationSystemSupports
        {
                get
                {
                        return _OperationSystemSupports;
                }
                set
                {
                        _OperationSystemSupports = value;
                        OnPropertyChanged(nameof(OperationSystemSupports));
                }
        }

        private int _PostId;
        public int PostId
        {
                get
                {
                        return _PostId;
                }
                set
                {
                        _PostId = value;
                        OnPropertyChanged(nameof(PostId));
                }
        }

        private Agrin.Server.DataBase.Models.PostInfo _PostInfo;
        public Agrin.Server.DataBase.Models.PostInfo PostInfo
        {
                get
                {
                        return _PostInfo;
                }
                set
                {
                        _PostInfo = value;
                        OnPropertyChanged(nameof(PostInfo));
                }
        }


    }

    public class UserCreditInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private int _ToUserId;
        public int ToUserId
        {
                get
                {
                        return _ToUserId;
                }
                set
                {
                        _ToUserId = value;
                        OnPropertyChanged(nameof(ToUserId));
                }
        }

        private Agrin.Server.DataBase.Models.CreditType _Type;
        public Agrin.Server.DataBase.Models.CreditType Type
        {
                get
                {
                        return _Type;
                }
                set
                {
                        _Type = value;
                        OnPropertyChanged(nameof(Type));
                }
        }

        private int _Amount;
        public int Amount
        {
                get
                {
                        return _Amount;
                }
                set
                {
                        _Amount = value;
                        OnPropertyChanged(nameof(Amount));
                }
        }

        private int? _FromUserId;
        public int? FromUserId
        {
                get
                {
                        return _FromUserId;
                }
                set
                {
                        _FromUserId = value;
                        OnPropertyChanged(nameof(FromUserId));
                }
        }

        private System.Guid _Key;
        public System.Guid Key
        {
                get
                {
                        return _Key;
                }
                set
                {
                        _Key = value;
                        OnPropertyChanged(nameof(Key));
                }
        }

        private Agrin.Server.DataBase.Models.UserInfo _ToUserInfo;
        public Agrin.Server.DataBase.Models.UserInfo ToUserInfo
        {
                get
                {
                        return _ToUserInfo;
                }
                set
                {
                        _ToUserInfo = value;
                        OnPropertyChanged(nameof(ToUserInfo));
                }
        }

        private Agrin.Server.DataBase.Models.UserInfo _FromUserInfo;
        public Agrin.Server.DataBase.Models.UserInfo FromUserInfo
        {
                get
                {
                        return _FromUserInfo;
                }
                set
                {
                        _FromUserInfo = value;
                        OnPropertyChanged(nameof(FromUserInfo));
                }
        }


    }

    public class UserSessionInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private long _Id;
        public long Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private System.Guid _FirstKey;
        public System.Guid FirstKey
        {
                get
                {
                        return _FirstKey;
                }
                set
                {
                        _FirstKey = value;
                        OnPropertyChanged(nameof(FirstKey));
                }
        }

        private System.Guid _SecondKey;
        public System.Guid SecondKey
        {
                get
                {
                        return _SecondKey;
                }
                set
                {
                        _SecondKey = value;
                        OnPropertyChanged(nameof(SecondKey));
                }
        }

        private System.DateTime _CreatedDateTime;
        public System.DateTime CreatedDateTime
        {
                get
                {
                        return _CreatedDateTime;
                }
                set
                {
                        _CreatedDateTime = value;
                        OnPropertyChanged(nameof(CreatedDateTime));
                }
        }

        private string _OsName;
        public string OsName
        {
                get
                {
                        return _OsName;
                }
                set
                {
                        _OsName = value;
                        OnPropertyChanged(nameof(OsName));
                }
        }

        private string _OsVersionNumber;
        public string OsVersionNumber
        {
                get
                {
                        return _OsVersionNumber;
                }
                set
                {
                        _OsVersionNumber = value;
                        OnPropertyChanged(nameof(OsVersionNumber));
                }
        }

        private string _OsVersionName;
        public string OsVersionName
        {
                get
                {
                        return _OsVersionName;
                }
                set
                {
                        _OsVersionName = value;
                        OnPropertyChanged(nameof(OsVersionName));
                }
        }

        private int _UserId;
        public int UserId
        {
                get
                {
                        return _UserId;
                }
                set
                {
                        _UserId = value;
                        OnPropertyChanged(nameof(UserId));
                }
        }

        private bool _IsActive;
        public bool IsActive
        {
                get
                {
                        return _IsActive;
                }
                set
                {
                        _IsActive = value;
                        OnPropertyChanged(nameof(IsActive));
                }
        }

        private Agrin.Server.DataBase.Models.UserInfo _UserInfo;
        public Agrin.Server.DataBase.Models.UserInfo UserInfo
        {
                get
                {
                        return _UserInfo;
                }
                set
                {
                        _UserInfo = value;
                        OnPropertyChanged(nameof(UserInfo));
                }
        }


    }

    public class TagInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private string _Title;
        public string Title
        {
                get
                {
                        return _Title;
                }
                set
                {
                        _Title = value;
                        OnPropertyChanged(nameof(Title));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.PostCategoryTagRelationInfo> _PostCategoryTagRelationInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.PostCategoryTagRelationInfo> PostCategoryTagRelationInfoes
        {
                get
                {
                        return _PostCategoryTagRelationInfoes;
                }
                set
                {
                        _PostCategoryTagRelationInfoes = value;
                        OnPropertyChanged(nameof(PostCategoryTagRelationInfoes));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.PostTagRelationInfo> _PostTagRelationInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.PostTagRelationInfo> PostTagRelationInfoes
        {
                get
                {
                        return _PostTagRelationInfoes;
                }
                set
                {
                        _PostTagRelationInfoes = value;
                        OnPropertyChanged(nameof(PostTagRelationInfoes));
                }
        }


    }

    public class DirectFileInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private long _Id;
        public long Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private System.DateTime _CreatedDateTime;
        public System.DateTime CreatedDateTime
        {
                get
                {
                        return _CreatedDateTime;
                }
                set
                {
                        _CreatedDateTime = value;
                        OnPropertyChanged(nameof(CreatedDateTime));
                }
        }

        private int _ServerId;
        public int ServerId
        {
                get
                {
                        return _ServerId;
                }
                set
                {
                        _ServerId = value;
                        OnPropertyChanged(nameof(ServerId));
                }
        }

        private bool _IsComplete;
        public bool IsComplete
        {
                get
                {
                        return _IsComplete;
                }
                set
                {
                        _IsComplete = value;
                        OnPropertyChanged(nameof(IsComplete));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.DirectFileToUserRelationInfo> _DirectFileToUserRelationInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.DirectFileToUserRelationInfo> DirectFileToUserRelationInfoes
        {
                get
                {
                        return _DirectFileToUserRelationInfoes;
                }
                set
                {
                        _DirectFileToUserRelationInfoes = value;
                        OnPropertyChanged(nameof(DirectFileToUserRelationInfoes));
                }
        }

        private Agrin.Server.DataBase.Models.ServerInfo _ServerInfo;
        public Agrin.Server.DataBase.Models.ServerInfo ServerInfo
        {
                get
                {
                        return _ServerInfo;
                }
                set
                {
                        _ServerInfo = value;
                        OnPropertyChanged(nameof(ServerInfo));
                }
        }


    }

    public class DirectFolderInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private string _Name;
        public string Name
        {
                get
                {
                        return _Name;
                }
                set
                {
                        _Name = value;
                        OnPropertyChanged(nameof(Name));
                }
        }

        private int? _ParentId;
        public int? ParentId
        {
                get
                {
                        return _ParentId;
                }
                set
                {
                        _ParentId = value;
                        OnPropertyChanged(nameof(ParentId));
                }
        }

        private Agrin.Server.DataBase.Models.DirectFolderInfo _ParentFolderInfo;
        public Agrin.Server.DataBase.Models.DirectFolderInfo ParentFolderInfo
        {
                get
                {
                        return _ParentFolderInfo;
                }
                set
                {
                        _ParentFolderInfo = value;
                        OnPropertyChanged(nameof(ParentFolderInfo));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.DirectFolderInfo> _Children;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.DirectFolderInfo> Children
        {
                get
                {
                        return _Children;
                }
                set
                {
                        _Children = value;
                        OnPropertyChanged(nameof(Children));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.DirectFolderToUserRelationInfo> _DirectFolderToUserRelationInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.Relations.DirectFolderToUserRelationInfo> DirectFolderToUserRelationInfoes
        {
                get
                {
                        return _DirectFolderToUserRelationInfoes;
                }
                set
                {
                        _DirectFolderToUserRelationInfoes = value;
                        OnPropertyChanged(nameof(DirectFolderToUserRelationInfoes));
                }
        }


    }

    public class ServerInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Id;
        public int Id
        {
                get
                {
                        return _Id;
                }
                set
                {
                        _Id = value;
                        OnPropertyChanged(nameof(Id));
                }
        }

        private string _Domain;
        public string Domain
        {
                get
                {
                        return _Domain;
                }
                set
                {
                        _Domain = value;
                        OnPropertyChanged(nameof(Domain));
                }
        }

        private string _IpAddress;
        public string IpAddress
        {
                get
                {
                        return _IpAddress;
                }
                set
                {
                        _IpAddress = value;
                        OnPropertyChanged(nameof(IpAddress));
                }
        }

        private System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.DirectFileInfo> _DirectFileInfoes;
        public System.Collections.Generic.ICollection<Agrin.Server.DataBase.Models.DirectFileInfo> DirectFileInfoes
        {
                get
                {
                        return _DirectFileInfoes;
                }
                set
                {
                        _DirectFileInfoes = value;
                        OnPropertyChanged(nameof(DirectFileInfoes));
                }
        }


    }

}

namespace Agrin.Server.Models.Filters
{
    public class FilterBaseInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _Index;
        public int Index
        {
                get
                {
                        return _Index;
                }
                set
                {
                        _Index = value;
                        OnPropertyChanged(nameof(Index));
                }
        }

        private int _Length;
        public int Length
        {
                get
                {
                        return _Length;
                }
                set
                {
                        _Length = value;
                        OnPropertyChanged(nameof(Length));
                }
        }

        private System.DateTime? _StartDateTime;
        public System.DateTime? StartDateTime
        {
                get
                {
                        return _StartDateTime;
                }
                set
                {
                        _StartDateTime = value;
                        OnPropertyChanged(nameof(StartDateTime));
                }
        }

        private System.DateTime? _EndDateTime;
        public System.DateTime? EndDateTime
        {
                get
                {
                        return _EndDateTime;
                }
                set
                {
                        _EndDateTime = value;
                        OnPropertyChanged(nameof(EndDateTime));
                }
        }


    }

    public class FilterPostInfo : Agrin.Server.Models.Filters.FilterBaseInfo
    {
        private int? _CategoryId;
        public int? CategoryId
        {
                get
                {
                        return _CategoryId;
                }
                set
                {
                        _CategoryId = value;
                        OnPropertyChanged(nameof(CategoryId));
                }
        }


    }

}

namespace Agrin.Server.DataBase.Models.Relations
{
    public class PostTagRelationInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _PostId;
        public int PostId
        {
                get
                {
                        return _PostId;
                }
                set
                {
                        _PostId = value;
                        OnPropertyChanged(nameof(PostId));
                }
        }

        private int _TagId;
        public int TagId
        {
                get
                {
                        return _TagId;
                }
                set
                {
                        _TagId = value;
                        OnPropertyChanged(nameof(TagId));
                }
        }

        private Agrin.Server.DataBase.Models.PostInfo _PostInfo;
        public Agrin.Server.DataBase.Models.PostInfo PostInfo
        {
                get
                {
                        return _PostInfo;
                }
                set
                {
                        _PostInfo = value;
                        OnPropertyChanged(nameof(PostInfo));
                }
        }

        private Agrin.Server.DataBase.Models.TagInfo _TagInfo;
        public Agrin.Server.DataBase.Models.TagInfo TagInfo
        {
                get
                {
                        return _TagInfo;
                }
                set
                {
                        _TagInfo = value;
                        OnPropertyChanged(nameof(TagInfo));
                }
        }


    }

    public class PostCategoryTagRelationInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private bool _IsHeaderTag;
        public bool IsHeaderTag
        {
                get
                {
                        return _IsHeaderTag;
                }
                set
                {
                        _IsHeaderTag = value;
                        OnPropertyChanged(nameof(IsHeaderTag));
                }
        }

        private int _PostCategoryId;
        public int PostCategoryId
        {
                get
                {
                        return _PostCategoryId;
                }
                set
                {
                        _PostCategoryId = value;
                        OnPropertyChanged(nameof(PostCategoryId));
                }
        }

        private int _TagId;
        public int TagId
        {
                get
                {
                        return _TagId;
                }
                set
                {
                        _TagId = value;
                        OnPropertyChanged(nameof(TagId));
                }
        }

        private Agrin.Server.DataBase.Models.PostCategoryInfo _PostCategoryInfo;
        public Agrin.Server.DataBase.Models.PostCategoryInfo PostCategoryInfo
        {
                get
                {
                        return _PostCategoryInfo;
                }
                set
                {
                        _PostCategoryInfo = value;
                        OnPropertyChanged(nameof(PostCategoryInfo));
                }
        }

        private Agrin.Server.DataBase.Models.TagInfo _TagInfo;
        public Agrin.Server.DataBase.Models.TagInfo TagInfo
        {
                get
                {
                        return _TagInfo;
                }
                set
                {
                        _TagInfo = value;
                        OnPropertyChanged(nameof(TagInfo));
                }
        }


    }

    public class DirectFileToUserRelationInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _UserId;
        public int UserId
        {
                get
                {
                        return _UserId;
                }
                set
                {
                        _UserId = value;
                        OnPropertyChanged(nameof(UserId));
                }
        }

        private long _DirectFileId;
        public long DirectFileId
        {
                get
                {
                        return _DirectFileId;
                }
                set
                {
                        _DirectFileId = value;
                        OnPropertyChanged(nameof(DirectFileId));
                }
        }

        private Agrin.Server.DataBase.Models.Relations.DirectFileFolderAccessType _AccessType;
        public Agrin.Server.DataBase.Models.Relations.DirectFileFolderAccessType AccessType
        {
                get
                {
                        return _AccessType;
                }
                set
                {
                        _AccessType = value;
                        OnPropertyChanged(nameof(AccessType));
                }
        }

        private Agrin.Server.DataBase.Models.UserInfo _UserInfo;
        public Agrin.Server.DataBase.Models.UserInfo UserInfo
        {
                get
                {
                        return _UserInfo;
                }
                set
                {
                        _UserInfo = value;
                        OnPropertyChanged(nameof(UserInfo));
                }
        }

        private Agrin.Server.DataBase.Models.DirectFileInfo _DirectFileInfo;
        public Agrin.Server.DataBase.Models.DirectFileInfo DirectFileInfo
        {
                get
                {
                        return _DirectFileInfo;
                }
                set
                {
                        _DirectFileInfo = value;
                        OnPropertyChanged(nameof(DirectFileInfo));
                }
        }


    }

    public class DirectFolderToUserRelationInfo : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private int _UserId;
        public int UserId
        {
                get
                {
                        return _UserId;
                }
                set
                {
                        _UserId = value;
                        OnPropertyChanged(nameof(UserId));
                }
        }

        private int _DirectFolderId;
        public int DirectFolderId
        {
                get
                {
                        return _DirectFolderId;
                }
                set
                {
                        _DirectFolderId = value;
                        OnPropertyChanged(nameof(DirectFolderId));
                }
        }

        private Agrin.Server.DataBase.Models.Relations.DirectFileFolderAccessType _AccessType;
        public Agrin.Server.DataBase.Models.Relations.DirectFileFolderAccessType AccessType
        {
                get
                {
                        return _AccessType;
                }
                set
                {
                        _AccessType = value;
                        OnPropertyChanged(nameof(AccessType));
                }
        }

        private Agrin.Server.DataBase.Models.UserInfo _UserInfo;
        public Agrin.Server.DataBase.Models.UserInfo UserInfo
        {
                get
                {
                        return _UserInfo;
                }
                set
                {
                        _UserInfo = value;
                        OnPropertyChanged(nameof(UserInfo));
                }
        }

        private Agrin.Server.DataBase.Models.DirectFolderInfo _DirectFolderInfo;
        public Agrin.Server.DataBase.Models.DirectFolderInfo DirectFolderInfo
        {
                get
                {
                        return _DirectFolderInfo;
                }
                set
                {
                        _DirectFolderInfo = value;
                        OnPropertyChanged(nameof(DirectFolderInfo));
                }
        }


    }

}

namespace AgrinMainServer.ClientServices
{
}

namespace Agrin.Server.Models
{
    public enum MessageType : int
    {
        None = 0,
        Success = 1,
        PleaseFillAllData = 2,
        PleaseInstallNewVersion = 3,
        WrongData = 4,
        ServerException = 5,
        FileNotFound = 6,
        DataOverFlow = 7,
        ClientIpIsNotValid = 8,
        SessionAccessDenied = 9,
        UsernameOrPasswordIncorrect = 10,
        NotSupportYet = 11,
        Duplicate = 12,
        StorageFull = 13,
    }

}

namespace Agrin.Server.DataBase.Models
{
    public enum PostType : byte
    {
        None = 0,
        Video = 1,
        Sound = 2,
    }

    public enum UserStatus : byte
    {
        None = 0,
        JustRegistred = 1,
        Confirm = 2,
        Blocked = 3,
    }

    public enum FileType : byte
    {
        None = 0,
        SmallLogo = 1,
        BackGroundImage = 2,
    }

    public enum OperationSystemType : int
    {
        None = 0,
        Windows = 1,
        WindowsUniversal = 2,
        Android = 4,
        IOS = 8,
        Mac = 16,
        Linux = 32,
        Any = 63,
    }

    public enum CreditType : byte
    {
        Unknown = 0,
        Gift = 1,
        BuyCredit = 2,
        Transfer = 3,
    }

}

namespace Agrin.Server.DataBase.Models.Relations
{
    public enum DirectFileFolderAccessType : byte
    {
        None = 0,
        Creator = 1,
        Viewer = 2,
    }

}

