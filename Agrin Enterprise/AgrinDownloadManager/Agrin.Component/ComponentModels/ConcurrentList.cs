using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ComponentModels
{
    /// <summary>
    /// fixed bugs and new features by ali.visual.studio@gmail.com
    /// a thread-safe list with support for:
    /// 1) negative indexes (read from end).  "myList[-1]" gets the last value
    /// 2) modification while enumerating: enumerates a copy of the collection.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ConcurrentList<TValue> : IList<TValue>, IEnumerable
    {
        private object _lock = new object();
        private List<TValue> _storage = new List<TValue>();
        /// <summary>
        /// support for negative indexes (read from end).  "myList[-1]" gets the last value
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue this[int index]
        {
            get
            {
                lock (_lock)
                {
                    if (index < 0)
                    {
                        index = this.Count - index;
                    }
                    return _storage[index];
                }
            }
            set
            {
                lock (_lock)
                {
                    if (index < 0)
                    {
                        index = this.Count - index;
                    }
                    _storage[index] = value;
                }
            }
        }

        /// <summary>
        /// sort
        /// </summary>
        public void Sort()
        {
            lock (_lock)
            {
                _storage.Sort();
            }
        }

        /// <summary>
        /// count
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _storage.Count;
                }
            }
        }

        bool ICollection<TValue>.IsReadOnly
        {
            get
            {
                lock (_lock)
                {
                    return ((IList<TValue>)_storage).IsReadOnly;
                }
            }
        }

        /// <summary>
        /// add item
        /// </summary>
        /// <param name="item"></param>
        public void Add(TValue item)
        {
            lock (_lock)
            {
                _storage.Add(item);
            }
        }
        /// <summary>
        /// add range
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<TValue> items)
        {
            lock (_lock)
            {
                _storage.AddRange(items);
            }
        }

        /// <summary>
        /// clear list
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _storage.Clear();
            }
        }

        /// <summary>
        /// contine
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TValue item)
        {
            lock (_lock)
            {
                return _storage.Contains(item);
            }
        }

        /// <summary>
        /// copy
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            lock (_lock)
            {
                _storage.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// index of
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TValue item)
        {
            lock (_lock)
            {
                return _storage.IndexOf(item);
            }
        }

        /// <summary>
        /// insert
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, TValue item)
        {
            lock (_lock)
            {
                _storage.Insert(index, item);
            }
        }

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TValue item)
        {
            lock (_lock)
            {
                return _storage.Remove(item);
            }
        }

        /// <summary>
        /// remove at
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _storage.RemoveAt(index);
            }
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            lock (_lock)
            {
                return _storage.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
