using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Helper.ComponentModel
{
    /// <summary>
    /// Represents a thread-safe list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentList<T> : IList<T>
    {
        #region Fields

        private List<T> _internalList;

        private readonly object lockObject = new object();

        #endregion

        #region ctor

        public ConcurrentList()
        {
            _internalList = new List<T>();
        }

        public ConcurrentList(int capacity)
        {
            _internalList = new List<T>(capacity);
        }

        public ConcurrentList(IEnumerable<T> list)
        {
            _internalList = list.ToList();
        }

        #endregion

        public T this[int index]
        {
            get
            {
                return LockInternalListAndGet(l => l[index]);
            }
            set
            {
                LockInternalListAndCommand(l => l[index] = value);
            }
        }

        public int Count
        {
            get
            {
                return LockInternalListAndQuery(l => l.Count());
            }
        }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            LockInternalListAndCommand(l => l.Add(item));
        }

        public void AddRange(IEnumerable<T> items)
        {
            LockInternalListAndCommand(l => l.AddRange(items));
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            LockInternalListAndCommand(l => l.InsertRange(index, items));
        }

        public List<T> GetRange(int index, int count)
        {
            return LockInternalListAndQuery(l => l.GetRange(index, count));
        }

        public void Clear()
        {
            LockInternalListAndCommand(l => l.Clear());
        }

        public bool Contains(T item)
        {
            return LockInternalListAndQuery(l => l.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            LockInternalListAndCommand(l => l.CopyTo(array, arrayIndex));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return LockInternalListAndQuery(l => l.GetEnumerator());
        }

        public int IndexOf(T item)
        {
            return LockInternalListAndQuery(l => l.IndexOf(item));
        }

        public void Insert(int index, T item)
        {
            LockInternalListAndCommand(l => l.Insert(index, item));
        }

        public bool Remove(T item)
        {
            return LockInternalListAndQuery(l => l.Remove(item));
        }

        public void RemoveAt(int index)
        {
            LockInternalListAndCommand(l => l.RemoveAt(index));
        }

        public List<T> ToList()
        {
            return LockInternalListAndQuery(l => l.ToList());
        }

        public T[] ToArray()
        {
            return LockInternalListAndQuery(l => l.ToArray());
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return LockInternalListAndQuery(l => l.GetEnumerator());
        }

        #region Utilities

        protected virtual void LockInternalListAndCommand(Action<List<T>> action)
        {
            lock (lockObject)
            {
                action(_internalList);
            }
        }

        protected virtual T LockInternalListAndGet(Func<List<T>, T> func)
        {
            lock (lockObject)
            {
                return func(_internalList);
            }
        }

        protected virtual TObject LockInternalListAndQuery<TObject>(Func<List<T>, TObject> query)
        {
            lock (lockObject)
            {
                return query(_internalList);
            }
        }


        #endregion
    }
}
