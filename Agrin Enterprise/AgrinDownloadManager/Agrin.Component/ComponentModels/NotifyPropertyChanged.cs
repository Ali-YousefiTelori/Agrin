using Agrin.Interfaces;
using Agrin.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Agrin.ComponentModels
{
    /// <summary>
    /// notify property changed for bindings
    /// </summary>
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged, IObjectDisposable
    {
        static NotifyPropertyChanged()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    bool foundOne = false;
                    try
                    {
                        foreach (KeyValuePair<NotifyPropertyChanged, ConcurrentQueue<string>> notify in ChangedItems)
                        {
                            //break
                            int count = notify.Value.Count;
                            int index = 0;
                            while (notify.Value.TryDequeue(out string name) && index < count)
                            {
                                foundOne = true;
                                notify.Key.RunPropertyChanged(name);
                                index++;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, "NotifyPropertyChanged static thread :");
                    }
                    if (foundOne)
                        await Task.Delay(200);
                    else
                        await Task.Delay(700);
                }
            });
        }

        private bool _IsDispose = false;
        /// <summary>
        /// if object is disposed
        /// </summary>
        public bool IsDispose
        {
            get
            {
                return _IsDispose;
            }

            set
            {
                _IsDispose = value;
            }
        }

        private static ConcurrentDictionary<NotifyPropertyChanged, ConcurrentQueue<string>> ChangedItems = new ConcurrentDictionary<NotifyPropertyChanged, ConcurrentQueue<string>>();

        /// <summary>
        /// propertychanged action called when property is changed
        /// </summary>
        public Action<string> OnPropertyChangedAction { get; set; }

        /// <summary>
        /// when you want to rise to UI property is changed 
        /// </summary>
        /// <param name="propertyName">property name</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (ChangedItems.TryGetValue(this, out ConcurrentQueue<string> queue))
            {
                if (!queue.Contains(propertyName))
                    queue.Enqueue(propertyName);
            }
            else
            {
                queue = new ConcurrentQueue<string>();
                ChangedItems.TryAdd(this, queue);
                queue.Enqueue(propertyName);
            }
            //RunPropertyChanged(propertyName);
        }

        private void RunPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            OnPropertyChangedAction?.Invoke(propertyName);
        }



        public virtual void Dispose()
        {
            IsDispose = true;
        }

        /// <summary>
        /// property changed Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
