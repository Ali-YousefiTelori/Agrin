using Agrin.Download.Web;
using System;

namespace Agrin.Download.Engine.Interfaces
{
    public interface IPlayConnection : IDisposable
    {
        LinkInfo Parent { get; set; }

        void CreateRequestData();
        void Connect();
        void Play();
        void DownloadData();
        void Pause();
        void Complete();
        void Stop();
    }
}
