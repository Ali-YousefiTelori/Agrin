using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;

namespace Agrin.MonoAndroid.UI
{
    public class CustomAdapter<T> : BaseAdapter<T>
    {
        private List<T> _items;
        private int _templateResourceId;

        public Func<int, View, ViewGroup, CustomAdapter<T>, int, View> GetViewFunc { get; set; }

        Func<int, long> _getIDFunc = null;

        public CustomAdapter(int templateResourceId, List<T> items, Func<int, long> getIDFunc, Func<int, View, ViewGroup, CustomAdapter<T>, int, View> _getViewFunc)
            : base()
        {

            GetViewFunc = _getViewFunc;
            _getIDFunc = getIDFunc;
            _templateResourceId = templateResourceId;
            _items = items;
        }

        public override T this[int position]
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override long GetItemId(int position)
        {
            return _getIDFunc(position);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return GetViewFunc(position, convertView, parent, this, _templateResourceId);
        }
    }
}

