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
using System.IO;
using Agrin.Log;
using Agrin.IO.Helper;

namespace Agrin.MonoAndroid.UI.Models
{
    //[Serializable]
    //public class AndroidApplicationSetting
    //{
    //    public static AndroidApplicationSetting Current { get; set; }

    //    private string _DownloadPath;
    //    public string DownloadPath
    //    {
    //        get { return _DownloadPath; }
    //        set { _DownloadPath = value; }
    //    }

    //    private string _Language;
    //    public string Language
    //    {
    //        get { return _Language; }
    //        set { _Language = value; }
    //    }

    //    public static void LoadSetting(string downloadPath)
    //    {
    //        AutoLogger.LogError(null, "Loaded Setting Started", true);
    //        string path = Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "AgrinAndroidSetting.log");
    //        try
    //        {
    //            if (File.Exists(path))
    //            {
    //                Current = Newtonsoft.Json.JsonConvert.DeserializeObject<AndroidApplicationSetting>(File.ReadAllText(path, Encoding.UTF8));
    //                //Current = (AndroidApplicationSetting)Agrin.IO.Helper.SerializeStream.OpenSerializeStream(path);
    //                AutoLogger.LogError(null, "Loaded Setting: |" + Current.Language + "|" + Current.DownloadPath, true);
    //                //Current.TestProperty = Current.Language;
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            AutoLogger.LogError(e, "Load Setting Error", true);
    //        }
    //        if (Current == null)
    //        {
    //            Current = new AndroidApplicationSetting() { DownloadPath = downloadPath, Language = "new" };
    //            SaveSetting();
    //        }
    //    }

    //    public static void SaveSetting()
    //    {
    //        string path = Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "AgrinAndroidSetting.log");
    //        try
    //        {
    //            AutoLogger.LogError(null, "serial Saving Setting: |" + Current.Language + "|" + Current.DownloadPath, true);
    //            var item = Agrin.IO.Helper.SerializeStream.Deserialize(Agrin.IO.Helper.SerializeStream.Serialize(Current)) as AndroidApplicationSetting;
    //            AutoLogger.LogError(null, "desrial Saving Setting: |" + item.Language + "|" + item.DownloadPath, true);
    //            while (true)
    //            {
    //                File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(Current), Encoding.UTF8);
    //                //SerializeStream.SaveSerializeStream(path, Current);
    //                var des = (AndroidApplicationSetting)Newtonsoft.Json.JsonConvert.DeserializeObject<AndroidApplicationSetting>(File.ReadAllText(path, Encoding.UTF8));
    //                if (des.DownloadPath == Current.DownloadPath && des.Language == Current.Language)
    //                {
    //                    AutoLogger.LogError(null, "is True |" + des.Language + "|" + des.DownloadPath, true);
    //                    break;
    //                }
    //                AutoLogger.LogError(null, "Writing |" + des.Language + "|" + des.DownloadPath, true);
    //                System.Threading.Thread.Sleep(100);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            AutoLogger.LogError(e, "Save Setting Error", true);
    //        }
    //    }
    //}
}