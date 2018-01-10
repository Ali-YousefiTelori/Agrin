using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace Agrin.Helper.ComponentModel
{
    //#if(!MobileApp || !XamarinApp)
    //        [Serializable]
    //    public abstract class ANotifyPropertyChanged : INotifyPropertyChanged
    //    {
    //        static ANotifyPropertyChanged()
    //        {
    //            Initialize();
    //        }
    //        public Action<object, string> ChangedPropertyAction { get; set; }
    //        static ManualResetEvent resetEvent = new ManualResetEvent(false);
    //        static Dictionary<ANotifyPropertyChanged, Dictionary<string, TimeSpan>> notifyItems = new Dictionary<ANotifyPropertyChanged, Dictionary<string, TimeSpan>>();
    //        static void Initialize()
    //        {
    //            Task thread = new Task(() =>
    //            {
    //                while (true)
    //                {
    //                    if (notifyItems.Count == 0)
    //                        resetEvent.WaitOne();
    //                    else
    //                        resetEvent.WaitOne(pmiliSecound);

    //                    resetEvent.Reset();
    //                    foreach (var item in GetAllForChangedItems())
    //                    {
    //                        foreach (var propertyName in item.Value.ToArray())
    //                        {
    //                            item.Key.OnMainPropertyChanged(propertyName);
    //                            lock (lockBJ)
    //                            {
    //                                item.Value.Remove(propertyName);
    //                                if (item.Value.Count == 0)
    //                                    notifyItems.Remove(item.Key);
    //                            }

    //                        }
    //                    }
    //                }
    //            });
    //            thread.Start();
    //        }
    //        static TimeSpan pmiliSecound = new TimeSpan(0, 0, 0, 0, 300);
    //        static Dictionary<ANotifyPropertyChanged, List<string>> GetAllForChangedItems()
    //        {
    //            Dictionary<ANotifyPropertyChanged, List<string>> items = new Dictionary<ANotifyPropertyChanged, List<string>>();
    //            lock (lockBJ)
    //            {
    //                foreach (var item in notifyItems)
    //                {
    //                    foreach (var value in item.Value)
    //                    {
    //                        if (value.Value < DateTime.Now.TimeOfDay - pmiliSecound)
    //                        {
    //                            if (items.ContainsKey(item.Key))
    //                                items[item.Key].Add(value.Key);
    //                            else
    //                                items.Add(item.Key, new List<string>() { value.Key });
    //                        }
    //                    }
    //                }
    //            }
    //            return items;
    //        }

    //        static object lockBJ = new object();
    //        static void AddItem(ANotifyPropertyChanged item, string property)
    //        {
    //            lock (lockBJ)
    //            {
    //                if (notifyItems.ContainsKey(item))
    //                {
    //                    if (!notifyItems[item].ContainsKey(property))
    //                    {
    //                        notifyItems[item].Add(property, DateTime.Now.TimeOfDay);
    //                        resetEvent.Set();
    //                    }
    //                }
    //                else
    //                {
    //                    notifyItems.Add(item, new Dictionary<string, TimeSpan> { { property, DateTime.Now.TimeOfDay } });
    //                    resetEvent.Set();
    //                    //item.PropertyChanged += (o, e) =>
    //                    //{

    //                    //};
    //                }

    //            }
    //        }
    //        bool _canClick = true;

    //        public bool CanClick
    //        {
    //            get { return _canClick; }
    //            set { _canClick = value; OnPropertyChanged("CanClick"); }
    //        }

    //        public void OnPropertyChanged(string propertyName)
    //        {
    //            AsyncActions.Action(() =>
    //                {
    //                    AddItem(this, propertyName);
    //                });
    //        }

    //        void OnMainPropertyChanged(string propertyName)
    //        {
    //            if (PropertyChanged != null)
    //                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //            if (ChangedPropertyAction != null)
    //                ChangedPropertyAction(this, propertyName);
    //        }

    //        [field: NonSerializedAttribute()]
    //        public event PropertyChangedEventHandler PropertyChanged;
    //    }

    //#else
    public abstract class ANotifyPropertyChanged : INotifyPropertyChanged
    {
        static bool isStoped = false;
        static Dictionary<ANotifyPropertyChanged, List<string>> items = new Dictionary<ANotifyPropertyChanged, List<string>>();
        public static void StopNotifyChanging()
        {
            isStoped = true;
        }

        public static void StartNotifyChanging()
        {
            isStoped = false;
            foreach (var notify in items)
            {
                foreach (var pName in notify.Value)
                {
                    notify.Key.OnPropertyChanged(pName);
                }
            }
            items.Clear();
        }


        bool _canClick = true;

        public bool CanClick
        {
            get { return _canClick; }
            set { _canClick = value; OnPropertyChanged("CanClick"); }
        }

        public bool IgnoreStopChanged { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            if (isStoped && !IgnoreStopChanged)
            {
                if (items.ContainsKey(this))
                {
                    if (!items[this].Contains(propertyName))
                        items[this].Add(propertyName);
                }
                else
                    items.Add(this, new List<string>() { propertyName });
            }
            else
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
    }
    //#endif



}