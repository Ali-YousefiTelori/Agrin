using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gtk;
using System.Linq;
using Agrin.Log;
using GLib;

namespace Agrin.Mono.UI
{
	public static class BindingHelper
	{
		static Dictionary<object, List<IBindingExpresion>> Items = new Dictionary<object, List<IBindingExpresion>> ();
		static object lockOBJ = new object ();

		public static void BindOneWay (object disposeObject, object bindObject, string bindObjectProperty, object toSourceObject, string toSourceObjectProperty)
		{
			lock (lockOBJ) {
				var p1 = FindProperty (bindObject.GetType (), bindObjectProperty);
				var p2 = FindProperty (toSourceObject.GetType (), toSourceObjectProperty);
				if (p1 == null || p2 == null)
					throw new Exception ("یکی از پروپرتی ها یافت نشد");
				else if (p1.PropertyType != p2.PropertyType)
					throw new Exception ("نوع پروپرتی ها بهم نمی خورد لطفاً نوع های یکسان انتخاب کنید");

				if (!Items.ContainsKey (disposeObject)) {
					Items.Add (disposeObject, new List<IBindingExpresion> ());
				}

				var exp = FindBindingExpresion (bindObject, bindObjectProperty, Items [disposeObject]);
				if (exp == null) {
					exp = new BindingExpresion () {
						BindingObject = bindObject,
						BindObjectProperty = bindObjectProperty,
						SourceBindingObject = new Dictionary<object, Dictionary<string, BindingExpresionEventManager>> ()
					};
					Items [disposeObject].Add (exp);
				}

				if (!exp.SourceBindingObject.ContainsKey (toSourceObject))
					exp.SourceBindingObject.Add (toSourceObject, new Dictionary<string, BindingExpresionEventManager> ());
				if (!exp.SourceBindingObject [toSourceObject].ContainsKey (toSourceObjectProperty)) {
					var expManager = new BindingExpresionEventManager () {
						BindingExpresion = exp,
						BindingObject = bindObject,
						BindObjectProperty = bindObjectProperty,
						SourceBindingProperty = new List<string> () { toSourceObjectProperty }
					};
					exp.SourceBindingObject [toSourceObject].Add (toSourceObjectProperty, expManager);
					if (!BindingExpresionEventManager.Items.ContainsKey (toSourceObject))
						BindingExpresionEventManager.Items.Add (toSourceObject, new List<BindingExpresionEventManager> () { expManager });
					else
						BindingExpresionEventManager.Items [toSourceObject].Add (expManager);
					if (toSourceObject is Entry) {
						((Entry)toSourceObject).Changed -= BindingExpresionEventManager.TextChanged;
						((Entry)toSourceObject).Changed += BindingExpresionEventManager.TextChanged;
					} else {
						((INotifyPropertyChanged)toSourceObject).PropertyChanged -= BindingExpresionEventManager.BindingHelper_PropertyChanged;
						((INotifyPropertyChanged)toSourceObject).PropertyChanged += BindingExpresionEventManager.BindingHelper_PropertyChanged;
					}
				}
			}
		}

		public static void BindTwoway (object disposeObject, object bindObject, string bindObjectProperty, object toSourceObject, string toSourceObjectProperty)
		{
			BindOneWay (disposeObject, bindObject, bindObjectProperty, toSourceObject, toSourceObjectProperty);
			BindOneWay (disposeObject, toSourceObject, toSourceObjectProperty, bindObject, bindObjectProperty);
		}

		public static bool IsBinded (object toSourceObject, string toSourceObjectProperty)
		{
			if (!BindingExpresionEventManager.Items.ContainsKey (toSourceObject))
				return false;
			foreach (var item in BindingExpresionEventManager.Items[toSourceObject]) {
				if (item.BindingObject is Action<string>) {
					if (item.SourceBindingProperty.Contains (toSourceObjectProperty)) {
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsBinded (object toSourceObject, List<string> toSourceObjectProperties)
		{
			if (!BindingExpresionEventManager.Items.ContainsKey (toSourceObject))
				return false;
			foreach (var item in BindingExpresionEventManager.Items[toSourceObject]) {
				if (item.BindingObject is Action<string>) {
					foreach (var item2 in toSourceObjectProperties) {
						if (item.SourceBindingProperty.Contains (item2)) {
							return true;
						}
					}
				}
			}
			return false;
		}

		public static void BindAction (object disposeObject, object toSourceObject, List<string> toSourceObjectProperties, Action<string> changed)
		{
			lock (lockOBJ) {
				if (!Items.ContainsKey (disposeObject)) {
					Items.Add (disposeObject, new List<IBindingExpresion> ());
				}

				var expManager = new BindingExpresionEventManager () {
					BindingObject = changed,
					SourceBindingProperty = toSourceObjectProperties,
					SenderObject = toSourceObject
				};
				Items [disposeObject].Add (expManager);
				if (!BindingExpresionEventManager.Items.ContainsKey (toSourceObject))
					BindingExpresionEventManager.Items.Add (toSourceObject, new List<BindingExpresionEventManager> () { expManager });
				else {
					foreach (var item in BindingExpresionEventManager.Items[toSourceObject]) {
						if (item.BindingObject is Action<string>) {
							foreach (var item2 in toSourceObjectProperties) {
								if (item.SourceBindingProperty.Contains (item2)) {
									throw new Exception ("یکی از آیتم ها مجدد در حال بایند شدن است : " + item2);
								}
							}
						}
					}

					BindingExpresionEventManager.Items [toSourceObject].Add (expManager);
				}
				if (toSourceObject is Entry) {
					((Entry)toSourceObject).Changed -= BindingExpresionEventManager.TextChanged;
					((Entry)toSourceObject).Changed += BindingExpresionEventManager.TextChanged;
				} else {
					((INotifyPropertyChanged)toSourceObject).PropertyChanged -= BindingExpresionEventManager.BindingHelper_PropertyChanged;
					((INotifyPropertyChanged)toSourceObject).PropertyChanged += BindingExpresionEventManager.BindingHelper_PropertyChanged;
				}
			}
		}


		static BindingExpresion FindBindingExpresion (object obj, string property, List<IBindingExpresion> items)
		{
			foreach (IBindingExpresion item in items) {
				if (item is BindingExpresion && item.BindingObject == obj && property == item.BindObjectProperty)
					return (BindingExpresion)item;
			}
			return null;
		}

		public static System.Reflection.PropertyInfo FindProperty (Type type, string name)
		{
			foreach (var item in type.GetProperties()) {
				if (item.Name == name)
					return item;
			}
			return null;
		}

		public static bool ExistObject (object disposeObject)
		{
			lock (lockOBJ) {
				return  Items.ContainsKey (disposeObject);
			}
		}

		public static void DisposeObject (object disposeObject)
		{
			lock (lockOBJ) {
				if (Items.ContainsKey (disposeObject)) {
					foreach (var exp in Items[disposeObject]) {
						if (exp is BindingExpresion) {
							foreach (var source in ((BindingExpresion)exp).SourceBindingObject) {
								foreach (var dis in source.Value.Values) {
									if (source.Key is Entry)
										((Entry)source.Key).Changed -= BindingExpresionEventManager.TextChanged;
									else
										((INotifyPropertyChanged)source.Key).PropertyChanged -= BindingExpresionEventManager.BindingHelper_PropertyChanged;
									BindingExpresionEventManager.Items.Remove (source.Key);
								}
							}
						} else {
							var expM = ((BindingExpresionEventManager)exp);
							expM.BindingObject = null;
							if (expM.SenderObject is Entry)
								((Entry)expM.SenderObject).Changed -= BindingExpresionEventManager.TextChanged;
							else
								((INotifyPropertyChanged)expM.SenderObject).PropertyChanged -= BindingExpresionEventManager.BindingHelper_PropertyChanged;
							BindingExpresionEventManager.Items.Remove (expM.SenderObject);
						}
					}
					Items.Remove (disposeObject);
				}
			}
		}

		public static void DisposeAll ()
		{
			foreach (var item in Items.ToArray()) {
				DisposeObject (item.Key);
			}
		}

	}

	public interface IBindingExpresion
	{
		object BindingObject { get; set; }

		string BindObjectProperty { get; set; }
	}

	public class BindingExpresion : IBindingExpresion
	{
		public object BindingObject { get; set; }

		public string BindObjectProperty { get; set; }

		public bool IsBusy { get; set; }

		/// <summary>
		/// key=source class binding,value=properties bindings
		/// </summary>
		public Dictionary<object, Dictionary<string, BindingExpresionEventManager>> SourceBindingObject { get; set; }
	}

	public class BindingExpresionEventManager : IBindingExpresion
	{
		public BindingExpresion BindingExpresion { get; set; }

		public object BindingObject { get; set; }

		public string BindObjectProperty { get; set; }

		public List<string> SourceBindingProperty { get; set; }

		public object SenderObject { get; set; }

		public static Dictionary<object, List<BindingExpresionEventManager>> Items = new Dictionary<object, List<BindingExpresionEventManager>> ();

		public static void BindingHelper_PropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			ChangedValue (sender, e.PropertyName);
		}

		public static void TextChanged (object sender, EventArgs e)
		{
			ChangedValue (sender, "Text");
		}

		static void ChangedValue (object sender, string propertyName)
		{
			try {
				if (!Items.ContainsKey (sender))
					return;
				var items = Items [sender].ToList ();

				foreach (var exp in items) {
					if (exp.BindingExpresion != null) {
						if (exp.BindingExpresion.IsBusy)
							return;
						exp.BindingExpresion.IsBusy = true;
					}
					bool IsThread = false;
					try {
						if (exp.BindingObject is Action<string>) {
							if (exp.SourceBindingProperty.Contains (propertyName))
								((Action<string>)exp.BindingObject) (propertyName);
						} else {
							if (exp.SourceBindingProperty.Contains (propertyName)) {
								System.Action run = () => {
									int currentIndex = 0;
									if (exp.BindingObject is Entry) {
										var entry = ((Entry)exp.BindingObject);
										currentIndex = entry.CursorPosition;
									}
										
									var pInfo = BindingHelper.FindProperty (exp.BindingObject.GetType (), exp.BindObjectProperty);
									var propValue = BindingHelper.FindProperty (sender.GetType (), propertyName);
									object value = propValue.GetValue (sender, null);
									if (pInfo.CanWrite)
										pInfo.SetValue (exp.BindingObject, value, null);
									if (exp.BindingObject is Entry && ((Entry)exp.BindingObject).Text.Length >= currentIndex)
										((Entry)exp.BindingObject).SelectRegion (currentIndex, currentIndex);
								};

								if ((exp.BindingObject as IWrapper) != null) {
									IsThread = true;
									exp.RunOnUI (() => {
										try {
											run ();
										} catch (Exception ex) {
											AutoLogger.LogError (ex, "ChangedValue 3");
										}
									});
								} else {
									IsThread = false;
									run ();
								}
							}
						}
					} catch (Exception e) {
						AutoLogger.LogError (e, "ChangedValue 1 : IsThread:" + IsThread + " " + exp.BindingObject == null ? "null" : exp.BindingObject.ToString ());
					}
					if (exp.BindingExpresion != null)
						exp.BindingExpresion.IsBusy = false;
				}
			} catch (Exception ex) {
				AutoLogger.LogError (ex, "ChangedValue 2");
			}
		}
	}
}

