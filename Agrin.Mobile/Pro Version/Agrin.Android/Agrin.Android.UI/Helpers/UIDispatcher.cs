using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using Agrin.Helpers;
using System.Diagnostics;
using Agrin.Log;

namespace System
{
    //public class UIDispatcherItem
    //{
    //    public Action Run { get; set; }
    //    public Activity Context { get; set; }
    //    public System.Threading.ManualResetEvent OldResetEvent { get; set; }
    //}

    //public static class UIDispatcher
    //{
    //    static List<UIDispatcherItem> itemsForRender = new List<UIDispatcherItem>();
    //    static bool IsBusy { get; set; }
    //    static object lockOBJ = new object();

    //    static UIDispatcher()
    //    {
    //        System.Threading.Tasks.Task task = new Threading.Tasks.Task(() =>
    //        {
    //            while (true)
    //            {
    //                if (!IsBusy && itemsForRender.Count > 0)
    //                {
    //                    var first = itemsForRender.First();
    //                    RunOnUI(first.Context, first.Run, oldResetEvent: first.OldResetEvent);
    //                    lock (lockOBJ)
    //                    {
    //                        itemsForRender.Remove(first);
    //                    }
    //                }
    //                else
    //                {
    //                    System.Threading.Thread.Sleep(1000);
    //                }
    //            }
    //        });
    //        task.Start();
    //    }

    //    public static void RunOnUI(this Activity activity, Action run, bool isWait = false, System.Threading.ManualResetEvent oldResetEvent = null)
    //    {
    //        System.Threading.ManualResetEvent resetEvent = null;
    //        if (isWait)
    //        {
    //            if (oldResetEvent == null)
    //            {
    //                resetEvent = new System.Threading.ManualResetEvent(false);
    //            }
    //            else
    //                resetEvent = oldResetEvent;
    //        }
    //        lock (lockOBJ)
    //        {
    //            if (IsBusy)
    //            {
    //                UIDispatcherItem uIDispatcherItem = new UIDispatcherItem() { Run = run, Context = activity };
    //                itemsForRender.Add(uIDispatcherItem);
    //                if (isWait)
    //                {
    //                    IsBusy = true;
    //                    uIDispatcherItem.OldResetEvent = resetEvent;
    //                    resetEvent.WaitOne();
    //                    IsBusy = false;
    //                }
    //                return;
    //            }
    //            IsBusy = true;
    //        }


    //        activity.RunOnUiThread(() =>
    //        {
    //            run();
    //            if (isWait)
    //                resetEvent.Set();
    //            lock (lockOBJ)
    //            {
    //                IsBusy = false;
    //            }
    //        });
    //        if (isWait && oldResetEvent == null)
    //            resetEvent.WaitOne();
    //    }
    //}

    public class UIDispatcherItem
    {
        public Action Run { get; set; }
        public Activity Context { get; set; }
        public ManualResetEvent OldResetEvent { get; set; }
    }

    public static class UIDispatcher
    {
        public static void RunOnUI(this Activity activity, Action run, bool isWait = false)
        {
            ManualResetEvent resetEvent = null;
            if (isWait)
                resetEvent = new ManualResetEvent(false);
            if (activity == null)
            {
                //try
                //{
                //    AutoLogger.LogText("RunOnUI activity is null!!!!");
                //    StringBuilder builder = new StringBuilder();
                //    builder.AppendLine("<------------------------------StackTrace One Begin------------------------------>");
                //    var stackTrace = new StackTrace(true);
                //    StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

                //    // write call stack method names
                //    foreach (StackFrame stackFrame in stackFrames)
                //    {
                //        var method = stackFrame.GetMethod();
                //        builder.AppendLine("<---Method Begin--->");
                //        builder.AppendLine("File Name: " + stackFrame.GetFileName());
                //        builder.AppendLine("Line Number: " + stackFrame.GetFileLineNumber());
                //        builder.AppendLine("Column Number: " + stackFrame.GetFileColumnNumber());

                //        builder.AppendLine("Name: " + method.Name);
                //        builder.AppendLine("Class: " + method.DeclaringType.Name);
                //        var param = method.GetParameters();
                //        builder.AppendLine("Params Count: " + param.Length);
                //        int i = 1;
                //        foreach (var p in param)
                //        {
                //            builder.AppendLine("Param " + i + ":" + p.ParameterType.Name);
                //            i++;
                //        }
                //        builder.AppendLine("<---Method End--->");
                //    }
                //    builder.AppendLine("<------------------------------StackTrace One End------------------------------>");
                //    AutoLogger.LogText(builder.ToString());
                //}
                //catch (Exception ex)
                //{
                //    InitializeApplication.GoException(ex, "Run On UI TH?");
                //}
                return;
            }
            activity.RunOnUiThread(() =>
            {
                try
                {
                    run();
                    if (isWait)
                        resetEvent.Set();
                }
                catch (Exception ex)
                {
                    InitializeApplication.GoException(ex, "RunOnUI" + (activity == null ? "nullA" : activity.GetType().FullName));
                    if (isWait)
                        resetEvent.Set();
                }

            });
            if (isWait)
                resetEvent.WaitOne();
        }
    }
}