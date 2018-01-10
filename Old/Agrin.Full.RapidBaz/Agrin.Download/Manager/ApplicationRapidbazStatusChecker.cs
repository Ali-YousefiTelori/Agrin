using Agrin.Download.Data.Settings;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.RapidBaz.Users;
using Agrin.RapidBaz.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agrin.Download.Manager
{
    public static class ApplicationRapidbazStatusChecker
    {
        public static void Start()
        {
            AsyncActions.Action(() =>
            {
                while (true)
                {
                    try
                    {
                    Check:
                        if (UserManager.IsLogin)
                        {
                            var list = ApplicationSetting.Current.RapidBazSetting.DownloadAfterCompleteInRapidBaz.ToList();
                            if (list.Count > 0)
                            {
                                foreach (var item in list)
                                {
                                    var file = WebManager.FileStatus(item.QueueID);
                                    if (file.Status == "0")
                                    {
                                        var uriAddress = GetUrlForDownload(file.ID);
                                        if (string.IsNullOrEmpty(uriAddress))
                                            continue;
                                        ApplicationSetting.Current.RapidBazSetting.AddToDownloadList(uriAddress, item);
                                    }
                                }
                            }
                            Thread.Sleep(5000);
                        }
                        else
                        {
                            WebManager.CheckLogin();
                            if (UserManager.IsLogin)
                                goto Check;
                            Thread.Sleep(10000);
                        }
                    }
                    catch
                    {
                        Thread.Sleep(5000);
                    }
                }
            });
        }

        public static string GetUrlForDownload(string queueID)
        {
            var list = WebManager.GetCompleteList();
            foreach (var item in list)
            {
                if (item.ID == queueID)
                    return item.Link;
            }
            return "";
        }
    }
}
