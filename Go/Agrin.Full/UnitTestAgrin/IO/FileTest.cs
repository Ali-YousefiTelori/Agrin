using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Agrin.IO.Mixer;
using Agrin.IO.Helper;

namespace UnitTestAgrin.IO
{
    [TestClass]
    public class FileTest
    {
        [TestMethod]
        public void FindFileNameTest()
        {
            try
            {
                string text = GetLinksFileName("ali.rar");
                text = GetLinksFileName("c:\\dsdfdf\\ali.rar");
                text = GetLinksFileName("aa\\ali.rar");
                text = GetLinksFileName("aa//ali.rar");
                text = GetLinksFileName("/ali.rar");
                text = GetLinksFileName("http://tinyez.tv/dl/dl/x/X-Men.5.First.Class.Dubbed.Audio.TinyMoviez_us.mp3?hash=21b6c0af06e4c059360ba6c3201ee536_187730_3853&s=");
                text = GetLinksFileName("http://tinyez.tv/dl/dl/x/X-Men.5.First.Class.Dubbed.Audio.TinyMoviez_us.mp3");
                text = GetLinksFileName("http://cdn.p30download.com/?b=p30dl-software&f=");
                text = GetLinksFileName("");
            }
            catch
            {

            }
        }

        public static string GetLinksFileName(string newFileName)
        {
            string decode = Agrin.IO.Strings.Decodings.FullDecodeString(newFileName.Trim().Trim(new char[] { '/', '\\' }));
            Uri uri = null;
            string fileName = null;
            if (Uri.TryCreate(decode, UriKind.Absolute, out uri))
            {
                fileName = Agrin.IO.Strings.Decodings.FullDecodeString(System.IO.Path.GetFileName(uri.AbsolutePath));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Agrin.IO.Strings.Decodings.FullDecodeString(System.IO.Path.GetFileName(decode));
                    if (fileName.Contains("="))
                    {
                        int l = fileName.LastIndexOf('=');
                        if (l < fileName.Length - 1)
                        {
                            string name = fileName.Substring(l + 1, fileName.Length - l - 1);
                            if (!string.IsNullOrEmpty(name))
                                fileName = name;
                        }
                    }
                }
            }
            else
                fileName = System.IO.Path.GetFileName(decode);

            foreach (var item in System.IO.Path.GetInvalidPathChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            if (string.IsNullOrEmpty(fileName))
                return "notName.html";
            else if (string.IsNullOrEmpty(MPath.GetFileExtention(fileName)))
                return fileName + ".html";
            return fileName;
        }

        [TestMethod]
        public void FileSpliter()
        {
            Agrin.IO.Spliter.FileSpliter.SplitFile(@"D:\Test\HiHelp_Full_1.1.5.rar", @"D:\Test\", "SplitedFiles", 1024 * 1024 * 7);
        }

        [TestMethod]
        public void FileMixer()
        {
            string dir = "D:\\Test";
            List<string> files = new List<string>() {   System.IO.Path.Combine(dir, "SplitedFiles0"), System.IO.Path.Combine(dir, "SplitedFiles1"), System.IO.Path.Combine(dir, "SplitedFiles2"),
                    System.IO.Path.Combine(dir, "SplitedFiles3"),System.IO.Path.Combine(dir, "SplitedFiles4"),System.IO.Path.Combine(dir, "SplitedFiles5"),System.IO.Path.Combine(dir, "SplitedFiles6"),
                    System.IO.Path.Combine(dir, "SplitedFiles7")};
            Agrin.IO.Mixer.FileRevercerMixer mixer = new Agrin.IO.Mixer.FileRevercerMixer(files);

            string savePath = @"D:\Test\Mix\MixerData.agn";
            string saveBackupPath = @"D:\Test\Mix\MixerDataBackup.agn";
            MixerInfo mixerInfo = MixerInfo.LoadFromFile(savePath, saveBackupPath, true);/// new MixerInfo();
            if (mixerInfo.FilePath == null)
            {
                mixerInfo.MixerPath = savePath;
                mixerInfo.MixerBackupPath = saveBackupPath;
                //foreach (var item in files)
                //{
                //    mixerInfo.Files.Add(new FileConnection() { Path = item });
                //}
                mixerInfo.FilePath = @"D:\Test\Mix\mixed.rar";
            }
            
            mixer.Start(mixerInfo);
        }

        public void CheckIsTrue(bool reverce)
        {
            //byte[] f1 = System.IO.File.ReadAllBytes(@"D:\Test\HiHelp_Full_1.1.5.rar");
            //byte[] f2 = System.IO.File.ReadAllBytes(CurrentMixer.FilePath);
            //if (reverce)
            //    f2 = f2.Reverse().ToArray();
            //if (f1.Length != f2.Length)
            //{
            //    throw new Exception("not size true");
            //}
            //for (int i = 0; i < f1.Length; i++)
            //{
            //    if (f1[i] != f2[i])
            //    {
            //        throw new Exception("no dataOK");
            //    }
            //}
        }

    }
}
