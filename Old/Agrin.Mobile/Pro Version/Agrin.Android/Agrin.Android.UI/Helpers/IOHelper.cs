using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Agrin.IO.Helper;
using Agrin.IO.Streams;
using Agrin.Log;
using Agrin.Streams;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Agrin.Helpers
{
    public class IOHelper : IOHelperBase
    {
        public ContextWrapper Activity { get; set; }

        public override IStreamWriter OpenFileStreamForRead(string fileName, FileMode fileMode, FileAccess fileAccess = FileAccess.ReadWrite)
        {
            //if (OpenFileStreamForReadAction != null)
            //{
            //    retStream = OpenFileStreamForReadAction(fileName);
            //    AutoLogger.LogText($"OpenFileStreamForRead {fileName}");
            //}
            return base.OpenFileStreamForRead(fileName, fileMode, fileAccess);
        }

        public override IStreamWriter OpenFileStreamForWrite(string fileAddress, FileMode fileMode, FileAccess fileAccess = FileAccess.ReadWrite, string fileName = null, Action<string> newSecurityFileName = null, object data = null)
        {
            if (fileAddress.StartsWith("content://"))
            {
                //StringBuilder bulder = new StringBuilder();
                try
                {
                    //bulder.AppendLine("child == null");
                    //bulder.AppendLine("fileAddress : " + fileAddress ?? "null");
                    //bulder.AppendLine("fileName : " + fileName ?? "null");

                    //var linkInfo = link as LinkInfo;
                    Android.Net.Uri fileUri = null;
                    //if (linkInfo != null && string.IsNullOrEmpty(linkInfo.PathInfo.SecurityPath))
                    //{
                    //    //InitializeApplication.GoException("null linkInfo.PathInfo.SecurityPath" + bulder.ToString());
                    //    return null;
                    //}

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        Android.Net.Uri childUri = null;
                        Android.Net.Uri path = Android.Net.Uri.Parse(fileAddress);
                        if (ViewsUtility.GetApiVersion() >= 21)
                        {
                            string treedocId = Android.Provider.DocumentsContract.GetTreeDocumentId(path);
                            Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUriUsingTree(path, treedocId);
                            childUri = Android.Provider.DocumentsContract.CreateDocument(Activity.ContentResolver, docUri, ViewsUtility.GetMimeType(fileName), fileName);
                            if (childUri == null)
                            {
                                throw new Exception("Could not create file! Please change save address...");
                            }
                        }
                        else
                        {
                            throw new Exception("Could not create file! Please change save address...");
                            //var id = Android.Provider.DocumentsContract.GetDocumentId(path);
                            ////Android.Provider.
                            ////return new Agrin.Streams.AndroidStreamWriter(new Java.IO.FileOutputStream(realPath));
                            //var aq = currentActivity.ContentResolver.AcquireContentProviderClient(Android.Provider.ContactsContract.Authority);

                            //childUri = FileUtils.createDocumentWithFlags(aq, id, ViewsUtility.GetMimeType(fileName), fileName, 0);
                            ////Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUri("com.example.documentsprovider.authority", id);
                            ////childUri = docUri;
                        }
                        newSecurityFileName?.Invoke(childUri.ToString());
                        fileUri = childUri;
                    }
                    else
                        fileUri = Android.Net.Uri.Parse(fileAddress);
                    ParcelFileDescriptor pfd = Activity.ContentResolver.OpenFileDescriptor(fileUri, "w");
                    Java.IO.FileDescriptor fileD = pfd.FileDescriptor;
                    Java.IO.FileOutputStream fileOutputStream = new Java.IO.FileOutputStream(fileD);
                    //InitializeApplication.GoException("OK-o : " + bulder.ToString());

                    return new AndroidStreamWriter(fileOutputStream) { FileDescriptor = fileD, ParcelFileDescriptor = pfd };
                }
                catch (Exception ex)
                {
                    AutoLogger.LogError(ex, "OpenFileStreamForWriteAction");
                }
            }
            //if (OpenFileStreamForWriteAction != null)
            //{
            //    AutoLogger.LogText($"OpenFileStreamForWrite new {fileAddress} {(fileName == null ? "" : fileName)}");
            //    retStream = OpenFileStreamForWriteAction(fileAddress, fileName, fileMode, newSecurityFileName, data);
            //}
            return base.OpenFileStreamForWrite(fileAddress, fileMode, fileAccess, fileName, newSecurityFileName, data);
        }

    }
}