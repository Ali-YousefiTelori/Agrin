//using Agrin.Helper.ComponentModel;
//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;

//namespace Agrin.Helper.Collections
//{
//    public class FastCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
//    {
//        public FastCollection(object dispatcherThread)
//        {
//            _dispatcherThread = dispatcherThread;
//        }

//        public void SetDispatcherThread(object dispatcherThread)
//        {
//            _dispatcherThread = dispatcherThread;
//        }

//        object _dispatcherThread;
//        object _syncRoot;
//        public T this[int index]
//        {
//            get
//            {
//                if (index >= _bindList.Count)
//                {
//                    return default(T);
//                }
//                return _bindList[index];
//            }
//            set
//            {
//                OnCollectionChanged();
//                _bindList[index] = value;
//            }
//        }

//        List<T> _bindList = new List<T>();
//        readonly List<T> _changedList = new List<T>();
//        object changedItem;
//        object lockobj = new object();

//        public void ActionDispatcher(Action action)
//        {
//#if (DEBUG)
//            if (_dispatcherThread == null)
//                action();
//            else
//                ApplicationHelperMono.EnterDispatcherThreadActionForCollections(action);
//#else
//            ApplicationHelperMono.EnterDispatcherThreadActionForCollections(action);
//#endif
//        }
//        public void Add(T item)
//        {
//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    _bindList.Add(item);
//                    changedItem = item;
//                    OnCollectionChanged(NotifyCollectionChangedAction.Add);
//                });
//            }
//        }

//        public void SortBy<TK>(Func<T, TK> pro)
//        {
//            lock (lockobj)
//            {
//                _bindList = _bindList.OrderByDescending(pro).ToList();
//                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
//            }
//        }

//        public void SortByAscending<TK>(Func<T, TK> pro)
//        {
//            lock (lockobj)
//            {
//                _bindList = _bindList.OrderBy(pro).ToList();
//                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
//            }
//        }

//        public void AddNotChanged(T item)
//        {
//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    _bindList.Add(item);
//                    _changedList.Add(item);
//                });
//            }
//        }

//        public void AddRange(IEnumerable<T> items)
//        {
//            if (items == null || items.Count() == 0)
//                return;
//            if (items.Count() == 1)
//            {
//                Add(items.First());
//                return;
//            }

//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    _bindList.AddRange(items);
//                    //_changedList.AddRange(items);
//                    //OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
//                    OnCollectionChanged();
//                });
//                //OnCollectionChanged(NotifyCollectionChangedAction.Reset);
//            }
//        }

//        public void InsertRange(int index, IEnumerable<T> items)
//        {
//            if (items == null || items.Count() == 0)
//                return;
//            if (items.Count() == 1)
//            {
//                Insert(index, items.First());
//                return;
//            }

//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    _bindList.InsertRange(index, items);
//                    OnCollectionChanged();
//                });
//            }
//        }
//        public void InsertRangeNotChanged(int index, IEnumerable<T> items)
//        {
//            if (items == null || items.Count() == 0)
//                return;
//            if (items.Count() == 1)
//            {
//                InsertNotChanged(index, items.First());
//                return;
//            }

//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    _bindList.InsertRange(index, items);
//                });
//            }
//        }

//        //public void AddRange(Hashtable items)
//        //{
//        //    foreach (T item in items.Values)
//        //    {
//        //        _bindList.Add(item);
//        //    }
//        //    OnCollectionChanged(NotifyCollectionChangedAction.Add);
//        //    //OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs());
//        //}

//        public List<T> GetRange(int index, int count)
//        {
//            return _bindList.GetRange(index, count);
//        }

//        public List<T> GetRange(int index)
//        {
//            return _bindList.GetRange(index, _bindList.Count - index);
//        }

//        public void Clear()
//        {
//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    _bindList.Clear();
//                    OnCollectionChanged();
//                });
//            }
//        }

//        public bool Contains(T item)
//        {
//            return _bindList.Contains(item);
//        }

//        public void CopyTo(T[] array, int arrayIndex)
//        {
//            _bindList.CopyTo(array, arrayIndex);
//        }

//        public int Count
//        {
//            get
//            {
//                return _bindList.Count;
//            }
//        }

//        public bool IsReadOnly
//        {
//            get
//            {
//                return true;
//            }
//        }

//        public bool Remove(T item)
//        {
//            lock (lockobj)
//            {
//                bool remove = false;
//                ActionDispatcher(() =>
//                {
//                    changedItem = item;
//                    remove = _bindList.Remove(item);
//                    OnCollectionChanged(NotifyCollectionChangedAction.Reset);
//                });
//                return remove;
//            }
//        }

//        public bool RemoveNotChanged(T item)
//        {
//            lock (lockobj)
//            {
//                bool remove = false;
//                ActionDispatcher(() =>
//                {
//                    changedItem = item;
//                    remove = _bindList.Remove(item);
//                });
//                return remove;
//            }
//        }

//        public void RemoveRangeNotChanged(IEnumerable<T> items)
//        {
//            if (items.Count() == 0)
//                return;
//            if (items.Count() == 1)
//            {
//                RemoveNotChanged(items.First());
//                return;
//            }
//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    foreach (var item in items.ToList())
//                    {
//                        _bindList.Remove(item);
//                    }
//                });
//            }
//        }

//        public void RemoveRange(IEnumerable<T> items)
//        {
//            if (items.Count() == 0)
//                return;
//            if (items.Count() == 1)
//            {
//                Remove(items.First());
//                return;
//            }
//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    foreach (var item in items.ToList())
//                    {
//                        _bindList.Remove(item);
//                    }
//                    OnCollectionChanged();
//                });
//            }
//        }

//        public void Remove(object value)
//        {
//            if (value is Int32)
//                RemoveAt((int)value);
//            else
//                Remove((T)value);
//        }

//        public void RemoveAt(int index)
//        {
//            Remove(_bindList[index]);
//        }

//        public bool IsSynchronized
//        {
//            get { return false; }
//        }

//        public object SyncRoot
//        {
//            get
//            {
//                if (this._syncRoot == null)
//                {
//                    System.Threading.Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
//                }
//                return this._syncRoot;
//            }
//        }

//        public int IndexOf(T item)
//        {
//            return _bindList.IndexOf(item);
//        }

//        public void Insert(int index, T item)
//        {
//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    changedItem = item;
//                    _bindList.Insert(index, item);
//                    OnCollectionChanged(NotifyCollectionChangedAction.Add);
//                });
//            }
//        }

//        public void InsertNotChanged(int index, T item)
//        {
//            lock (lockobj)
//            {
//                ActionDispatcher(() =>
//                {
//                    changedItem = item;
//                    _bindList.Insert(index, item);
//                });
//            }
//        }

//        public List<T> ToList()
//        {
//            lock (lockobj)
//            {
//                return _bindList.ToList();
//            }
//        }

//        public int Add(object value)
//        {
//            Add((T)value);
//            return _bindList.Count - 1;
//        }

//        public bool Contains(object value)
//        {
//            if (value == null)
//            {
//                return false;
//            }
//            return _bindList.Contains((T)value);
//        }

//        public int IndexOf(object value)
//        {
//            if (value == null || !(value is T))
//            {
//                if (value is List<T> && _bindList.Count > 0)
//                    return 0;
//                return -1;
//            }
//            return _bindList.IndexOf((T)value);
//        }

//        public void Insert(int index, object value)
//        {
//            Insert(index, (T)value);
//        }

//        public bool IsFixedSize
//        {
//            get { return false; }
//        }

//        object IList.this[int index]
//        {
//            get
//            {
//                return this[index];
//            }
//            set
//            {

//                this[index] = (T)value;
//            }
//        }

//        public void CopyTo(Array array, int arrayIndex)
//        {
//            if ((array != null) && (array.Rank != 1))
//            {
//                //ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
//            }
//            try
//            {
//                Array.Copy(this._bindList.ToArray(), 0, array, arrayIndex, this.Count);
//            }
//            catch (ArrayTypeMismatchException)
//            {
//                //ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
//            }
//        }

//        public IEnumerator<T> GetEnumerator()
//        {
//            return _bindList.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return _bindList.GetEnumerator();
//        }

//        public void OnCollectionChanged(NotifyCollectionChangedAction mode = NotifyCollectionChangedAction.Reset)
//        {
//            if (CollectionChanged != null)
//            {
//                if (mode == NotifyCollectionChangedAction.Reset)
//                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
//                else
//                {
//                    if (_changedList.Count > 0)
//                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(mode, _changedList));
//                    else
//                    {
//                        int index = _bindList.IndexOf((T)changedItem);

//                        if (index > -1)
//                        {
//                            CollectionChanged(this, new NotifyCollectionChangedEventArgs(mode, changedItem, index));
//                        }
//                    }
//                }
//            }
//            if (ChangedCollection != null)
//                ChangedCollection();
//            OnPropertyChanged("Count");
//            _changedList.Clear();
//        }

//        //protected virtual void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
//        //{
//        //    NotifyCollectionChangedEventHandler handlers = this.CollectionChanged;
//        //    if (handlers != null)
//        //    {
//        //        foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
//        //        {
//        //            //if (handler.Target is System.Windows.Data.CollectionView)
//        //            //    ((System.Windows.Data.CollectionView)handler.Target).Refresh();
//        //            //else
//        //            handler(this, e);
//        //        }
//        //    }

//        //    if (ChangedCollection != null)
//        //        ChangedCollection();
//        //    _changedList.Clear();
//        //}

//        public void OnPropertyChanged(string name)
//        {
//            if (PropertyChanged != null)
//                PropertyChanged(this, new PropertyChangedEventArgs(name));
//        }

//        public Action ChangedCollection;
//        public event NotifyCollectionChangedEventHandler CollectionChanged;
//        public event PropertyChangedEventHandler PropertyChanged;

//    }
//}
using Agrin.Helper.ComponentModel;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Agrin.Helper.Collections
{
    public class FastCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public FastCollection(object dispatcherThread)
        {
            _dispatcherThread = dispatcherThread;
        }

        public void SetDispatcherThread(object dispatcherThread)
        {
            _dispatcherThread = dispatcherThread;
        }

        object _dispatcherThread;
        object _syncRoot;
        public T this[int index]
        {
            get
            {
                if (index >= _bindList.Count)
                {
                    return default(T);
                }
                return _bindList[index];
            }
            set
            {
                OnCollectionChanged();
                _bindList[index] = value;
            }
        }

        List<T> _bindList = new List<T>();
        readonly List<T> _changedList = new List<T>();
        object changedItem;
        object lockobj = new object();

        public void ActionDispatcher(Action action)
        {
//#if (DEBUG)
            if (_dispatcherThread == null)
                action();
            else
                ApplicationHelperMono.EnterDispatcherThreadActionForCollectionsByDispatcher(action, _dispatcherThread);
//#else
//            ApplicationHelperMono.EnterDispatcherThreadActionForCollections(action);
//#endif
        }
        public void Add(T item)
        {
            lock (lockobj)
            {
                _bindList.Add(item);
                changedItem = item;
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
            }
        }

        public void SortBy<TK>(Func<T, TK> pro)
        {
            lock (lockobj)
            {
                _bindList = _bindList.OrderByDescending(pro).ToList();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
        }

        public void SortByAscending<TK>(Func<T, TK> pro)
        {
            lock (lockobj)
            {
                _bindList = _bindList.OrderBy(pro).ToList();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
        }

        public void AddNotChanged(T item)
        {
            lock (lockobj)
            {
                _bindList.Add(item);
                _changedList.Add(item);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0)
                return;
            if (items.Count() == 1)
            {
                Add(items.First());
                return;
            }

            lock (lockobj)
            {
                _bindList.AddRange(items);
                //_changedList.AddRange(items);
                //OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
                OnCollectionChanged();
                //OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0)
                return;
            if (items.Count() == 1)
            {
                Insert(index, items.First());
                return;
            }

            lock (lockobj)
            {
                _bindList.InsertRange(index, items);
                OnCollectionChanged();
            }
        }
        public void InsertRangeNotChanged(int index, IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0)
                return;
            if (items.Count() == 1)
            {
                InsertNotChanged(index, items.First());
                return;
            }

            lock (lockobj)
            {
                _bindList.InsertRange(index, items);
            }
        }

        //public void AddRange(Hashtable items)
        //{
        //    foreach (T item in items.Values)
        //    {
        //        _bindList.Add(item);
        //    }
        //    OnCollectionChanged(NotifyCollectionChangedAction.Add);
        //    //OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs());
        //}

        public List<T> GetRange(int index, int count)
        {
            return _bindList.GetRange(index, count);
        }

        public List<T> GetRange(int index)
        {
            return _bindList.GetRange(index, _bindList.Count - index);
        }

        public void Clear()
        {
            lock (lockobj)
            {
                _bindList.Clear();
                OnCollectionChanged();
            }
        }

        public bool Contains(T item)
        {
            return _bindList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _bindList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _bindList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool Remove(T item)
        {
            lock (lockobj)
            {
                bool remove = false;
                changedItem = item;
                remove = _bindList.Remove(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
                return remove;
            }
        }

        public bool RemoveNotChanged(T item)
        {
            lock (lockobj)
            {
                bool remove = false;
                changedItem = item;
                remove = _bindList.Remove(item);
                return remove;
            }
        }

        public void RemoveRangeNotChanged(IEnumerable<T> items)
        {
            if (items.Count() == 0)
                return;
            if (items.Count() == 1)
            {
                RemoveNotChanged(items.First());
                return;
            }
            lock (lockobj)
            {
                foreach (var item in items.ToList())
                {
                    _bindList.Remove(item);
                }
            }
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            if (items.Count() == 0)
                return;
            if (items.Count() == 1)
            {
                Remove(items.First());
                return;
            }
            lock (lockobj)
            {
                foreach (var item in items.ToList())
                {
                    _bindList.Remove(item);
                }
                OnCollectionChanged();
            }
        }

        public void Remove(object value)
        {
            if (value is Int32)
                RemoveAt((int)value);
            else
                Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            Remove(_bindList[index]);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }

        public int IndexOf(T item)
        {
            return _bindList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (lockobj)
            {
                changedItem = item;
                _bindList.Insert(index, item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
            }
        }

        public void InsertNotChanged(int index, T item)
        {
            lock (lockobj)
            {
                changedItem = item;
                _bindList.Insert(index, item);
            }
        }

        public List<T> ToList()
        {
            lock (lockobj)
            {
                return _bindList.ToList();
            }
        }

        public int Add(object value)
        {
            Add((T)value);
            return _bindList.Count - 1;
        }

        public bool Contains(object value)
        {
            if (value == null)
            {
                return false;
            }
            return _bindList.Contains((T)value);
        }

        public int IndexOf(object value)
        {
            if (value == null || !(value is T))
            {
                if (value is List<T> && _bindList.Count > 0)
                    return 0;
                return -1;
            }
            return _bindList.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {

                this[index] = (T)value;
            }
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            if ((array != null) && (array.Rank != 1))
            {
                //ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            try
            {
                Array.Copy(this._bindList.ToArray(), 0, array, arrayIndex, this.Count);
            }
            catch (ArrayTypeMismatchException)
            {
                //ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _bindList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bindList.GetEnumerator();
        }

        public void OnCollectionChanged(NotifyCollectionChangedAction mode = NotifyCollectionChangedAction.Reset)
        {
            ActionDispatcher(() =>
            {
                if (CollectionChanged != null)
                {
                    if (mode == NotifyCollectionChangedAction.Reset)
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    else
                    {
                        if (_changedList.Count > 0)
                            CollectionChanged(this, new NotifyCollectionChangedEventArgs(mode, _changedList));
                        else
                        {
                            int index = _bindList.IndexOf((T)changedItem);

                            if (index > -1)
                            {
                                CollectionChanged(this, new NotifyCollectionChangedEventArgs(mode, changedItem, index));
                            }
                        }
                    }
                }
                if (ChangedCollection != null)
                    ChangedCollection();
                OnPropertyChanged("Count");
                _changedList.Clear();
            });
        }

        //protected virtual void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        //{
        //    NotifyCollectionChangedEventHandler handlers = this.CollectionChanged;
        //    if (handlers != null)
        //    {
        //        foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
        //        {
        //            //if (handler.Target is System.Windows.Data.CollectionView)
        //            //    ((System.Windows.Data.CollectionView)handler.Target).Refresh();
        //            //else
        //            handler(this, e);
        //        }
        //    }

        //    if (ChangedCollection != null)
        //        ChangedCollection();
        //    _changedList.Clear();
        //}

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public Action ChangedCollection;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
