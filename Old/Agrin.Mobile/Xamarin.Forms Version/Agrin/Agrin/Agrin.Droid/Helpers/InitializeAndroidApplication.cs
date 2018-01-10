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
using Agrin.ViewModels.Helpers;
using System.IO;

namespace Agrin.Droid.Helpers
{
    public class InitializeAndroidApplication : InitializeApplication
    {
        public static InitializeAndroidApplication Current { get; set; }
        string _DownloadsDirectory;
        string _StoragePublicDirectory;

        public override string DownloadsDirectory
        {
            get
            {
                return _DownloadsDirectory;
            }

            set
            {
                _DownloadsDirectory = value;
            }
        }

        public override string StoragePublicDirectory
        {
            get
            {
                return _StoragePublicDirectory;
            }

            set
            {
                _StoragePublicDirectory = value;
            }
        }

        public override void AddToGallery(string fileName)
        {

        }

        public override void Initialize()
        {
            Run();
        }

        public override void InitializeLimitDrawing()
        {

        }

        public override Func<string, string, FileMode, Action<string>, object, Agrin.IO.Streams.IStreamWriter> OpenFileStreamForWriteAction()
        {
            return null;
        }

        public override void ShowToast(string msg, bool isLong)
        {

        }

        public override void TriggerStorageAccessFramework()
        {

        }
    }
}