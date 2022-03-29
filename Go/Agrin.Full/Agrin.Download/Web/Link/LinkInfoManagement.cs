using Agrin.Download.Data.Serializition;
using Agrin.Download.Manager;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web.Link
{
    public enum CompleteDownloadSystemMode
    {
        ShutDown = 0,
        Sleep = 1,
        Restart = 2
    }
    public class LinkInfoManagement : ANotifyPropertyChanged
    {
        public static Action<Exception> LinkInfoErrorAction { get; set; }
        public Action<List<object>> UserMustSelectItemAction { get; set; }//وقتی فراخونی بشه یعنی کاربر باید یکی از لینک ها رو به صورت پیشفرض انتخاب کنه
        public Action<object> UserSelectedItemAction { get; set; }//وقتی فراخونی بشه یعنی کاربر یک ایتم رو انتخاب کرد

        /// <summary>
        /// لینک اصلی
        /// </summary>
        LinkInfo _parent;
        public LinkInfo Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        int _readBuffer = 1024 * 30;

        public int ReadBuffer
        {
            get { return _readBuffer; }
            set
            {
                _readBuffer = value;
            }
        }

        private int _TryAginCount = 3;
        public int TryAginCount
        {
            get { return _TryAginCount; }
            set { _TryAginCount = value; }
        }
        public bool IsTryExtreme { get; set; }
        //AlgoritmEnum DownloadFileAlgoritm { get; set; }
        private List<ExceptionSerializable> _errors = new List<ExceptionSerializable>();

        public List<ExceptionSerializable> Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }

        List<MultiLinkAddress> _MultiLinks = new List<MultiLinkAddress>();
        public List<MultiLinkAddress> MultiLinks
        {
            get { return _MultiLinks; }
            set { _MultiLinks = value; }
        }

        public void AddMultiLink(MultiLinkAddress address, bool save = true)
        {
            MultiLinks.Add(address);
            if (save)
                Parent.SaveThisLink();
            ApplicationHelperBase.EnterDispatcherThreadAction(() =>
            {
                Parent.HostInfoProperties = null;
            });
        }

        public string GetLastMultiLinkAddress()
        {
            var list = MultiLinks.ToList();
            list.Reverse();
            foreach (var item in list)
            {
                if (item.IsSelected)
                    return item.Address;
            }
            return Parent.PathInfo.Address;
        }

        List<object> _SharingSettings = new List<object>();
        public List<object> SharingSettings
        {
            get
            {
                return _SharingSettings;
            }
            set { _SharingSettings = value; }
        }

        public bool ContainsAddress(string text)
        {
            foreach (var item in MultiLinks)
            {
                if (item.Address.ToLower().Contains(text))
                    return true;
            }
            return false;
        }

        List<ProxyInfo> _MultiProxy = new List<ProxyInfo>();
        public List<ProxyInfo> MultiProxy
        {
            get { return _MultiProxy; }
            set { _MultiProxy = value; }
        }

        private bool _IsLimit;

        public bool IsLimit
        {
            get { return _IsLimit; }
            set
            {
                _IsLimit = value;
                Parent.ResetBufferSizeAndLimiter();
            }
        }

        private int _limitPerSecound = 1024 * 10;
        public int LimitPerSecound
        {
            get { return _limitPerSecound; }
            set { _limitPerSecound = value; }
        }
        public bool IsEndDownload { get; set; }

        public CompleteDownloadSystemMode EndDownloadSystemMode { get; set; }

        NetworkCredentialInfo _NetworkUserPass;
        public NetworkCredentialInfo NetworkUserPass
        {
            get { return _NetworkUserPass; }
            set { _NetworkUserPass = value; }
        }

        string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        bool _IsShowBalloon;
        public bool IsShowBalloon
        {
            get { return _IsShowBalloon; }
            set { _IsShowBalloon = value; }
        }

        public string TaskIndex
        {
            get
            {
                var task = LinkTaskInfo;
                if (task == null)
                    return "";
                return ApplicationTaskManager.Current.FindLinkInfoIndexFromTaskInfo(Parent, task).ToString();
            }
        }

        public TaskInfo LinkTaskInfo
        {
            get
            {
                return ApplicationTaskManager.Current.FindTaskInfoByLinkInfo(Parent).FirstOrDefault();
            }
        }

        public string FullTaskNameAndIndex
        {
            get
            {
                var task = LinkTaskInfo;
                if (task == null)
                    return "-";
                return task.Name + "-" + TaskIndex;
            }
        }

        public void ChangedTaskInfo()
        {
            OnPropertyChanged("FullTaskNameAndIndex");
            OnPropertyChanged("TaskIndex");
            OnPropertyChanged("LinkTaskInfo");
        }

        public string LastErrorDescription
        {
            get
            {
                if (Errors.Count > 0)
                {
                    var error = Errors.Last();
#if(MobileApp || __ANDROID__)
                    if (error.ExceptionData is WebException)
                    {
                        if (error.OtherData.Count >= 2 && error.OtherData[1] is HttpStatusCode)
                            return ErrorDescription.GetErrorMessageByStatusCode((HttpStatusCode)error.OtherData[1]);
                        else if (error.OtherData.FirstOrDefault() is WebExceptionStatus)
                            return ErrorDescription.GetErrorMessageByWebExceptionStatus((WebExceptionStatus)error.OtherData[0]);
                    }
#endif
                    return error.ExceptionData.Message;
                }
                else
                    return "No Error Found!";
            }
        }

        public bool IsApplicationSetting { get; set; }

        object lockError = new object();
        public void AddError(Exception error)
        {
            try
            {
                if (LinkInfoErrorAction != null)
                    LinkInfoErrorAction(error);
                if (error == null || error.ToString().Contains("Java."))
                    return;
                lock (lockError)
                {
                    //var list = Errors.ToList();
                    //for (int i = 0; i < list.Count; i++)
                    //{
                    //    var item = list[i];
                    //    if (item == null) continue;
                    //    if (item.ExceptionData is WebException && error is WebException && item.ExceptionData.Message == error.Message)
                    //    {
                    //        list.Remove(item);
                    //        list.Add(item);
                    //        item.Count++;
                    //        Errors = list;
                    //        OnPropertyChanged("LastErrorDescription");
                    //        return;
                    //    }
                    //}
                    var last = Errors.LastOrDefault();
                    if (last != null && last.ExceptionData != null && last.ExceptionData.Message == error.Message)
                        return;
                    Errors.Add(ExceptionSerializable.ExceptionToSerializable(error));
                    if (Errors.Count > 3)
                    {
                        Errors.RemoveAt(0);
                    }
                    OnPropertyChanged("LastErrorDescription");
                }

                OnPropertyChanged("LastErrorDescription");
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "AddError");
            }
            Parent.SaveThisLink();
        }
    }
}
