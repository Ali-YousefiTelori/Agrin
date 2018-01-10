using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace Agrin.Mono.UI
{
	public static class BindingHelper
	{
		public static Dictionary<ObjectFinder,List<ObjectFinder>> Items = new Dictionary<ObjectFinder, List<ObjectFinder>>();

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
				findValue.Add(new ObjectFinder (){ Value = bindingObject, Property = propertybinding });
			else
				Items.Add(new ObjectFinder (){ DisposeObject = disposeObject, Value = bindingObject, Property = propertybinding }, new List<ObjectFinder>()
				{ new ObjectFinder () {
						Value = toBindingObject,
						Property = toBindingProperty
					}
				});
			if (bindingObject is Gtk.Entry)
			{
				((Gtk.Entry)bindingObject).Changed += TextChanged;
			}
			else if (bindingObject is INotifyPropertyChanged)
			{
				((INotifyPropertyChanged)bindingObject).PropertyChanged += PropertyChanged;
			}
		}

		public static void AddBindingOneWayConverter(object disposeObject, object bindingObject, string propertybinding, object toBindingObject, string toBindingProperty, Func<object,object> converter)
		{
			var findValue = FindValue(bindingObject);

			if (findValue != null)
				findValue.Add(new ObjectFinder (){ Value = bindingObject, Property = propertybinding,Converter=converter });
			else
				Items.Add(new ObjectFinder (){ DisposeObject = disposeObject, Value = bindingObject, Property = propertybinding,Converter=converter }, new List<ObjectFinder>()
				{ new ObjectFinder () {
						Value = toBindingObject,
						Property = toBindingProperty,Converter=converter
					}
				});
			if (bindingObject is Gtk.Entry)
			{
				((Gtk.Entry)bindingObject).Changed += TextChanged;
			}
			else if (bindingObject is INotifyPropertyChanged)
			{
				((INotifyPropertyChanged)bindingObject).PropertyChanged += PropertyChanged;
			}
		}

		static	List<ActionFinder> addedActionItems = new List<ActionFinder>();

		public static List<ActionFinder> ExistItem(INotifyPropertyChanged propertyChanged, string propertyName)
		{
			List<ActionFinder> items = new List<ActionFinder>();
			try
			{
				List<ActionFinder> allItems = null;

				allItems = addedActionItems.ToList();

				foreach (var item in allItems)
				{
					if (item == null)
						addedActionItems.Remove(item);
					if (item.Value == propertyChanged && item.Properties.Contains(propertyName))
						items.Add(item);
				}
			}
			catch (Exception e)
			{

			}
			return items;
		}

		public static void AddActionPropertyChanged(Action action, INotifyPropertyChanged iNotifyPropertyChanged, List<string> properties, Guid guid)
		{
			addedActionItems.Add(new ActionFinder(){Value=iNotifyPropertyChanged,action=action,Properties=properties,GuidForDispose= guid});
			action();
			iNotifyPropertyChanged.PropertyChanged += PropertyChangedChanged;
		}

		public static void AddActionPropertyChanged(Action<string> action, INotifyPropertyChanged iNotifyPropertyChanged, List<string> properties, Guid guid)
		{
			addedActionItems.Add(new ActionFinder(){Value=iNotifyPropertyChanged,action=action,Properties=properties,GuidForDispose= guid});
			iNotifyPropertyChanged.PropertyChanged += PropertyChangedChanged;
		}

		static void PropertyChangedChanged(object sender, PropertyChangedEventArgs e)
		{
			try
			{
				var finder = ExistItem((INotifyPropertyChanged)sender, e.PropertyName);
				foreach (var item in finder)
				{
					if (item.action is Action<string>)
					{
						((Action<string>)item.action)(e.PropertyName);
					}
					else if (item.action is Action)
						((Action)item.action)();
				}
			}
			catch (Exception c)
			{
			}
		}

		public static void DisposeBindingAction(Guid guid)
		{
			foreach (var item in addedActionItems.ToList())
			{
				if (item.GuidForDispose == guid)
				{
					item.Value.PropertyChanged -= PropertyChangedChanged;
					addedActionItems.Remove(item);
				}
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

		static	void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var findValue = FindKey(sender);
			if (findValue == null)
			{
				((INotifyPropertyChanged)sender).PropertyChanged -= PropertyChanged;
				return;
			}
			foreach (var item in Items[findValue])
			{
				System.Reflection.PropertyInfo pInfo = item.Value.GetType().GetProperty(item.Property);
				object value = findValue.Value.GetType().GetProperty(findValue.Property).GetValue(findValue.Value, new object[] { });
				if (findValue.Converter != null)
					pInfo.SetValue (item.Value,findValue.Converter( value),new Object[]{});
				else
					pInfo.SetValue (item.Value, value, new Object[]{ });
			}
		}
		static void TextChanged(object sender,EventArgs e)
		{
			var findValue = FindKey (sender);
			if (findValue == null) {
				((Gtk.Entry)sender).Changed -= TextChanged;
				return;
			}
			foreach (var item in Items[findValue]) {
				System.Reflection.PropertyInfo pInfo=	item.Value.GetType ().GetProperty (item.Property);
				object value = findValue.Value.GetType ().GetProperty (findValue.Property).GetValue (findValue.Value, new object[]{ });
				pInfo.SetValue (item.Value, value, new Object[]{ });
			}
		}
	}

	public class ObjectFinder
	{
		public object Value{ get; set; }
		public string Property{get;set;}
		public object DisposeObject{ get; set; }
		public Func<object,object> Converter{get;set;}
	}

	public class ActionFinder
	{
		public INotifyPropertyChanged Value{ get; set; }
		public object action{get;set;}
		public List<string> Properties{get;set;}
		public Guid GuidForDispose{get;set;}
	}
}

