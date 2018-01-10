using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Agrin.Data.Mapping
{


    public static class Mapper
    {
        //private static Dictionary<KeyValuePair<Type, Type>, object> _maps = new Dictionary<KeyValuePair<Type, Type>, object>();

        //public static void AddMap<TFrom, TTo>(Action<TFrom, TTo> Map = null)
        //    where TFrom : class
        //    where TTo : class
        //{
        //    _maps.Add(new KeyValuePair<Type, Type>(typeof(TFrom), typeof(TTo)), Map);
        //}

        public static void Map<FromType, ToType>(FromType From, ToType To, Func<string, object, object> manualMapReturns = null, List<string> manualMapProperties = null)
        {
            //zzvar key = new KeyValuePair<Type, Type>(typeof(FromType), typeof(ToType));
            //var map = (Action<FromType, ToType>)_maps[key];

            //var hasMapping = _maps.Any(x => x.Key.Equals(key));

            //if (!hasMapping)
            //    throw new Exception(
            //        string.Format("No map defined for {0} => {1}",
            //            typeof(FromType).Name, typeof(ToType).Name));

            var tFrom = typeof(FromType);
            var tTo = typeof(ToType);

            //TFromProperties = tFrom.GetProperties();
            //TFromFields = tFrom.GetFields();
            //TToProperties = tTo.GetProperties();
            //TToFields = tTo.GetFields();

            SyncProperties(From, To, manualMapReturns, manualMapProperties);
            // Sync and mapped data, override anything that auto synced with mapping action.
            //if (map != null)
            //    map(From, To);
        }

        private static void SyncProperties<FromType, ToType>(FromType objFrom, ToType objTo, Func<string, object, object> manualMapReturns = null, List<string> manualMapProperties = null)
        {
            //Log.AutoLogger.AppedLog(Log.StateMode.Start, "Agrin.Data.Mapping.Mapper", "SyncProperties<FromType, ToType>", new object[] { objFrom, objTo, manualMapReturns, manualMapProperties });
            if (objFrom == null)
                return;
            var prop = objFrom.GetType().GetProperties();
            foreach (var fromProperty in prop)
            {
                if (fromProperty == null || string.IsNullOrEmpty(fromProperty.Name))
                    continue;
                try
                {
                    var destinationProperty = objTo.GetType().GetProperty(fromProperty.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    object obj1 = destinationProperty;
                    if (fromProperty.CanWrite && obj1 != null)
                    {
                        //if (fromProperty.Name == "Capacity" || fromProperty.Name == "DownloadRa0ngePositions")
                        //    continue;
                        if (MatchingProps(fromProperty, destinationProperty))
                        {
                            var val = fromProperty.GetValue(objFrom, new object[] { });
                            object newVal = null;
                            try
                            {
                                if (val is IList)
                                    newVal = Enumerable.ToList(((dynamic)val));
                                else
                                    newVal = val;

                                if (manualMapProperties != null && manualMapProperties.Contains(fromProperty.Name))
                                    destinationProperty.SetValue(objTo, manualMapReturns(fromProperty.Name, newVal), null);
                                else
                                    destinationProperty.SetValue(objTo, Convert.ChangeType(newVal, fromProperty.PropertyType), null);
                            }
                            catch (Exception e)
                            {
                                try
                                {
                                    if (manualMapProperties != null && manualMapProperties.Contains(fromProperty.Name))
                                        destinationProperty.SetValue(objTo, manualMapReturns(fromProperty.Name, val), null);
                                    else
                                        destinationProperty.SetValue(objTo, Convert.ChangeType(val, fromProperty.PropertyType), null);
                                }
                                catch (Exception ex)
                                {

                                }
                                Agrin.Log.AutoLogger.LogError(e, "Map Error");
                            }
                        }
                        else if (MatchingClass(fromProperty, destinationProperty))
                        {
                            if (manualMapProperties != null && manualMapProperties.Contains(fromProperty.Name))
                            {
                                var getValue = fromProperty.GetValue(objFrom, new object[] { });
                                try
                                {
                                    if (getValue is IList)
                                        getValue = Enumerable.ToList(((dynamic)getValue));
                                    destinationProperty.SetValue(objTo, manualMapReturns(fromProperty.Name, getValue), new object[] { });
                                }
                                catch (Exception ex)
                                {
                                    getValue = fromProperty.GetValue(objFrom, new object[] { });
                                    destinationProperty.SetValue(objTo, manualMapReturns(fromProperty.Name, getValue), new object[] { });
                                }
                            }
                            else if (fromProperty.GetIndexParameters().Length == 0)
                            {
                                var getValue = fromProperty.GetValue(objFrom, null);
                                Action run = () =>
                                {
                                    List<object> parameters = new List<object>();
                                    foreach (var par in destinationProperty.GetIndexParameters())
                                    {
                                        parameters.Add(par.DefaultValue);
                                    }
                                    var instance = Activator.CreateInstance(destinationProperty.PropertyType, parameters.ToArray());
                                    destinationProperty.SetValue(objTo, instance, null);
                                    SyncProperties(getValue, instance, manualMapReturns, manualMapProperties);
                                };
                                try
                                {
                                    if (getValue is IList)
                                        getValue = Enumerable.ToList(((dynamic)getValue));
                                    run();
                                }
                                catch (Exception ex)
                                {
                                    getValue = fromProperty.GetValue(objFrom, new object[] { });
                                    run();
                                }


                            }
                        }
                        else
                        {
                            var val = fromProperty.GetValue(objFrom, new object[] { });
                            destinationProperty.SetValue(objTo, val, null);
                            //Map(fromProperty, destinationProperty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Agrin.Log.AutoLogger.LogError(ex, "Map Error 2 " + GetPropertyInfo(fromProperty));
                }
            }
            //Log.AutoLogger.AppedLog(Log.StateMode.End, "Agrin.Data.Mapping.Mapper", "SyncProperties<FromType, ToType>", new object[] { objFrom, objTo, manualMapReturns, manualMapProperties });
        }

        static string GetPropertyInfo(PropertyInfo p)
        {
            if (p == null)
                return "Property Is Null";
            if (string.IsNullOrEmpty(p.Name))
                return "Property Name Is Null";
            return p.Name;
        }

        // Rules...
        static Func<PropertyInfo, PropertyInfo, bool> MatchingProps = (T1, T2) => T1.Name == T2.Name && T1.PropertyType.Name == T2.PropertyType.Name;
        static Func<PropertyInfo, PropertyInfo, bool> MatchingClass = (T1, T2) => T1.Name == T2.Name && T1.PropertyType.IsClass && T2.PropertyType.IsClass;
        static Func<FieldInfo, FieldInfo, bool> MatchingFields = (T1, T2) => T1.Name == T2.Name && T1.FieldType.Name == T2.FieldType.Name;
        static Func<PropertyInfo, FieldInfo, bool> MatchingPropertyToField = (T1, T2) => T1.Name == T2.Name && T1.PropertyType.Name == T2.FieldType.Name;
        static Func<FieldInfo, PropertyInfo, bool> MatchingFieldToProperty = (T1, T2) => T1.Name == T2.Name && T1.FieldType.Name == T2.PropertyType.Name;

    }
}
