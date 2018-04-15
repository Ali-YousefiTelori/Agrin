using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Agrin.Foundation
{
    public abstract class DataBaseFoundation<T>
    {
        public static DataBaseFoundation<T> Current { get; set; }

        public abstract void Initialize<TResult>() where TResult : T;
        public abstract void Initialize();
        public abstract void Update(T data);
        public abstract void Delete(T data);
        public abstract void Delete(Expression<Func<T, bool>> predicate);
        public abstract void Add(T data);
        public abstract List<TResult> GetList<TResult>();
        public abstract List<T> GetList();
        public abstract T FindItem(Expression<Func<T, bool>> predicate);
    }
}
