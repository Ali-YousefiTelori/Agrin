using Agrin.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Agrin.ComponentModels
{
    /// <summary>
    /// notify property changed for bindings
    /// </summary>
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged, IObjectDisposable
    {
        volatile bool _IsDispose = false;
        /// <summary>
        /// if object is disposed
        /// </summary>
        public bool IsDispose { get => _IsDispose; set => _IsDispose = value; }

        /// <summary>
        /// propertychanged action called when property is changed
        /// </summary>
        public Action<string> OnPropertyChangedAction { get; set; }
        
        /// <summary>
        /// when you want to rise to UI property is changed 
        /// </summary>
        /// <param name="propertyName">property name</param>
        public void OnPropertyChanged(string propertyName)
        {
            RunPropertyChanged(propertyName);
        }

        void RunPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            OnPropertyChangedAction?.Invoke(propertyName);
        }

        public virtual void Dispose()
        {
            IsDispose = true;
        }

        /// <summary>
        /// property changed Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
