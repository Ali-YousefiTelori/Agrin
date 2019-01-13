using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AgrinSetup.Models;
using Microsoft.Win32;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;

namespace AgrinSetup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Closing += MainWindow_Closing;
            InitializeComponent();
            //txt_address.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            //if (String.IsNullOrEmpty(txt_address.Text))
            //{
            txt_address.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            //}
            txt_address.Text += "\\Agrin Download Manager";
            if (IsInstall())
            {
                rdo_ReInstall.IsEnabled = true;
                rdo_ReInstall.IsChecked = true;
                rdo_Install.IsEnabled = false;
                btn_nextInstall.Visibility = System.Windows.Visibility.Collapsed;
                btn_nextUnInstall.Visibility = System.Windows.Visibility.Collapsed;
                btn_nextReInstall.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                rdo_ReInstall.Visibility = System.Windows.Visibility.Collapsed;
                rdo_ReInstall.IsEnabled = false;
                rdo_UnInstall.IsEnabled = false;
                rdo_Install.IsChecked = true;
                btn_nextInstall.Visibility = System.Windows.Visibility.Visible;
                btn_nextUnInstall.Visibility = System.Windows.Visibility.Collapsed;
            }
            CheckIsRun();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isDispose = true;
        }
        bool isDispose = false;
        public void CheckIsRun()
        {
            Task task = new Task(() =>
            {
                while (!isDispose)
                {
                    try
                    {
                        if (Process.GetProcessesByName("Agrin.Windows.UI").Length > 0)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                txt_isRunAgrin.Visibility = System.Windows.Visibility.Visible;
                                btn_nextInstall.IsEnabled = btn_nextReInstall.IsEnabled = btn_nextUnInstall.IsEnabled = false;
                            }));
                        }
                        else
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                txt_isRunAgrin.Visibility = System.Windows.Visibility.Collapsed;
                                btn_nextInstall.IsEnabled = btn_nextReInstall.IsEnabled = btn_nextUnInstall.IsEnabled = true;
                            }));
                        }

                    }
                    catch
                    {

                    }
                    System.Threading.Thread.Sleep(1000);
                }
            });
            task.Start();
        }

        private void Window_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = txt_address.Text;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_address.Text = dialog.SelectedPath;
            }

        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_Install_Click_1(object sender, RoutedEventArgs e)
        {
            Install();
        }

        //bool reInstall = false;
        public void Install()
        {
            string address = txt_address.Text;
            bool shortCut = chkShortCut.IsChecked.Value;
            System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                    bool install = false;
                    try
                    {
                        CreateDirectory(address);
                        string saveAddress = System.IO.Path.Combine(new string[] { System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ADM" });
                        CreateDirectory(saveAddress);

                        //DirectorySecurity security = new DirectorySecurity(address, AccessControlSections.All);
                        //SetProgValue(10);
                        //try
                        //{
                        //    FileSystemRights rights= FileSystemRights.AppendData| FileSystemRights.ChangePermissions| FileSystemRights.CreateDirectories| FileSystemRights.CreateFiles| FileSystemRights.Delete|  FileSystemRights.DeleteSubdirectoriesAndFiles| FileSystemRights.ExecuteFile |
                        //         FileSystemRights.FullControl| FileSystemRights.ListDirectory| FileSystemRights.Modify| FileSystemRights.Read | FileSystemRights.ReadAndExecute| FileSystemRights.ReadAttributes| FileSystemRights.ReadData| FileSystemRights.ReadExtendedAttributes| FileSystemRights.ReadPermissions| FileSystemRights.Synchronize
                        //         | FileSystemRights.TakeOwnership| FileSystemRights.Traverse| FileSystemRights.Write| FileSystemRights.WriteAttributes| FileSystemRights.WriteData| FileSystemRights.WriteExtendedAttributes;
                        //    MessageBox.Show(rights.ToString());
                        //    MessageBox.Show(infoAddresss.FullName);
                        //    directorySecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, rights, AccessControlType.Allow));
                        //    directorySecurity.AddAccessRule(new FileSystemAccessRule("Users", rights, AccessControlType.Allow));
                        //    directorySecurity.AddAccessRule(new FileSystemAccessRule("SYSTEM", rights, AccessControlType.Allow));
                        //    directorySecurity.AddAccessRule(new FileSystemAccessRule("ALL APPLICATION PACKAGES", rights, AccessControlType.Allow));
                        //}
                        //catch(Exception e)
                        //{
                        //    MessageBox.Show(e.Message );
                        //}
                        //SetProgValue(10);
                        //InstallFonts();

                        Dictionary<string, string> files = new Dictionary<string, string>()
                        {{"Agrin_Windows_UI","Agrin.Windows.UI.exe"}, {"Agrin_Download","Agrin.Download.dll"}, {"Agrin_Data","Agrin.Data.dll"},
                        {"Agrin_Drawing","Agrin.Drawing.dll"}, {"Agrin_LinkExtractor","Agrin.LinkExtractor.dll"},{"Newtonsoft_Json","Newtonsoft.Json.dll"},
                        {"Agrin_Helper","Agrin.Helper.dll"},{"Agrin_IO","Agrin.IO.dll"},{"Agrin_About","Agrin.About.dll"},{"Agrin_Log","Agrin.Log.dll"},
                        {"Agrin_OS","Agrin.OS.dll"},{"Agrin_Framesoft","Agrin.Framesoft.dll"},{"Agrin_BaseViewModels","Agrin.BaseViewModels.dll"},
                        {"Agrin_ViewModels","Agrin.ViewModels.dll"},{"Microsoft_Windows_Shell","Microsoft.Windows.Shell.dll"},{"Agrin_Network","Agrin.Network.dll"},
                        {"Agrin_NotifyIcon","Agrin.NotifyIcon.dll"},{"BCMakeCert","BCMakeCert.dll"},{"CertMaker","CertMaker.dll"},{"FiddlerCore4","FiddlerCore4.dll"},
                        {"Microsoft_WindowsAPICodePack","Microsoft.WindowsAPICodePack.dll"},{ "Microsoft_WindowsAPICodePack_Shell","Microsoft.WindowsAPICodePack.Shell.dll"},
                        {"Microsoft_WindowsAPICodePack_ShellExtensions","Microsoft.WindowsAPICodePack.ShellExtensions.dll"}};
                        double count = files.Count, index = 0;
                        foreach (var file in files)
                        {
                            var bytes = (byte[])AgrinResource.ResourceManager.GetObject(file.Key);
                            CreateFile(address + "\\" + file.Value, bytes);
                            index++;
                            SetProgValue((int)((index / count) * 100));
                        }

                        installedPath = address + "\\Agrin.Windows.UI.exe";

                        if (shortCut)
                        {
                            configStep_addShortcutToStartupGroup(installedPath);
                        }

                        Registry.CurrentUser.CreateSubKey("Software\\Agrin\\Install").SetValue("", address + "\\Agrin.Windows.UI.exe");
                        SetProgValue(100);
                        install = true;
                    }
                    catch
                    {
                        install = false;
                    }
                    if (install)
                        End("نرم افزار با موفقیت نصب شد.");
                    else
                        End("خطایی در نصب نرم افزار رخ داده است.");
                });
            thread.Start();
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
            DirectorySecurity sec = Directory.GetAccessControl(path);
            // Using this instead of the "Everyone" string means we work on non-English systems.
            try
            {
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                sec.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.Modify | FileSystemRights.Synchronize | FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                sec.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.Modify | FileSystemRights.Synchronize | FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                sec.AddAccessRule(new FileSystemAccessRule("SYSTEM", FileSystemRights.Modify | FileSystemRights.Synchronize | FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                //sec.AddAccessRule(new FileSystemAccessRule("", FileSystemRights.Modify | FileSystemRights.Synchronize | FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                Directory.SetAccessControl(path, sec);
            }
            catch
            {

            }
        }

        string installedPath = "";

        public bool InstallFlashGotProfile()
        {
            return SetFlashGotSettingForMozilla(installedPath);
        }

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

        //[DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        //public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
        //                             string lpFileName);
        //public void InstallFonts()
        //{
        //    string agrinFonts = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AgrinFonts";
        //    Directory.CreateDirectory(agrinFonts);
        //    string fonts = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

        //    //Createfonts("segoepr", fonts);
        //    //Createfonts("segoeprb", fonts);
        //    //Createfonts("segoesc", fonts);
        //    //Createfonts("segoescb", fonts);
        //    string fileName = "SegoeMarker";
        //    try
        //    {
        //        Createfonts(fileName, agrinFonts);
        //        AddFontResource(agrinFonts + "\\" + fileName + ".ttf");
        //        fileName = "segoeui";
        //        Createfonts(fileName, agrinFonts);
        //        AddFontResource(agrinFonts + "\\" + fileName + ".ttf");
        //        fileName = "segoeuib";
        //        Createfonts(fileName, agrinFonts);
        //        AddFontResource(agrinFonts + "\\" + fileName + ".ttf");
        //    }
        //    catch { }
        //    //Createfonts("segoeuii", fonts);
        //    //Createfonts("segoeuil", fonts);
        //    //Createfonts("segoeuisl", fonts);
        //    //Createfonts("segoeuiz", fonts);
        //    //Createfonts("seguibl", fonts);
        //    //Createfonts("seguibli", fonts);
        //    //Createfonts("seguiemj", fonts);
        //    //Createfonts("seguili", fonts);
        //    //Createfonts("seguisb", fonts);
        //    //Createfonts("seguisbi", fonts);
        //    //Createfonts("seguisli", fonts);
        //    //Createfonts("seguisym", fonts);
        //    fileName = "Tehran";
        //    Createfonts(fileName, agrinFonts);
        //    AddFontResource(agrinFonts + "\\" + fileName + ".ttf");
        //}

        //public void Createfonts(string name, string path)
        //{

        //    string fullName = path + "\\" + name + ".ttf";
        //   // MessageBox.Show(fullName);
        //    //if (!File.Exists(fullName))
        //    //{
        //    CreateFile(fullName, (byte[])AgrinResource.ResourceManager.GetObject(name));
        //    //}
        //    //MessageBox.Show("end");
        //}

        public void CreateFile(string address, byte[] data)
        {
            //MessageBox.Show("Creating... " + address);
            File.WriteAllBytes(address, data);
        }

        public void CreateFile(string address, string data)
        {
            File.WriteAllText(address, data);
        }

        private static void configStep_addShortcutToStartupGroup(string path)
        {
            using (ShellLink shortcut = new ShellLink())
            {
                shortcut.Target = path;
                shortcut.WorkingDirectory = Path.GetDirectoryName(path);
                shortcut.Description = "Agrin Download Manager";
                shortcut.DisplayMode = ShellLink.LinkDisplayMode.edmNormal;
                shortcut.Save(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Agrin Download Manager.lnk");
            }
        }

        public void UnInstall()
        {
            string address = Path.GetDirectoryName(Registry.CurrentUser.OpenSubKey(@"Software\Agrin\Install").GetValue("").ToString());
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {

                System.Threading.Thread.Sleep(2000);
                bool un = false;
                try
                {
                    //UnInstallIEExtension(address);
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Agrin Download Manager.lnk"))
                        File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Agrin Download Manager.lnk");
                    SetProgValue(30);
                    foreach (var fileInfo in System.IO.Directory.GetFiles(address, "*.*", System.IO.SearchOption.AllDirectories))
                    {
                        System.IO.File.Delete(fileInfo);
                    }
                    SetProgValue(60);
                    foreach (var item in System.IO.Directory.GetDirectories(address, "*.*", System.IO.SearchOption.AllDirectories))
                    {
                        System.IO.Directory.Delete(item);
                    }
                    SetProgValue(90);
                    System.IO.Directory.Delete(address);
                    un = true;
                }
                catch
                {
                    un = false;
                }

                SetProgValue(100);
                if (un)
                    End("نرم افزار حذف شد.");
                else
                    End("خطایی در حذف نرم افزار رخ داده است.");
            });
            thread.Start();
        }

        void End(string message)
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    btn_End.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    btn_End.IsEnabled = true;
                    txt_Message.Text = message;
                }));
        }

        void SetProgValue(int val)
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    progbar.Value = val;
                    txt_Percent.Text = val + "%";
                }));
        }

        public static bool InstallIEExtension(string installPath)
        {
            try
            {
                //System.Runtime.InteropServices.Marshal.
                string currentDirectory = installPath;
                Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\MenuExt\Agrin دانلود همه لینک ها توسط ").SetValue("", currentDirectory + "\\IEFullExtention.html");
                Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\MenuExt\Agrin دانلود لینک های انتخاب شده توسط ").SetValue("", currentDirectory + "\\IESSHExtention.html");
                Registry.CurrentUser.CreateSubKey(@"Software\Agrin\Install").SetValue("", currentDirectory + "\\Agrin Download Manager.exe");
                Assembly asm = Assembly.LoadFrom(currentDirectory + "\\AgrinBrowserSetting.dll");
                RegistrationServices regAsm = new RegistrationServices();
                bool bResult = regAsm.RegisterAssembly(asm, AssemblyRegistrationFlags.SetCodeBase);
                return bResult;
            }
            catch
            {
                return false;
            }
        }

        public static bool UnInstallIEExtension(string installPath)
        {
            try
            {
                string currentDirectory = installPath;
                Registry.CurrentUser.DeleteSubKey(@"Software\Microsoft\Internet Explorer\MenuExt\Agrin دانلود همه لینک ها توسط ");
                Registry.CurrentUser.DeleteSubKey(@"Software\Microsoft\Internet Explorer\MenuExt\Agrin دانلود لینک های انتخاب شده توسط ");
                Registry.CurrentUser.DeleteSubKey(@"Software\Agrin\Install");
                Assembly asm = Assembly.Load(File.ReadAllBytes(currentDirectory + "\\AgrinBrowserSetting.dll"));
                RegistrationServices regAsm = new RegistrationServices();
                bool bResult = regAsm.UnregisterAssembly(asm);
                return bResult;
            }
            catch
            {
                return false;
            }
        }

        bool IsInstall()
        {
            try
            {
                string currentDirectory = Registry.CurrentUser.OpenSubKey(@"Software\Agrin\Install").GetValue("").ToString();
                if (File.Exists(currentDirectory))
                    return true;
            }
            catch { }
            return false;
        }

        private void btn_nextUnInstall_Click_1(object sender, RoutedEventArgs e)
        {
            UnInstall();
        }

        private void btn_End_Click_1(object sender, RoutedEventArgs e)
        {
            if (chkrun.IsChecked.Value && File.Exists(installedPath))
                Process.Start(installedPath);
            Close();
        }

        private void rdo_ReInstall_Checked_1(object sender, RoutedEventArgs e)
        {

            if (rdo_ReInstall.IsChecked.Value)
            {
                //reInstall = true;
                btn_nextUnInstall.Visibility = System.Windows.Visibility.Collapsed;
                btn_nextInstall.Visibility = System.Windows.Visibility.Visible;
                //chkrun.Visibility = chkrun.IsChecked = true;
            }
            else
            {
                //reInstall = false;
                btn_nextUnInstall.Visibility = System.Windows.Visibility.Visible;
                btn_nextInstall.Visibility = System.Windows.Visibility.Collapsed;
                //chkrun.Visibility =chkrun.IsChecked = false;
            }
        }

        public void SetFlashGotMessage(string message)
        {
            txtFlashGotMessage.Text = message;
            btnInstallFlashGotSetting.IsEnabled = btnDownloadFlashGot.IsEnabled = false;
            Task task = new Task(() =>
            {
                System.Threading.Thread.Sleep(3000);
                Dispatcher.Invoke(new Action(() =>
                {
                    btnInstallFlashGotSetting.IsEnabled = btnDownloadFlashGot.IsEnabled = true;
                }));
            });
            task.Start();
        }

        private void btnDownloadFlashGot_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("https://secure.informaction.com/download/releases/flashgot-1.5.6.12.xpi");
            Process.Start("https://addons.mozilla.org/en-US/firefox/addon/flashgot/");
            SetFlashGotMessage("در پنجره ی باز شده روی دکمه ی Add to Firefox کلیک کنید و منتظر بمانید تا مرورگر برای دانلود و نصب اقدام کند و افزونه را نصب کنید.");
        }

        private void btnInstallFlashGotSetting_Click(object sender, RoutedEventArgs e)
        {
            if (InstallFlashGotProfile())
            {
                txtFlashGotMessage.Foreground = Brushes.Green;
                SetFlashGotMessage("تنظیمات با موفقیت ثبت شد.");
            }
            else
            {
                txtFlashGotMessage.Foreground = new BrushConverter().ConvertFromString("#FFAA1903") as Brush;
                SetFlashGotMessage("خطا در ثبت تنظیمات رخ داده است.");
            }
        }
    }
}
