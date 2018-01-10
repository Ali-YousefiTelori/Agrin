using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
namespace Xamarin.InAppBilling.Reflection
{
    [GeneratedCode("reflection-utils", "1.0.0")]
    internal class ReflectionUtils
    {
        // Fields
        private static readonly object[] EmptyObjects = new object[0];

        // Methods
        public static BinaryExpression Assign(Expression left, Expression right)
        {
            Type[] typeArguments = new Type[] { left.Type };
            MethodInfo method = typeof(Assigner<>).MakeGenericType(typeArguments).GetMethod("Assign");
            return Expression.Add(left, right, method);
        }

        public static Attribute GetAttribute(MemberInfo info, Type type)
        {
            if (((info != null) && (type != null)) && Attribute.IsDefined(info, type))
            {
                return Attribute.GetCustomAttribute(info, type);
            }
            return null;
        }

        public static Attribute GetAttribute(Type objectType, Type attributeType)
        {
            if (((objectType != null) && (attributeType != null)) && Attribute.IsDefined(objectType, attributeType))
            {
                return Attribute.GetCustomAttribute(objectType, attributeType);
            }
            return null;
        }

        public static ConstructorDelegate GetConstructorByExpression(ConstructorInfo constructorInfo)
        {
            ParameterExpression expression = null;
            c__AnonStorey1 storey = new c__AnonStorey1();
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            Expression[] arguments = new Expression[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type parameterType = parameters[i].ParameterType;
                arguments[i] = Expression.Convert(Expression.ArrayIndex(expression = Expression.Parameter(typeof(object[]), "args"), index), parameterType);
            }
            ParameterExpression[] expressionArray1 = new ParameterExpression[] { expression };
            storey.compiledLambda = Expression.Lambda<Func<object[], object>>(Expression.New(constructorInfo, arguments), expressionArray1).Compile();
            return new ConstructorDelegate(storey.m__0);
        }

        public static ConstructorDelegate GetConstructorByExpression(Type type, params Type[] argsType)
        {
            ConstructorInfo constructorInfo = GetConstructorInfo(type, argsType);
            return ((constructorInfo != null) ? GetConstructorByExpression(constructorInfo) : null);
        }

        public static ConstructorDelegate GetConstructorByReflection(ConstructorInfo constructorInfo)
        {
            c__AnonStorey0 storey = new c__AnonStorey0
            {
                constructorInfo = constructorInfo
            };
            return new ConstructorDelegate(storey.m__0);
        }

        public static ConstructorDelegate GetConstructorByReflection(Type type, params Type[] argsType)
        {
            ConstructorInfo constructorInfo = GetConstructorInfo(type, argsType);
            return ((constructorInfo != null) ? GetConstructorByReflection(constructorInfo) : null);
        }

        public static ConstructorInfo GetConstructorInfo(Type type, params Type[] argsType)
        {
            foreach (ConstructorInfo info in GetConstructors(type))
            {
                ParameterInfo[] parameters = info.GetParameters();
                if (argsType.Length == parameters.Length)
                {
                    int index = 0;
                    bool flag = true;
                    foreach (ParameterInfo info2 in info.GetParameters())
                    {
                        if (info2.ParameterType != argsType[index])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
            return type.GetConstructors();
        }

        public static ConstructorDelegate GetContructor(ConstructorInfo constructorInfo)
        {
            return GetConstructorByExpression(constructorInfo);
        }

        public static ConstructorDelegate GetContructor(Type type, params Type[] argsType)
        {
            return GetConstructorByExpression(type, argsType);
        }

        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public static Type GetGenericListElementType(Type type)
        {
            foreach (Type type2 in type.GetInterfaces())
            {
                if (IsTypeGeneric(type2) && (type2.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    return GetGenericTypeArguments(type2)[0];
                }
            }
            return GetGenericTypeArguments(type)[0];
        }

        public static Type[] GetGenericTypeArguments(Type type)
        {
            return type.GetGenericArguments();
        }

        public static GetDelegate GetGetMethod(FieldInfo fieldInfo)
        {
            return GetGetMethodByExpression(fieldInfo);
        }

        public static GetDelegate GetGetMethod(PropertyInfo propertyInfo)
        {
            return GetGetMethodByExpression(propertyInfo);
        }

        public static GetDelegate GetGetMethodByExpression(FieldInfo fieldInfo)
        {
            ParameterExpression expression;
            c__AnonStorey5 storey = new c__AnonStorey5();
            MemberExpression expression2 = Expression.Field(Expression.Convert(expression = Expression.Parameter(typeof(object), "instance"), fieldInfo.DeclaringType), fieldInfo);
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            storey.compiled = Expression.Lambda<GetDelegate>(Expression.Convert(expression2, typeof(object)), parameters).Compile();
            return new GetDelegate(storey.m__0);
        }

        public static GetDelegate GetGetMethodByExpression(PropertyInfo propertyInfo)
        {
            ParameterExpression expression = null;
            c__AnonStorey4 storey = new c__AnonStorey4();
            MethodInfo getterMethodInfo = GetGetterMethodInfo(propertyInfo);
            UnaryExpression instance = IsValueType(propertyInfo.DeclaringType) ? Expression.Convert(expression, propertyInfo.DeclaringType) : Expression.TypeAs(expression = Expression.Parameter(typeof(object), "instance"), propertyInfo.DeclaringType);
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            storey.compiled = Expression.Lambda<Func<object, object>>(Expression.TypeAs(Expression.Call(instance, getterMethodInfo), typeof(object)), parameters).Compile();
            return new GetDelegate(storey.m__0);
        }

        public static GetDelegate GetGetMethodByReflection(FieldInfo fieldInfo)
        {
            c__AnonStorey3 storey = new c__AnonStorey3
            {
                fieldInfo = fieldInfo
            };
            return new GetDelegate(storey.m__0);
        }

        public static GetDelegate GetGetMethodByReflection(PropertyInfo propertyInfo)
        {
            c__AnonStorey2 storey = new c__AnonStorey2
            {
                methodInfo = GetGetterMethodInfo(propertyInfo)
            };
            return new GetDelegate(storey.m__0);
        }

        public static MethodInfo GetGetterMethodInfo(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetGetMethod(true);
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public static SetDelegate GetSetMethod(FieldInfo fieldInfo)
        {
            return GetSetMethodByExpression(fieldInfo);
        }

        public static SetDelegate GetSetMethod(PropertyInfo propertyInfo)
        {
            return GetSetMethodByExpression(propertyInfo);
        }

        public static SetDelegate GetSetMethodByExpression(FieldInfo fieldInfo)
        {
            ParameterExpression expression = null;
            ParameterExpression expression2 = null;
            c__AnonStorey9 storey = new c__AnonStorey9();
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
            storey.compiled = Expression.Lambda<Action<object, object>>(Assign(Expression.Field(Expression.Convert(expression = Expression.Parameter(typeof(object), "instance"), fieldInfo.DeclaringType), fieldInfo), Expression.Convert(expression2 = Expression.Parameter(typeof(object), "value"), fieldInfo.FieldType)), parameters).Compile();
            return new SetDelegate(storey.m__0);
        }

        public static SetDelegate GetSetMethodByExpression(PropertyInfo propertyInfo)
        {
            ParameterExpression expression = null;
            ParameterExpression expression2 = null;
            c__AnonStorey8 storey = new c__AnonStorey8();
            MethodInfo setterMethodInfo = GetSetterMethodInfo(propertyInfo);
            UnaryExpression instance = IsValueType(propertyInfo.DeclaringType) ? Expression.Convert(expression, propertyInfo.DeclaringType) : Expression.TypeAs(expression = Expression.Parameter(typeof(object), "instance"), propertyInfo.DeclaringType);
            UnaryExpression expression4 = IsValueType(propertyInfo.PropertyType) ? Expression.Convert(expression2, propertyInfo.PropertyType) : Expression.TypeAs(expression2 = Expression.Parameter(typeof(object), "value"), propertyInfo.PropertyType);
            Expression[] arguments = new Expression[] { expression4 };
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
            storey.compiled = Expression.Lambda<Action<object, object>>(Expression.Call(instance, setterMethodInfo, arguments), parameters).Compile();
            return new SetDelegate(storey.m__0);
        }

        public static SetDelegate GetSetMethodByReflection(FieldInfo fieldInfo)
        {
            c__AnonStorey7 storey = new c__AnonStorey7
            {
                fieldInfo = fieldInfo
            };
            return new SetDelegate(storey.m__0);
        }

        public static SetDelegate GetSetMethodByReflection(PropertyInfo propertyInfo)
        {
            c__AnonStorey6 storey = new c__AnonStorey6
            {
                methodInfo = GetSetterMethodInfo(propertyInfo)
            };
            return new SetDelegate(storey.m__0);
        }

        public static MethodInfo GetSetterMethodInfo(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSetMethod(true);
        }

        public static Type GetTypeInfo(Type type)
        {
            return type;
        }

        public static bool IsAssignableFrom(Type type1, Type type2)
        {
            return GetTypeInfo(type1).IsAssignableFrom(GetTypeInfo(type2));
        }

        public static bool IsNullableType(Type type)
        {
            return (GetTypeInfo(type).IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static bool IsTypeDictionary(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return true;
            }
            if (!GetTypeInfo(type).IsGenericType)
            {
                return false;
            }
            return (type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        public static bool IsTypeGeneric(Type type)
        {
            return GetTypeInfo(type).IsGenericType;
        }

        public static bool IsTypeGenericeCollectionInterface(Type type)
        {
            if (!IsTypeGeneric(type))
            {
                return false;
            }
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            return (((genericTypeDefinition == typeof(IList<>)) || (genericTypeDefinition == typeof(ICollection<>))) || (genericTypeDefinition == typeof(IEnumerable<>)));
        }

        public static bool IsValueType(Type type)
        {
            return GetTypeInfo(type).IsValueType;
        }

        public static object ToNullableType(object obj, Type nullableType)
        {
            return ((obj != null) ? Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture) : null);
        }

        // Nested Types
        [CompilerGenerated]
        private sealed class c__AnonStorey1
        {
            // Fields
            internal Func<object[], object> compiledLambda;

            // Methods
            internal object m__0(object[] args)
            {
                return this.compiledLambda(args);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey0
        {
            // Fields
            internal ConstructorInfo constructorInfo;

            // Methods
            internal object m__0(object[] args)
            {
                return this.constructorInfo.Invoke(args);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey4
        {
            // Fields
            internal Func<object, object> compiled;

            // Methods
            internal object m__0(object source)
            {
                return this.compiled(source);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey5
        {
            // Fields
            internal ReflectionUtils.GetDelegate compiled;

            // Methods
            internal object m__0(object source)
            {
                return this.compiled(source);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey2
        {
            // Fields
            internal MethodInfo methodInfo;

            // Methods
            internal object m__0(object source)
            {
                return this.methodInfo.Invoke(source, ReflectionUtils.EmptyObjects);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey3
        {
            // Fields
            internal FieldInfo fieldInfo;

            // Methods
            internal object m__0(object source)
            {
                return this.fieldInfo.GetValue(source);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey8
        {
            // Fields
            internal Action<object, object> compiled;

            // Methods
            internal void m__0(object source, object val)
            {
                this.compiled(source, val);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey9
        {
            // Fields
            internal Action<object, object> compiled;

            // Methods
            internal void m__0(object source, object val)
            {
                this.compiled(source, val);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey6
        {
            // Fields
            internal MethodInfo methodInfo;

            // Methods
            internal void m__0(object source, object value)
            {
                object[] parameters = new object[] { value };
                this.methodInfo.Invoke(source, parameters);
            }
        }

        [CompilerGenerated]
        private sealed class c__AnonStorey7
        {
            // Fields
            internal FieldInfo fieldInfo;

            // Methods
            internal void m__0(object source, object value)
            {
                this.fieldInfo.SetValue(source, value);
            }
        }

        private static class Assigner<T>
        {
            // Methods
            public static T Assign(ref T left, T right)
            {
                T local;
                left = local = right;
                return local;
            }
        }

        public delegate object ConstructorDelegate(params object[] args);

        public delegate object GetDelegate(object source);

        public delegate void SetDelegate(object source, object value);

        public sealed class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
        {
            // Fields
            private Dictionary<TKey, TValue> _dictionary;
            private readonly object _lock;
            private readonly ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> _valueFactory;

            // Methods
            public ThreadSafeDictionary(ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> valueFactory)
            {
                this._lock = new object();
                this._valueFactory = valueFactory;
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void Add(TKey key, TValue value)
            {
                throw new NotImplementedException();
            }

            private TValue AddValue(TKey key)
            {
                TValue local = this._valueFactory(key);
                object obj2 = this._lock;
                lock (obj2)
                {
                    TValue local2;
                    if (this._dictionary == null)
                    {
                        this._dictionary = new Dictionary<TKey, TValue>();
                        this._dictionary[key] = local;
                        return local;
                    }
                    if (this._dictionary.TryGetValue(key, out local2))
                    {
                        return local2;
                    }
                    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._dictionary);
                    dictionary[key] = local;
                    this._dictionary = dictionary;
                }
                return local;
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(TKey key)
            {
                return this._dictionary.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            private TValue Get(TKey key)
            {
                TValue local;
                if (this._dictionary == null)
                {
                    return this.AddValue(key);
                }
                if (!this._dictionary.TryGetValue(key, out local))
                {
                    return this.AddValue(key);
                }
                return local;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return this._dictionary.GetEnumerator();
            }

            public bool Remove(TKey key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this._dictionary.GetEnumerator();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                value = this[key];
                return true;
            }

            // Properties
            public int Count
            {
                get
                {
                    return this._dictionary.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public TValue this[TKey key]
            {
                get
                {
                    return this.Get(key);
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<TKey> Keys
            {
                get
                {
                    return this._dictionary.Keys;
                }
            }

            public ICollection<TValue> Values
            {
                get
                {
                    return this._dictionary.Values;
                }
            }
        }

        public delegate TValue ThreadSafeDictionaryValueFactory<TKey, TValue>(TKey key);
    }
}