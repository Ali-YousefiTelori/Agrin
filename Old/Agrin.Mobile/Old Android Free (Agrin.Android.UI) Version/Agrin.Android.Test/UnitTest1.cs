using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Agrin.MonoAndroid.UI;
using System.Collections.Generic;

namespace Agrin.Android.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestBindingHelper()
        {
            PersonTest person = new PersonTest();

            Action<string> ali = (s) =>
                {

                };

            Action<string> bbb = (s) =>
               {

               };

            BindingHelper.AddActionPropertyChanged(ali, person, new List<string>() {
				"DownloadedSize",
				"State",
				"Length","Name"
			});

            BindingHelper.AddActionPropertyChanged(bbb, person, new List<string>() {
				"DownloadedSize",
				"State",
				"Length","Name"
			});
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                int i = 0;
                while(true)
                {
                    try
                    {
                        person.Name = System.IO.Path.GetRandomFileName();
                        i++;
                        if (i == 100)
                        {
                            BindingHelper.RemoveActionPropertyChanged(person, ali);
                        }
                    }
                    catch
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                   
                }
            });
            thread.Start();
        }
    }
}
