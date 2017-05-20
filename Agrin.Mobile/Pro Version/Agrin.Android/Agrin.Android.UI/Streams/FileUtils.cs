using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;
using Android.Database;
using Android.Provider;
using static Android.Provider.DocumentsContract;
using Java.IO;
using Java.Util;
using Agrin.Log;

namespace Agrin.Streams
{
    public class StorageInfo
    {
        public string path;
        public bool readonlyValue;
        public bool removable;
        public int number;

        StorageInfo(string path, bool _readonly, bool removable, int number)
        {
            this.path = path;
            this.readonlyValue = _readonly;
            this.removable = removable;
            this.number = number;
        }

        public string getDisplayName()
        {
            StringBuilder res = new StringBuilder();
            if (!removable)
            {
                res.Append("Internal SD card");
            }
            else if (number > 1)
            {
                res.Append("SD card " + number);
            }
            else
            {
                res.Append("SD card");
            }
            if (readonlyValue) {
                res.Append(" (Read only)");
            }
            return res.ToString();
        }

        private static string TAG = "StorageUtils";

        public static List<StorageInfo> GetStorageList()
        {

            List<StorageInfo> list = new List<StorageInfo>();
            var def_path = Environment.ExternalStorageDirectory.Path;
            bool def_path_removable = Environment.IsExternalStorageRemovable;
            var def_path_state = Environment.ExternalStorageState;
            bool def_path_available = def_path_state.Equals(Environment.MediaMounted)
                                        || def_path_state.Equals(Environment.MediaMountedReadOnly);
            bool def_path_readonly = Environment.ExternalStorageState.Equals(Environment.MediaMountedReadOnly);

            HashSet<string> paths = new HashSet<string>();
            int cur_removable_number = 1;

            if (def_path_available)
            {
                paths.Add(def_path);
                list.Insert(0, new StorageInfo(def_path, def_path_readonly, def_path_removable, def_path_removable ? cur_removable_number++ : -1));
            }

            BufferedReader buf_reader = null;
            try
            {
                buf_reader = new BufferedReader(new FileReader("/proc/mounts"));
                string line;
                while ((line = buf_reader.ReadLine()) != null)
                {
                    if (line.Contains("vfat") || line.Contains("/mnt"))
                    {
                        StringTokenizer tokens = new StringTokenizer(line, " ");
                        string unused = tokens.NextToken(); //device
                        string mount_point = tokens.NextToken(); //mount point
                        if (paths.Contains(mount_point))
                        {
                            continue;
                        }
                        unused = tokens.NextToken(); //file system
                        List<string> flags = Arrays.AsList(tokens.NextToken().Split(',')).Cast<string>().ToList(); //flags
                        bool readonlyValue = flags.Contains("ro");

                        if (line.Contains("/dev/block/vold"))
                        {
                            if (!line.Contains("/mnt/secure")
                                && !line.Contains("/mnt/asec")
                                && !line.Contains("/mnt/obb")
                                && !line.Contains("/dev/mapper")
                                && !line.Contains("tmpfs"))
                            {
                                paths.Add(mount_point);
                                list.Add(new StorageInfo(mount_point, readonlyValue, true, cur_removable_number++));
                            }
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                AutoLogger.LogError(ex, "getStorageList");
            }
            finally
            {
                if (buf_reader != null)
                {
                    try
                    {
                        buf_reader.Close();
                    }
                    catch
                    {

                    }
                }
            }
            return list;
        }
    }

    
}

/**
 * @version 2009-07-03
 * @author Peli
 * @version 2013-12-11
 * @author paulburke (ipaulpro)
 */
public class FileUtils
{
    /** TAG for log messages. */
    static string TAG = "FileUtils";
    private static bool DEBUG = false; // Set to true to enable logging

    public static string MIME_TYPE_AUDIO = "audio/*";
    public static string MIME_TYPE_TEXT = "text/*";
    public static string MIME_TYPE_IMAGE = "image/*";
    public static string MIME_TYPE_VIDEO = "video/*";
    public static string MIME_TYPE_APP = "application/*";

    public static string HIDDEN_PREFIX = ".";

    /**
     * Gets the extension of a file name, like ".png" or ".jpg".
     *
     * @param uri
     * @return Extension including the dot("."); "" if there is no extension;
     *         null if uri was null.
     */
    public static string getExtension(string uri)
    {
        if (uri == null)
        {
            return null;
        }

        int dot = uri.LastIndexOf(".");
        if (dot >= 0)
        {
            return uri.Substring(dot);
        }
        else
        {
            // No extension.
            return "";
        }
    }

    /**
     * @return Whether the URI is a local one.
     */
    public static bool isLocal(string url)
    {
        if (url != null && !url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            return true;
        }
        return false;
    }

    public static Uri createDocumentWithFlags(ContentProviderClient mClient, string documentId, string mimeType, string name, int flags)
    {
        Bundle bundle = new Bundle();
        bundle.PutInt("com.android.documentsui.stubprovider.FLAGS", flags);
        bundle.PutString("com.android.documentsui.stubprovider.PARENT", documentId);
        bundle.PutString(Document.ColumnMimeType, mimeType);
        bundle.PutString(Document.ColumnDisplayName, name);
        Bundle bout = mClient.Call("createDocumentWithFlags", null, bundle);
        var obj = bout.GetParcelable("uri");
        Uri uri = (Uri)obj;
        return uri;
    }

    /**
     * @return True if Uri is a MediaStore Uri.
     * @author paulburke
     */
    public static bool isMediaUri(Uri uri)
    {
        return "media".Equals(uri.Authority.ToLower());
    }

    /**
     * Convert File into Uri.
     *
     * @param file
     * @return uri
     */
    public static Uri getUri(Java.IO.File file)
    {
        if (file != null)
        {
            return Uri.FromFile(file);
        }
        return null;
    }

    /**
     * Returns the path only (without file name).
     *
     * @param file
     * @return
     */
    public static Java.IO.File getPathWithoutFilename(Java.IO.File file)
    {
        if (file != null)
        {
            if (file.IsDirectory)
            {
                // no file to be split off. Return everything
                return file;
            }
            else
            {
                string filename = file.Name;
                string filepath = file.AbsolutePath;

                // Construct path without file name.
                string pathwithoutname = filepath.Substring(0,
                        filepath.Length - filename.Length);
                if (pathwithoutname.EndsWith("/"))
                {
                    pathwithoutname = pathwithoutname.Substring(0, pathwithoutname.Length - 1);
                }
                return new Java.IO.File(pathwithoutname);
            }
        }
        return null;
    }

    /**
     * @return The MIME type for the given file.
     */
    public static string getMimeType(Java.IO.File file)
    {

        string extension = getExtension(file.Name);

        if (extension.Length > 0)
            return Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.Substring(1));

        return "application/octet-stream";
    }

    /**
     * @return The MIME type for the give Uri.
     */
    public static string getMimeType(Context context, Uri uri)
    {
        Java.IO.File file = new Java.IO.File(getPath(context, uri));
        return getMimeType(file);
    }

    /**
     * @param uri The Uri to check.
     * @return Whether the Uri authority is {@link LocalStorageProvider}.
     * @author paulburke
     */
    public static bool isLocalStorageDocument(Uri uri)
    {
        return "com.ianhanniballake.localstorage.documents".Equals(uri.Authority);
    }

    /**
     * @param uri The Uri to check.
     * @return Whether the Uri authority is ExternalStorageProvider.
     * @author paulburke
     */
    public static bool isExternalStorageDocument(Uri uri)
    {
        return "com.android.externalstorage.documents".Equals(uri.Authority);
    }

    /**
     * @param uri The Uri to check.
     * @return Whether the Uri authority is DownloadsProvider.
     * @author paulburke
     */
    public static bool isDownloadsDocument(Uri uri)
    {
        return "com.android.providers.downloads.documents".Equals(uri.Authority);
    }

    /**
     * @param uri The Uri to check.
     * @return Whether the Uri authority is MediaProvider.
     * @author paulburke
     */
    public static bool isMediaDocument(Uri uri)
    {
        return "com.android.providers.media.documents".Equals(uri.Authority);
    }

    /**
     * @param uri The Uri to check.
     * @return Whether the Uri authority is Google Photos.
     */
    public static bool isGooglePhotosUri(Uri uri)
    {
        return "com.google.android.apps.photos.content".Equals(uri.Authority);
    }

    /**
     * Get the value of the data column for this Uri. This is useful for
     * MediaStore Uris, and other file-based ContentProviders.
     *
     * @param context The context.
     * @param uri The Uri to query.
     * @param selection (Optional) Filter used in the query.
     * @param selectionArgs (Optional) Selection arguments used in the query.
     * @return The value of the _data column, which is typically a file path.
     * @author paulburke
     */
    public static string getDataColumn(Context context, Uri uri, string selection,
            string[] selectionArgs)
    {

        ICursor cursor = null;
        string column = "_data";
        string[] projection = {
                column
            };

        try
        {
            cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,
                    null);
            if (cursor != null && cursor.MoveToFirst())
            {
                if (DEBUG)
                    DatabaseUtils.DumpCursor(cursor);

                int column_index = cursor.GetColumnIndexOrThrow(column);
                return cursor.GetString(column_index);
            }
        }
        finally
        {
            if (cursor != null)
                cursor.Close();
        }
        return null;
    }

    /**
     * Get a file path from a Uri. This will get the the path for Storage Access
     * Framework Documents, as well as the _data field for the MediaStore and
     * other file-based ContentProviders.<br>
     * <br>
     * Callers should check whether the path is local before assuming it
     * represents a local file.
     * 
     * @param context The context.
     * @param uri The Uri to query.
     * @see #isLocal(String)
     * @see #getFile(Context, Uri)
     * @author paulburke
     */
    public static string getPath(Context context, Uri uri)
    {

        //if (DEBUG)
        //    Log.d(TAG + " File -",
        //            "Authority: " + uri.getAuthority() +
        //                    ", Fragment: " + uri.getFragment() +
        //                    ", Port: " + uri.getPort() +
        //                    ", Query: " + uri.getQuery() +
        //                    ", Scheme: " + uri.getScheme() +
        //                    ", Host: " + uri.getHost() +
        //                    ", Segments: " + uri.getPathSegments().toString()
        //            );

        bool isKitKat = (int)Build.VERSION.SdkInt >= 19;

        // DocumentProvider
        if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
        {
            // LocalStorageProvider
            if (isLocalStorageDocument(uri))
            {
                // The path is the id
                return DocumentsContract.GetDocumentId(uri);
            }
            // ExternalStorageProvider
            else if (isExternalStorageDocument(uri))
            {
                string docId = DocumentsContract.GetDocumentId(uri);
                string[] split = docId.Split(':');
                string type = split[0];

                if ("primary".Equals(type))
                {
                    return Environment.ExternalStorageDirectory + "/" + split[1];
                }

                // TODO handle non-primary volumes
            }
            // DownloadsProvider
            else if (isDownloadsDocument(uri))
            {

                string id = DocumentsContract.GetDocumentId(uri);
                Uri contentUri = ContentUris.WithAppendedId(
                       Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                return getDataColumn(context, contentUri, null, null);
            }
            // MediaProvider
            else if (isMediaDocument(uri))
            {
                string docId = DocumentsContract.GetDocumentId(uri);
                string[] split = docId.Split(':');
                string type = split[0];

                Uri contentUri = null;
                if ("image".Equals(type))
                {
                    contentUri = MediaStore.Images.Media.ExternalContentUri;
                }
                else if ("video".Equals(type))
                {
                    contentUri = MediaStore.Video.Media.ExternalContentUri;
                }
                else if ("audio".Equals(type))
                {
                    contentUri = MediaStore.Audio.Media.ExternalContentUri;
                }

                string selection = "_id=?";
                string[] selectionArgs = new string[] {
                        split[1]
                };

                return getDataColumn(context, contentUri, selection, selectionArgs);
            }
        }
        // MediaStore (and general)
        else if ("content".Equals(uri.Scheme))
        {

            // Return the remote address
            if (isGooglePhotosUri(uri))
                return uri.LastPathSegment;

            return getDataColumn(context, uri, null, null);
        }
        // File
        else if ("file".Equals(uri.Scheme))
        {
            return uri.Path;
        }

        return null;
    }

    /**
     * Convert Uri into File, if possible.
     *
     * @return file A local file that the Uri was pointing to, or null if the
     *         Uri is unsupported or pointed to a remote resource.
     * @see #getPath(Context, Uri)
     * @author paulburke
     */
    public static Java.IO.File getFile(Context context, Uri uri)
    {
        if (uri != null)
        {
            string path = getPath(context, uri);
            if (path != null && isLocal(path))
            {
                return new Java.IO.File(path);
            }
        }
        return null;
    }

    /**
     * Get the file size in a human-readable string.
     *
     * @param size
     * @return
     * @author paulburke
     */
    public static string getReadableFileSize(int size)
    {
        int BYTES_IN_KILOBYTES = 1024;
        Java.Text.DecimalFormat dec = new Java.Text.DecimalFormat("###.#");
        string KILOBYTES = " KB";
        string MEGABYTES = " MB";
        string GIGABYTES = " GB";
        float fileSize = 0;
        string suffix = KILOBYTES;

        if (size > BYTES_IN_KILOBYTES)
        {
            fileSize = size / BYTES_IN_KILOBYTES;
            if (fileSize > BYTES_IN_KILOBYTES)
            {
                fileSize = fileSize / BYTES_IN_KILOBYTES;
                if (fileSize > BYTES_IN_KILOBYTES)
                {
                    fileSize = fileSize / BYTES_IN_KILOBYTES;
                    suffix = GIGABYTES;
                }
                else
                {
                    suffix = MEGABYTES;
                }
            }
        }
        return Java.Lang.String.ValueOf(dec.Format(fileSize) + suffix);
    }

}