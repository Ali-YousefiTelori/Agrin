using Agrin.Download.Web.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web.Connections
{
    public class PageConnectionInfo : AConnectionInfo
    {
        public PageConnectionInfo(string address, LinkWebRequest parentLinkWebRequest)
        {
            ParentLinkWebRequest = parentLinkWebRequest;
            ParentLinkWebRequest.UriDownload = new Uri(address);
        }
        public PageConnectionInfo(Uri address, LinkWebRequest parentLinkWebRequest)
        {
            ParentLinkWebRequest = parentLinkWebRequest;
            ParentLinkWebRequest.UriDownload = address;
        }

        public override void DownloadData()
        {
            if (_responseStream == null)
                _responseStream = _response.GetResponseStream();
            ParentLinkWebRequest.BufferRead = ParentLinkWebRequest.Parent.Management.ReadBuffer;
            SetState(ConnectionState.Downloading);
            byte[] _read = new byte[ParentLinkWebRequest.BufferRead];
            int readCount = _responseStream.Read(_read, 0, _read.Length);
            do
            {
                _saveStream.Write(_read, 0, readCount);
                ParentLinkWebRequest.DownloadedSize = _saveStream.Position;
                if (isDispose || ParentLinkWebRequest.IsPauseCheck())
                    break;
                else if (ParentLinkWebRequest.State != ConnectionState.Downloading)
                    break;
                if (ParentLinkWebRequest.BufferRead != _read.Length)
                    _read = new byte[ParentLinkWebRequest.BufferRead];
                readCount = _responseStream.Read(_read, 0, ParentLinkWebRequest.BufferRead);
                if (readCount <= 0)
                {
                    _saveStream.Write(_read, 0, readCount);
                    ParentLinkWebRequest.Complete();
                    break;
                }
                //if (!ThreadInfo.LinkInfo.Management.IsLimit)
                ParentLinkWebRequest.BufferRead = ParentLinkWebRequest.Parent.Management.ReadBuffer;
                //else
                //{
                //    //Buffer = (_threadInfo.LinkInfo.Management.LimitSizePerTime / 10) + _threadInfo.LinkInfo.Management.LimitSizePerTime % 10;
                //    Thread.Sleep(100);
                //}

            } while (true);
        }

        bool canAddThisError(Exception e)
        {
            if (e.Message.Contains("Unable to read data from the transport connection: A blocking operation was interrupted by a call to WSACancelBlockingCall.") || e.Message.Contains("The request was aborted: The request was canceled."))
                return false;
            return true;
        }
    }
}
