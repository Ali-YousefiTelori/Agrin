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
using Agrin.Download.Manager;

namespace Agrin.MonoAndroid.UI
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class LinksListDataActivity : Activity
    {
        LinksListDataViewModel linksListdata;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                ActivitesManager.AddActivity(this);
                base.OnCreate(bundle);
                // Create your application here
                SetContentView(Resource.Layout.LinksListData);
                this.Title = "DownloadsLink_Language";
                menuItems = new string[] { ViewUtility.FindTextLanguage(this, "OpenFile_Language"), ViewUtility.FindTextLanguage(this, "OpenFileLocation_Language"), ViewUtility.FindTextLanguage(this, "ChangeFileLocation_Language"), ViewUtility.FindTextLanguage(this, "ShareFile_Language"), ViewUtility.FindTextLanguage(this, "ShowDetail_Language"), ViewUtility.FindTextLanguage(this, "ChangeLinkAddress_Language"), ViewUtility.FindTextLanguage(this, "CopyLinkAddress_Language"), ViewUtility.FindTextLanguage(this, "ShowLastError_Language"), ViewUtility.FindTextLanguage(this, "RepairLink_Language"), ViewUtility.FindTextLanguage(this, "RenameLink_Language"), };
                if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
                {
                    ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.LinksListData.LinearLayoutReverce1, Resource.LinksListData.LinearLayoutReverce2, Resource.LinksListData.LinearLayoutReverce3 });
                }

                ViewUtility.SetTextLanguage(this, new List<int>() { Resource.LinksListData.btnDelete, Resource.LinksListData.btnDeleteComplete, Resource.LinksListData.btnPlay, Resource.LinksListData.btnSelectAll, Resource.LinksListData.btnStop, Resource.LinksListData.btnDownloadQueue });
                Agrin.Download.Manager.ApplicationLinkInfoManager.Current.LinkInfoes.SortBy(x => x.DownloadingProperty.DateLastDownload);
                linksListdata = new LinksListDataViewModel(this, Resource.Layout.LinksListItem, Agrin.Download.Manager.ApplicationLinkInfoManager.Current.LinkInfoes.ToList());
                RegisterForContextMenu(linksListdata._listView);

            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "FATALLLLLLLL EEEEEEE:", true);
            }
        }

        string[] menuItems;
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id == Resource.LinksListData.mainListView)
            {
                var info = (AdapterView.AdapterContextMenuInfo)menuInfo;
                var listItemName = ApplicationLinkInfoManager.Current.LinkInfoes[info.Position];
                //if (!System.IO.File.Exists(listItemName.PathInfo.FullAddressFileName))
                //    return;

                menu.SetHeaderTitle(listItemName.PathInfo.FileName);

                for (var i = 0; i < menuItems.Length; i++)
                    menu.Add(Menu.None, i, i, menuItems[i]);
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            var menuItemIndex = item.ItemId;
            var menuItemName = menuItems[menuItemIndex];
            var listItemName = ApplicationLinkInfoManager.Current.LinkInfoes[info.Position];
            if (menuItemIndex == 0)
            {
                try
                {
                    if (System.IO.File.Exists(listItemName.PathInfo.FullAddressFileName))
                    {
                        Java.IO.File file = new Java.IO.File(listItemName.PathInfo.FullAddressFileName);
                        Intent intent = new Intent(Intent.ActionView);
                        intent.SetDataAndType(Android.Net.Uri.FromFile(file), getMimeType(listItemName.PathInfo.FullAddressFileName));
                        StartActivity(intent);
                    }
                    else
                    {
                        var builder = new AlertDialog.Builder(this);
                        builder.SetMessage(ViewUtility.FindTextLanguage(this, "Filedoesnotexist_Language"));
                        builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (s, ee) =>
                        {
                        });
                        builder.Show();
                    }
                }
                catch (Exception error)
                {
                    Agrin.Log.AutoLogger.LogError(error, "TTTTTTTT 1 " + listItemName.PathInfo.FullAddressFileName + " TT " + getMimeType(listItemName.PathInfo.FullAddressFileName) + " S! ", true);
                    var builder = new AlertDialog.Builder(this);
                    builder.SetMessage(ViewUtility.FindTextLanguage(this, "CouldNotOpenThisFile_Language"));
                    builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (s, ee) =>
                    {
                    });
                    builder.Show();
                }
            }
            else if (menuItemIndex == 1)
            {
                try
                {
                    if (System.IO.Directory.Exists(listItemName.PathInfo.SavePath))
                    {
                        Android.Net.Uri selectedUri = Android.Net.Uri.Parse(listItemName.PathInfo.SavePath);
                        Intent intent = new Intent(Intent.ActionView);
                        intent.SetDataAndType(selectedUri, "resource/folder");
                        StartActivity(intent);
                    }
                    else
                    {
                        var builder = new AlertDialog.Builder(this);
                        builder.SetMessage(ViewUtility.FindTextLanguage(this, "Folderdoesnotexist_Language"));
                        builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (s, ee) =>
                        {
                        });
                        builder.Show();
                    }
                }
                catch (Exception error)
                {
                    Agrin.Log.AutoLogger.LogError(error, "TTTTTTTT 5 " + listItemName.PathInfo.FullAddressFileName + " TT " + getMimeType(listItemName.PathInfo.FullAddressFileName) + " S! ", true);
                    var builder = new AlertDialog.Builder(this);
                    builder.SetMessage(ViewUtility.FindTextLanguage(this, "CouldNotOpenThisLocation_Language"));
                    builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (s, ee) =>
                    {

                    });
                    builder.Show();
                }
            }
            else if (menuItemIndex == 2)
            {
                try
                {
                    ActivitesManager.FolderBrowserDialogActive(this, listItemName.PathInfo.SavePath, (path) =>
                    {
                        if (listItemName.PathInfo.AppSavePath != path)
                        {
                            listItemName.PathInfo.UserSavePath = path;
                            listItemName.SaveThisLink();
                        }
                    });
                }
                catch (Exception error)
                {
                    Agrin.Log.AutoLogger.LogError(error, "TTTTTTTT 5 " + listItemName.PathInfo.FullAddressFileName + " TT " + getMimeType(listItemName.PathInfo.FullAddressFileName) + " S! ", true);
                    var builder = new AlertDialog.Builder(this);
                    builder.SetMessage(ViewUtility.FindTextLanguage(this, "Error_Language"));
                    builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (s, ee) =>
                    {
                    });
                    builder.Show();
                }
            }
            else if (menuItemIndex == 3)
            {
                try
                {
                    if (System.IO.File.Exists(listItemName.PathInfo.FullAddressFileName))
                    {
                        Java.IO.File file = new Java.IO.File(listItemName.PathInfo.FullAddressFileName);
                        Intent intent = new Intent(Intent.ActionSend);
                        intent.SetType("*/*");//getMimeType(listItemName.PathInfo.FullAddressFileName)
                        intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(file));
                        StartActivity(Intent.CreateChooser(intent, ViewUtility.FindTextLanguage(this, "ShareFileUsing_Language")));
                    }
                    else
                    {
                        var builder = new AlertDialog.Builder(this);
                        builder.SetMessage(ViewUtility.FindTextLanguage(this, "Filedoesnotexist_Language"));
                        builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (s, ee) =>
                        {
                        });
                        builder.Show();
                    }
                }
                catch (Exception error)
                {
                    Agrin.Log.AutoLogger.LogError(error, "TTTTTTTT 1 " + listItemName.PathInfo.FullAddressFileName + " TT " + getMimeType(listItemName.PathInfo.FullAddressFileName) + " S! ", true);
                    var builder = new AlertDialog.Builder(this);
                    builder.SetMessage(ViewUtility.FindTextLanguage(this, "ErrorShareFile_Language"));
                    builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (s, ee) =>
                    {
                    });
                    builder.Show();
                }
            }
            else if (menuItemIndex == 4)
            {
                ActivitesManager.LinkInfoDetailActivity(listItemName, this);
            }
            else if (menuItemIndex == 5)
            {
                //change link here
                var activity = this;
                AlertDialog.Builder builder = new AlertDialog.Builder(activity);
                builder.SetTitle(ViewUtility.FindTextLanguage(activity, "ChangeLinkAddress_Language"));
                LinearLayout layout = new LinearLayout(activity);
                layout.Orientation = Orientation.Vertical;
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

                layoutParams.SetMargins(5, 5, 5, 5);
                layout.LayoutParameters = layoutParams;

                EditText txtAddress = new EditText(activity);
                txtAddress.Hint = ViewUtility.FindTextLanguage(this, "LinkAddressTitle_Language");
                txtAddress.VerticalScrollBarEnabled = true;
                layout.AddView(txtAddress);
                ScrollView scroll = new ScrollView(activity);
                scroll.AddView(layout);
                builder.SetView(scroll);
                builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "Save_Language"), (EventHandler<DialogClickEventArgs>)null);
                builder.SetNegativeButton(ViewUtility.FindTextLanguage(this, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);
                AlertDialog dialogW = null;
                builder.SetCancelable(false);
                dialogW = builder.Create();
                dialogW.Show();
                // Get the buttons.
                var yesBtn = dialogW.GetButton((int)DialogButtonType.Positive);
                var noBtn = dialogW.GetButton((int)DialogButtonType.Negative);
                // Set up the buttons
                yesBtn.Click += (s, args) =>
                {
                    try
                    {
                        Uri uri = null;
                        if (Uri.TryCreate(txtAddress.Text, UriKind.Absolute, out uri))
                        {
                            foreach (var link in listItemName.Management.MultiLinks)
                            {
                                link.IsSelected = false;
                            }
                            listItemName.Management.MultiLinks.Add(new Download.Web.Link.MultiLinkAddress() { IsSelected = true, Address = txtAddress.Text });
                            listItemName.SaveThisLink();
                            Toast.MakeText(this, ViewUtility.FindTextLanguage(this, "SaveSuccess_Language"), ToastLength.Long).Show();
                            dialogW.Dismiss();
                        }
                        else
                        {
                            Toast.MakeText(this, ViewUtility.FindTextLanguage(this, "LinkInvariable_Language"), ToastLength.Long).Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                    }
                };
                noBtn.Click += (s, args) =>
                {
                    dialogW.Dismiss();
                };

            }
            else if (menuItemIndex == 6)
            {
                Android.Text.ClipboardManager clipboardManager = (Android.Text.ClipboardManager)this.GetSystemService(Context.ClipboardService);
                if (listItemName.Management.MultiLinks.Count > 0)
                    clipboardManager.Text = listItemName.Management.MultiLinks.FirstOrDefault().Address;
                else
                    clipboardManager.Text = listItemName.PathInfo.Address;
            }
            else if (menuItemIndex == 7)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle(ViewUtility.FindTextLanguage(this, "ShowLastError_Language"));
                LinearLayout layout = new LinearLayout(this);
                layout.Orientation = Orientation.Vertical;
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

                layoutParams.SetMargins(5, 5, 5, 5);
                layout.LayoutParameters = layoutParams;

                TextView txtMessage = new TextView(this);
                txtMessage.SetTextAppearance(this, global::Android.Resource.Style.TextAppearanceSmall);
                txtMessage.SetSingleLine(false);
                txtMessage.VerticalScrollBarEnabled = true;
                string error = listItemName.Management.LastErrorDescription;
                if (error == "No Error Found!")
                    txtMessage.Text = ViewUtility.FindTextLanguage(this, "NoErrorFound_Language");
                else
                    txtMessage.Text = error;

                layout.AddView(txtMessage);
                ScrollView scroll = new ScrollView(this);
                scroll.AddView(layout);
                builder.SetView(scroll);
                // Set up the buttons
                builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (dialog, which) =>
                {
                    builder.Dispose();
                });

                builder.Show();
            }
            else if (menuItemIndex == 8)
            {
                //change link here
                var activity = this;
                AlertDialog.Builder builder = new AlertDialog.Builder(activity);
                builder.SetTitle(ViewUtility.FindTextLanguage(activity, "RepairLink_Language"));
                LinearLayout layout = new LinearLayout(activity);
                layout.Orientation = Orientation.Vertical;
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

                layoutParams.SetMargins(5, 5, 5, 5);
                layout.LayoutParameters = layoutParams;

                bool fileExist = Agrin.Download.Engine.Repairer.LinkRepairer.FileExist(listItemName);
                TextView txtState = null;
                ProgressBar progressStage = null, progressMain = null;
                if (!fileExist)
                {
                    TextView txtMSG = new TextView(activity);
                    txtMSG.Text = ViewUtility.FindTextLanguage(this, "CompleteFileForRepairNotFound_Language");
                    txtMSG.VerticalScrollBarEnabled = true;
                    layout.AddView(txtMSG);
                }
                else
                {
                    LayoutInflater inflater = this.LayoutInflater;
                    View dialoglayout = inflater.Inflate(Resource.Layout.RepairLink, null);

                    txtState = dialoglayout.FindViewById(Resource.RepairLink.txtState) as TextView;
                    txtState.Text = ViewUtility.FindTextLanguage(this, "Stop_Language");

                    progressStage = dialoglayout.FindViewById(Resource.RepairLink.stageProgress) as ProgressBar;
                    progressMain = dialoglayout.FindViewById(Resource.RepairLink.mainProgress) as ProgressBar;

                    progressMain.Max = 4;
                    progressMain.Progress = 0;
                    progressStage.Progress = 0;
                    layout.AddView(dialoglayout);
                }


                ScrollView scroll = new ScrollView(activity);
                scroll.AddView(layout);
                builder.SetView(scroll);
                if (fileExist)
                    builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "StartRepairLink_Language"), (EventHandler<DialogClickEventArgs>)null);
                else
                    builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "OK_Language"), (EventHandler<DialogClickEventArgs>)null);

                builder.SetNegativeButton(ViewUtility.FindTextLanguage(this, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);
                AlertDialog dialogW = null;
                builder.SetCancelable(false);
                dialogW = builder.Create();
                dialogW.Show();
                // Get the buttons.
                var yesBtn = dialogW.GetButton((int)DialogButtonType.Positive);
                var noBtn = dialogW.GetButton((int)DialogButtonType.Negative);
                bool isStop = false;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    Action<string> runOnUI = (msg) =>
                    {
                        activity.RunOnUiThread(() =>
                        {
                            txtState.Text = msg;
                        });
                    };
                    try
                    {
                        Agrin.Download.Engine.Repairer.LinkRepairer.LinkRepairerProcessAction = (val, max, state) =>
                        {
                            if (isStop)
                                return;

                            if (state == Download.Engine.Repairer.LinkRepairerState.FindConnectionProblems)
                            {
                                activity.RunOnUiThread(() =>
                                {
                                    txtState.Text = ViewUtility.FindTextLanguage(this, "FindConnectionProblems_Language");
                                    progressMain.Progress = 1;
                                });
                            }
                            else if (state == Download.Engine.Repairer.LinkRepairerState.FindPositionOfProblems)
                            {
                                activity.RunOnUiThread(() =>
                                {
                                    txtState.Text = ViewUtility.FindTextLanguage(this, "FindPositionOfProblems_Language");
                                    progressMain.Progress = 2;
                                });
                            }
                            else if (state == Download.Engine.Repairer.LinkRepairerState.DownloadingProblems)
                            {
                                activity.RunOnUiThread(() =>
                                {
                                    txtState.Text = ViewUtility.FindTextLanguage(this, "DownloadingProblems_Language");
                                    progressMain.Progress = 3;
                                });
                            }
                            else if (state == Download.Engine.Repairer.LinkRepairerState.FixingProblems)
                            {
                                activity.RunOnUiThread(() =>
                                {
                                    txtState.Text = ViewUtility.FindTextLanguage(this, "FixingProblems_Language");
                                    progressMain.Progress = 4;
                                });
                            }

                            activity.RunOnUiThread(() =>
                            {
                                int newMax = 0, newVal = 0;
                                if (max > int.MaxValue)
                                {
                                    newMax = int.MaxValue;
                                    long nval = max - int.MaxValue;
                                    if (val - nval > 0)
                                    {
                                        newVal = (int)(val - nval);
                                    }
                                    else
                                        newVal = 0;
                                }
                                else
                                {
                                    newMax = (int)max;
                                    newVal = (int)val;
                                }
                                progressStage.Max = newMax;
                                progressStage.Progress = newVal;
                            });
                        };
                        activity.RunOnUiThread(() =>
                        {
                            txtState.Text = ViewUtility.FindTextLanguage(this, "Connecting_Language");
                        });
                        var repair = Agrin.Download.Engine.Repairer.LinkRepairer.RepairFile(listItemName.PathInfo.BackUpCompleteAddress, listItemName.PathInfo.FullAddressFileName);
                        if (isStop)
                            return;
                        if (repair == "OK")
                        {
                            activity.RunOnUiThread(() =>
                            {
                                txtState.Text = ViewUtility.FindTextLanguage(this, "SuccessRepairLink_Language");
                                noBtn.Text = ViewUtility.FindTextLanguage(this, "OK_Language");
                            });
                        }
                        else
                        {
                            runOnUI(repair);
                        }
                    }
                    catch (Exception e)
                    {
                        if (isStop)
                            return;
                        runOnUI(e.Message);
                    }
                });
                // Set up the buttons
                yesBtn.Click += (s, args) =>
                {
                    if (!fileExist)
                    {
                        dialogW.Dismiss();
                        return;
                    }
                    yesBtn.Enabled = false;
                    thread.Start();
                };
                noBtn.Click += (s, args) =>
                {
                    isStop = true;
                    try
                    {
                        thread.Abort();
                    }
                    catch
                    {

                    }
                    dialogW.Dismiss();
                };
            }
            else if (menuItemIndex == 9)
            {
                //change link here
                var activity = this;
                AlertDialog.Builder builder = new AlertDialog.Builder(activity);
                builder.SetTitle(ViewUtility.FindTextLanguage(activity, "RenameLink_Language"));
                LinearLayout layout = new LinearLayout(activity);
                layout.Orientation = Orientation.Vertical;
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

                layoutParams.SetMargins(5, 5, 5, 5);
                layout.LayoutParameters = layoutParams;

                EditText txtFileName = new EditText(activity);
                txtFileName.Hint = ViewUtility.FindTextLanguage(this, "NewName_Language");
                txtFileName.VerticalScrollBarEnabled = true;
                layout.AddView(txtFileName);
                ScrollView scroll = new ScrollView(activity);
                scroll.AddView(layout);
                builder.SetView(scroll);
                builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "Save_Language"), (EventHandler<DialogClickEventArgs>)null);
                builder.SetNegativeButton(ViewUtility.FindTextLanguage(this, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);
                AlertDialog dialogW = null;
                builder.SetCancelable(false);
                dialogW = builder.Create();
                dialogW.Show();
                // Get the buttons.
                var yesBtn = dialogW.GetButton((int)DialogButtonType.Positive);
                var noBtn = dialogW.GetButton((int)DialogButtonType.Negative);
                // Set up the buttons
                yesBtn.Click += (s, args) =>
                {
                    try
                    {
                        string fileName = Agrin.IO.Helper.MPath.GetFileNameValidChar(txtFileName.Text);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            listItemName.PathInfo.UserFileName = fileName + System.IO.Path.GetExtension(listItemName.PathInfo.FileName);
                            listItemName.SaveThisLink();
                            Toast.MakeText(this, ViewUtility.FindTextLanguage(this, "SaveSuccess_Language"), ToastLength.Long).Show();
                            dialogW.Dismiss();
                        }
                        else
                        {
                            Toast.MakeText(this, ViewUtility.FindTextLanguage(this, "NoName_Language"), ToastLength.Long).Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                    }
                };
                noBtn.Click += (s, args) =>
                {
                    dialogW.Dismiss();
                };

            }
            return true;
        }
        // url = file path or whatever suitable URL you want.
        public String getMimeType(String url)
        {
            String type = null;
            //String extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(url);
            String extension = System.IO.Path.GetExtension(url);
            if (extension != null)
            {
                if (extension.Length > 1 && extension.First() == '.')
                    extension = extension.Remove(0, 1);
                Android.Webkit.MimeTypeMap mime = Android.Webkit.MimeTypeMap.Singleton;
                type = mime.GetMimeTypeFromExtension(extension);
            }
            return type;
        }
        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            linksListdata.DisposeAll();
            ActivitesManager.ToolbarActive(null);
            base.Finish();
        }
    }
}

