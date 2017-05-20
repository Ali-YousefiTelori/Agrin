using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Agrin.Helper.Collections
{
    public class NormalCollection<T> : ObservableCollection<T>
    {
        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection)
        {

            if (collection == null) throw new ArgumentNullException("collection");

            foreach (var i in collection)
            {
                Items.Add(i);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
        /// </summary> 
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (var i in collection)
                Items.Remove(i);
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, collection.ToList()));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified item. 
        /// </summary> 
        public void Replace(T item)
        {
            ReplaceRange(new T[] { item });
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection. 
        /// </summary> 
        public void ReplaceRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            Items.Clear();
            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
        /// </summary> 
        public NormalCollection(object dispatcherThread)
            : base()
        {
            _dispatcherThread = dispatcherThread;
        }


        public List<T> ToList()
        {
            IEnumerable<T> obsCollection = (IEnumerable<T>)this;
            var list = new List<T>(obsCollection);
            return list;
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public Action ChangedCollection;
        object _dispatcherThread;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            
            ApplicationHelperMono.EnterDispatcherThreadAction(new Action(() =>
            {
                try
                {
                    base.OnCollectionChanged(e);
                }
                catch
                {
                    base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
                if (ChangedCollection != null)
                    ChangedCollection();
            }));
        }

        public void Reset()
        {

            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        }
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        public void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(name));
        }
    }
    //[Serializable]
    //public class NormalCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    //{
    //    public NormalCollection(object dispatcherThread)
    //    {
    //        _dispatcherThread = dispatcherThread;
    //    }

    //    public void SetDispatcherThread(object dispatcherThread)
    //    {
    //        _dispatcherThread = dispatcherThread;
    //    }

    //    //Type castData = null;
    //    //public void FastCollectionLimitEnumerable(Type _castData)
    //    //{
    //    //    castData = _castData;
    //    //}
    //    //public FastCollection()
    //    //{
    //    //    _dispatcherThread = StaticDispatcherThread;
    //    //}
    //    [NonSerialized]
    //    object _dispatcherThread;
    //    //private static Dispatcher _StaticDispatcherThread;

    //    //public static Dispatcher StaticDispatcherThread
    //    //{
    //    //    get { return FastCollection<T>._StaticDispatcherThread; }
    //    //    set { FastCollection<T>._StaticDispatcherThread = value; }
    //    //}

    //    object _syncRoot;
    //    public T this[int index]
    //    {
    //        get
    //        {
    //            return _bindList[index];
    //        }
    //        set
    //        {
    //            OnCollectionChanged();
    //            _bindList[index] = value;
    //        }
    //    }

    //    //ChangedDataRange<T> _changedRange = new ChangedDataRange<T>();

    //    //internal ChangedDataRange<T> ChangedRange
    //    //{
    //    //    get { return _changedRange; }
    //    //    set { _changedRange = value; }
    //    //}

    //    //void Collactions_DelegateMain()
    //    //{
    //    //    try
    //    //    {
    //    //        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //    //    }
    //    //    catch
    //    //    {
    //    //    }
    //    //}

    //    readonly List<T> _bindList = new List<T>();
    //    readonly List<T> _changedList = new List<T>();
    //    //readonly Hashtable _lst = new Hashtable();
    //    //readonly Hashtable _lstAX = new Hashtable();

    //    public void Add(T item)
    //    {
    //        _bindList.Add(item);
    //        _changedList.Add(item);
    //        //_lst.Add(_lst.Count, item);
    //        //_lstAX.Add(item, _lstAX.Count);
    //        OnCollectionChanged(NotifyCollectionChangedAction.Add);
    //    }

    //    public void AddNotChanged(T item)
    //    {
    //        _bindList.Add(item);
    //        _changedList.Add(item);
    //        //_lst.Add(_lst.Count, item);
    //        //_lstAX.Add(item, _lstAX.Count);
    //    }

    //    public void AddRange(IEnumerable<T> items)
    //    {
    //        if (items == null || items.Count() == 0) return;

    //        //foreach (var item in items)
    //        //{
    //        //_lst.Add(_lst.Count, item);
    //        //_lstAX.Add(item, _lstAX.Count);
    //        _bindList.AddRange(items);
    //        _changedList.AddRange(items);
    //        //}
    //        //ChangedRange.StartRange = bindList.Count - items.Count<T>();
    //        //ChangedRange.EndRange = bindList.Count;
    //        OnCollectionChanged(NotifyCollectionChangedAction.Add);
    //        //OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList()));
    //    }

    //    public void AddRange(Hashtable items)
    //    {
    //        foreach (T item in items.Values)
    //        {
    //            //_lst.Add(_lst.Count, item);
    //            //_lstAX.Add(item, _lstAX.Count);
    //            _bindList.Add(item);
    //            _changedList.Add(item);
    //        }
    //        //ChangedRange.StartRange = bindList.Count - items.Count;
    //        //ChangedRange.EndRange = bindList.Count;
    //        //OnCollectionChanged(NotifyCollectionChangedAction.Add);
    //        OnCollectionChanged(NotifyCollectionChangedAction.Add);
    //    }

    //    public List<T> GetRange(int index, int count)
    //    {
    //        return _bindList.GetRange(index, count);
    //    }

    //    public List<T> GetRange(int index)
    //    {
    //        return _bindList.GetRange(index, _bindList.Count - index);
    //    }

    //    public void Clear()
    //    {
    //        _bindList.Clear();
    //        //_lst.Clear();
    //        //_lstAX.Clear();
    //        OnCollectionChanged();
    //    }

    //    public bool Contains(T item)
    //    {
    //        return _bindList.Contains(item);
    //    }

    //    public void CopyTo(T[] array, int arrayIndex)
    //    {
    //        _bindList.CopyTo(array, arrayIndex);
    //    }

    //    public int Count
    //    {
    //        get
    //        {
    //            return _bindList.Count;
    //        }
    //    }

    //    public bool IsReadOnly
    //    {
    //        get
    //        {
    //            return true;
    //        }
    //    }

    //    public bool Remove(T item)
    //    {
    //        int index = IndexOf(item);
    //        if (index > -1 && _bindList.Count == 1)
    //        {
    //            Clear();
    //            return true;
    //        }
    //        //ChangedRange.StartRange = index;
    //        //ChangedRange.EndRange = index + 1;
    //        _changedList.Add(item);
    //        bool ret = _bindList.Remove(item);
    //        OnCollectionChanged(NotifyCollectionChangedAction.Remove);
    //        if (ret)
    //        {
    //            //SetNewKeys();
    //            OnPropertyChanged("Count");
    //        }
    //        return ret;
    //    }

    //    public void RemoveRange(IEnumerable<T> items)
    //    {
    //        if (items.Count() == 0)
    //            return;
    //        _changedList.Clear();
    //        //_bindList.Clear();
    //        //int id = 0;
    //        foreach (var item in items.ToList())
    //        {
    //            _bindList.Remove(item);
    //        }
    //        //foreach (var item in items)
    //        //{
    //        //    if (_lstAX.ContainsKey(item))
    //        //    {
    //        //        id = (int)_lstAX[item];
    //        //        _lst.Remove(id);
    //        //        _lstAX.Remove(item);
    //        //    }
    //        //}
    //        //_bindList.AddRange(_lstAX.Keys.Cast<T>());
    //        SetNewKeys();
    //        OnPropertyChanged("Count");
    //        OnCollectionChanged();
    //    }

    //    public void Remove(object value)
    //    {
    //        if (value is Int32)
    //            RemoveAt((int)value);
    //        else
    //            Remove((T)value);
    //        //SetNewKeys();
    //        //OnCollectionChanged();
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        _bindList.RemoveAt(index);
    //        SetNewKeys();
    //        //OnCollectionChanged();
    //    }

    //    void SetNewKeys()
    //    {
    //        //_lst.Clear();
    //        //_lstAX.Clear();
    //        //int i = 0;
    //        //foreach (var item in _bindList)
    //        //{
    //        //    _lst.Add(i, item);
    //        //    _lstAX.Add(item, i);
    //        //    i++;
    //        //}
    //    }

    //    public bool IsSynchronized
    //    {
    //        get { return false; }
    //    }

    //    public object SyncRoot
    //    {
    //        get
    //        {
    //            if (this._syncRoot == null)
    //            {
    //                System.Threading.Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
    //            }
    //            return this._syncRoot;
    //        }
    //    }

    //    public int IndexOf(T item)
    //    {
    //        return _bindList.IndexOf(item);
    //        //if (item == null || !_lstAX.ContainsKey(item))
    //        //    return -1;
    //        //return (int)_lstAX[item];
    //    }

    //    public void Insert(int index, T item)
    //    {
    //        _bindList.Insert(index, item);
    //        SetNewKeys();
    //        OnPropertyChanged("Count");
    //        //ChangedRange.StartRange = index;
    //        //ChangedRange.EndRange = 1;
    //        OnCollectionChanged(NotifyCollectionChangedAction.Add);
    //    }

    //    public List<T> ToList()
    //    {
    //        return _bindList.ToList();
    //    }

    //    public int Add(object value)
    //    {
    //        Add((T)value);
    //        return _bindList.Count - 1; //lst.Count - 1;
    //    }

    //    public bool Contains(object value)
    //    {
    //        if (value == null)
    //        {
    //            return false;
    //        }
    //        return _bindList.Contains((T)value);
    //    }

    //    public int IndexOf(object value)
    //    {
    //        if (value == null)
    //            return -1;
    //        return _bindList.IndexOf((T)value);
    //        //if (_lstAX.ContainsKey(value))
    //        //{
    //        //    int i = (int)_lstAX[(T)value];
    //        //    return i;
    //        //}
    //        //return -1;
    //    }

    //    public void Insert(int index, object value)
    //    {
    //        _bindList.Insert(index, (T)value);
    //        _changedList.Add((T)value);
    //        SetNewKeys();
    //        OnPropertyChanged("Count");
    //        OnCollectionChanged(NotifyCollectionChangedAction.Add);
    //    }

    //    public bool IsFixedSize
    //    {
    //        get { return false; }
    //    }

    //    object IList.this[int index]
    //    {
    //        get
    //        {
    //            return this[index];
    //        }
    //        set
    //        {

    //            this[index] = (T)value;
    //        }
    //    }

    //    public void CopyTo(Array array, int arrayIndex)
    //    {
    //        if ((array != null) && (array.Rank != 1))
    //        {
    //            //ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
    //        }
    //        try
    //        {
    //            Array.Copy(this._bindList.ToArray(), 0, array, arrayIndex, this.Count);
    //        }
    //        catch (ArrayTypeMismatchException)
    //        {
    //            //ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
    //        }
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return _bindList.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return _bindList.GetEnumerator();//lst.GetEnumerator();
    //    }

    //    //void CollectionChangedValue(T item, NotifyCollectionChangedAction mode)
    //    //{
    //    //    if (CollectionChanged != null)
    //    //        CollectionChanged(this, new NotifyCollectionChangedEventArgs(mode, item));
    //    //}

    //    public void OnCollectionChanged(NotifyCollectionChangedAction mode = NotifyCollectionChangedAction.Reset)
    //    {
    //        if (CollectionChanged != null)
    //        {
    //            ApplicationHelperMono.EnterDispatcherThreadAction(new Action(() =>
    //            {
    //                try
    //                {
    //                    if (_changedList.Count == 0 || mode == NotifyCollectionChangedAction.Reset)
    //                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //                    else
    //                    {
    //                        //List<T> list = bindList.GetRange(ChangedRange.StartRange, ChangedRange.EndRange - ChangedRange.StartRange);
    //                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(mode, _changedList));
    //                    }
    //                }
    //                catch (Exception e)
    //                {
    //                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //                }

    //                OnPropertyChanged("Count");
    //            }));
    //        }
    //        if (ChangedCollection != null)
    //            ChangedCollection();
    //        _changedList.Clear();
    //    }

    //    //protected virtual void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
    //    //{
    //    //    NotifyCollectionChangedEventHandler handlers = this.CollectionChanged;
    //    //    if (handlers != null)
    //    //    {
    //    //        foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
    //    //        {
    //    //            if (handler.Target is System.Windows.Data.CollectionView)
    //    //                ((System.Windows.Data.CollectionView)handler.Target).Refresh();
    //    //            else
    //    //                handler(this, e);
    //    //        }
    //    //    }
    //    //    _changedList.Clear();
    //    //    OnPropertyChanged("Count");
    //    //    if (ChangedCollection != null)
    //    //        ChangedCollection();
    //    //}

    //    public void OnPropertyChanged(string name)
    //    {
    //        if (PropertyChanged != null)
    //            PropertyChanged(this, new PropertyChangedEventArgs(name));
    //    }

    //    [NonSerialized]
    //    public Action ChangedCollection;
    //    [field: NonSerialized]
    //    public event NotifyCollectionChangedEventHandler CollectionChanged;
    //    [field: NonSerialized]
    //    public event PropertyChangedEventHandler PropertyChanged;

    //}
}
