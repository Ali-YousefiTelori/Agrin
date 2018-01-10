using System;
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
                var destinationProperty = objTo.GetType().GetProperty(fromProperty.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                object obj1 = destinationProperty;
                if (fromProperty.CanWrite && obj1 != null)
                {
                    //if (fromProperty.Name == "Capacity" || fromProperty.Name == "DownloadRangePositions")
                    //    continue;
                    if (MatchingProps(fromProperty, destinationProperty))
                    {
                        var val = fromProperty.GetValue(objFrom, new object[] { });
                        try
                        {
                            if (manualMapProperties != null && manualMapProperties.Contains(fromProperty.Name))
                                destinationProperty.SetValue(objTo, manualMapReturns(fromProperty.Name, val), null);
                            else
                                destinationProperty.SetValue(objTo, Convert.ChangeType(val, fromProperty.PropertyType), null);
                        }
                        catch (Exception e)
                        {
                            Agrin.Log.AutoLogger.LogError(e, "Map Error");
                        }
                    }
                    else if (MatchingClass(fromProperty, destinationProperty))
                    {
                        if (manualMapProperties != null && manualMapProperties.Contains(fromProperty.Name))
                        {
                            var getValue = fromProperty.GetValue(objFrom, new object[] { });
                            //var getValue = cannotConvert(objFrom, null);
                            //var instance = Activator.CreateInstance(destinationProperty.PropertyType);
                            //instance = cannotConvert(getValue, instance);
                            destinationProperty.SetValue(objTo, manualMapReturns(fromProperty.Name, getValue), new object[] { });
                            //SyncProperties(getValue, instance, cannotConvert);
                        }
                        else if (fromProperty.GetIndexParameters().Length == 0)
                        {
                            var getValue = fromProperty.GetValue(objFrom, null);
                            List<object> parameters = new List<object>();
                            foreach (var par in destinationProperty.GetIndexParameters())
                            {
                                parameters.Add(par.DefaultValue);
                            }
                            var instance = Activator.CreateInstance(destinationProperty.PropertyType, parameters.ToArray());
                            destinationProperty.SetValue(objTo, instance, null);
                            SyncProperties(getValue, instance, manualMapReturns, manualMapProperties);
                        }
                        //Map(fromProperty, destinationProperty);
                    }
                }
            }
            //Log.AutoLogger.AppedLog(Log.StateMode.End, "Agrin.Data.Mapping.Mapper", "SyncProperties<FromType, ToType>", new object[] { objFrom, objTo, manualMapReturns, manualMapProperties });
        }

        // Rules...
        static Func<PropertyInfo, PropertyInfo, bool> MatchingProps = (T1, T2) => T1.Name == T2.Name && T1.PropertyType.Name == T2.PropertyType.Name;
        static Func<PropertyInfo, PropertyInfo, bool> MatchingClass = (T1, T2) => T1.Name == T2.Name && T1.PropertyType.IsClass && T2.PropertyType.IsClass;
        static Func<FieldInfo, FieldInfo, bool> MatchingFields = (T1, T2) => T1.Name == T2.Name && T1.FieldType.Name == T2.FieldType.Name;
        static Func<PropertyInfo, FieldInfo, bool> MatchingPropertyToField = (T1, T2) => T1.Name == T2.Name && T1.PropertyType.Name == T2.FieldType.Name;
        static Func<FieldInfo, PropertyInfo, bool> MatchingFieldToProperty = (T1, T2) => T1.Name == T2.Name && T1.FieldType.Name == T2.PropertyType.Name;

    }
}
