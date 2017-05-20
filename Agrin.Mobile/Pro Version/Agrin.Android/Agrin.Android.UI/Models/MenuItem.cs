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

namespace Agrin.Models
{
    public class MenuItem
    {
        public string Content { get; set; }
        public MenuItemModeEnum Mode { get; set; }
    }

    public enum MenuItemModeEnum
    {
        OpenFile = 0,
        OpenFileLocation = 1,
        ChangeSavePath = 2,
        ShareFile = 3,
        ChangeLinkAddress = 4,
        CopyLink = 5,
        CheckLastError = 6,
        RepairLink = 7,//
        Play = 8,
        Stop = 9,
        Delete = 10,
        DownloadByQueue = 11,
        StopByQueue = 12,
        DeleteLinkInfoQueue = 13,
        //--------
        DeleteComplete = 14,
        SelectAll = 15,
        DeSelectAll = 16,
        MultiSelection = 17,
        MenuSelection = 18,
        StopAll = 19,
        Learning = 20,
        About = 21,
        TaskInfoesManagement = 22,
        ActiveTask = 23,
        DeActiveTask = 24,
        //--------
        LinksSearchAll = 25,
        LinksSearchComplete = 26,
        LinksSearchDownloading = 27,
        LinksSearchNotComplete = 28,
        LinksSearchError = 29,
        LinksSearchQueue = 30,
        RenameFileName = 31,
        ForcePlay = 32,
        GenerateAutoMixerReport = 33,
        AgrinBrowser = 34,
    }
}