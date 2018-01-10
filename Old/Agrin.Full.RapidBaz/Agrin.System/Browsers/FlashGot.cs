using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.OS.Browsers
{
    public static class FlashGot
    {
        public static bool SetFlashGotSettingForMozilla(string savePath)
        {
            try
            {
                string strSystemUname = Environment.UserName.ToString().Trim();
                string systemDrive = Environment.ExpandEnvironmentVariables("%SystemDrive%");
                string strDirectory = "";
                string strPrefFolder = "";
                if (Directory.Exists(systemDrive + "\\Documents and Settings\\" + strSystemUname + "\\Application Data\\Mozilla\\Firefox\\Profiles"))
                {
                    strDirectory = systemDrive + "\\Documents and Settings\\" + strSystemUname + "\\Application Data\\Mozilla\\Firefox\\Profiles";
                }
                else if (Directory.Exists(systemDrive + "\\WINDOWS\\Application Data\\Mozilla\\Firefox\\Profiles"))
                {
                    strDirectory = systemDrive + "\\WINDOWS\\Application Data\\Mozilla\\Firefox\\Profiles";
                }
                if (strDirectory.Trim().Length != 0)
                {
                    System.IO.DirectoryInfo oDir = new DirectoryInfo(strDirectory);
                    //System.IO.DirectoryInfo[] oSubDir;
                    //oSubDir = oDir.GetDirectories(strDirectory);
                    foreach (DirectoryInfo oFolder in oDir.GetDirectories())
                    {
                        if (oFolder.FullName.IndexOf(".default") >= 0)
                        {
                            strPrefFolder = oFolder.FullName;
                            CreatePrefs(savePath, strPrefFolder);
                        }
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                //AutoLogger.LogError(e, "SetMozilla");
            }
            return false;
        }

        static void CreatePrefs(string appStartUp, string strFolder)
        {
            List<string> lines = File.ReadAllLines(strFolder + "\\prefs.js", Encoding.UTF8).ToList();
            List<string> writeLines = new List<string>();
            Dictionary<string, string> editLines = new Dictionary<string, string>() { 
            { "\"flashgot.custom\"", "\"Agrin Download Manager\"" } ,
            { "\"flashgot.custom.Agrin_Download_Manager.args\"", "\"[ULIST]\"" } ,
            { "\"flashgot.custom.Agrin_Download_Manager.exe\"","\""+ appStartUp.Replace("\\","\\\\")+"\"" } ,
            { "\"flashgot.defaultDM\"","\"Agrin Download Manager\"" } ,
            { "\"flashgot.detect.cache\"","\"(Browser Built In),pyLoad,Internet Download Manager,Free Download Manager,FlashGet 2.x,FlashGet 2,DTA (Turbo),DTA,Agrin Download Manager\"" } ,
            { "\"flashgot.dmsopts.Agrin_Download_Manager.shownInContextMenu\"","true" } ,
            { "\"flashgot.media.dm\"","\"Agrin Download Manager\"" } , };
            int lastLines = 0;
            int index = 0;
            foreach (var item in lines)
            {
                if (lastLines == 0 && item.Contains("user_pref("))
                {
                    lastLines = index;
                }
                bool aded = false;
                foreach (var key in editLines.ToList())
                {
                    if (item.Contains(key.Key))
                    {
                        writeLines.Add("user_pref(" + key.Key + ", " + key.Value + ");");
                        editLines.Remove(key.Key);
                        lastLines = index;
                        aded = true;
                        break;
                    }
                }
                if (!aded)
                    writeLines.Add(item);
                index++;
            }
            foreach (var key in editLines.ToList())
            {
                writeLines.Insert(index, "user_pref(" + key.Key + ", " + key.Value + ");");
            }
            File.WriteAllLines(strFolder + "\\prefs.js", writeLines.ToArray(), Encoding.UTF8);
        }
    }
}
