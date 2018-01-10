namespace Agrin.ViewModels.UI.DragDrop.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class TypeUtilities
    {
        public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
        {
            Type commonBaseClass = GetCommonBaseClass(source);
            Type type2 = typeof(List<>).MakeGenericType(new Type[] { commonBaseClass });
            MethodInfo method = type2.GetMethod("Add");
            object obj2 = type2.GetConstructor(Type.EmptyTypes).Invoke(null);
            foreach (object obj3 in source)
            {
                method.Invoke(obj2, new object[] { obj3 });
            }
            return (IEnumerable) obj2;
        }

        public static Type GetCommonBaseClass(IEnumerable e)
        {
            return GetCommonBaseClass((from o in e.Cast<object>() select o.GetType()).ToArray<Type>());
        }

        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
            {
                return typeof(object);
            }
            Type c = types[0];
            for (int i = 1; i < types.Length; i++)
            {
                if (types[i].IsAssignableFrom(c))
                {
                    c = types[i];
                }
                else
                {
                    while (!c.IsAssignableFrom(types[i]))
                    {
                        c = c.BaseType;
                    }
                }
            }
            return c;
        }
    }
}

