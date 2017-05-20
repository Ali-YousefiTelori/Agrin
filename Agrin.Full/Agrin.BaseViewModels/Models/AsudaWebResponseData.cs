using Agrin.Download.Responses;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
#if (!MobileApp && !XamarinApp)
using Agrin.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Models
{
    /// <summary>
    /// یک آیتم از لیست درخواست هایی که به آسودا ارسال میشود
    /// </summary>
    public class AsudaWebResponseData : ANotifyPropertyChanged,IDisposable
    {
        public AsudaWebResponseData()
        {
            base.IgnoreStopChanged = true;
            ReponseInformation = new HttpResponseInformation();
        }

        WebResponseData _OrginalData;
        public WebResponseData OrginalData
        {
            get
            {
                return _OrginalData;
            }
            set
            {
                _OrginalData = value;
                ReponseInformation.Uri = value.Uri;
                ReponseInformation.Headers = value.Headers;
                ReponseInformation.GenerateInformation();
                OnPropertyChanged("OrginalData");
            }
        }

        public GroupInfo Group
        {
            get
            {
                return Agrin.Download.Manager.ApplicationGroupManager.Current.FindGroupByFileName(OrginalData.FileName);
            }
        }

        string _UserSavePath;
        public string UserSavePath
        {
            get
            {
                return _UserSavePath;
            }
            set
            {
                _UserSavePath = value;
                OnPropertyChanged("UserSavePath");
            }
        }

        public string SavePath
        {
            get
            {
                if (string.IsNullOrEmpty(UserSavePath))
                {
                    return Group.SavePath;
                }

                return UserSavePath;
            }
        }

        HttpResponseInformation _ReponseInformation;
        public HttpResponseInformation ReponseInformation
        {
            get
            {
                return _ReponseInformation;
            }
            set
            {
                _ReponseInformation = value;
                OnPropertyChanged("ReponseInformation");
            }
        }

        bool _isGettingFileIcon = false;
        byte[] _FileIcon = null;

        public byte[] FileIcon
        {
            get
            {
                if (_FileIcon != null)
                    return _FileIcon;
                if (_isGettingFileIcon)
                    return _FileIcon;
                _isGettingFileIcon = true;
                AsyncActions.Action(() =>
                {
                    _FileIcon = Agrin.IO.FileStatic.GetFileIcon(OrginalData.Extension);
                    if (_FileIcon != null)
                        OnPropertyChanged("FileIcon");
                    _isGettingFileIcon = false;
                }, (ex) =>
                {
                    _isGettingFileIcon = false;
                });
                return _FileIcon;
            }
            set
            {
                _FileIcon = value;
                OnPropertyChanged("FileIcon");
            }
        }

        public void Dispose()
        {

        }
    }
}
#endif