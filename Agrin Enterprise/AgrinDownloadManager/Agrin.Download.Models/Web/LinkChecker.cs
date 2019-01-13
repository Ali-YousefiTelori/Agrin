using Agrin.Download.Web.Requests;
using Agrin.Models;
using MvvmGo.ViewModels;
using SignalGo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Agrin.Download.Web
{
    public class LinkChecker : BaseViewModel, IDisposable
    {
        public Exception Exception { get; set; }

        CheckStatus _hostStatus = CheckStatus.Unknown;
        CheckStatus _linkStatus = CheckStatus.Unknown;
        CheckStatus _resumableStatus = CheckStatus.Unknown;
        long _LinkSize = -2;
        string _Country = "نامشخص";

        public CheckStatus HostStatus
        {
            get
            {
                return _hostStatus;
            }
            set
            {
                _hostStatus = value;
                OnPropertyChanged(nameof(HostStatus));
            }
        }

        public CheckStatus LinkStatus
        {
            get
            {
                return _linkStatus;
            }
            set
            {
                _linkStatus = value;
                OnPropertyChanged(nameof(LinkStatus));
            }
        }

        public CheckStatus ResumableStatus
        {
            get
            {
                return _resumableStatus;
            }
            set
            {
                _resumableStatus = value;
                OnPropertyChanged(nameof(ResumableStatus));
            }
        }

        public long LinkSize
        {
            get
            {
                return _LinkSize;
            }
            set
            {
                _LinkSize = value;
                OnPropertyChanged(nameof(LinkSize));
            }
        }

        public string Country
        {
            get
            {
                return _Country;
            }
            set
            {
                _Country = value;
                OnPropertyChanged(nameof(Country));
            }
        }

        public void Restart(string linkAddress)
        {
            _isStartHost = false;
            _isStartLink = false;
            _isLinkStatistics = false;
            try
            {
                if (lastHostThread != null)
                {
                    lastHostThread.Abort();
                }
                if (lastLinkThread != null)
                {
                    lastLinkThread.Abort();
                }
                if (lastLinkStatisticsThread != null)
                {
                    lastLinkStatisticsThread.Abort();
                }
            }
            catch (Exception)
            {

            }
            ResetProperties(CheckStatus.Busy);
            CheckHost(linkAddress);
            CheckLink(linkAddress);
            CheckStatistics(linkAddress);
        }

        public void ResetProperties(CheckStatus checkStatus)
        {
            HostStatus = checkStatus;
            LinkStatus = checkStatus;
            ResumableStatus = checkStatus;
            LinkSize = -2;
            Country = "نامشخص";
        }

        bool _isStartHost = false;
        bool _isStartLink = false;
        bool _isLinkStatistics = false;
        System.Threading.Thread lastHostThread = null;
        System.Threading.Thread lastLinkThread = null;
        System.Threading.Thread lastLinkStatisticsThread = null;
        void CheckHost(string linkAddress)
        {
            if (_isStartHost)
                return;
            if (!Uri.TryCreate(linkAddress, UriKind.Absolute, out Uri uri))
                return;

            _isStartHost = true;
            System.Threading.Thread currentThread = null;
            lastHostThread = currentThread = AsyncActions.StartNew(() =>
            {
                TcpClient tcpClient = new TcpClient(uri.Host, uri.Port);
                tcpClient.Close();
                if (currentThread.ThreadState == System.Threading.ThreadState.Aborted || currentThread.ThreadState == System.Threading.ThreadState.AbortRequested)
                    return;
                HostStatus = CheckStatus.Success;

                _isStartHost = false;
            }, (ex) =>
            {
                _isStartHost = false;
                if (currentThread.ThreadState == System.Threading.ThreadState.Aborted || currentThread.ThreadState == System.Threading.ThreadState.AbortRequested)
                    return;
                Exception = ex;
                HostStatus = CheckStatus.Error;
            });
        }

        void CheckLink(string linkAddress)
        {
            if (_isStartLink)
                return;
            if (!Uri.TryCreate(linkAddress, UriKind.RelativeOrAbsolute, out Uri uri))
                return;

            _isStartLink = true;
            System.Threading.Thread currentThread = null;
            lastLinkThread = currentThread = AsyncActions.StartNew(() =>
            {
                string _authentication = null;
                System.Net.IWebProxy _currentProxy = null;
                using (var currentWebRequestExchanger = WebRequestExchangerBase.Create(RequestExchangerType.NetFrameworkWebRequest))
                {

                    currentWebRequestExchanger.CreateRequest(linkAddress, _authentication, 60000, _currentProxy, null, int.MaxValue, null);
                    currentWebRequestExchanger.GetResponse();
                    LinkSize = currentWebRequestExchanger.ContentLength;
                    if (currentThread.ThreadState == System.Threading.ThreadState.Aborted || currentThread.ThreadState == System.Threading.ThreadState.AbortRequested)
                        return;
                    LinkStatus = CheckStatus.Success;
                    using (var resumableChecker = new LinkResumableChecker())
                    {
                        if (resumableChecker.CheckAddressContentForSupportResumableJustHeader(linkAddress, _authentication, _currentProxy, null, null))
                            ResumableStatus = CheckStatus.Success;
                        else
                        {
                            if (resumableChecker.CheckAddressContentForSupportResumable(linkAddress, _authentication, _currentProxy, null, null))
                                ResumableStatus = CheckStatus.Success;
                            else
                                ResumableStatus = CheckStatus.Error;
                        }
                    }
                    _isStartLink = false;
                }

            }, (ex) =>
            {
                _isStartLink = false;
                if (currentThread.ThreadState == System.Threading.ThreadState.Aborted || currentThread.ThreadState == System.Threading.ThreadState.AbortRequested)
                    return;
                Exception = ex;
                LinkStatus = CheckStatus.Error;
                ResumableStatus = CheckStatus.Error;
            });
        }

        void CheckStatistics(string linkAddress)
        {
            if (_isLinkStatistics)
                return;
            if (!Uri.TryCreate(linkAddress, UriKind.Absolute, out Uri uri))
                return;
            _isLinkStatistics = true;
            System.Threading.Thread currentThread = null;
            lastLinkStatisticsThread = currentThread = AsyncActions.StartNew(() =>
            {

                var statistics = HostStatisticsHelper.GetHostStatistics(uri.Host);
                if (currentThread.ThreadState == System.Threading.ThreadState.Aborted || currentThread.ThreadState == System.Threading.ThreadState.AbortRequested)
                    return;
                Country = statistics.Country;

                _isLinkStatistics = false;
            }, (ex) =>
            {
                _isLinkStatistics = false;
                if (currentThread.ThreadState == System.Threading.ThreadState.Aborted || currentThread.ThreadState == System.Threading.ThreadState.AbortRequested)
                    return;
                Exception = ex;
            });
        }

        public void Dispose()
        {
            try
            {
                if (lastLinkThread != null)
                    lastLinkThread.Abort();
                if (lastHostThread != null)
                    lastHostThread.Abort();
                if (lastLinkStatisticsThread != null)
                    lastLinkStatisticsThread.Abort();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
