using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
namespace Agrin.ViewModels.Helper.ComponentModel
{
    [Serializable]
    public abstract class ANotifyPropertyChanged<T> : INotifyPropertyChanged
    {
        Thread CurrentThread { get; set; }

        public ANotifyPropertyChanged()
        {
            CurrentThread = Thread.CurrentThread;
        }

        [field: NonSerializedAttribute()]
        T _viewElement;

        public T ViewElement
        {
            get { return _viewElement; }
            set
            {
                _viewElement = value;
                if (SetViewElement != null)
                    SetViewElement();
                if (ViewElementLoaded != null)
                    ((FrameworkElement)(object)_viewElement).Loaded += ANotifyPropertyChanged_Loaded;
                if (ViewElementInited != null)
                    ViewElementInited();
            }
        }

        void ANotifyPropertyChanged_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).Loaded -= ANotifyPropertyChanged_Loaded;
            ViewElementLoaded();
        }

        public Action SetViewElement;
        public Action ViewElementLoaded;
        public Action ViewElementInited;

        bool _canClick = true;

        public bool CanClick
        {
            get { return _canClick; }
            set { _canClick = value; OnPropertyChanged("CanClick"); }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (Thread.CurrentThread != CurrentThread)
            {
                var dispatcher = ApplicationHelperBase.GetDispatcherByThread(CurrentThread);
                if (dispatcher != null)
                {
                    ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                    {
                        RunPropertyChanged(propertyName);
                    }, dispatcher);
                }
                else
                {
                    RunPropertyChanged(propertyName);
                }
            }
            else
            {
                RunPropertyChanged(propertyName);
            }
        }

        void RunPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

namespace System
{
    public static class ViewModelViewElement
    {
        public static void SetViewModelViewElement<T>(this T obj)
        {
            FrameworkElement element = (FrameworkElement)(object)obj;
            if (element.DataContext is ANotifyPropertyChanged<T>)
            {
                ((ANotifyPropertyChanged<T>)element.DataContext).ViewElement = obj;
            }
        }

        public static void SetViewModelViewElement<T>(this T obj, object viewModel)
        {
            FrameworkElement element = (FrameworkElement)(object)obj;
            if (viewModel is ANotifyPropertyChanged<T>)
            {
                ((ANotifyPropertyChanged<T>)viewModel).ViewElement = obj;
            }
        }
    }
}
