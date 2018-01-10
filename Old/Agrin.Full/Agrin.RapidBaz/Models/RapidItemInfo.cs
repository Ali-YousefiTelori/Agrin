using Agrin.Helper.ComponentModel;
using Agrin.IO.File;
using Agrin.RapidBaz.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Models
{
    public class RapidItemInfo : ANotifyPropertyChanged
    {
        public static void OnChangedFlag()
        {
            if (ChangedFlag != null)
                ChangedFlag();
        }

        public static event Action ChangedFlag;
        public static Func<string, byte[]> GetFlagByCountryCode { get; set; }
        public static Func<RapidItemInfo, bool> IsOnceDownloadUserFunction { get; set; }
        public static Action<RapidItemInfo, bool> DownloadAfterCompleteChangedAction { get; set; }
        public static Func<RapidItemInfo, bool> DownloadAfterCompleteChangedFunction { get; set; }

        public string ID { get; set; }
        string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        string _Link;

        public string Link
        {
            get { return _Link; }
            set
            {
                _Link = value;
                OnPropertyChanged("Link");
            }
        }

        string _Size;
        public string Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                OnPropertyChanged("Size");
                Validate();
            }
        }

        string _Status;

        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
                Validate();
            }
        }

        string _Progress;

        public string Progress
        {
            get { return _Progress; }
            set
            {
                _Progress = value;
                OnPropertyChanged("Progress");
                Validate();
            }
        }

        string _Url;

        public string Url
        {
            get { return _Url; }
            set { _Url = value; OnPropertyChanged("Url"); }
        }

        public bool IsDownloadOnOrOff
        {
            get
            {
                if (DownloadAfterCompleteChangedFunction == null)
                    return false;
                return DownloadAfterCompleteChangedFunction(this);
            }
            set
            {
                if (DownloadAfterCompleteChangedAction != null)
                    DownloadAfterCompleteChangedAction(this, value);
            }
        }

        public string StatusString
        {
            get
            {
                return WebManager.GetStatusString(Status);
            }
        }

        public string FileType
        {
            get
            {
                return FileProperties.GetFileType(Name);
            }
        }

        public bool IsComplete
        {
            get
            {
                return Progress == "100";
            }
        }

        public bool IsStop
        {
            get
            {
                return WebManager.IsStopStatus(Status);
            }
        }

        public bool IsOnceDownloadUser
        {
            get
            {
                if (IsOnceDownloadUserFunction != null)
                    return IsOnceDownloadUserFunction(this);
                return false;
            }
        }

        public bool IsError
        {
            get
            {
                return WebManager.IsErrorStatus(Status);
            }
        }

        public bool IsSizeValue
        {
            get
            {
                int val = -1;
                int.TryParse(Size, out val);
                return val > 0;
            }
        }

        public string GetPercent
        {
            get
            {
                return Progress + "%";
            }
        }


        DateTime _ts = DateTime.Now;

        public DateTime Ts
        {
            get { return _ts; }
            set { _ts = value; OnPropertyChanged("Ts"); }
        }

        private string _cc;

        public string Cc
        {
            get { return _cc; }
            set { _cc = value; OnPropertyChanged("CountryFlag"); }
        }

        byte[] _CountryFlag;
        public byte[] CountryFlag
        {
            get
            {
                if (GetFlagByCountryCode != null && !string.IsNullOrEmpty(Cc))
                    _CountryFlag = GetFlagByCountryCode(Cc);
                if (_CountryFlag == null && !string.IsNullOrEmpty(Cc) && GetFlagByCountryCode != null)
                {
                    ChangedFlag -= RapidItemInfo_ChangedFlag;
                    ChangedFlag += RapidItemInfo_ChangedFlag;
                }
                return _CountryFlag;
            }
        }

        void RapidItemInfo_ChangedFlag()
        {
            OnPropertyChanged("CountryFlag");
        }

        public void Validate()
        {
            OnPropertyChanged("GetPercent");
            OnPropertyChanged("IsSizeValue");
            OnPropertyChanged("IsError");
            OnPropertyChanged("IsComplete");
            OnPropertyChanged("StatusString");
            OnPropertyChanged("IsOnceDownloadUser");
            OnPropertyChanged("IsStop");
        }
    }
}
