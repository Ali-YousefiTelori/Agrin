using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Agrin.Helper.ComponentModel;
using Agrin.Download.Data;

namespace UnitTestAgrin.Serializtion
{
    sealed class DeserializeBinder : SerializationBinder
    {
        public static Assembly load = null;
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (assemblyName.ToLower().Contains("mscorlib"))
            {
                if (load == null)
                    load = Assembly.LoadFile(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\MonoAndroid\v1.0\mscorlib.dll");
                var tt = load.GetType(typeName);
                return tt;
            }
            return null;
        }
    }
    class ProxyTestClass
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public Object GetData(string name)
        {
            if (data.ContainsKey(name))
            {
                return data[name];
            }
            return null;
        }
        public void SetData(string name, object value)
        {
            data[name] = value;
        }

        public IEnumerable<KeyValuePair<string, object>> Dump()
        {
            return data;
        }
    }

    class SurrogateTestClassConstructor : ISerializationSurrogate
    {
        private ProxyTestClass mProxy;
        /// <summary>
        /// Populates the provided <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the object.
        /// </summary>
        /// <param name="obj">The object to serialize. </param>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Populates the object using the information in the <see cref="T:System.Runtime.Serialization.SerializationInfo"/>.
        /// </summary>
        /// <returns>
        /// The populated deserialized object.
        /// </returns>
        /// <param name="obj">The object to populate. </param>
        /// <param name="info">The information to populate the object. </param>
        /// <param name="context">The source from which the object is deserialized. </param>
        /// <param name="selector">The surrogate selector where the search for a compatible surrogate begins. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (mProxy == null) mProxy = new ProxyTestClass();
            var en = info.GetEnumerator();
            while (en.MoveNext())
            {
                mProxy.SetData(en.Current.Name, en.Current.Value);


            }
            return mProxy;

        }



    }

    public static class TestSerial
    {
        static int _SaveingCount = 0;
        static object loclCH = new object();
        static void ChangeSavingCountValu(int val)
        {
            lock (loclCH)
            {
                _SaveingCount += val;
            }
        }

        //static bool _IsDispose = false;
        //static int errorCount = 0;
        public static void CloseApplicationWaitForSavingAllComplete()
        {
            //_IsDispose = true;
            while (_SaveingCount > 0)
            {
                Thread.Sleep(500);
            }
        }

        public static void VirtualSaveToFile(VirtualSaver virtualSaver, Action action, bool force = false)
        {
            //if (force)
            //    ChangeSavingCountValu(1);
            //if ((virtualSaver.IsSaveing || _IsDispose) && !force)
            //    return;
            //AsyncActions.Action(() =>
            //{
            //    if ((virtualSaver.IsSaveing || _IsDispose) && !force)
            //        return;
            //    virtualSaver.LockCount++;
            //    lock (virtualSaver.Lockobj)
            //    {
            //        virtualSaver.LockCount--;
            //        if ((virtualSaver.LockCount != 0 || _IsDispose) && !force)
            //            return;
            //        ChangeSavingCountValu(1);
            //        virtualSaver.IsSaveing = true;
            //        Thread.Sleep(10);
            //        virtualSaver.IsSaveing = false;
            //        action();
            //        ChangeSavingCountValu(-1);
            //        if (force)
            //            ChangeSavingCountValu(-1);
            //        errorCount = 0;
            //    }
            //}, (error) =>
            //{
            //    ChangeSavingCountValu(-1);
            //    Agrin.Log.AutoLogger.LogError(error, action.ToString());
            //    errorCount++;
            //    if (force)
            //    {
            //        if (errorCount >= 3)
            //            errorCount = 0;
            //        else
            //            VirtualSaveToFile(virtualSaver, action, force);
            //        ChangeSavingCountValu(-1);
            //    }
            //});
        }
    }
    [TestClass]
    public class Serialization
    {
        [TestMethod]
        public void TestDeserializeObject()
        {
            try
            {
                var binaryFormatter = new BinaryFormatter();
                using (FileStream file = new FileStream(@"C:\Users\Ali Visual Studio\AppData\Roaming\ADM\35\Link.agi", FileMode.Open))
                {
                    binaryFormatter.Binder = new DeserializeBinder();
                    var myObject = binaryFormatter.Deserialize(file);
                    var objectProperties = myObject.GetType().GetProperties();
                    foreach (var property in objectProperties)
                    {
                        var propertyTypeName = property.PropertyType.Name; //This will tell you the property Type Name. I.e. string, int64 (long)
                    }
                }

            }
            catch
            {

            }
        }

        [TestMethod]
        public void TestVirtualSaver()
        {
            VirtualSaver ali = new VirtualSaver() { };
            VirtualSaver reza = new VirtualSaver() { };
            VirtualSaver javad = new VirtualSaver() { };

            for (int i = 0; i < 1000; i++)
            {
                TestSerial.VirtualSaveToFile(ali, () =>
                {
                });
                TestSerial.VirtualSaveToFile(reza, () =>
                {

                });
                TestSerial.VirtualSaveToFile(javad, () =>
                {
                    throw new Exception();
                });
            }
            for (int i = 0; i < 300; i++)
            {
                TestSerial.VirtualSaveToFile(reza, () =>
                {

                }, true);
                TestSerial.VirtualSaveToFile(javad, () =>
                {
                    throw new Exception();
                }, true);
            }

            TestSerial.CloseApplicationWaitForSavingAllComplete();
        }
    }
}
