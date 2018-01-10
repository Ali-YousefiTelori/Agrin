using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Agrin.MonoAndroid.UI
{
    public static class BindingHelper
    {
        public static Dictionary<ObjectFinder, List<ObjectFinder>> Items = new Dictionary<ObjectFinder, List<ObjectFinder>>();

        static List<ObjectFinder> FindValue(object obj)
        {
            foreach (var item in Items)
            {
                if (item.Key.Value == obj)
                    return item.Value;
            }
            return null;
        }

        static ObjectFinder FindKey(object obj)
        {
            foreach (var item in Items)
            {
                if (item.Key.Value == obj)
                    return item.Key;
            }
            return null;
        }

        static ObjectFinder FindDisposeObject(object obj)
        {
            foreach (var item in Items)
            {
                if (item.Key.DisposeObject == obj)
                    return item.Key;
            }
            return null;
        }

        public static void AddBindingTwoWay(object disposeObject, object bindingObject, string propertybinding, object toBindingObject, string toBindingProperty)
        {
            AddBindingOneWay(disposeObject, bindingObject, propertybinding, toBindingObject, toBindingProperty);
            AddBindingOneWay(disposeObject, toBindingObject, toBindingProperty, bindingObject, propertybinding);
        }

        public static void AddBindingOneWay(object disposeObject, object bindingObject, string propertybinding, object toBindingObject, string toBindingProperty)
        {
            var findValue = FindValue(bindingObject);

            if (findValue != null)
                findValue.Add(new ObjectFinder() { Value = bindingObject, Property = propertybinding });
            else
                Items.Add(new ObjectFinder() { DisposeObject = disposeObject, Value = bindingObject, Property = propertybinding }, new List<ObjectFinder>() { new ObjectFinder () {
						Value = toBindingObject,
						Property = toBindingProperty
					}
				});
            if (bindingObject is EditText)
            {
                ((EditText)bindingObject).TextChanged += TextChanged;
            }
            else if (bindingObject is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)bindingObject).PropertyChanged += PropertyChanged;
                //PropertyChanged (bindingObject, null);
            }
        }
        //static	List<INotifyPropertyChanged> addedActionItems = new List<INotifyPropertyChanged> ();
        static Dictionary<INotifyPropertyChanged, Dictionary<string, List<Action<string>>>> addedActionItems = new Dictionary<INotifyPropertyChanged, Dictionary<string, List<Action<string>>>>();

        public static void AddActionPropertyChanged(Action<string> action, INotifyPropertyChanged iNotifyPropertyChanged, List<string> properties)
        {
            if (addedActionItems.ContainsKey(iNotifyPropertyChanged))
            {
                foreach (var item in properties)
                {
                    if (!addedActionItems[iNotifyPropertyChanged].ContainsKey(item))
                    {
                        addedActionItems[iNotifyPropertyChanged].Add(item, new List<Action<string>>() { action });
                    }
                    else
                    {
                        if (!addedActionItems[iNotifyPropertyChanged][item].Contains(action))
                            addedActionItems[iNotifyPropertyChanged][item].Add(action);
                    }
                }
                return;
            }
            else
            {
                Dictionary<string, List<Action<string>>> items = new Dictionary<string, List<Action<string>>>();
                foreach (var item in properties)
                {
                    if (!items.ContainsKey(item))
                    {
                        items.Add(item, new List<Action<string>>() { action });
                    }
                }
                addedActionItems.Add(iNotifyPropertyChanged, items);
                iNotifyPropertyChanged.PropertyChanged += PropertyChangedEvent;
            }
        }

        public static void RemoveActionPropertyChanged(INotifyPropertyChanged iNotifyPropertyChanged)
        {
            if (addedActionItems.ContainsKey(iNotifyPropertyChanged))
            {
                addedActionItems[iNotifyPropertyChanged].Clear();
                iNotifyPropertyChanged.PropertyChanged -= PropertyChangedEvent;
                addedActionItems.Remove(iNotifyPropertyChanged);
            }
        }

        public static void RemoveActionPropertyChanged(INotifyPropertyChanged iNotifyPropertyChanged, Action<string> setAllItems)
        {
            if (addedActionItems.ContainsKey(iNotifyPropertyChanged))
            {
                foreach (var item in addedActionItems[iNotifyPropertyChanged].ToArray())
                {
                    if (item.Value.Contains(setAllItems))
                        item.Value.Remove(setAllItems);
                }
            }
        }

        static void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (addedActionItems.ContainsKey((INotifyPropertyChanged)sender))
            {
                if (addedActionItems[(INotifyPropertyChanged)sender].ContainsKey(e.PropertyName))
                {
                    foreach (var runfunc in addedActionItems[(INotifyPropertyChanged)sender][e.PropertyName])
                    {
                        runfunc(e.PropertyName);
                    }
                }
            }
            else
            {
                ((INotifyPropertyChanged)sender).PropertyChanged -= PropertyChangedEvent;
            }
        }

        public static void DisposeBindingOneWay(object bindingObject)
        {
            var item = FindDisposeObject(bindingObject);
            while (item != null)
            {
                Items.Remove(item);
                item = FindDisposeObject(bindingObject);
            }

        }

        static void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var findValue = FindKey(sender);
            if (findValue == null)
            {
                ((INotifyPropertyChanged)sender).PropertyChanged -= PropertyChanged;
                return;
            }
            foreach (var item in Items[findValue])
            {
                var pInfo = FindProperty(item.Value.GetType(), item.Property);
                object value = FindProperty(findValue.Value.GetType(), findValue.Property).GetValue(findValue.Value, new object[] { });
                if (item.Value is TextView || item.Value is EditText)
                    pInfo.SetValue(item.Value, value == null ? null : value.ToString(), new Object[] { });
                else
                    pInfo.SetValue(item.Value, value, new Object[] { });
            }
        }

        static void TextChanged(object sender, EventArgs e)
        {
            var findValue = FindKey(sender);
            if (findValue == null)
            {
                ((EditText)sender).TextChanged -= TextChanged;
                return;
            }
            foreach (var item in Items[findValue])
            {
                try
                {
                    var pInfo = FindProperty(item.Value.GetType(), item.Property);

                    object classValue = pInfo.GetValue(item.Value, null);
                    var prop = FindProperty(findValue.Value.GetType(), findValue.Property);
                    object value = findValue.Value is EditText ? ((EditText)findValue.Value).Text : prop.GetValue(findValue.Value, null);
                    string valueText = value == null ? null : value.ToString();
                    string classValueText = classValue == null ? null : classValue.ToString();
                    if (valueText != classValueText)
                        pInfo.SetValue(item.Value, value, null);
                }
                catch (Exception ssss)
                {
                    //MainActivity.This.SetContentView (Resource.Layout.MessageBox);
                    //EditText textBox = MainActivity.This.FindViewById<EditText> (Resource.ApplicationLog.TxTmessageBox);
                    //textBox.Text = ssss.Message + " " + ssss.StackTrace;
                }
            }
        }

        static System.Reflection.PropertyInfo FindProperty(Type type, string name)
        {
            //Action<string> isnull = (str) =>
            //{

            //};

            foreach (var item in type.GetProperties())
            {
                if (item.Name == name)
                    return item;
            }
            return null;
        }
    }

    public class ObjectFinder
    {
        public object Value { get; set; }

        public string Property { get; set; }

        public object DisposeObject { get; set; }
    }

    public class EditText : INotifyPropertyChanged
    {
        string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                OnPropertyChanged("Text");
                TextChanged(this, EventArgs.Empty);
            }
        }
        public event EventHandler TextChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PersonTest : INotifyPropertyChanged
    {

        string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }

        string _State;
        public string State
        {
            get { return _State; }
            set { _State = value; OnPropertyChanged("State"); }
        }

        double _Length;

        public double Length
        {
            get { return _Length; }
            set { _Length = value; OnPropertyChanged("Length"); }
        }

        double _DownloadedSize;
        public double DownloadedSize
        {
            get { return _DownloadedSize; }
            set { _DownloadedSize = value; OnPropertyChanged("DownloadedSize"); }
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class TextView : INotifyPropertyChanged
    {
        string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                OnPropertyChanged("Text");
                TextChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler TextChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

