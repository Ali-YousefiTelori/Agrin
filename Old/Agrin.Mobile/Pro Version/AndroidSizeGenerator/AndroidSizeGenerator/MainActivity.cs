using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Agrin.Helpers;
using Android.Net;
using System.Text;
using Java.Lang;
using System.Linq;
using System.Threading.Tasks;

namespace AndroidSizeGenerator
{
    //private class LoadViewTask : AsyncTask<System.Action, int, System.Action>
    //{
    //    //Before running code in the separate thread
    //    protected override void OnPreExecute()
    //    {

    //    }
    //    protected override System.Action RunInBackground(System.Action[] param)
    //    {
    //        /* This is just a code that delays the thread execution 4 times, 
    //          * during 850 milliseconds and updates the current progress. This 
    //          * is where the code that is going to be executed on a background
    //          * thread must be placed. 
    //          */
    //        try
    //        {
    //            //Get the current thread's token

    //            //Initialize an integer (that will act as a counter) to zero
    //            int counter = 0;
    //            //While the counter is smaller than four
    //            while (counter <= 4)
    //            {
    //                //Wait 850 milliseconds
    //                this.Wait(1000);
    //                //Increment the counter 
    //                counter++;
    //                //Set the current progress. 
    //                //This value is going to be passed to the onProgressUpdate() method.
    //                PublishProgress(counter * 25);
    //            }

    //        }
    //        catch (InterruptedException e)
    //        {

    //        }
    //        return null;
    //    }
    //    //Update the progress
    //    protected override void OnProgressUpdate(int values)
    //    {
    //        //set the current progress of the progress dialog
    //        //progressDialog.setProgress(values[0]);
    //    }

    //    //after executing the code in the thread
    //    protected override void OnPostExecute(Java.Lang.Void result)
    //    {
    //        //close the progress dialog
    //        //progressDialog.dismiss();
    //        //initialize the View
    //        //setContentView(R.layout.main);
    //    }
    //}
    [Activity(Label = "AndroidSizeGenerator", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public async System.Threading.Tasks.Task<int> MyMethodAsync()
        {
            System.Threading.Tasks.Task<int> longRunningTask = LongRunningOperationAsync();
            // independent work which doesn't need the result of LongRunningOperationAsync can be done here

            //and now we call await on the task 
            int result = await longRunningTask;
            //use the result 
            //Console.WriteLine(result);
            return result;
        }

        public async System.Threading.Tasks.Task<int> LongRunningOperationAsync() // assume we return an int from this long running operation 
        {
            await System.Threading.Tasks.Task.Delay(1000); //1 seconds delay
            return 1;
        }

        int count = 1;
        public static MainActivity This;
        protected override void OnCreate(Bundle bundle)
        {
            This = this;
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            var packageName = this.PackageName;
            string add = "/"; System.IO.Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, "Android", "data", packageName, "files");


            try
            {
                var value = MyMethodAsync();
                Button btn = this.FindViewById<Button>(Resource.Id.MyButton);
                Task task = new Task(() =>
                {

                    Button btn222 = new Button(this);
                    btn222.Text = "alisefsdf";
                    LinearLayout layout = new LinearLayout(this);
                    layout.AddView(btn222);
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            mainLayout.AddView(layout);
                        }
                        catch (Exception ex2)
                        {

                        }
                    });

                });
                task.Start();
                //btn.Text = value.Result.ToString();
                return;
                ////var locations = ExternalStorage.getAllStorageLocations();
                //var drives = System.IO.DriveInfo.GetDrives();
                ////List<string> locations = new List<string>();

                //TextView text = new TextView(MainActivity.This);
                //ViewsUtility.SetTextViewTextColor(MainActivity.This, text, Resource.Color.foreground);
                //System.Text.StringBuilder str = new System.Text.StringBuilder();

                //try
                //{
                //string ramAddress = "";
                //string createdOK = "";
                //foreach (var item in drives)
                //{
                //    string pt = "";
                //    try
                //    {
                //        if (item.DriveType == System.IO.DriveType.Ram)
                //        {
                //            pt= add = System.IO.Path.Combine(item.RootDirectory.FullName, "Android", "data", packageName, "files");
                //            if (!System.IO.Directory.Exists(add))
                //                System.IO.Directory.CreateDirectory(add);
                //            str.AppendLine("ok: " + add);
                //            if (string.IsNullOrEmpty(createdOK))
                //                createdOK = add;
                //            Toast.MakeText(this, "Yes", ToastLength.Long).Show();
                //        }
                //    }
                //    catch (Java.Lang.Exception javaex)
                //    {
                //        str.AppendLine("Jex: " + javaex.Message + " path = " + pt);
                //    }
                //    catch (System.Exception ex)
                //    {
                //        str.AppendLine("ex: " + ex.Message + " path = " + pt);
                //    }

                //}

                //text.Text = str.ToString();
                //ViewsUtility.ShowControlDialog(this, text, "title", null);
                //if (!string.IsNullOrEmpty(createdOK))
                //{
                ViewsUtility.ShowFolderBrowseInLayout(this, mainLayout, add, (path) =>
                {

                });
                //}
                //else
                //{
                //    Toast.MakeText(this, "Not Created", ToastLength.Long).Show();
                //}
                //if (locations.Count > 0)
                //{

                //}
                //else
                //{
                //    Toast.MakeText(this, "no path found", ToastLength.Long).Show();
                //}

                //}
                //catch (Java.Lang.Exception javaex)
                //{
                //    Toast.MakeText(this, "Jcreate err:" + javaex.Message, ToastLength.Long).Show();
                //}
                //catch (System.Exception ex)
                //{
                //    Toast.MakeText(this, "create err:" + ex.Message, ToastLength.Long).Show();
                //}


            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message + " path: " + add, ToastLength.Long).Show();
            }
            // Get our button from the layout resource,
            // and attach an event to it
        }

        public static List<string> getExternalMounts()
        {
            List<string> list = new List<string>();
            string reg = "(?i).*vold.*(vfat|ntfs|exfat|fat32|ext3|ext4).*rw.*";
            String s = null;
            try
            {
                var process = new ProcessBuilder().Command("mount")
                       .RedirectErrorStream(true).Start();
                process.WaitFor();
                var stream = new System.IO.StreamReader(process.InputStream);
                byte[] buffer = new byte[1024];
                s = new String(stream.ReadToEnd());
                stream.Close();
            }
            catch (Exception e)
            {


            }

            // parse output
            string[] lines = s.Split("\n");
            foreach (var sl in lines)
            {
                String line = new String(sl);
                if (!line.ToLowerCase(Java.Util.Locale.Us).Contains("asec"))
                {
                    if (line.Matches(reg))
                    {
                        string[] parts = line.Split(" ");
                        foreach (var pl in parts)
                        {
                            String part = new String(pl);
                            if (part.StartsWith("/"))
                                if (!part.ToLowerCase(Java.Util.Locale.Us).Contains("vold"))
                                    list.Add(part.ToString());
                        }


                    }
                }
            }
            return list;
        }
        public static void triggerStorageAccessFramework()
        {
            Android.Content.Intent intent = new Android.Content.Intent(Intent.ActionOpenDocumentTree);
            MainActivity.This.StartActivityForResult(intent, 42);
        }

        public static void triggerStorageAccessCreateDocumentFramework()
        {
            Android.Content.Intent intent = new Android.Content.Intent(Intent.ActionCreateDocument);
            intent.SetType(Android.Provider.DocumentsContract.Document.MimeTypeDir);
            MainActivity.This.StartActivityForResult(intent, 52);
        }

        public static System.Action<Uri> ActionSetPermission = null;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 42)
            {
                Toast.MakeText(MainActivity.This, resultCode.ToString(), ToastLength.Short).Show();
                Uri treeUri = null;
                if (resultCode == Result.Ok)
                {
                    // Get Uri from Storage Access Framework.
                    treeUri = data.Data;
                    var takeFlags = data.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                    // Check for the freshest data.
                    MainActivity.This.ContentResolver.TakePersistableUriPermission(treeUri, takeFlags);
                    if (ActionSetPermission != null)
                        ActionSetPermission(treeUri);
                }
                // Persist URI in shared preference so that you can use it later.
                // Use your own framework here instead of PreferenceUtil.
                //    PreferenceUtil.setSharedPreferenceUri(R.string.key_internal_uri_extsdcard, treeUri);

                //    // Persist access permissions.
                //    int takeFlags = resultData.getFlags()
                //        & (Android.Content.Intent.FLAG_GRANT_READ_URI_PERMISSION | Intent.FLAG_GRANT_WRITE_URI_PERMISSION);
                //getActivity().getContentResolver().takePersistableUriPermission(treeUri, takeFlags);
            }

        }
    }
    public class ExternalStorage
    {

        public static string SD_CARD = "sdCard";
        public static string EXTERNAL_SD_CARD = "externalSdCard";

        /**
         * @return True if the external storage is available. False otherwise.
         */
        public static bool isAvailable()
        {
            string state = Environment.ExternalStorageState;
            if (Environment.MediaMounted.Equals(state) || Environment.MediaMountedReadOnly.Equals(state))
            {
                return true;
            }
            return false;
        }

        public static string getSdCardPath()
        {
            return Environment.ExternalStorageDirectory.Path + "/";
        }

        /**
         * @return True if the external storage is writable. False otherwise.
         */
        public static bool isWritable()
        {
            string state = Environment.ExternalStorageState;
            if (Environment.MediaMounted.Equals(state))
            {
                return true;
            }
            return false;

        }

        /**
         * @return A map of all storage locations available
         */
        public static Dictionary<string, string> getAllStorageLocations()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            List<string> mMounts = new List<string>();
            List<string> mVold = new List<string>();
            mMounts.Add("/mnt/sdcard");
            mVold.Add("/mnt/sdcard");

            try
            {
                var mountFile = new Java.IO.File("/proc/mounts");
                if (mountFile.Exists())
                {
                    Java.Util.Scanner scanner = new Java.Util.Scanner(mountFile);
                    while (scanner.HasNext)
                    {
                        string line = scanner.NextLine();
                        if (line.StartsWith("/dev/block/vold/"))
                        {
                            string[] lineElements = line.Split(new char[] { ' ' });
                            string element = lineElements[1];

                            // don't add the default mount path
                            // it's already in the list.
                            if (!element.Equals("/mnt/sdcard"))
                                mMounts.Add(element);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {

            }

            try
            {
                var voldFile = new Java.IO.File("/system/etc/vold.fstab");
                if (voldFile.Exists())
                {
                    Java.Util.Scanner scanner = new Java.Util.Scanner(voldFile);
                    while (scanner.HasNext)
                    {
                        string line = scanner.NextLine();
                        if (line.StartsWith("dev_mount"))
                        {
                            string[] lineElements = line.Split(new char[] { ' ' });
                            string element = lineElements[2];

                            if (element.Contains(":"))
                                element = element.Substring(0, element.IndexOf(":"));
                            if (!element.Equals("/mnt/sdcard"))
                                mVold.Add(element);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {


            }


            for (int i = 0; i < mMounts.Count; i++)
            {
                string mount = mMounts[i];
                if (!mVold.Contains(mount))
                    mMounts.RemoveAt(i--);
            }
            mVold.Clear();

            List<string> mountHash = new List<string>();

            foreach (var mount in mMounts)
            {
                Java.IO.File root = new Java.IO.File(mount);
                if (root.Exists() && root.IsDirectory && root.CanWrite())
                {
                    Java.IO.File[] list = root.ListFiles();
                    string hash = "[";
                    if (list != null)
                    {
                        foreach (var f in list)
                        {
                            hash += f.Name.GetHashCode() + ":" + f.Length() + ", ";

                        }
                    }
                    hash += "]";
                    if (!mountHash.Contains(hash))
                    {
                        string key = SD_CARD + "_" + map.Count;
                        if (map.Count == 0)
                        {
                            key = SD_CARD;
                        }
                        else if (map.Count == 1)
                        {
                            key = EXTERNAL_SD_CARD;
                        }
                        mountHash.Add(hash);
                        map.Add(SD_CARD, root.AbsolutePath);
                    }
                }
            }
            mMounts.Clear();

            if (map.Count == 0)
            {
                map.Add(SD_CARD, Environment.ExternalStorageDirectory.AbsolutePath);
            }
            return map;
        }
    }
}

