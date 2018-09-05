using Agrin.Helper.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.Controls
{
    /// <summary>
    /// Interaction logic for AutoCompleteTextBox.xaml
    /// </summary>
    public partial class AutoCompleteTextBox : UserControl, INotifyPropertyChanged
    {
        public AutoCompleteTextBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnItemsSourcePropertyChanged)));

        //public static readonly DependencyProperty FindItemsSourceProperty = DependencyProperty.Register("FindItemsSource", typeof(IEnumerable), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnFindItemsSourcePropertyChanged)));

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        {
            AutoCompleteTextBox autoCompleteTextBox = sender as AutoCompleteTextBox;
            autoCompleteTextBox.ItemTemplate = (DataTemplate)e.NewValue;
        })));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        {

            AutoCompleteTextBox autoCompleteTextBox = sender as AutoCompleteTextBox;
            autoCompleteTextBox.SelectedItem = (object)e.NewValue;
        })));

        //public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        //{
        //    AutoCompleteTextBox autoCompleteTextBox = sender as AutoCompleteTextBox;
        //    if (autoCompleteTextBox.ItemsSource.Cast<object>().Count() == 0)
        //        return;
        //    autoCompleteTextBox.SelectedIndex = (int)e.NewValue;
        //})));
        public static readonly DependencyProperty OpenerWidthProperty = DependencyProperty.Register("OpenerWidth", typeof(double), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(30.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        {
            AutoCompleteTextBox autoCompleteTextBox = sender as AutoCompleteTextBox;
            autoCompleteTextBox.OpenerWidth = (double)e.NewValue;
        })));

        public static readonly DependencyProperty PathPropertyNameProperty = DependencyProperty.Register("PathPropertyName", typeof(string), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        {
            AutoCompleteTextBox autoCompleteTextBox = sender as AutoCompleteTextBox;
            autoCompleteTextBox.PathPropertyName = (string)e.NewValue;
        })));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        {
            AutoCompleteTextBox autoCompleteTextBox = sender as AutoCompleteTextBox;
            autoCompleteTextBox.Text = (string)e.NewValue;
        })));

        public static readonly DependencyProperty NullableSelectedItemProperty = DependencyProperty.Register("NullableSelectedItem", typeof(bool), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        {
            AutoCompleteTextBox autoCompleteTextBox = sender as AutoCompleteTextBox;
            autoCompleteTextBox.NullableSelectedItem = (bool)e.NewValue;
        })));

        public static readonly DependencyProperty IsReadOnlyTextProperty = DependencyProperty.Register("IsReadOnlyText", typeof(bool), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        {
            AutoCompleteTextBox autoCompleteTextBox = sender as AutoCompleteTextBox;
            autoCompleteTextBox.IsReadOnlyText = (bool)e.NewValue;
        })));


        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as AutoCompleteTextBox;
            if (control != null)
            {
                control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
                control.mainListBox.DataContext = (IEnumerable)e.NewValue;
            }
        }

        //private static void OnFindItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = sender as AutoCompleteTextBox;
        //    if (control != null)
        //    {
        //        control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        //        control.mainListBox.DataContext = (IEnumerable)e.NewValue;
        //    }
        //}



        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Remove handler for oldValue.CollectionChanged
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }
            // Add handler for newValue.CollectionChanged (if possible)
            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }
        }

        void newValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Do your stuff here.
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(ItemsSourceProperty);
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
                OnPropertyChanged("ItemsSource");
            }
        }

        //List<object> findedItems = new List<object>();
        //private IEnumerable FindItemsSource
        //{
        //    get
        //    {
        //        if (findedItems.Count == 0)
        //            findedItems.Add(notFoundData);
        //        return findedItems;
        //    }
        //}

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ItemTemplateProperty);
            }
            set
            {
                this.SetValue(ItemTemplateProperty, value);
                OnPropertyChanged("ItemTemplate");
            }
        }

        public object SelectedItem
        {
            get
            {
                return this.GetValue(SelectedItemProperty);
            }
            set
            {
                //if (value != null)
                //{
                //    int index = ItemsSource.Cast<object>().ToList().IndexOf(value);
                //    if (index != SelectedIndex)
                //        SelectedIndex = index;
                //}
                this.SetValue(SelectedItemProperty, value);
                canFind = false;
                if (value != null)
                {
                    BindingExpression bindingExpression = BindingOperations.GetBindingExpression(maintextBox, TextBox.TextProperty);
                    if (bindingExpression != null)
                    {
                        System.Reflection.PropertyInfo property = bindingExpression.DataItem.GetType().GetProperty(bindingExpression.ParentBinding.Path.Path);
                        if (property != null)
                            property.SetValue(bindingExpression.DataItem, string.IsNullOrEmpty(PathPropertyName) ? value : GetPropertyValue(value, PathPropertyName), null);
                    }
                    else
                    {
                        Text = string.IsNullOrEmpty(PathPropertyName) ? value.ToString() : GetPropertyValue(value, PathPropertyName).ToString();
                    }
                }

                canFind = true;
                OnPropertyChanged("SelectedItem");
            }
        }

        //public int SelectedIndex
        //{
        //    get
        //    {
        //        return (int)this.GetValue(SelectedIndexProperty);
        //    }
        //    set
        //    {
        //        this.SetValue(SelectedIndexProperty, value);

        //        //if (value >= 0)
        //        //{
        //        //    canFind = false;
        //        //    SelectedItem = ItemsSource.Cast<object>().ToList()[value];
        //        //    if (!String.IsNullOrEmpty(PathPropertyName))
        //        //    {
        //        //        object val = null;
        //        //        foreach (var item in PathPropertyName.Split(new char[] { ',' }))
        //        //        {
        //        //            val = GetPropertyValue(SelectedItem, item);
        //        //            if (val != null)
        //        //                break;
        //        //        }
        //        //        Text = val == null ? null : val.ToString();
        //        //    }
        //        //    canFind = true;
        //        //}
        //        //else if (value == -1)
        //        //    Text = "";
        //        OnPropertyChanged("SelectedIndex");
        //    }
        //}

        public string PathPropertyName
        {
            get
            {
                return (string)this.GetValue(PathPropertyNameProperty);
            }
            set
            {
                this.SetValue(PathPropertyNameProperty, value);
                OnPropertyChanged("PathPropertyName");
            }
        }


        public double OpenerWidth
        {
            get
            {
                return (double)this.GetValue(OpenerWidthProperty);
            }
            set
            {
                this.SetValue(OpenerWidthProperty, value);
                OnPropertyChanged("OpenerWidth");
            }
        }

        public bool NullableSelectedItem
        {
            get
            {
                return (bool)this.GetValue(NullableSelectedItemProperty);
            }
            set
            {
                this.SetValue(NullableSelectedItemProperty, value);
                OnPropertyChanged("NullableSelectedItem");
            }
        }
        public bool IsReadOnlyText
        {
            get
            {
                return (bool)this.GetValue(IsReadOnlyTextProperty);
            }
            set
            {
                this.SetValue(IsReadOnlyTextProperty, value);
                OnPropertyChanged("IsReadOnlyText");
            }
        }

        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        //FastCollection<object> _bindingCollection;


        //List<object> clearFindData = new List<object>();
        ListBoxItem notFoundData = new ListBoxItem() { Foreground = (Brush)new BrushConverter().ConvertFromString("#4a4a45"), Content = "یافت نشد", Focusable = false };
        bool canFind = true;
        void FindData()
        {
            if (!canFind)
                return;
            foreach (var item in ItemsSource)
            {
                if (string.IsNullOrEmpty(PathPropertyName))
                {

                }
                else
                {
                    foreach (var itemFind in PathPropertyName.Split(new char[] { ',' }))
                    {
                        var obj = GetPropertyValue(item, itemFind);
                        if (obj is string)
                        {
                            var value = obj.ToString();
                            var mustContains = Text.Substring(0, maintextBox.SelectionStart);
                            if (value.IndexOf(mustContains) == 0 && !string.IsNullOrEmpty(mustContains))
                            {
                                canFind = false;
                                int selIndex = value.IndexOf(mustContains) + mustContains.Length;// maintextBox.SelectionStart;
                                SelectedItem = item;
                                BindingExpression bindingExpression = BindingOperations.GetBindingExpression(maintextBox, TextBox.TextProperty);
                                if (bindingExpression != null)
                                {
                                    System.Reflection.PropertyInfo property = bindingExpression.DataItem.GetType().GetProperty(bindingExpression.ParentBinding.Path.Path);
                                    if (property != null)
                                        property.SetValue(bindingExpression.DataItem, value, null);
                                }
                                else
                                {
                                    Text = value;
                                }
                                maintextBox.Select(selIndex, value.Length - selIndex);
                                canFind = true;
                                return;
                            }
                        }
                    }


                }
            }
            if (NullableSelectedItem)
                SelectedItem = null;

            //if (Text == null)
            //    return;
            //string text = Text.ToLower();
            //if (String.IsNullOrEmpty(text))
            //{
            //    mainPopUp.IsOpen = false;
            //    return;
            //}

            //bool isNull = String.IsNullOrEmpty(PathPropertyName);
            //clearFindData.Clear();
            //Dictionary<string, object> nullableSelectedItems = new Dictionary<string, object>();
            //foreach (var item in ItemsSource)
            //{
            //    bool canClear = true;
            //    if (isNull)
            //    {
            //        string pValue;
            //        if (item is string)
            //        {
            //            if (!nullableSelectedItems.ContainsKey(item.ToString()))
            //                nullableSelectedItems.Add(item.ToString(), item.ToString());
            //        }
            //        else
            //        {
            //            foreach (var propertyValue in GetPropertiesValue(item))
            //            {
            //                pValue = propertyValue.ToString().ToLower();
            //                if (!string.IsNullOrEmpty(pValue) && pValue.Contains(text))
            //                {
            //                    canClear = false;
            //                    if (!nullableSelectedItems.ContainsKey(pValue))
            //                        nullableSelectedItems.Add(pValue, item);
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        object val = null;
            //        string pValue;
            //        foreach (var itemFind in PathPropertyName.Split(new char[] { ',' }))
            //        {
            //            val = GetPropertyValue(item, itemFind);
            //            pValue = val.ToString().ToLower();
            //            if (!string.IsNullOrEmpty(pValue) && pValue.Contains(text))
            //            {
            //                canClear = false;
            //                if (!nullableSelectedItems.ContainsKey(pValue))
            //                    nullableSelectedItems.Add(pValue, item);
            //                break;
            //            }
            //        }
            //    }
            //    if (canClear)
            //    {
            //        clearFindData.Add(item);

            //    }
            //    else
            //    {
            //        if (!_bindingCollection.Contains(item))
            //            _bindingCollection.Add(item);
            //    }
            //}

            //_bindingCollection.RemoveRange(clearFindData);
            //if (_bindingCollection.Count == 0)
            //{
            //    _bindingCollection.Add(notFoundData);
            //}
            //else if (_bindingCollection.Contains(notFoundData) && _bindingCollection.Count > 1)
            //{
            //    _bindingCollection.Remove(notFoundData);
            //    mainListBox.SelectedItem = _bindingCollection.First();
            //}

            //if (NullableSelectedItem && !nullableSelectedItems.ContainsKey(text))
            //    SelectedItem = null;
            //else if (nullableSelectedItems.ContainsKey(text))
            //{
            //    SelectedItem = nullableSelectedItems[text];
            //}

            //if (maintextBox.IsFocused)
            //{
            //    //mainBorder.BorderThickness = new Thickness(2, 2, 2, 0);
            //    mainPopUp.IsOpen = true;
            //    mainPopUp.Focus();
            //}

        }
        void ShowAllData()
        {
            bool isNull = String.IsNullOrEmpty(PathPropertyName);
            Dictionary<string, object> nullableSelectedItems = new Dictionary<string, object>();
            if (ItemsSource != null && mainListBox.ItemsSource != ItemsSource)
            {
                if (SelectedItem == null)
                    SelectedItem = null;
                var selItem = SelectedItem;
                mainListBox.ItemsSource = null;
                mainListBox.ItemsSource = ItemsSource;
                if (selItem != null)
                {
                    SelectedItem = selItem;
                }
            }
            //mainBorder.BorderThickness = new Thickness(2, 2, 2, 0);
            mainPopUp.IsOpen = true;
            mainPopUp.Focus();
        }
        public static object GetPropertyValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static List<object> GetPropertiesValue(object src)
        {
            List<object> values = new List<object>();
            foreach (var item in src.GetType().GetProperties())
            {
                values.Add(item.GetValue(src, null));
            }
            return values;
        }

        private void mainPopUp_Loaded(object sender, RoutedEventArgs e)
        {
            Window w = Window.GetWindow(mainPopUp);
            if (null != w)
            {
                w.LocationChanged -= w_LocationChanged;
                w.LocationChanged += w_LocationChanged;
            }
        }

        void w_LocationChanged(object sender, EventArgs e)
        {
            if (mainPopUp.IsOpen)
            {
                var mi = typeof(System.Windows.Controls.Primitives.Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                mi.Invoke(mainPopUp, null);
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                return;
            else if (e.Key == Key.Enter && SelectedItem != null)
            {
                if (!String.IsNullOrEmpty(PathPropertyName))
                {
                    object val = null;
                    foreach (var item in PathPropertyName.Split(new char[] { ',' }))
                    {
                        val = GetPropertyValue(SelectedItem, item);
                        if (val != null)
                            break;
                    }
                    Text = val == null ? null : val.ToString();
                }
                maintextBox.SelectAll();
                mainPopUp.IsOpen = false;
            }
            else if (e.Key == Key.Down)
            {
                IList list = ItemsSource as IList;
                if (list.Count == 0 || list[0] == notFoundData)
                    return;
                if (SelectedItem == null)
                {
                    SelectedItem = list[0];
                }
                else
                {
                    int index = list.IndexOf(SelectedItem);
                    index++;
                    if (index == list.Count)
                        index = 0;
                    SelectedItem = list[index];
                }
                SelectPopupItem();
            }
            else if (e.Key == Key.Up)
            {
                IList list = ItemsSource as IList;
                if (list.Count == 0 || list[0] == notFoundData)
                    return;
                if (SelectedItem == null)
                {
                    SelectedItem = list[0];
                }
                else
                {
                    int index = list.IndexOf(SelectedItem);
                    index--;
                    if (index < 0)
                        index = list.Count - 1;
                    SelectedItem = list[index];
                }
                SelectPopupItem();
            }
        }

        void SelectPopupItem()
        {
            if (mainPopUp.IsOpen)
            {
                //var lItem = mainListBox.ItemContainerGenerator.ContainerFromItem(SelectedItem) as ListBoxItem;
                mainListBox.Focus();
            }
        }

        private void mainListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;

            while (source is ContentElement)
                source = LogicalTreeHelper.GetParent(source);

            while (source != null && !(source is ListBoxItem))
                source = VisualTreeHelper.GetParent(source);

            if (source is ListBoxItem)
            {
                IList list = ItemsSource as IList;
                int index = list.IndexOf(((ListBoxItem)source).DataContext);
                if (index >= 0)
                {
                    SelectedItem = list[index];
                    if (!String.IsNullOrEmpty(PathPropertyName))
                    {
                        object val = null;
                        foreach (var item in PathPropertyName.Split(new char[] { ',' }))
                        {
                            val = GetPropertyValue(SelectedItem, item);
                            if (val != null)
                                break;
                        }
                        Text = val == null ? null : val.ToString();
                    }
                    else if (SelectedItem != null)
                    {
                        Text = SelectedItem.ToString();
                    }
                    mainPopUp.IsOpen = false;
                    maintextBox.SelectAll();
                }
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void mainPopUp_Closed(object sender, EventArgs e)
        {
            //mainBorder.BorderThickness = new Thickness(2);
        }

        private void mainToggle_Click(object sender, RoutedEventArgs e)
        {
            ShowAllData();
        }

        private void readOnlyBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowAllData();
        }

        private void maintextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (canFind)
                FindData();
        }

        private void maintextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (maintextBox.SelectionLength > 0)
                {
                    var selLen = maintextBox.SelectionLength;
                    var selStart = maintextBox.SelectionStart;
                    if (maintextBox.SelectionStart == 0)
                    {
                        if (maintextBox.SelectionLength == maintextBox.Text.Length)
                        {
                            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(maintextBox, TextBox.TextProperty);
                            if (bindingExpression != null)
                            {
                                System.Reflection.PropertyInfo property = bindingExpression.DataItem.GetType().GetProperty(bindingExpression.ParentBinding.Path.Path);
                                if (property != null)
                                    property.SetValue(bindingExpression.DataItem, "", null);
                            }
                            else
                            {
                                Text = "";
                            }
                        }
                        else
                            return;
                    }
                    else
                    {
                        maintextBox.Select(selStart - 1, selLen + 1);
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
