namespace Xamarin.InAppBilling
{
    using System;
    using System.CodeDom.Compiler;
    using System.Runtime.InteropServices;

    [GeneratedCode("simple-json", "1.0.0")]
    internal interface IJsonSerializerStrategy
    {
        object DeserializeObject(object value, Type type);
        bool TrySerializeNonPrimitiveObject(object input, out object output);
    }
}

