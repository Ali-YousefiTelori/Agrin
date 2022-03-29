using Agrin.Download.Manager;
using Agrin.Helper.ComponentModel;
using Agrin.IO.File;
using Agrin.IO.HardWare;
using Agrin.IO.Helper;
using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web.Link
{
    public class LinkInfoPath : ANotifyPropertyChanged
    {
        public LinkInfoPath(string address, LinkInfo linkInfo)
        {
            Address = address;
            AddressFileName = Agrin.IO.FileStatic.GetLinksFileName(address);
            Parent = linkInfo;
        }

        public LinkInfoPath()
        {

        }
        /// <summary>
        /// لینک اصلی
        /// </summary>
        LinkInfo _parent;
        public LinkInfo Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged("Parent"); }
        }
        /// <summary>
        /// آی دی لینک
        /// </summary>
        int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("Id"); }
        }

        /// <summary>
        /// آدرس لینک
        /// </summary>
        string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged("Address"); OnPropertyChanged("HostAddress"); }
        }

        public string HostAddress
        {
            get
            {
                Uri uri;
                Uri.TryCreate(Address, UriKind.Absolute, out uri);
                return uri.Host;
            }
        }

        string _MixerSavePath = null;
        public string MixerSavePath
        {
            get
            {
                if (string.IsNullOrEmpty(_MixerSavePath))
                {
                    _MixerSavePath = MPath.CreateOneFileByAddress(MPath.BackUpMixersPath);
                }
                return _MixerSavePath;
            }
            set
            {
                _MixerSavePath = value;
            }
        }

        string _MixerBackupSavePath = null;
        public string MixerBackupSavePath
        {
            get
            {
                if (string.IsNullOrEmpty(_MixerBackupSavePath))
                {
                    _MixerBackupSavePath = MPath.CreateOneFileByAddress(MPath.BackUpMixersPath);
                }
                return _MixerBackupSavePath;
            }
            set
            {
                _MixerBackupSavePath = value;
            }
        }

        /// <summary>
        /// محل ذخیره اصلی کانکشن های در حال دانلود فایل
        /// </summary>
        string _connectionsSavedAddress;
        public string ConnectionsSavedAddress
        {
            get
            {
                if (_connectionsSavedAddress == null)
                    _connectionsSavedAddress = System.IO.Path.Combine(MPath.SaveDataPath, Id.ToString());
                MPath.CreateDoesNotExistDirectory(_connectionsSavedAddress);
                return _connectionsSavedAddress;
            }
            set { _connectionsSavedAddress = value; OnPropertyChanged("ConnectionsSavedAddress"); }
        }

        /// <summary>
        /// محل ذخیره ای که نرم افزار برای فایل انتخاب میکند
        /// </summary>
        public string AppSavePath
        {
            get
            {
                return CurrentGroupInfo.SavePath;
            }
        }

        /// <summary>
        /// آدرس پوشه ی امنیتی نرم افزار
        /// </summary>
        public string AppSecurityPath
        {
            get
            {
                return CurrentGroupInfo.SecurityPath;
            }
        }

        string _UserSecurityPath;
        /// <summary>
        /// آدرس پوشه ی امنیتی که کاربر داده
        /// </summary>
        public string UserSecurityPath
        {
            get { return _UserSecurityPath; }
            set
            {
                _UserSecurityPath = value;
                OnPropertyChanged("UserSecurityPath");
            }
        }

        /// <summary>
        /// آدرس پوشه ی امینیتی برای ذخیره
        /// </summary>
        public string SecurityPath
        {
            get
            {
                string path = "";
                if (string.IsNullOrEmpty(UserSecurityPath))
                {
                    path = AppSecurityPath;
                }
                else if (string.IsNullOrEmpty(UserSavePath))//در صورتی که کاربر آدرس وارد کرد مقدار خالی برگردونه تا نرم افزار دچار مشکل نشه
                    path = UserSecurityPath;
                return path;
            }
        }

        /// <summary>
        /// آدرس فایل ساخته شده در نهایت و به روش امنیتی
        /// </summary>
        string _SecurityFileSavePath;

        public string SecurityFileSavePath
        {
            get { return _SecurityFileSavePath; }
            set { _SecurityFileSavePath = value; OnPropertyChanged("SecurityFileSavePath"); }
        }
       
        string _userSavePath;
        /// <summary>
        /// محل ذخیره ای که کاربر انتخاب میکند
        /// </summary>
        public string UserSavePath
        {
            get { return _userSavePath; }
            set
            {
                _userSavePath = value;
                OnPropertyChanged("UserSavePath");
                OnPropertyChanged("SavePath");
                OnPropertyChanged("FullAddressFileName");
            }
        }

        public string SavePath
        {
            get
            {
                if (!string.IsNullOrEmpty(SecurityPath))
                    return SecurityPath;
                else if (!string.IsNullOrEmpty(UserSavePath))
                    return UserSavePath;
                else
                    return AppSavePath;
            }
        }

        /// <summary>
        /// نامی که نرم افزار از آدرس میگیرد
        /// </summary>
        string _addressFileName;
        public string AddressFileName
        {
            get { return _addressFileName; }
            set
            {
                string oldExtension = FileExtension;
                _addressFileName = value;
                OnPropertyChanged("AddressFileName");
                OnPropertyChanged("FileName");
                OnPropertyChanged("FileExtension");
                if (oldExtension != FileExtension && Parent != null)
                {
                    Parent.FileIcon = null;
                    Parent.OnPropertyChanged("FileIcon");
                } 
                OnPropertyChanged("FullAddressFileName");
                OnPropertyChanged("FileType");
                if (ApplicationGroupManager.Current != null)
                    ApplicationGroupInfo = ApplicationGroupManager.Current.FindGroupByFileName(value);
            }
        }

        /// <summary>
        /// نامی که کاربر برای فایل خود انتخاب میکند
        /// </summary>
        string _userFileName;
        public string UserFileName
        {
            get { return _userFileName; }
            set
            {
                string oldExtension = FileExtension;
                _userFileName = value;
                OnPropertyChanged("UserFileName");
                OnPropertyChanged("FileName");
                OnPropertyChanged("FileExtension");
                if (oldExtension != FileExtension && Parent != null)
                {
                    Parent.FileIcon = null;
                    Parent.OnPropertyChanged("FileIcon");
                }
                OnPropertyChanged("FullAddressFileName");
                OnPropertyChanged("FileType");
                if (ApplicationGroupManager.Current != null)
                    ApplicationGroupInfo = ApplicationGroupManager.Current.FindGroupByFileName(value);
            }
        }

        /// <summary>
        /// نام فایلی که به صورت پیشفرض از یکی از فیلد های نام آدرس فایل یا نام یوزر فایل میگیرد
        /// </summary>
        public string FileName
        {
            get
            {
                if (!String.IsNullOrEmpty(UserFileName))
                    return UserFileName;
                else
                    return AddressFileName;
            }
        }

        public string FileExtension
        {
            get
            {
                try
                {
                    return MPath.GetFileExtention(FileName);

                }
                catch (Exception ex)
                {
                    AutoLogger.LogError(ex, "get FileExtension");
                    return null;
                }
            }
        }

        public string FullAddressFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(SecurityFileSavePath))
                    return SecurityFileSavePath;
                else if (!string.IsNullOrEmpty(SecurityPath))
                    return SecurityPath;
                return System.IO.Path.Combine(SavePath, FileName);
            }
        }

        public string FileType
        {
            get
            {
                return FileProperties.GetFileType(FileName);
            }
        }

        public long FreeSpace
        {
            get
            {
                return Disk.GetDriveSize(SavePath);
            }
        }

        /// <summary>
        /// مسیر ذخیره ی ادرس بکآپ لینک جهت ترمیم
        /// </summary>
        public string BackUpCompleteAddress { get; set; } = "";

        GroupInfo _UserGroupInfo;
        public GroupInfo UserGroupInfo
        {
            get { return _UserGroupInfo; }
            set
            {
                if (value == _UserGroupInfo)
                    return;
                _UserGroupInfo = value;
                OnPropertyChanged("SavePath");
                OnPropertyChanged("UserGroupInfo");
                Agrin.Download.Engine.SearchEngine.Search(Parent);
                if (ApplicationGroupManager.Current.ChangedGroups != null)
                    ApplicationGroupManager.Current.ChangedGroups(this.Parent);
            }
        }

        GroupInfo _ApplicationGroupInfo;
        public GroupInfo ApplicationGroupInfo
        {
            get
            {
                if (_ApplicationGroupInfo == null)
                    return ApplicationGroupManager.Current.NoGroup;
                return _ApplicationGroupInfo;
            }
            set
            {
                if (value == _ApplicationGroupInfo)
                    return;
                _ApplicationGroupInfo = value;
                OnPropertyChanged("SavePath");
                OnPropertyChanged("ApplicationGroupInfo");
                Agrin.Download.Engine.SearchEngine.Search(Parent);
                if (ApplicationGroupManager.Current.ChangedGroups != null)
                    ApplicationGroupManager.Current.ChangedGroups(this.Parent);
            }
        }

        public GroupInfo CurrentGroupInfo
        {
            get
            {
                if (UserGroupInfo == null)
                    return ApplicationGroupInfo;
                return UserGroupInfo;
            }
        }
    }
}
