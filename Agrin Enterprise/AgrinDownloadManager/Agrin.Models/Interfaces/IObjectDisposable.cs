using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Interfaces
{
    /// <summary>
    /// object have disposable
    /// </summary>
    public interface IObjectDisposable : IDisposable
    {
        /// <summary>
        /// when object is disposed
        /// </summary>
        bool IsDispose { get; set; }
    }
}
