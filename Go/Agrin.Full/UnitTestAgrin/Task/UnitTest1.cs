using System;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestAgrin.Task
{
    [TestClass]
    public class TaskTest
    {
        [TestMethod]
        public void TaskTestStart()
        {
            Agrin.IO.Helper.MPath.CurrentAppDirectory = Directory.GetCurrentDirectory();
            var current = ApplicationTaskManager.Current = new ApplicationTaskManager((tsk) =>
            {

            }, (tsk) =>
            {

            });
            var task = new TaskInfo() { IsActive = true };
            task.DateTimes.Add(DateTime.Now.AddMinutes(1));
            current.AddTask(task);
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
