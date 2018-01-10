using Agrin.Data.Mapping;
using Agrin.Download.Data.Serializition;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Download.Web;
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
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Download.Data
{
    public class VirtualSaver
    {
        //public bool IsSaveing { get; set; }
        //public int LockCount { get; set; }
        public object Lockobj = new object();
    }


    public static class SerializeData
    {
        static int _SaveingCount = 0;
        static object loclCH = new object();
        static void ChangeSavingCountValu(int val)
        {
            lock (loclCH)
            {
                _SaveingCount += val;
            }
        }

        //static bool _IsDispose = false;
        //static int errorCount = 0;
        public static void CloseApplicationWaitForSavingAllComplete()
        {
            //_IsDispose = true;
            while (_SaveingCount > 0)
            {
                Thread.Sleep(500);
            }
        }

        public static void VirtualSaveToFile(VirtualSaver virtualSaver, Action action)
        {
            ChangeSavingCountValu(1);
            //if (_IsDispose)
            //    return;
            AsyncActions.Action(() =>
            {
                //if (_IsDispose && !force)
                //    return;
                lock (virtualSaver.Lockobj)
                {
                    //if (_IsDispose && !force)
                    //    return;
                    //ChangeSavingCountValu(1);
                    //virtualSaver.IsSaveing = true;
                    //Thread.Sleep(100);
                    //virtualSaver.IsSaveing = false;
                    action();
                    ChangeSavingCountValu(-1);
                    //if (force)
                    //    ChangeSavingCountValu(-1);
                    //errorCount = 0;
                }
            }, (error) =>
            {
                lock (virtualSaver.Lockobj)
                {
                    ChangeSavingCountValu(-1);
                }
                //errorCount++;
                //if (force)
                //{
                //    if (errorCount >= 3)
                //        errorCount = 0;
                //    else
                //        VirtualSaveToFile(virtualSaver, action, force);
                //    ChangeSavingCountValu(-1);
                //}
            });
        }

        //static bool IsSavingLinkInfoesToFile = false, IsSavingApplicationTaskToFile=false;
        //count lock
        //static int CountSavingLinkInfoesToFile = 0, CountSavingApplicationTaskToFile=0;
        static VirtualSaver LinkInfoesToFileVirtualSaver = new VirtualSaver(), ApplicationTaskToFileVirtualSaver = new VirtualSaver(),
           AppServiceDataVirtualSaver = new VirtualSaver(), NetworkProxySettingsVirtualSaver = new VirtualSaver(), ApplicationSettingToFileVirtualSaver = new VirtualSaver(),
           SaveGroupInfoesToFileVirtualSaver = new VirtualSaver(), SaveLinkInfoDataToFileVirtualSaver = new VirtualSaver(), SaveLinkInfoBackUpDataToFileVirtualSaver = new VirtualSaver(),
           SaveNotificationToFileVirtualSaver = new VirtualSaver(), ApplicationIPsDataVirtualSaver = new VirtualSaver(), ApplicationNoticesVirtualSaver = new VirtualSaver();
        public static void SaveLinkInfoesToFile()
        {
            VirtualSaveToFile(LinkInfoesToFileVirtualSaver, SaveLinkInfoesToFileNoThread);

            //if (IsSavingLinkInfoesToFile)
            //    return;
            //AsyncActions.Action(() =>
            //{
            //    if (IsSavingLinkInfoesToFile)
            //        return;
            //    CountSavingLinkInfoesToFile++;
            //    lock (saveLockObj)
            //    {
            //        CountSavingLinkInfoesToFile--;
            //        if (CountSavingLinkInfoesToFile != 0)
            //            return;
            //        IsSavingLinkInfoesToFile = true;
            //        Thread.Sleep(1500);
            //        IsSavingLinkInfoesToFile = false;
            //        SaveLinkInfoesToFileNoThread();
            //    }
            //}, (error) =>
            //{
            //    Agrin.Log.AutoLogger.LogError(error, "SaveLinkInfoesToFile");
            //});
        }

        public static void SaveApplicationIPsDataToFile()
        {
            VirtualSaveToFile(ApplicationIPsDataVirtualSaver, SaveApplicationIPsDataNoThread);
        }

        public static void SaveApplicationTaskToFile()
        {
            VirtualSaveToFile(ApplicationTaskToFileVirtualSaver, SaveApplicationTaskToFileNoThread);
            //AsyncActions.Action(() =>
            //{
            //    lock (saveLockObj)
            //    {
            //        SaveApplicationTaskToFileNoThread();
            //    }
            //}, (error) =>
            //{
            //    Agrin.Log.AutoLogger.LogError(error, "SaveApplicationTaskToFile");
            //});
        }



        public static void SaveAppServiceDataToFile()
        {
            VirtualSaveToFile(AppServiceDataVirtualSaver, SaveAppServiceData);

            //AsyncActions.Action(() =>
            //{
            //    lock (saveLockObj)
            //    {
            //        SaveAppServiceData();
            //    }
            //}, (error) =>
            //{
            //    Agrin.Log.AutoLogger.LogError(error, "SaveAppServiceDataToFile");
            //});
        }
#if (!MobileApp && !XamarinApp)
        public static void SaveNetworkProxySettingsFile()
        {
            VirtualSaveToFile(NetworkProxySettingsVirtualSaver, SaveNetworkProxySettings);
        }
#endif

        public static void SaveApplicationSettingToFile()
        {
            if (ApplicationSetting.Current.IsSettingForAllLinks)
            {
                foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes)
                {
                    var findedAccount = AppUserAccountsSetting.FindFromAddress(item.PathInfo.Address);
                    if (findedAccount != null)
                        item.Management.NetworkUserPass = findedAccount;
                    DeSerializeData.AppSettingToLinkInfo(item);
                }
            }
            VirtualSaveToFile(ApplicationSettingToFileVirtualSaver, SaveApplicationSettingToFileNoThread);
            //AsyncActions.Action(() =>
            //{
            //    lock (saveLockObj)
            //    {
            //        SaveApplicationSettingToFileNoThread();
            //    }
            //}, (error) =>
            //{
            //    Agrin.Log.AutoLogger.LogError(error, "SaveApplicationSettingToFile");
            //});
        }

        public static void SaveGroupInfoesToFile()
        {
            VirtualSaveToFile(SaveGroupInfoesToFileVirtualSaver, SaveGroupInfoesToFileNoThread);
            //AsyncActions.Action(() =>
            //{
            //    lock (saveLockObj)
            //    {
            //        SaveGroupInfoesToFileNoThread();
            //    }
            //}, (error) =>
            //{
            //    Agrin.Log.AutoLogger.LogError(error, "SaveGroupInfoesToFile");
            //});
        }

        static object saveLockObj = new object();
        public static void SaveLinkInfoDataToFile(LinkInfo info, string fileName = "Link.agn", string address = null, bool force = false)
        {
            //if (fileName == "Link.agn")
            //    return; 
            VirtualSaver saver = null;
            if (fileName == "Link_BackUp.agn")
                saver = SaveLinkInfoBackUpDataToFileVirtualSaver;
            else
                saver = SaveLinkInfoDataToFileVirtualSaver;
            VirtualSaveToFile(saver, () =>
            {
                Func<string, object, object> convert = null;
                List<string> manualNames = new List<string>() { "Connections", "UserGroupInfo", "ApplicationGroupInfo", "ListSpeedByteDownloaded" };

                convert = (mapName, mapType) =>
                {
                    if (mapName == "Connections")
                    {
                        List<ConnectionInfoSerialize> ret = new List<ConnectionInfoSerialize>();
                        foreach (var item in ((IList<LinkWebRequest>)mapType).ToList())
                        {
                            ConnectionInfoSerialize cloned = new ConnectionInfoSerialize();
                            Mapper.Map<LinkWebRequest, ConnectionInfoSerialize>(item, cloned, convert, manualNames);
                            ret.Add(cloned);
                        }
                        return ret;
                    }
                    else if (mapName == "UserGroupInfo" || mapName == "ApplicationGroupInfo")
                    {
                        GroupInfo groupInfo = (GroupInfo)mapType;
                        if (groupInfo == null)
                            return null;
                        return groupInfo.Name;
                    }
                    else if (mapName == "ListSpeedByteDownloaded")
                    {
                        List<double> items = new List<double>();
                        var list = ((IList<double>)mapType);
                        items.AddRange(list);
                        return items;
                    }
                    return null;
                };
                LinkInfoSerialize serialed = new LinkInfoSerialize();
                Mapper.Map<LinkInfo, LinkInfoSerialize>(info, serialed, convert, manualNames);
                if (address != null)
                    SerializeStream.SaveSerializeStream(System.IO.Path.Combine(address, fileName), serialed);
                else
                    SerializeStream.SaveSerializeStream(System.IO.Path.Combine(info.PathInfo.ConnectionsSavedAddress, fileName), serialed);
            });
        }

        public static void SaveNotificationToFile()
        {
            VirtualSaveToFile(SaveNotificationToFileVirtualSaver, () =>
            {
                Func<string, object, object> convert = null;
                List<string> manualNames = new List<string>() { "Items" };

                convert = (mapName, mapType) =>
                {
                    if (mapName == "Items")
                    {
                        List<int> ret = new List<int>();
                        foreach (var item in ((IList<LinkInfo>)mapType).ToList())
                        {
                            ret.Add(item.PathInfo.Id);
                        }
                        return ret;
                    }
                    return null;
                };
                ApplicationNotificationSerialize serialed = new ApplicationNotificationSerialize() { NotificationInfoes = new List<NotificationSerialize>() };
                foreach (var item in ApplicationNotificationManager.Current.NotificationInfoes.ToList())
                {
                    NotificationSerialize serial = new NotificationSerialize();
                    Mapper.Map<NotificationInfo, NotificationSerialize>(item, serial, convert, manualNames);
                    serialed.NotificationInfoes.Add(serial);
                }
                SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Notifications.agn"), serialed);
                SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Notifications_BackUp.agn"), serialed);

            });
            //AsyncActions.Action(() =>
            //{
            //    lock (saveLockObj)
            //    {
            //    }
            //}, (exception) =>
            //{
            //    Agrin.Log.AutoLogger.LogError(exception, "SaveNotificationToFile");
            //});
        }



        public static void SaveLinkInfoesToFileNoThread()
        {
            string fileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Links.agn");
            AgrinApplicationDataShortSerialize serialed = new AgrinApplicationDataShortSerialize();
            List<LinkInfoShortSerialize> saveLists = new List<LinkInfoShortSerialize>();
            if (ApplicationLinkInfoManager.Current == null)
                AutoLogger.LogText("SaveLinkInfoesToFileNoThread ApplicationLinkInfoManager.Current is null");
            foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToList())
            {
                saveLists.Add(new LinkInfoShortSerialize() { Address = item.PathInfo.Address, AddressFileName = item.PathInfo.AddressFileName, ApplicationGroupInfo = item.PathInfo.ApplicationGroupInfo != null ? item.PathInfo.ApplicationGroupInfo.Name : null, CreateDateTime = item.DownloadingProperty.CreateDateTime, Id = item.PathInfo.Id, UserFileName = item.PathInfo.UserFileName, UserGroupInfo = item.PathInfo.UserGroupInfo != null ? item.PathInfo.UserGroupInfo.Name : null, UserSavePath = item.PathInfo.UserSavePath, Size = item.DownloadingProperty.Size, State = item.DownloadingProperty.State, UserNameAuthorization = item.Management.NetworkUserPass == null ? "" : item.Management.NetworkUserPass.UserName, PasswordAuthorization = item.Management.NetworkUserPass == null ? "" : item.Management.NetworkUserPass.Password, DomainNameAuthorization = item.Management.NetworkUserPass == null ? "" : item.Management.NetworkUserPass.ServerAddress, IsApplicationSetting = item.Management.IsApplicationSetting, BackUpCompleteAddress = item.PathInfo.BackUpCompleteAddress, SharingSettings = item.Management.SharingSettings, MixerBackupSavePath = item.PathInfo.MixerBackupSavePath, MixerSavePath = item.PathInfo.MixerSavePath, CustomHeaders = item.DownloadingProperty.CustomHeaders });
            }
            serialed.LinkInfoes = saveLists;
            SerializeStream.SaveSerializeStream(fileName, serialed);
            SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Links_BackUp.agn"), serialed);
        }

        public static void SaveNoticesToFile()
        {
            VirtualSaveToFile(ApplicationNoticesVirtualSaver, SaveNoticesToFileNoThread);
        }

        static void SaveNoticesToFileNoThread()
        {
            string fileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Notices.agn");
            AgrinApplicationNoticeSerialize serialed = new AgrinApplicationNoticeSerialize();

            serialed.Items = ApplicationNoticeManager.Current.Items.ToList();
            SerializeStream.SaveSerializeStream(fileName, serialed);
            SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Notices_BackUp.agn"), serialed);
        }

        public static void SaveApplicationIPsDataNoThread()
        {
            string fileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "ApplicationIPsData.agn");
            SerializeStream.SaveSerializeStream(fileName, ApplicationIPsData.Current);
        }

        public static void SaveGroupInfoesToFileNoThread()
        {
            string fileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Groups.agn");
            AgrinApplicationGroupDataSerialize serialed = new AgrinApplicationGroupDataSerialize();
            List<GroupInfoSerialize> saveLists = new List<GroupInfoSerialize>();
            foreach (var item in ApplicationGroupManager.Current.GroupInfoes.ToList())
            {
                saveLists.Add(new GroupInfoSerialize() { Name = item.Name, UserSavePath = item.UserSavePath, SaveFolderName = item.SaveFolderName, Extentions = item.Extentions, Id = item.Id, IsExpanded = item.IsExpanded, UserSecurityPath = item.UserSecurityPath });
            }
            serialed.GroupInfoes = saveLists;
            SerializeStream.SaveSerializeStream(fileName, serialed);
            SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "Groups_BackUp.agn"), serialed);
        }

        static void SaveApplicationTaskToFileNoThread()
        {
            string fileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppTasks.agn");
            AgrinApplicationTasksSerialize serialed = new AgrinApplicationTasksSerialize();

            serialed.TaskInfoes = new List<TaskInfoSerialize>();
            Func<string, object, object> convert = null;
            List<string> manualNames = new List<string>() { "TaskItemInfoes" };

            convert = (mapName, mapType) =>
            {
                if (mapName == "TaskItemInfoes")
                {
                    if (mapType is List<TaskItemInfo>)
                    {
                        List<TaskItemInfoSerialize> ret = new List<TaskItemInfoSerialize>();
                        foreach (var item in ((List<TaskItemInfo>)mapType).ToList())
                        {
                            TaskItemInfoSerialize cloned = new TaskItemInfoSerialize();
                            Mapper.Map<TaskItemInfo, TaskItemInfoSerialize>(item, cloned);
                            ret.Add(cloned);
                        }
                        return ret;
                    }
                }
                return null;
            };
            foreach (var item in ApplicationTaskManager.Current.TaskInfoes.ToList())
            {
                var serialedItem = new TaskInfoSerialize();
                Mapper.Map<TaskInfo, TaskInfoSerialize>(item, serialedItem, convert, manualNames);
                serialed.TaskInfoes.Add(serialedItem);
            }
            SerializeStream.SaveSerializeStream(fileName, serialed);
            SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppTasks_BackUp.agn"), serialed);
        }

        static void SaveApplicationSettingToFileNoThread()
        {
            string fileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppSetting.agn");
            SerializeStream.SaveSerializeStream(fileName, ApplicationSetting.Current);
            SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppSetting_BackUp.agn"), ApplicationSetting.Current);
        }

        static void SaveAppServiceData()
        {
            string fileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppServiceData.agn");
            SerializeStream.SaveSerializeStream(fileName, ApplicationServiceData.Current);
            SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "AppServiceData_BackUp.agn"), ApplicationServiceData.Current);
        }

#if (!MobileApp && !XamarinApp)
        static void SaveNetworkProxySettings()
        {
            string fileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "NetworkProxySettings.agn");
            SerializeStream.SaveSerializeStream(fileName, NetworkProxySettings.Current);
            SerializeStream.SaveSerializeStream(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "NetworkProxySettings_BackUp.agn"), NetworkProxySettings.Current);
        }
#endif
        //static AgrinApplicationDataSerialize SerializeAgrinApplicationData()
        //{
        //    AgrinApplicationDataSerialize serializedData = new AgrinApplicationDataSerialize();
        //    serializedData.LinkInfoAddresses = new Dictionary<int, string>();
        //    foreach (var item in AgrinApplicationData.This.LinkInfoes.ToList())
        //    {
        //        serializedData.LinkInfoAddresses.Add(item.PathInfo.Id, item.PathInfo.Address);
        //    }
        //    return serializedData;
        //}

        //static List<LinkInfoSerialize> SerializeLinkInfoes(List<LinkInfo> items)
        //{
        //    List<LinkInfoSerialize> serializedData = new List<LinkInfoSerialize>();
        //    foreach (var item in items.ToList())
        //    {
        //        serializedData.Add(SerializeLinkInfo(item));
        //    }
        //    return serializedData;
        //}

        //static LinkInfoSerialize SerializeLinkInfo(LinkInfo serializeInfo)
        //{
        //    LinkInfoSerialize serialized = new LinkInfoSerialize();
        //    serialized.DownloadingProperty = SerializeLinkInfoDownloadingProperty(serializeInfo.DownloadingProperty);
        //    serialized.Management = SerializeLinkInfoManagement(serializeInfo.Management);
        //    serialized.PathInfo = SerializeLinkInfoPath(serializeInfo.PathInfo);
        //    serialized.Properties = SerializeLinkInfoProperties(serializeInfo.Properties);
        //    serialized.Connections = new List<ConnectionInfoSerialize>();
        //    foreach (var item in serializeInfo.Connections.ToList())
        //    {
        //        serialized.Connections.Add(SerializeConnectionInfo(item));
        //    }
        //    return serialized;
        //}

        //static LinkInfoPropertiesSerialize SerializeLinkInfoProperties(LinkInfoProperties serializeInfo)
        //{
        //    LinkInfoPropertiesSerialize serialized = new LinkInfoPropertiesSerialize() { };
        //    return serialized;
        //}

        //static LinkInfoPathSerialize SerializeLinkInfoPath(LinkInfoPath serializeInfo)
        //{
        //    LinkInfoPathSerialize serialized = new LinkInfoPathSerialize() { Address = serializeInfo.Address, AddressFileName = serializeInfo.AddressFileName, ConnectionsSavedAddress = serializeInfo.ConnectionsSavedAddress, Id = serializeInfo.Id, UserSavePath = serializeInfo.UserSavePath, UserFileName = serializeInfo.UserFileName };
        //    return serialized;
        //}

        //static LinkInfoManagementSerialize SerializeLinkInfoManagement(LinkInfoManagement serializeInfo)
        //{
        //    LinkInfoManagementSerialize serialized = new LinkInfoManagementSerialize() { ConnectionThreadCount = serializeInfo.ConnectionThreadCount, Errors = serializeInfo.Errors, IsTryExtreme = serializeInfo.IsTryExtreme, ReadBuffer = serializeInfo.ReadBuffer, TryAginCount = serializeInfo.TryAginCount };
        //    return serialized;
        //}

        //static LinkInfoDownloadingPropertySerialize SerializeLinkInfoDownloadingProperty(LinkInfoDownloadingProperty serializeInfo)
        //{
        //    LinkInfoDownloadingPropertySerialize serialized = new LinkInfoDownloadingPropertySerialize() { ConnectionCount = serializeInfo.ConnectionCount, DateLastDownload = serializeInfo.DateLastDownload, DownloadAlgoritm = serializeInfo.DownloadAlgoritm, ResumeCapability = serializeInfo.ResumeCapability, Size = serializeInfo.Size, DownloadRangePositions = serializeInfo.DownloadRangePositions };
        //    return serialized;
        //}

        //static ConnectionInfoSerialize SerializeConnectionInfo(ConnectionInfo serializeInfo)
        //{
        //    ConnectionInfoSerialize serialized = new ConnectionInfoSerialize() {  ConnectionId = serializeInfo.ConnectionId, EndPosition = serializeInfo.EndPosition, Length = serializeInfo.Length, RequestCookieContainer = serializeInfo.RequestCookieContainer, StartPosition = serializeInfo.StartPosition, UriDownload = serializeInfo.UriDownload };
        //    return serialized;
        //}
    }
}
