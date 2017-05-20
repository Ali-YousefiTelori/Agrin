using Agrin.Helper.ComponentModel;
using System;
using System.ComponentModel;
using System.Windows;
namespace Agrin.Helper.ComponentModel
{
    [Serializable]
    public abstract class ANotifyPropertyChanged<T> : INotifyPropertyChanged
    {
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

        public void OnPropertyChanged(string valueName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(valueName));
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
