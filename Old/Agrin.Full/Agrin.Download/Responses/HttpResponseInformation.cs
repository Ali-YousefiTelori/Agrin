using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Download.Responses
{
    public enum HttpReponseStatusEnum
    {
        Stoped,
        Connectiong,
        Complete,
        Error
    }

    /// <summary>
    /// کلاس دریافت مشخصات یک لینک
    /// </summary>
    public class HttpResponseInformation : ANotifyPropertyChanged, IDisposable
    {
        public HttpResponseInformation()
        {
            base.IgnoreStopChanged = true;
        }

        public string Uri { get; set; }

        public Exception LastError { get; set; }

        HttpReponseStatusEnum _Status = HttpReponseStatusEnum.Stoped;
        public HttpReponseStatusEnum Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

        private long _size = -2;
        public long Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                OnPropertyChanged("Size");
            }
        }

        public Dictionary<string, string> Headers { get; set; }

        public void GenerateInformation()
        {
            if (Status == HttpReponseStatusEnum.Connectiong || _isDispose)
                return;
            Task task = new Task(() =>
            {
                LastError = null;
                try
                {
                    Status = HttpReponseStatusEnum.Connectiong;
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Uri);
                    request.Timeout = 60000;
                    request.AllowAutoRedirect = true;
                    request.Proxy = null;
                    request.ServicePoint.ConnectionLimit = int.MaxValue;

                    request.KeepAlive = true;
                    if (Headers.Count == 0)
                    {
                        request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                        request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                    }
                    else
                    {
                        foreach (var item in Headers)
                        {
                            request.SetRawHeader(item.Key, item.Value);
                        }
                    }

                    using (var response = request.GetResponse())
                    {
                        Size = response.ContentLength;
                    }
                    Status = HttpReponseStatusEnum.Complete;
                }
                catch (Exception ex)
                {
                    Status = HttpReponseStatusEnum.Error;
                    LastError = ex;
                }
            });
            task.Start();
        }

        bool _isDispose = false;
        public void Dispose()
        {
            _isDispose = true;
        }
    }
}
