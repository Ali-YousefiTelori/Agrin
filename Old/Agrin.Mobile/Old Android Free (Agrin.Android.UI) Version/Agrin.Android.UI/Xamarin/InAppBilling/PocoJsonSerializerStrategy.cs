// Type: Xamarin.InAppBilling.PocoJsonSerializerStrategy
// Assembly: Xamarin.InAppBilling, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 312DE16D-65DB-4E32-94A1-D49884001984
// Assembly location: D:\Projects\xamarin.inappbilling-2.2\xamarin.inappbilling-2.2\lib\android\Xamarin.InAppBilling.dll

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Xamarin.InAppBilling.Reflection;

namespace Xamarin.InAppBilling
{
    [GeneratedCode("simple-json", "1.0.0")]
    internal class PocoJsonSerializerStrategy : IJsonSerializerStrategy
    {
        internal static readonly Type[] EmptyTypes = new Type[0];
        internal IDictionary<Type, ReflectionUtils.ConstructorDelegate> ConstructorCache;
        internal IDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>> GetCache;
        internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> SetCache;
        internal static readonly Type[] ArrayConstructorParameterTypes;
        private static readonly string[] Iso8601Format;

        static PocoJsonSerializerStrategy()
        {
            Type[] typeArray = new Type[1];
            int index1 = 0;
            Type type = typeof(int);
            typeArray[index1] = type;
            PocoJsonSerializerStrategy.ArrayConstructorParameterTypes = typeArray;
            string[] strArray = new string[3];
            int index2 = 0;
            string str1 = "yyyy-MM-dd\\THH:mm:ss.FFFFFFF\\Z";
            strArray[index2] = str1;
            int index3 = 1;
            string str2 = "yyyy-MM-dd\\THH:mm:ss\\Z";
            strArray[index3] = str2;
            int index4 = 2;
            string str3 = "yyyy-MM-dd\\THH:mm:ssK";
            strArray[index4] = str3;
            PocoJsonSerializerStrategy.Iso8601Format = strArray;
        }

        public PocoJsonSerializerStrategy()
        {
            this.ConstructorCache = (IDictionary<Type, ReflectionUtils.ConstructorDelegate>)new ReflectionUtils.ThreadSafeDictionary<Type, ReflectionUtils.ConstructorDelegate>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, ReflectionUtils.ConstructorDelegate>(this.ContructorDelegateFactory));
            this.GetCache = (IDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>>)new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(this.GetterValueFactory));
            this.SetCache = (IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>)new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(this.SetterValueFactory));
        }

        protected virtual string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            return clrPropertyName;
        }

        internal virtual ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
        {
            return ReflectionUtils.GetContructor(key, !key.IsArray ? PocoJsonSerializerStrategy.EmptyTypes : PocoJsonSerializerStrategy.ArrayConstructorParameterTypes);
        }

        internal virtual IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
        {
            IDictionary<string, ReflectionUtils.GetDelegate> dictionary = (IDictionary<string, ReflectionUtils.GetDelegate>)new Dictionary<string, ReflectionUtils.GetDelegate>();
            foreach (PropertyInfo propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (propertyInfo.CanRead)
                {
                    MethodInfo getterMethodInfo = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
                    if (!getterMethodInfo.IsStatic && getterMethodInfo.IsPublic)
                        dictionary[this.MapClrMemberNameToJsonFieldName(propertyInfo.Name)] = ReflectionUtils.GetGetMethod(propertyInfo);
                }
            }
            foreach (FieldInfo fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (!fieldInfo.IsStatic && fieldInfo.IsPublic)
                    dictionary[this.MapClrMemberNameToJsonFieldName(fieldInfo.Name)] = ReflectionUtils.GetGetMethod(fieldInfo);
            }
            return dictionary;
        }

        internal virtual IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
        {
            IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> dictionary = (IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>)new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
            foreach (PropertyInfo propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (propertyInfo.CanWrite)
                {
                    MethodInfo setterMethodInfo = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
                    if (!setterMethodInfo.IsStatic && setterMethodInfo.IsPublic)
                        dictionary[this.MapClrMemberNameToJsonFieldName(propertyInfo.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(propertyInfo.PropertyType, ReflectionUtils.GetSetMethod(propertyInfo));
                }
            }
            foreach (FieldInfo fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (!fieldInfo.IsInitOnly && !fieldInfo.IsStatic && fieldInfo.IsPublic)
                    dictionary[this.MapClrMemberNameToJsonFieldName(fieldInfo.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType, ReflectionUtils.GetSetMethod(fieldInfo));
            }
            return dictionary;
        }

        public virtual bool TrySerializeNonPrimitiveObject(object input, out object output)
        {
            if (!this.TrySerializeKnownTypes(input, out output))
                return this.TrySerializeUnknownTypes(input, out output);
            else
                return true;
        }

        public virtual object DeserializeObject(object value, Type type)
        {
            if (type == (Type)null)
                throw new ArgumentNullException("type");
            string str = value as string;
            if (type == typeof(Guid) && string.IsNullOrEmpty(str))
                return (object)new Guid();
            if (value == null)
                return (object)null;
            object source = (object)null;
            if (str != null)
            {
                if (str.Length != 0)
                {
                    if (type == typeof(DateTime) || ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTime))
                        return (object)DateTime.ParseExact(str, PocoJsonSerializerStrategy.Iso8601Format, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
                    if (type == typeof(DateTimeOffset) || ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset))
                        return (object)DateTimeOffset.ParseExact(str, PocoJsonSerializerStrategy.Iso8601Format, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
                    if (type == typeof(Guid) || ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
                        return (object)new Guid(str);
                    if (type == typeof(Uri))
                    {
                        Uri result;
                        if (Uri.IsWellFormedUriString(str, UriKind.RelativeOrAbsolute) && Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out result))
                            return (object)result;
                        else
                            return (object)null;
                    }
                    else if (type == typeof(string))
                        return (object)str;
                    else
                        return Convert.ChangeType((object)str, type, (IFormatProvider)CultureInfo.InvariantCulture);
                }
                else
                {
                    source = !(type == typeof(Guid)) ? (!ReflectionUtils.IsNullableType(type) || !(Nullable.GetUnderlyingType(type) == typeof(Guid)) ? (object)str : (object)null) : (object)new Guid();
                    if (!ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
                        return (object)str;
                }
            }
            else if (value is bool)
                return value;
            bool flag1 = value is long;
            bool flag2 = value is double;
            if (flag1 && type == typeof(long) || flag2 && type == typeof(double))
                return value;
            if (flag2 && type != typeof(double) || flag1 && type != typeof(long))
            {
                object obj = type == typeof(int) || type == typeof(long) || (type == typeof(double) || type == typeof(float)) || (type == typeof(bool) || type == typeof(Decimal) || (type == typeof(byte) || type == typeof(short))) ? Convert.ChangeType(value, type, (IFormatProvider)CultureInfo.InvariantCulture) : value;
                if (ReflectionUtils.IsNullableType(type))
                    return ReflectionUtils.ToNullableType(obj, type);
                else
                    return obj;
            }
            else
            {
                IDictionary<string, object> dictionary1 = value as IDictionary<string, object>;
                if (dictionary1 != null)
                {
                    IDictionary<string, object> dictionary2 = dictionary1;
                    if (ReflectionUtils.IsTypeDictionary(type))
                    {
                        Type[] genericTypeArguments = ReflectionUtils.GetGenericTypeArguments(type);
                        Type type1 = genericTypeArguments[0];
                        Type type2 = genericTypeArguments[1];
                        Type type3 = typeof(Dictionary<,>);
                        Type[] typeArray = new Type[2];
                        int index1 = 0;
                        Type type4 = type1;
                        typeArray[index1] = type4;
                        int index2 = 1;
                        Type type5 = type2;
                        typeArray[index2] = type5;
                        IDictionary dictionary3 = (IDictionary)this.ConstructorCache[type3.MakeGenericType(typeArray)](new object[0]);
                        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>)dictionary2)
                            dictionary3.Add((object)keyValuePair.Key, this.DeserializeObject(keyValuePair.Value, type2));
                        source = (object)dictionary3;
                    }
                    else if (type == typeof(object))
                    {
                        source = value;
                    }
                    else
                    {
                        source = this.ConstructorCache[type](new object[0]);
                        foreach (KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> keyValuePair in (IEnumerable<KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>)this.SetCache[type])
                        {
                            object obj;
                            if (dictionary2.TryGetValue(keyValuePair.Key, out obj))
                            {
                                obj = this.DeserializeObject(obj, keyValuePair.Value.Key);
                                keyValuePair.Value.Value(source, obj);
                            }
                        }
                    }
                }
                else
                {
                    IList<object> list1 = value as IList<object>;
                    if (list1 != null)
                    {
                        IList<object> list2 = list1;
                        IList list3 = (IList)null;
                        if (type.IsArray)
                        {
                            object[] args = new object[] { list2.Count };
                            list3 = (IList)this.ConstructorCache[type](args);
                            int num = 0;
                            foreach (object obj4 in list2)
                            {
                                list3[num++] = this.DeserializeObject(obj4, type.GetElementType());
                            }

                        }
                        else if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || ReflectionUtils.IsAssignableFrom(typeof(IList), type))
                        {
                            Type[] typeArray2 = null;
                            Type genericListElementType = ReflectionUtils.GetGenericListElementType(type);
                            ReflectionUtils.ConstructorDelegate local1 = this.ConstructorCache[type];
                            if (local1 == null)
                            {
                                typeArray2 = new Type[] { genericListElementType };
                            }
                            object[] objArray2 = new object[] { list2.Count };
                            list3 = (IList)this.ConstructorCache[typeof(List<>).MakeGenericType(typeArray2)](objArray2);
                            foreach (object obj5 in list2)
                            {
                                list3.Add(this.DeserializeObject(obj5, genericListElementType));
                            }

                        }
                        source = (object)list3;
                    }
                }
                return source;
            }
        }

        protected virtual object SerializeEnum(Enum p)
        {
            return (object)Convert.ToDouble((object)p, (IFormatProvider)CultureInfo.InvariantCulture);
        }

        protected virtual bool TrySerializeKnownTypes(object input, out object output)
        {
            bool flag = true;
            if (input is DateTime)
                output = (object)((DateTime)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], (IFormatProvider)CultureInfo.InvariantCulture);
            else if (input is DateTimeOffset)
                output = (object)((DateTimeOffset)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], (IFormatProvider)CultureInfo.InvariantCulture);
            else if (input is Guid)
                output = (object)((Guid)input).ToString("D");
            else if (input is Uri)
            {
                output = (object)input.ToString();
            }
            else
            {
                Enum p = input as Enum;
                if (p != null)
                {
                    output = this.SerializeEnum(p);
                }
                else
                {
                    flag = false;
                    output = (object)null;
                }
            }
            return flag;
        }

        protected virtual bool TrySerializeUnknownTypes(object input, out object output)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            output = (object)null;
            Type type = input.GetType();
            if (type.FullName == null)
                return false;
            IDictionary<string, object> dictionary = (IDictionary<string, object>)new JsonObject();
            foreach (KeyValuePair<string, ReflectionUtils.GetDelegate> keyValuePair in (IEnumerable<KeyValuePair<string, ReflectionUtils.GetDelegate>>)this.GetCache[type])
            {
                if (keyValuePair.Value != null)
                    dictionary.Add(this.MapClrMemberNameToJsonFieldName(keyValuePair.Key), keyValuePair.Value(input));
            }
            output = (object)dictionary;
            return true;
        }
    }
}
