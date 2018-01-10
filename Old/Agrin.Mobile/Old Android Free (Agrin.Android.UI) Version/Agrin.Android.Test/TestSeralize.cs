using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Agrin.Android.Test
{
    [Serializable]
    public class AndroidApplicationSetting
    {
        public static AndroidApplicationSetting Current { get; set; }
        public string DownloadPath { get; set; }
        public string Language { get; set; }
        public static void LoadSetting(string downloadPath)
        {
            Agrin.IO.Helper.MPath.CurrentAppDirectory = "D:\\";
            string path = Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "AgrinAndroidSetting.agn");
            try
            {
                if (File.Exists(path))
                {
                    Current = (AndroidApplicationSetting)Agrin.IO.Helper.SerializeStream.OpenSerializeStream(path);
                    //AutoLogger.LogError(null, "Loaded Setting: |" + loaded.Language + "|" + loaded.DownloadPath, true);
                    //Current = null;
                    //Current = loaded;
                }
            }
            catch (Exception e)
            {
                //AutoLogger.LogError(e, "Load Setting Error", true);
            }
            if (Current == null)
            {
                Current = new AndroidApplicationSetting() { DownloadPath = downloadPath, Language = "new" };
                SaveSetting();
            }
        }
        public static void SaveSetting()
        {
            string path = Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "AgrinAndroidSetting.agn");
            try
            {
                Agrin.IO.Helper.SerializeStream.SaveSerializeStream(path, Current);
            }
            catch (Exception e)
            {
            }
        }
    }
    [TestClass]
    public class TestSeralize
    {
        [TestMethod]
        public void TestSeralizeAction()
        {
            AndroidApplicationSetting.LoadSetting("C:\\");
            SaveLanguage("persian");
            AndroidApplicationSetting.LoadSetting("C:\\");
            LoadLanguage();
        }

        bool IsSetLanguage;
        public void SaveLanguage(string lang)
        {
            if (lang == "english")
            {
                IsSetLanguage = true;
            }
            else if (lang == "persian")
            {
                IsSetLanguage = true;
            }
            AndroidApplicationSetting.Current.Language = lang;
            AndroidApplicationSetting.SaveSetting();
        }
        
        public void LoadLanguage()
        {
            IsSetLanguage = true;
            string lang = AndroidApplicationSetting.Current.Language;
            if (lang == "english")
            {

            }
            else if (lang == "persian")
            {

            }
            else
                IsSetLanguage = false;
        }
    }
}
