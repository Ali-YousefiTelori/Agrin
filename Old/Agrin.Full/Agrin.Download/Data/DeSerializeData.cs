using Agrin.Data.Mapping;
using Agrin.Download.Data.Serializition;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Download.Web.Connections;
using Agrin.Download.Web.Link;
using Agrin.Download.Web.Tasks;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using Agrin.Log;
#if (!MobileApp && !XamarinApp)
using Agrin.Network.Models;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Agrin.Download.Data
{
    public enum LoadingStateEnum
    {
        LoadFileApplicationSetting = 0,
        LoadApplicationIPsData = 1,
        LoadGroups = 3,
        LoadLinks = 4,
        LoadAppServiceData = 5,
        LoadNetworkProxySettings = 6,
        LoadTaskInfoesFromFile = 7,
        LoadNoticesFromFile = 8,
        InitializeServicesAndUI = 9,
        InitializeUI = 10,
    }

    public static class DeSerializeData
    {
        public static Action<LoadingStateEnum> ChangedStateAction { get; set; }
        public static void ChangeState(LoadingStateEnum state)
        {
            if (ChangedStateAction != null)
                ChangedStateAction(state);
        }

        static bool fixedBug = false;
        static void FixDotNetBug()
        {
            if (fixedBug)
                return;
            fixedBug = true;
            MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (getSyntax != null && flagsField != null)
            {
                foreach (string scheme in new[] { "http", "https" })
                {
                    UriParser parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                    if (parser != null)
                    {
                        int flagsValue = (int)flagsField.GetValue(parser);
                        // Clear the CanonicalizeAsFilePath attribute
                        if ((flagsValue & 0x1000000) != 0)
                            flagsField.SetValue(parser, flagsValue & ~0x1000000);
                    }
                }
            }
        }

        //public static Action<LinkInfoShortSerialize, string, bool> LoadLinkAction { get; set; }

        public static void LoadApplicationData()
        {
            FixDotNetBug();
            MoveLastVersionData();
            Engine.SearchEngine.IsAppLoading = true;
            Action loadLinks = () =>
            {
                ChangeState(LoadingStateEnum.LoadLinks);
                string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Links.agn"), backUpFileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Links_BackUp.agn");
                long downloadedSize = 0;
                if (File.Exists(fileName) || File.Exists(backUpFileName))
                {
                    AgrinApplicationDataShortSerialize appData = SerializeStream.OpenSerializeStream<AgrinApplicationDataShortSerialize>(fileName);
                    if (appData == null)
                        appData = SerializeStream.OpenSerializeStream<AgrinApplicationDataShortSerialize>(backUpFileName);
                    //appData.LinkInfoes.Remove(appData.LinkInfoes.Last());
                    if (appData != null)
                    {
                        List<LinkInfo> saveLists = new List<LinkInfo>();

                        //List<LinkInfo> openLinks = new List<LinkInfo>();
                        //foreach (var item in Directory.GetFiles(MPath.BackUpCompleteLinksPath))
                        //{
                        //    openLinks.Add(DeSerializeData.LoadFileToLinkInfoData(item));
                        //}
                        bool exist = true;
                        foreach (var item in appData.LinkInfoes)
                        {
                            if (item.Address.Contains("Kong"))
                            {

                            }
                            ////start
                            //if (item.Address.Contains("RavensCry"))
                            //{
                            //    foreach (var ll in openLinks)
                            //    {
                            //        if (ll.PathInfo.Address.Contains("RavensCry"))
                            //        {

                            //        }
                            //    }
                            //}
                            ////end
                            //if (item.Id == 131)
                            //{

                            //}
                            LinkInfo info = null;
                            string linkFileName = Path.Combine(MPath.SaveDataPath, item.Id.ToString(), "Link.agn");
                            //LoadLinkAction?.Invoke(item, linkFileName, File.Exists(linkFileName));
                            CreateNormal:
                            if (exist && File.Exists(linkFileName))
                            {
                                info = LoadFileToLinkInfoData(linkFileName);
                                if (info == null)
                                    info = LoadBackUpFileToLinkInfoData(linkFileName);

                                if (info == null)
                                {
                                    exist = false;
                                    goto CreateNormal;
                                }
                                else
                                {
                                    if (item.State != ConnectionState.Complete && item.State != ConnectionState.Error)
                                        info.DownloadingProperty.State = ConnectionState.Stoped;
                                    else
                                        info.DownloadingProperty.State = item.State;

                                    if (item.State == ConnectionState.Complete)
                                    {
                                        info.DownloadingProperty.DownloadedSize = (long)item.Size;
                                    }

                                    var group = ApplicationGroupManager.Current.FindGroupByName(item.UserGroupInfo);
                                    if (group != ApplicationGroupManager.Current.NoGroup)
                                    {
                                        info.PathInfo.UserGroupInfo = group;
                                    }
                                    else
                                        info.PathInfo.UserGroupInfo = null;

                                    //if (group == ApplicationGroupManager.Current.NoGroup)
                                    //{
                                    //    string fn = item.UserFileName;
                                    //    if (string.IsNullOrEmpty(fn))
                                    //        fn = item.AddressFileName;
                                    //    group = ApplicationGroupManager.Current.FindGroupByFileName(fn);
                                    //}
                                }
                            }
                            else
                            {
                                exist = true;
                                info = new LinkInfo(item.Address);
                                info.PathInfo.Address = item.Address;
                                info.PathInfo.AddressFileName = item.AddressFileName;
                                info.PathInfo.MixerSavePath = item.MixerSavePath;
                                info.PathInfo.MixerBackupSavePath = item.MixerBackupSavePath;
                                if (item.CustomHeaders != null)
                                    info.DownloadingProperty.CustomHeaders = item.CustomHeaders;

                                info.DownloadingProperty.DateLastDownload = info.DownloadingProperty.CreateDateTime = item.CreateDateTime;
                                info.PathInfo.Id = item.Id;
                                info.PathInfo.UserFileName = item.UserFileName;
                                info.PathInfo.BackUpCompleteAddress = item.BackUpCompleteAddress;
                                if (item.SharingSettings != null)
                                    info.Management.SharingSettings = item.SharingSettings.ToList();
                                var group = ApplicationGroupManager.Current.FindGroupByName(item.UserGroupInfo);
                                if (group == ApplicationGroupManager.Current.NoGroup)
                                {
                                    group = ApplicationGroupManager.Current.FindGroupByName(item.ApplicationGroupInfo);
                                    info.PathInfo.ApplicationGroupInfo = group;
                                    info.PathInfo.UserGroupInfo = null;
                                }
                                else
                                {
                                    info.PathInfo.UserGroupInfo = group;
                                }
                                if (group == ApplicationGroupManager.Current.NoGroup)
                                {
                                    string fn = item.UserFileName;
                                    if (string.IsNullOrEmpty(fn))
                                        fn = item.AddressFileName;
                                    group = ApplicationGroupManager.Current.FindGroupByFileName(fn);
                                    info.PathInfo.ApplicationGroupInfo = group;
                                }
                                info.PathInfo.UserSavePath = item.UserSavePath;
                                info.DownloadingProperty.Size = item.Size;
                                info.Management.IsApplicationSetting = item.IsApplicationSetting;
                                if (!String.IsNullOrEmpty(item.UserNameAuthorization) && !String.IsNullOrEmpty(item.PasswordAuthorization))
                                    info.Management.NetworkUserPass = new NetworkCredentialInfo() { UserName = item.UserNameAuthorization, Password = item.PasswordAuthorization, ServerAddress = item.DomainNameAuthorization, IsUsed = true };
                                if (item.State == ConnectionState.Complete)
                                {
                                    info.DownloadingProperty.State = ConnectionState.Complete;
                                    info.DownloadingProperty.DownloadedSize = (long)item.Size;
                                }
                                else if (item.State == ConnectionState.Error)
                                {
                                    info.DownloadingProperty.State = ConnectionState.Error;
                                }
                                else
                                    info.DownloadingProperty.State = ConnectionState.Stoped;
                            }
                            if (info.Management.IsApplicationSetting)
                            {
                                DeSerializeData.AppSettingToLinkInfo(info);
                            }
                            saveLists.Add(info);
                            downloadedSize += info.DownloadingProperty.DownloadedSize;
                        }

                        ApplicationLinkInfoManager.Current.AddRangeDeserializedData(saveLists);
                        LoadNotificationToFile();
                    }
                }
                ChangeState(LoadingStateEnum.LoadAppServiceData);
                LoadAppServiceData();
#if (!MobileApp && !XamarinApp)
                ChangeState(LoadingStateEnum.LoadNetworkProxySettings);
                LoadNetworkProxySettings();
#endif
                ChangeState(LoadingStateEnum.LoadTaskInfoesFromFile);
                LoadTaskInfoesFromFile();
                ChangeState(LoadingStateEnum.LoadNoticesFromFile);
                LoadNoticesFromFile();
                ApplicationLinkInfoManager.Current.DownloadedStopedLinksSize = downloadedSize;

                Engine.SearchEngine.IsAppLoading = false;
                Engine.SearchEngine.Search();
            };
            AsyncActions.Action(() =>
            {
                LoadFileApplicationSetting();
                try
                {
                    ChangeState(LoadingStateEnum.LoadGroups);
                    string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Groups.agn"), backUpFileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Groups_BackUp.agn");
                    if (File.Exists(fileName) || File.Exists(backUpFileName))
                    {
                        AgrinApplicationGroupDataSerialize appData = SerializeStream.OpenSerializeStream<AgrinApplicationGroupDataSerialize>(fileName);
                        if (appData == null)
                            appData = SerializeStream.OpenSerializeStream<AgrinApplicationGroupDataSerialize>(backUpFileName);
                        if (appData != null)
                        {
                            List<GroupInfo> saveLists = new List<GroupInfo>();
                            foreach (var item in appData.GroupInfoes)
                            {
                                saveLists.Add(new GroupInfo() { Id = item.Id, Name = item.Name, UserSavePath = item.UserSavePath, SaveFolderName = item.SaveFolderName, Extentions = item.Extentions, IsExpanded = item.IsExpanded, UserSecurityPath = item.UserSecurityPath });
                            }

                            ApplicationGroupManager.Current.AddRangeGroupInfoes(saveLists);
                        }
                    }
                }
                catch (Exception e)
                {
                    Agrin.Log.AutoLogger.LogError(e, "LoadApplicationData 1", true);
                }
                loadLinks();
            }, (error) =>
            {
                loadLinks();
                Agrin.Log.AutoLogger.LogError(error, "LoadApplicationData 1", true);
            }, false);
        }

        public static LinkInfo LoadBackUpFileToLinkInfoData(string fileName)
        {
            string backUpAddress = Path.GetDirectoryName(fileName);
            backUpAddress = Path.Combine(backUpAddress, "Link_BackUp.agn");
            if (!File.Exists(backUpAddress))
                return null;
            return DeSerializeData.LoadFileToLinkInfoData(backUpAddress);
        }
        public static LinkInfo LinkInfoSerializeToLinkInfoData(LinkInfoSerialize appData)
        {
            try
            {
                LinkInfo linkInfo = new LinkInfo();
                Func<string, object, object> convert = null;
                List<string> manualNames = new List<string>() { "Connections", "UserGroupInfo", "ApplicationGroupInfo", "Errors", "MultiLinks", "MultiProxy", "SharingSettings", "ListSpeedByteDownloaded" };

                convert = (mapName, mapType) =>
                {
                    if (mapName == "Connections")
                    {
                        if (mapType is List<ConnectionInfoSerialize>)
                        {
                            FastCollection<LinkWebRequest> ret = new FastCollection<LinkWebRequest>(ApplicationHelperBase.DispatcherThread);
                            foreach (var item in ((List<ConnectionInfoSerialize>)mapType).ToList())
                            {
                                LinkWebRequest cloned = new LinkWebRequest(linkInfo, linkInfo.PathInfo.Address);
                                Mapper.Map<ConnectionInfoSerialize, LinkWebRequest>(item, cloned);
                                ret.Add(cloned);
                            }
                            return ret;
                        }
                    }
                    else if (mapName == "Errors")
                    {
                        List<ExceptionSerializable> items = new List<ExceptionSerializable>();
                        List<ExceptionSerializable> list = null;
                        if (mapType is List<Exception>)
                        {
                            list = new List<ExceptionSerializable>();
                            foreach (var item in ((List<Exception>)mapType))
                            {
                                list.Add(new ExceptionSerializable() { ExceptionData = item });
                            }
                        }
                        else
                        {
                            list = ((List<ExceptionSerializable>)mapType);
                        }
                        int count = list.Capacity > list.Count ? list.Count : list.Capacity;
                        items.AddRange(list.GetRange(0, count));
                        return items;
                    }
                    else if (mapName == "MultiLinks")
                    {
                        List<MultiLinkAddress> items = new List<MultiLinkAddress>();
                        var list = ((List<MultiLinkAddress>)mapType);
                        int count = list.Capacity > list.Count ? list.Count : list.Capacity;
                        items.AddRange(list.GetRange(0, count));
                        return items;
                    }
                    else if (mapName == "MultiProxy")
                    {
                        List<ProxyInfo> items = new List<ProxyInfo>();
                        var list = ((List<ProxyInfo>)mapType);
                        int count = list.Capacity > list.Count ? list.Count : list.Capacity;
                        items.AddRange(list.GetRange(0, count));
                        return items;
                    }
                    else if (mapName == "UserGroupInfo" || mapName == "ApplicationGroupInfo")
                    {
                        string groupInfo = (string)mapType;
                        return Manager.ApplicationGroupManager.Current == null ? null : Manager.ApplicationGroupManager.Current.FindGroupByName(groupInfo);
                    }
                    else if (mapName == "SharingSettings")
                    {
                        List<object> items = new List<object>();
                        var list = ((List<object>)mapType);
                        int count = list.Capacity > list.Count ? list.Count : list.Capacity;
                        items.AddRange(list.GetRange(0, count));
                        return items;
                    }
                    else if (mapName == "ListSpeedByteDownloaded")
                    {
                        ObtimizeList<double> items = new ObtimizeList<double>(20);
                        var list = ((IList<double>)mapType);
                        items.AddRange(list);
                        return items;
                    }
                    //else if (mapName == "DownloadRangePositions")
                    //{
                    //    List<long> items = new List<long>();
                    //    var list = ((List<long>)mapType);
                    //    int count = list.Capacity > list.Count ? list.Count : list.Capacity;
                    //    items.AddRange(list.GetRange(0, count));
                    //    return items;
                    //}

                    return null;
                };

                Mapper.Map<LinkInfoSerialize, LinkInfo>(appData, linkInfo, convert, manualNames);

                linkInfo.DownloadingProperty.Parent = linkInfo;
                linkInfo.PathInfo.Parent = linkInfo;
                try
                {
                    linkInfo.DownloadingProperty.DownloadedSize = 0;
                    foreach (var item in linkInfo.Connections)
                    {
                        if (File.Exists(item.SaveFileName))
                        {
                            FileInfo fileInfo = new FileInfo(item.SaveFileName);
                            item.DownloadedSize = fileInfo.Length;
                        }
                    }
                }
                catch (Exception e)
                {
                    //ExceptionToLoad?.Invoke(e);
                    Agrin.Log.AutoLogger.LogError(e, "LoadFileToLinkInfoData 1", true);
                }
                //Manager.ApplicationLinkInfoManager.Current.LinkInfoes.AddRange(deSerialed.LinkInfoes);
                return linkInfo;
            }
            catch (Exception e)
            {
                //ExceptionToLoad?.Invoke(e);
                Agrin.Log.AutoLogger.LogError(e, "LinkInfoSerializeToLinkInfoData 2", true);
            }
            return null;
        }

        //public static Action<Exception> ExceptionToLoad { get; set; }

        public static LinkInfo LoadFileToLinkInfoData(string fileName)
        {
            try
            {
                LinkInfoSerialize appData = SerializeStream.OpenSerializeStream<LinkInfoSerialize>(fileName);
                //if (appData.PathInfo.Id == 130)
                //{
                //    appData.PathInfo.Id = 131;
                //    appData.PathInfo.ConnectionsSavedAddress = @"F:\NewSaveData\mySave\ADM\131";
                //}
                if (appData == null)
                    return null;
                return LinkInfoSerializeToLinkInfoData(appData);
            }
            catch (Exception e)
            {
                //ExceptionToLoad?.Invoke(e);
                Agrin.Log.AutoLogger.LogError(e, "LoadFileToLinkInfoData 2", true);
            }
            return null;
        }

        static void LoadFileApplicationSetting()
        {
            try
            {
                ChangeState(LoadingStateEnum.LoadFileApplicationSetting);
                string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppSetting.agn"), backUpFileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppSetting_BackUp.agn");
                if (File.Exists(fileName) || File.Exists(backUpFileName))
                {
                    ApplicationSetting.Current = SerializeStream.OpenSerializeStream<ApplicationSetting>(fileName);
                    if (ApplicationSetting.Current == null)
                        ApplicationSetting.Current = SerializeStream.OpenSerializeStream<ApplicationSetting>(backUpFileName);
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "LoadFileApplicationSetting", true);
            }

            if (ApplicationSetting.Current == null)
            {
                var appSetting = new ApplicationSetting() { IsShowNotification = true, IsSettingForNewLinks = true };
                appSetting.LinkInfoDownloadSetting = new AppLinkInfoDownloadSetting() { TryException = 5, IsShowBalloon = true, IsExtreme = true };
                appSetting.PathSetting = new AppPathSetting();
                appSetting.ProxySetting = new AppProxySetting() { Items = new List<ProxyInfo>() };
                //appSetting.ProxySetting.Items.Add(new Download.Web.Link.ProxyInfo() { Id = 1, IsSelected = false, IsUserPass = false, ServerAddress = "بدون پروکسی" });
                appSetting.SpeedSetting = new AppSpeedSetting() { ConnectionCount = 4, BufferSize = 1024 * 30, SpeedSize = 1024 * 10 };
                appSetting.UserAccountsSetting = new AppUserAccountsSetting() { Items = new List<NetworkCredentialInfo>() };
                appSetting.ApplicationOSSetting = new ApplicationOSSetting();

                appSetting.FramesoftSetting = new FramesoftSetting();
                ApplicationSetting.Current = appSetting;
            }
            if (ApplicationSetting.Current.FramesoftSetting == null)
                ApplicationSetting.Current.FramesoftSetting = new FramesoftSetting();

            if (ApplicationSetting.Current.FramesoftSetting.PurchaseProductIds == null)
                ApplicationSetting.Current.FramesoftSetting.PurchaseProductIds = new List<string>();
            else
            {
                var listP = ApplicationSetting.Current.FramesoftSetting.PurchaseProductIds.ToList();
                ApplicationSetting.Current.FramesoftSetting.PurchaseProductIds = new List<string>();
                ApplicationSetting.Current.FramesoftSetting.PurchaseProductIds.AddRange(listP);
            }
            if (ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress == null)
                ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress = new List<string>();
            else
            {
                var listP = ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.ToList();
                ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress = new List<string>();
                ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.AddRange(listP);
            }

            //new items for new versions
            if (ApplicationSetting.Current.PathSetting == null)
                ApplicationSetting.Current.PathSetting = new AppPathSetting();
            if (ApplicationSetting.Current.ApplicationOSSetting == null)
                ApplicationSetting.Current.ApplicationOSSetting = new ApplicationOSSetting();

            if (!string.IsNullOrEmpty(ApplicationSetting.Current.PathSetting.SecurityPath))
                MPath.SecurityPath = ApplicationSetting.Current.PathSetting.SecurityPath;

            try
            {
                if (!string.IsNullOrEmpty(ApplicationSetting.Current.PathSetting.SaveDataPath) && Directory.Exists(ApplicationSetting.Current.PathSetting.SaveDataPath))
                {
                    MPath.SaveDataPath = ApplicationSetting.Current.PathSetting.SaveDataPath;
                }
                if (!string.IsNullOrEmpty(ApplicationSetting.Current.PathSetting.RepairSaveDataPath) && Directory.Exists(ApplicationSetting.Current.PathSetting.RepairSaveDataPath))
                {
                    MPath.RepairSaveDataPath = ApplicationSetting.Current.PathSetting.RepairSaveDataPath;
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Error Get SaveDataPath From LoadFileApplicationSetting", true);
            }
            LoadApplicationIPsData();
        }

        static void LoadAppServiceData()
        {
            try
            {
                string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppServiceData.agn"), backUpFileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppServiceData_BackUp.agn");
                if (File.Exists(fileName) || File.Exists(backUpFileName))
                {
                    ApplicationServiceData.Current = SerializeStream.OpenSerializeStream<ApplicationServiceData>(fileName);
                    if (ApplicationServiceData.Current == null)
                        ApplicationServiceData.Current = SerializeStream.OpenSerializeStream<ApplicationServiceData>(backUpFileName);
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "LoadFileApplicationSetting", true);
            }

            if (ApplicationServiceData.Current == null)
            {
                ApplicationServiceData.Current = new ApplicationServiceData();
            }

            //played links

            if (ApplicationServiceData.Current.IsPlayLinks == null || ApplicationServiceData.Current.IsPlayLinks.Count == 0)
                ApplicationServiceData.Current.IsPlayLinks = new List<int>();
            else
            {
                var list = ApplicationServiceData.Current.IsPlayLinks.ToArray();
                ApplicationServiceData.Current.IsPlayLinks = new List<int>();
                ApplicationServiceData.Current.IsPlayLinks.AddRange(list);
            }
        }
#if (!MobileApp && !XamarinApp)
        static void LoadNetworkProxySettings()
        {
            try
            {
                string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "NetworkProxySettings.agn"), backUpFileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "NetworkProxySettings_BackUp.agn");
                if (File.Exists(fileName) || File.Exists(backUpFileName))
                {
                    NetworkProxySettings.Current = SerializeStream.OpenSerializeStream<NetworkProxySettings>(fileName);
                    if (NetworkProxySettings.Current == null)
                        NetworkProxySettings.Current = SerializeStream.OpenSerializeStream<NetworkProxySettings>(backUpFileName);
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "LoadNetworkProxySettings", true);
            }

            if (NetworkProxySettings.Current == null)
            {
                NetworkProxySettings.Current = new NetworkProxySettings();
                NetworkProxySettings.InitializeDefaultSettings();
            }

            //played links
            NetworkProxySettings.Current.SupportApps = NetworkProxySettings.Current.SupportApps.ToList();
            NetworkProxySettings.Current.SupportFileExtensions = NetworkProxySettings.Current.SupportFileExtensions.ToList();
        }
#endif
        public static void LoadApplicationIPsData()
        {
            ChangeState(LoadingStateEnum.LoadApplicationIPsData);
            string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "ApplicationIPsData.agn");
            if (File.Exists(fileName))
            {
                try
                {
                    ApplicationIPsData deserialed = SerializeStream.OpenSerializeStream<ApplicationIPsData>(fileName);
                    ApplicationIPsData.Current = deserialed;
                    if (ApplicationIPsData.Current != null)
                    {
                        var list = ApplicationIPsData.Current.HostIPData.ToArray();
                        ApplicationIPsData.Current.HostIPData = new List<IPPropertiesSerialize>();
                        ApplicationIPsData.Current.HostIPData.AddRange(list);

                        var list2 = ApplicationIPsData.Current.FlagsByCountryCodes.ToArray();
                        ApplicationIPsData.Current.FlagsByCountryCodes = new Dictionary<string, byte[]>();
                        foreach (var item in list2)
                        {
                            if (!ApplicationIPsData.Current.FlagsByCountryCodes.ContainsKey(item.Key))
                                ApplicationIPsData.Current.FlagsByCountryCodes.Add(item.Key, item.Value);
                        }
                    }
                }
                catch (Exception err)
                {
                    Agrin.Log.AutoLogger.LogError(err, "LoadApplicationIPsData", true);
                }
            }

            if (ApplicationIPsData.Current == null)
            {
                ApplicationIPsData.Current = new ApplicationIPsData();
                ApplicationIPsData.Current.FlagsByCountryCodes = new Dictionary<string, byte[]>();
                ApplicationIPsData.Current.HostIPData = new List<IPPropertiesSerialize>();
            }
        }

        public static void AppSettingToLinkInfo(LinkInfo linkInfo)
        {
            linkInfo.Management.EndDownloadSystemMode = (Download.Web.Link.CompleteDownloadSystemMode)ApplicationSetting.Current.LinkInfoDownloadSetting.EndDownloadSelectedIndex;
            linkInfo.Management.IsEndDownload = ApplicationSetting.Current.LinkInfoDownloadSetting.IsEndDownloaded;
            linkInfo.Management.IsTryExtreme = ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme;
            linkInfo.Management.TryAginCount = ApplicationSetting.Current.LinkInfoDownloadSetting.TryException;
            linkInfo.Management.IsApplicationSetting = true;
            linkInfo.Management.IsShowBalloon = ApplicationSetting.Current.LinkInfoDownloadSetting.IsShowBalloon;

            linkInfo.Management.MultiProxy = ApplicationSetting.Current.ProxySetting.Items;

            linkInfo.Management.ReadBuffer = ApplicationSetting.Current.SpeedSetting.BufferSize;
            linkInfo.DownloadingProperty.ConnectionCount = ApplicationSetting.Current.SpeedSetting.ConnectionCount;
            linkInfo.Management.LimitPerSecound = ApplicationSetting.Current.SpeedSetting.SpeedSize;
            linkInfo.Management.IsLimit = ApplicationSetting.Current.SpeedSetting.IsLimit;
        }

        public static void LoadNotificationToFile()
        {
            try
            {
                if (ApplicationNotificationManager.Current == null)
                    ApplicationNotificationManager.Current = new ApplicationNotificationManager();

                string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Notifications.agn"), backUpFileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Notifications_BackUp.agn");
                if (File.Exists(fileName) || File.Exists(backUpFileName))
                {
                    ApplicationNotificationSerialize deserialed = SerializeStream.OpenSerializeStream<ApplicationNotificationSerialize>(fileName);
                    if (deserialed == null)
                        deserialed = SerializeStream.OpenSerializeStream<ApplicationNotificationSerialize>(backUpFileName);
                    Func<string, object, object> convert = null;
                    List<string> manualNames = new List<string>() { "Items" };

                    convert = (mapName, mapType) =>
                    {
                        if (mapName == "Items")
                        {
                            if (mapType is List<int>)
                            {
                                List<LinkInfo> items = new List<LinkInfo>();
                                foreach (var item in (List<int>)mapType)
                                {
                                    LinkInfo find = ApplicationLinkInfoManager.Current.FindLinkInfoByID(item);
                                    if (find != null)
                                        items.Add(find);
                                }
                                FastCollection<LinkInfo> ret = new FastCollection<LinkInfo>(ApplicationHelperBase.DispatcherThread);
                                ret.AddRange(items);
                                return ret;
                            }
                        }
                        return null;
                    };
                    List<NotificationInfo> itemsN = new List<NotificationInfo>();
                    foreach (var item in deserialed.NotificationInfoes)
                    {
                        NotificationInfo deserial = new NotificationInfo();
                        Mapper.Map<NotificationSerialize, NotificationInfo>(item, deserial, convert, manualNames);
                        itemsN.Add(deserial);
                    }
                    ApplicationNotificationManager.Current.AddRangeDesrialized(itemsN);
                }

            }
            catch (Exception exception)
            {
                Agrin.Log.AutoLogger.LogError(exception, "LoadNotificationToFile", true);
            }
        }

        public static void LoadTaskInfoesFromFile()
        {
            try
            {
                string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppTasks.agn"), backUpFileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppTasks_BackUp.agn");
                if (File.Exists(fileName) || File.Exists(backUpFileName))
                {
                    AgrinApplicationTasksSerialize deserialed = SerializeStream.OpenSerializeStream<AgrinApplicationTasksSerialize>(fileName);
                    if (deserialed == null)
                        deserialed = SerializeStream.OpenSerializeStream<AgrinApplicationTasksSerialize>(backUpFileName);
                    Func<string, object, object> convert = null;
                    List<string> manualNames = new List<string>() { "TaskItemInfoes" };

                    convert = (mapName, mapType) =>
                    {
                        if (mapName == "TaskItemInfoes")
                        {
                            if (mapType is List<TaskItemInfoSerialize>)
                            {
                                List<TaskItemInfo> ret = new List<TaskItemInfo>();
                                foreach (var item in ((List<TaskItemInfoSerialize>)mapType).ToList())
                                {
                                    TaskItemInfo cloned = new TaskItemInfo();
                                    Mapper.Map<TaskItemInfoSerialize, TaskItemInfo>(item, cloned);
                                    ret.Add(cloned);
                                }
                                return ret;
                            }
                        }
                        return null;
                    };
                    ApplicationTaskManager.Current.IsDeserializing = true;
                    foreach (var item in deserialed.TaskInfoes)
                    {
                        TaskInfo task = new TaskInfo();
                        Mapper.Map<TaskInfoSerialize, TaskInfo>(item, task, convert, manualNames);
                        ApplicationTaskManager.Current.AddTask(task, true);
                    }
                }
            }
            catch (Exception exception)
            {
                Agrin.Log.AutoLogger.LogError(exception, "LoadTaskInfoesFromFile", true);
            }
            finally
            {
                ApplicationTaskManager.Current.IsDeserializing = false;
            }
        }

        public static void LoadNoticesFromFile()
        {
            try
            {
                if (ApplicationNoticeManager.Current == null)
                    ApplicationNoticeManager.Current = new ApplicationNoticeManager();
                string fileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Notices.agn"), backUpFileName = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Notices_BackUp.agn");
                if (File.Exists(fileName) || File.Exists(backUpFileName))
                {
                    AgrinApplicationNoticeSerialize deserialed = SerializeStream.OpenSerializeStream<AgrinApplicationNoticeSerialize>(fileName);
                    if (deserialed == null)
                        deserialed = SerializeStream.OpenSerializeStream<AgrinApplicationNoticeSerialize>(backUpFileName);
                    if (deserialed.Items != null)
                        ApplicationNoticeManager.Current.DeSerialiedData(deserialed.Items.ToArray());
                }
            }
            catch (Exception exception)
            {
                Agrin.Log.AutoLogger.LogError(exception, "LoadNoticesFromFile", true);
            }
        }

        public static void MoveLastVersionData()
        {
            Action<string, string> moveToDirectory = (lastAddress, newAddress) =>
                {
                    try
                    {
                        if (File.Exists(lastAddress))
                            IOHelper.FileMove(lastAddress, newAddress);
                    }
                    catch
                    {

                    }
                };
            moveToDirectory(Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "Links.agn"), Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Links.agn"));
            moveToDirectory(Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "Groups.agn"), Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Groups.agn"));
            moveToDirectory(Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "AppSetting.agn"), Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppSetting.agn"));
        }
        //static AgrinApplicationData DeSerializeAgrinApplicationData(AgrinApplicationDataSerialize serializeInfo)
        //{
        //    AgrinApplicationData deSerializedData = new AgrinApplicationData();
        //    foreach (var address in serializeInfo.LinkInfoAddresses)
        //    {
        //        string fileName = Path.Combine(Agrin.IO.Helper.MPath.SaveDataPath, address.Key.ToString(), "Link.agn");
        //        if (File.Exists(fileName))
        //            deSerializedData.LinkInfoes.Add(DeSerializeLinkInfo((LinkInfoSerialize)SerializeStream.OpenSerializeStream(fileName)));
        //        else
        //            deSerializedData.LinkInfoes.Add(new LinkInfo(address.Value));
        //    }
        //    return deSerializedData;
        //}

        //static List<LinkInfo> DeSerializeLinkInfoes(List<LinkInfoSerialize> items)
        //{
        //    List<LinkInfo> deSerializedData = new List<LinkInfo>();
        //    foreach (var item in items.ToList())
        //    {
        //        deSerializedData.Add(DeSerializeLinkInfo(item));
        //    }
        //    return deSerializedData;
        //}

        //static LinkInfo DeSerializeLinkInfo(LinkInfoSerialize serializeInfo)
        //{
        //    LinkInfo deSerialized = new LinkInfo();
        //    deSerialized.DownloadingProperty = DeSerializeLinkInfoDownloadingProperty(serializeInfo.DownloadingProperty, deSerialized);
        //    deSerialized.Management = DeSerializeLinkInfoManagement(serializeInfo.Management);
        //    deSerialized.PathInfo = DeSerializeLinkInfoPath(serializeInfo.PathInfo);
        //    deSerialized.Properties = DeSerializeLinkInfoProperties(serializeInfo.Properties);
        //    foreach (var item in serializeInfo.Connections.ToList())
        //    {
        //        deSerialized.Connections.Add(DeSerializeConnectionInfo(item, deSerialized));
        //    }
        //    return deSerialized;
        //}

        //static LinkInfoProperties DeSerializeLinkInfoProperties(LinkInfoPropertiesSerialize serializeInfo)
        //{
        //    LinkInfoProperties deSerialized = new LinkInfoProperties() { };
        //    return deSerialized;
        //}

        //static LinkInfoPath DeSerializeLinkInfoPath(LinkInfoPathSerialize serializeInfo)
        //{
        //    LinkInfoPath deSerialized = new LinkInfoPath(serializeInfo.Address) { AddressFileName = serializeInfo.AddressFileName, ConnectionsSavedAddress = serializeInfo.ConnectionsSavedAddress, Id = serializeInfo.Id, UserFileName = serializeInfo.UserFileName };
        //    return deSerialized;
        //}

        //static LinkInfoManagement DeSerializeLinkInfoManagement(LinkInfoManagementSerialize serializeInfo)
        //{
        //    LinkInfoManagement deSerialized = new LinkInfoManagement() { ConnectionThreadCount = serializeInfo.ConnectionThreadCount, Errors = serializeInfo.Errors, IsTryExtreme = serializeInfo.IsTryExtreme, ReadBuffer = serializeInfo.ReadBuffer, TryAginCount = serializeInfo.TryAginCount };
        //    return deSerialized;
        //}

        //static LinkInfoDownloadingProperty DeSerializeLinkInfoDownloadingProperty(LinkInfoDownloadingPropertySerialize serializeInfo, LinkInfo parent)
        //{
        //    LinkInfoDownloadingProperty serialized = new LinkInfoDownloadingProperty(parent) { ConnectionCount = serializeInfo.ConnectionCount, DateLastDownload = serializeInfo.DateLastDownload, DownloadAlgoritm = serializeInfo.DownloadAlgoritm, ResumeCapability = serializeInfo.ResumeCapability, Size = serializeInfo.Size, DownloadRangePositions = serializeInfo.DownloadRangePositions };
        //    return serialized;
        //}

        //static ConnectionInfo DeSerializeConnectionInfo(ConnectionInfoSerialize serializeInfo, LinkInfo parent)
        //{
        //    ConnectionInfo deSerialized = ConnectionInfo.GetConnectionFromAlgoritm(parent.PathInfo.Address, parent.DownloadingProperty.DownloadAlgoritm);
        //    deSerialized.ConnectionId = serializeInfo.ConnectionId;
        //    deSerialized.EndPosition = serializeInfo.EndPosition;
        //    deSerialized.Length = serializeInfo.Length;
        //    deSerialized.RequestCookieContainer = serializeInfo.RequestCookieContainer;
        //    deSerialized.StartPosition = serializeInfo.StartPosition;
        //    deSerialized.UriDownload = serializeInfo.UriDownload;
        //    deSerialized.Parent = parent;
        //    return deSerialized;
        //}
    }
}
