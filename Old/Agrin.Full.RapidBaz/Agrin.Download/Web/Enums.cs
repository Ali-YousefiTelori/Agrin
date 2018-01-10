using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web
{
    public enum TaskState
    {
        Working,
        Stoped,
        Started,
        WaitingForWork
    }

    public enum RapidStatusEnum
    {
        Success,
        IsQueueUpload,
        ErrorUploading,
        DuplicateLink,
        NoFreeSpaceInAccount,
        CannotUploadFile,
        ChekingFile,
        UploadingFile,
        FileIsCorrupt
    }

    public enum TaskUtilityModeEnum
    {
        StartLink,
        StopLink,
        StopTasks,
        DeactiveTasks,
        ActiveTasks,
        CloseApplication,
        WiFiOn,
        WiFiOff,
        InternetOn,
        InternetOf,
        TurrnOff,
        Sleep,
    }

    public enum TaskModeEnum
    {
        Download,
        Utility
    }

    public enum ConnectionState
    {
        Stoped = 0,
        Downloading = 1,
        Pausing = 2,
        Paused = 3,
        GetingData = 4,
        Complete = 5,
        Error = 6,
        Connecting = 7,
        CopyingFile = 8,
        Waiting = 9,
        Disposed = 10,
        ConnectingToSharing = 11,
        LoginToSharing = 12,
        GetAddressFromSharing = 13,
        FindingAddressFromSharing = 14,
        SharingTimer = 15,
        Uploading = 16,
        WaitForTick = 17,
        CreatingRequest = 18,
        BrokenLink = 19,
        CheckForSupportResumable = 20,
        //MustReCheck = 22
    }
    public enum AlgoritmEnum
    {
        Unknown,
        Normal,
        Page,
        Sharing,
        Torrent
    }
    public enum ResumeCapabilityEnum
    {
        Yes,
        No,
        Unknown
    }
    public enum SizeEnum
    {
        Byte = 0,
        KB = 1,
        MB = 2,
        GB = 3,
        TB = 4,
        EXB = 5
    }
    public enum StartableEnum
    {
        Started,
        Stoped
    }
}
