using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Helper.ComponentModel
{
    public interface IApplicationHelper
    {
        Action RefreshCommandAction { get; set; }
        Action RunOnUIThread { get; set; }
        Dictionary<Thread, object> Dispatchers { get; set; }
        Action CloseApplication { get; set; }
        void AddThreadDispather(object dispatcher);
        object GetDispatcherByThread(Thread thread);
        void EnterDispatcherThreadActionBegin(Action action);
        void EnterDispatcherThreadAction(Action action, object dispatcher = null);

        object DispatcherThread { get; set; }

        void EnterDispatcherThreadActionForCollections(Action action, object dispatcher = null);
        void EnterDispatcherThreadActionBegin(Action action, object dispatcher = null);
        string GetAppResource(object key, bool nullable = false);

        object GetAppResourceObject(object key);
        T GetAppResourceStyle<T>(object key) where T : class;

        T GetAppResource<T>(object key) where T : class;
        void CollectMemory();
        void SetShutDown(int state);
    }
}
