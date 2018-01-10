using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web
{
    public enum TaskState
    {
        Working = 0,
        Stoped = 1,
        Started = 2,
        WaitingForWork = 3
    }

    public enum RapidStatusEnum
    {
        Success = 1,
        IsQueueUpload = 2,
        ErrorUploading = 3,
        DuplicateLink = 4,
        NoFreeSpaceInAccount = 5,
        CannotUploadFile = 6,
        ChekingFile = 7,
        UploadingFile = 8,
        FileIsCorrupt = 9
    }

    public enum TaskUtilityModeEnum
    {
        StartLink = 0,
        StopLink = 1,
        StopTasks = 2,
        DeactiveTasks = 3,
        ActiveTasks = 4,
        CloseApplication = 5,
        WiFiOn = 6,
        WiFiOff = 7,
        InternetOn = 8,
        InternetOff = 9,
        TurrnOff = 10,
        Sleep = 11,
    }

    public enum TaskModeEnum
    {
        Download = 0,
        Utility = 1
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
        Unknown = 0,
        Normal = 1,
        Page = 2,
        Sharing = 3,
        Torrent = 4
    }
    public enum ResumeCapabilityEnum
    {
        Yes = 0,
        No = 1,
        Unknown = 2
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
        Started = 0,
        Stoped = 1
    }
}
