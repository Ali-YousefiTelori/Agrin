using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models
{
    public class ConcurrentCircularBuffer<T>
    {
        private readonly LinkedList<T> _buffer;
        private int _maxItemCount;
        public int Count
        {
            get
            {
                return _buffer.Count;
            }
        }
        public ConcurrentCircularBuffer(int maxItemCount)
        {
            _maxItemCount = maxItemCount;
            _buffer = new LinkedList<T>();
        }

        public void Put(T item)
        {
            lock (_buffer)
            {
                _buffer.AddFirst(item);
                if (_buffer.Count > _maxItemCount)
                {
                    _buffer.RemoveLast();
                }
            }
        }

        public IEnumerable<T> Read()
        {
            lock (_buffer) { return _buffer.ToArray(); }
        }
    }
}
