using Agrin.BaseViewModels.Lists;
using Agrin.BaseViewModels.Models;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
#if (!MobileApp && !XamarinApp)
using Agrin.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.WindowLayouts.Asuda
{
    public class AsudaDataOptimizerBaseViewModel : ANotifyPropertyChanged
    {
        public static object Dispatcher { get; set; }
        object lockobj = new object();
        public AsudaDataOptimizerBaseViewModel()
        {
            ProxyMonitor.ResponseCompleteAction = (item) =>
            {
                lock (lockobj)
                {
                    ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                    {
                        var asudaWebResponseData = new AsudaWebResponseData() { OrginalData = item };
                        asudaWebResponseData.ReponseInformation.PropertyChanged += AsudaWebResponseData_PropertyChanged;
                        Items.Add(asudaWebResponseData);
                    }, Dispatcher);
                }
            };
            ProxyMonitor.MultipeResponseCompleteAction = (data) =>
            {
                lock (lockobj)
                {
                    ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                    {
                        List<AsudaWebResponseData> items = new List<AsudaWebResponseData>();
                        foreach (var item in data.Items)
                        {
                            var asudaWebResponseData = new AsudaWebResponseData() { OrginalData = item };
                            asudaWebResponseData.ReponseInformation.PropertyChanged += AsudaWebResponseData_PropertyChanged;
                            items.Add(asudaWebResponseData);
                        }
                        Items.AddRange(items);
                    }, Dispatcher);
                }
            };
            //Items.Add(new AsudaWebResponseData() { OrginalData = new WebResponseData() { FileName = "CACW.2016.720p.HDTC.x265.HEVC.Team-x265mkv.mkv", Extension = ".mkv", Uri = "http://framesoft.ir/download/downloadonefile/CACW.2016.720p.HDTC.x265.HEVC.Team-x265mkv.mkv" } });
            //Items.Add(new AsudaWebResponseData() { OrginalData = new WebResponseData() { FileName = "ali abdolmaleki.mp3", Extension = ".mp3", Uri = "http://stackoverflow.com/questions/3830531/how-can-i-share-one-session-between-several-wcfs-clients" } });
            //Items.Add(new AsudaWebResponseData() { OrginalData = new WebResponseData() { FileName = "image.jpg", Extension = ".jpg", Uri = "http://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp" } });
        }

        private void AsudaWebResponseData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                OnPropertyChanged("SuccessCount");
                OnPropertyChanged("ErrorCount");
                OnPropertyChanged("ConnectingCount");
            }
        }

        FastCollection<AsudaWebResponseData> _Items = null;
        bool _IsAsudaOn = false;

        public FastCollection<AsudaWebResponseData> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new FastCollection<AsudaWebResponseData>(Dispatcher);
                    _Items.ChangedCollection = () =>
                    {
                        OnPropertyChanged("SuccessCount");
                        OnPropertyChanged("ErrorCount");
                        OnPropertyChanged("ConnectingCount");
                    };
                }
                return _Items;
            }
            set
            {
                _Items = value;
            }
        }

        public int SuccessCount
        {
            get
            {
                return (from x in Items where x.ReponseInformation.Status == Download.Responses.HttpReponseStatusEnum.Complete && x.ReponseInformation.Size > 0 select x).Count();
            }
        }

        public int ErrorCount
        {
            get
            {
                return (from x in Items where (x.ReponseInformation.Status != Download.Responses.HttpReponseStatusEnum.Complete && x.ReponseInformation.Status != Download.Responses.HttpReponseStatusEnum.Connectiong) || (x.ReponseInformation.Status == Download.Responses.HttpReponseStatusEnum.Complete && x.ReponseInformation.Size <= 0) select x).Count();

            }
        }

        public int ConnectingCount
        {
            get
            {
                return (from x in Items where x.ReponseInformation.Status == Download.Responses.HttpReponseStatusEnum.Connectiong select x).Count();

            }
        }

        public bool IsAsudaOn
        {
            get
            {
                return _IsAsudaOn;
            }
            set
            {
                if (value)
                    ProxyMonitor.Start();
                else
                    ProxyMonitor.Stop();

                _IsAsudaOn = value;
                OnPropertyChanged("IsAsudaOn");

            }
        }
        
        public void DownloadResponse(AsudaWebResponseData responseData)
        {
            LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(responseData.OrginalData.Uri, responseData.Group, responseData.SavePath, null, null, null, true, null, responseData.OrginalData.Headers);
            RemoveResponse(responseData);
        }

        public bool CanDownloadResponse(AsudaWebResponseData responseData)
        {
            return true;
        }

        public void AddResponse(AsudaWebResponseData responseData)
        {
            LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(responseData.OrginalData.Uri, responseData.Group, responseData.SavePath, null, null, null, false, null, responseData.OrginalData.Headers);
            RemoveResponse(responseData);
        }

        public bool CanAddResponse(AsudaWebResponseData responseData)
        {
            return true;
        }

        public void RefreshResponse(AsudaWebResponseData responseData)
        {
            responseData.ReponseInformation.GenerateInformation();
        }

        public bool CanRefreshResponse(AsudaWebResponseData responseData)
        {
            return responseData != null && responseData.ReponseInformation.Status != Download.Responses.HttpReponseStatusEnum.Connectiong;
        }

        public void RemoveResponse(AsudaWebResponseData responseData)
        {
            Items.Remove(responseData);
            responseData.Dispose();
        }

        public bool CanRemoveResponse(AsudaWebResponseData responseData)
        {
            return true;
        }

        public void RemoveAll()
        {
            Items.Clear();
        }

        public bool CanRemoveAll()
        {
            return Items.Count > 0;
        }

        public void AddAllResponses()
        {
            foreach (var responseData in Items.ToList())
            {
                LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(responseData.OrginalData.Uri, responseData.Group, responseData.SavePath, null, null, null, false, null);
            }
            Items.Clear();
        }

        public bool CanAddAllResponses()
        {
            return Items.Count > 0;
        }

       
    }
}
#endif