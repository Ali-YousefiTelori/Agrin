using Agrin.Download.Web;
using System;
using System.IO;
using System.Net;

namespace Agrin.Download.Engine.Interfaces
{
    public interface IConnection : IDisposable
    {
        int ConnectionId { get; set; }
        ConnectionState State { get; set; }
        Uri UriDownload { get; set; }
        //Stopwatch ReConnectTimer { get; set; }
        CookieContainer RequestCookieContainer { get; set; }

        long DownloadedSize { get; set; }
        int BufferRead { get; set; }
        string SaveFileName { get; set; }

        LinkInfo Parent { get; set; }

        void CreateRequestData();
        void Connect();
        void Play();
        void DownloadData();
        void Pause();
        void Complete();
        void Stop();
        //bool IsDispose { get; set; }
    }
}
