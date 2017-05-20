using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Engine.Interfaces
{
    public interface IPlayInfo : IDisposable
    {
        //string Name { get; set; }

        //event Interfaces.EventArgs CompletedGetDataEvent;
        bool CanStop { get; }
        bool CanPlay { get; }
        Func<IPlayInfo, Action> FinishAction { get; set; }
        Action DisposeAction { get; set; }
        //IQueueInfo QueueInfoParent { get; set; }
        //void Play();
        void Pause();
        //void Stop();
        void Complete();
    }
}
