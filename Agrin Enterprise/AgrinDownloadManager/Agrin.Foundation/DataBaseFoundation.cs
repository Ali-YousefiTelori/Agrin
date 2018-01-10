﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Agrin.Foundation
{
    public abstract class DataBaseFoundation<T>
    {
        public static DataBaseFoundation<T> Current { get; set; }

        public abstract void Initialize<TResult>() where TResult : T;
        public abstract void Update(T data);
        public abstract void Add(T data);
        public abstract List<TResult> GetList<TResult>();
    }
}