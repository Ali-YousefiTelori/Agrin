using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Agrin.Android.Test.Serializable
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Derialize()
        {
            try
            {
                var binaryFormatter = new BinaryFormatter();
                using (FileStream file = new FileStream(@"C:\Users\Ali Visual Studio\AppData\Roaming\ADM\35\Link.agi", FileMode.Open))
                {
                    var myObject = binaryFormatter.Deserialize(file);
                    var objectProperties = myObject.GetType().GetProperties();
                    foreach (var property in objectProperties)
                    {
                        var propertyTypeName = property.PropertyType.Name; //This will tell you the property Type Name. I.e. string, int64 (long)
                    }
                }

            }
            catch(Exception e)
            {
                var ali = e;
                var reza = ali;
            }
        }
    }
}
