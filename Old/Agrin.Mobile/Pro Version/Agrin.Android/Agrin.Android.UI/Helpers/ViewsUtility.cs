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
using Android.Graphics;
using Agrin.ViewModels.Dialogs;
using Android.Views.Animations;
using Android.Webkit;
using Android.Database;
using Agrin.Log;

namespace Agrin.Helpers
{
    public enum FlowDirection
    {
        LeftToRight,
        RightToLeft
    }

    public static class ViewsUtility
    {
        private static FlowDirection _ProjectDirection = FlowDirection.RightToLeft;
        public static FlowDirection ProjectDirection
        {
            get { return ViewsUtility._ProjectDirection; }
            set { ViewsUtility._ProjectDirection = value; }
        }

        public static string ApplicationLanguage = "_fa";
        public static void setListViewHeightBasedOnChildren(ListView listView)
        {
            if (listView.Adapter == null)
            {
                // pre-condition
                return;
            }

            int totalHeight = listView.PaddingTop + listView.PaddingBottom;
            int totalWidth = 0;

            for (int i = 0; i < listView.Count; i++)
            {
                View listItem = listView.Adapter.GetView(i, null, listView);
                if (listItem.GetType() == typeof(ViewGroup))
                {
                    listItem.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                }
                listItem.Measure(0, 0);
                totalHeight += listItem.MeasuredHeight;
                if (listItem.MeasuredWidth > totalWidth)
                    totalWidth = listItem.MeasuredWidth;
            }

            listView.LayoutParameters.Height = totalHeight + (listView.DividerHeight * (listView.Count - 1));
            listView.LayoutParameters.Width = totalWidth;
        }

        public static void SetRightToLeftMarginView(View view)
        {
            try
            {
                var param = view.LayoutParameters as LinearLayout.LayoutParams;
                if (param != null)
                {
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(param.Width, param.Height);
                    lp.SetMargins(param.RightMargin, param.TopMargin, param.LeftMargin, param.BottomMargin);
                    lp.Weight = param.Weight;
                    lp.Gravity = param.Gravity;
                    view.LayoutParameters = lp;
                }
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "SetRightToLeftMarginView");
            }
        }

        public static void SetRightToLeftLayout(View activity, List<int> layouts)
        {
            if (ProjectDirection == FlowDirection.LeftToRight)
                return;
            foreach (var item in layouts)
            {
                var view = activity.FindViewById<LinearLayout>(item);
                view.SetGravity(GravityFlags.Right);
                var param = view.LayoutParameters as LinearLayout.LayoutParams;
                if (param != null)
                {
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams((ViewGroup.MarginLayoutParams)param);
                    lp.Weight = param.Weight;
                    lp.Height = param.Height;
                    lp.Width = param.Width;
                    lp.Gravity = GravityFlags.Right;
                    view.LayoutParameters = lp;
                }
                List<View> views = new List<View>();
                for (int x = 0; x < view.ChildCount; x++)
                {
                    views.Add(view.GetChildAt(x));
                }
                view.RemoveAllViews();
                for (int x = views.Count - 1; x >= 0; x--)
                {
                    view.AddView(views[x]);
                    SetRightToLeftMarginView(views[x]);
                }
            }
        }

        public static void SetRightToLeftViews(View activity, List<int> views)
        {
            if (ProjectDirection == FlowDirection.LeftToRight)
                return;
            foreach (var item in views)
            {
                var view = activity.FindViewById<View>(item);
                SetRightToLeftViews(view);
            }
        }
        public static void SetRightToLeftViews(List<View> views)
        {
            if (ProjectDirection == FlowDirection.LeftToRight)
                return;
            foreach (var view in views)
            {
                SetRightToLeftViews(view);
            }
        }

        public static void SetLeftToRightCheckBox(View activity, int item)
        {
            var view = activity.FindViewById<View>(item);
            if (view is CheckBox)
            {
                var chk = (CheckBox)view;
                chk.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                chk.SetCompoundDrawablesWithIntrinsicBounds(Android.Graphics.Drawables.Drawable.CreateFromPath("?android:attr/listChoiceIndicatorMultiple"), null, null, null);
                chk.SetButtonDrawable(null);
            }
        }

        static void SetRightToLeftViews(View view)
        {
            if (view is TextView)
                ((TextView)view).Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
            else if (view is Button)
                ((TextView)view).Gravity = GravityFlags.Right;
            else if (view is EditText)
                ((EditText)view).Gravity = GravityFlags.Right;
            else if (view is CheckBox)
            {
                var chk = (CheckBox)view;
                //chk.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                //chk.SetCompoundDrawablesWithIntrinsicBounds(null, null, Android.Graphics.Drawables.Drawable.CreateFromPath("?android:attr/listChoiceIndicatorMultiple"), null);
                //chk.SetButtonDrawable(null);
            }
            else if (view is LinearLayout)
            {
                var linearLayout = (LinearLayout)view;
                linearLayout.SetGravity(GravityFlags.Right);
                linearLayout.SetHorizontalGravity(GravityFlags.Right);
            }
        }

        static Dictionary<string, string> CachResources = new Dictionary<string, string>();
        static object lockobj = new object();
        public static string FindTextLanguage(Android.Content.Context activity, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return text;
                if (!text.Contains("_Language"))
                {
                    return text;
                }
                lock (lockobj)
                {
                    string key = text + ApplicationLanguage;
                    if (CachResources.ContainsKey(key))
                        return CachResources[key];
                    var id = activity.Resources.GetIdentifier(key, "string", activity.PackageName);
                    string value = activity.Resources.GetString(id);
                    if (string.IsNullOrEmpty(value))
                        return text;
                    CachResources.Add(key, value);
                    return value;
                }
            }
            catch (Exception e)
            {
                InitializeApplication.GoException(e, "FindTextLanguage " + text);
                //Agrin.Log.AutoLogger.LogError(e, "Error Get Language FindTextLanguage : " + text == null ? "null" : text + " !", true);
                return text;
            }
        }

        public static string TimeSpanToText(TimeSpan timeSpan)
        {
            bool added = false;
            string msg = "";
            if (timeSpan.Days > 0)
            {
                added = true;
                msg += timeSpan.Days + " روز";
            }

            if (timeSpan.Hours > 0)
            {
                if (added)
                {
                    msg += " و ";
                }
                msg += timeSpan.Hours + " ساعت";
                added = true;
            }

            if (timeSpan.Minutes > 0)
            {
                if (added)
                {
                    msg += " و ";
                }
                msg += timeSpan.Minutes + " دقیقه";
            }
            if (timeSpan.Seconds > 0)
            {
                if (added)
                {
                    msg += " و ";
                }
                msg += timeSpan.Seconds + " ثانیه";
            }
            return msg;
        }

        public static string TimeSpanToShortText(TimeSpan timeSpan)
        {
            string msg = "";

            if (timeSpan.Days > 0)
                msg += timeSpan.Days + " / ";

            if (timeSpan.Hours > 0)
                msg += timeSpan.Hours + ":";
            if (timeSpan.Minutes > 0)
                msg += timeSpan.Minutes + ":";
            if (timeSpan.Seconds > 0)
                msg += timeSpan.Seconds;
            msg = msg.Trim().Trim(new char[] { '/', ':' });
            if (msg == "")
                msg = "0";
            return msg;
        }

        public static void SetTextLanguage(Activity activity, View mainView, List<int> items, bool setFonts = true)
        {
            foreach (var item in items)
            {
                var view = mainView.FindViewById(item);
                if (view is ToggleButton)
                {
                    ((ToggleButton)view).TextOff = FindTextLanguage(activity, ((ToggleButton)view).TextOff);
                    var textOn = ((ToggleButton)view).TextOn;
                    ((ToggleButton)view).Text = ((ToggleButton)view).TextOn = FindTextLanguage(activity, textOn);
                    //FindTextLanguage(activity, textOn);
                }
                else if (view is Button)
                {
                    ((Button)view).Text = FindTextLanguage(activity, ((Button)view).Text);
                }
                else if (view is TextView)
                {
                    ((TextView)view).Text = FindTextLanguage(activity, ((TextView)view).Text);
                }

                if (setFonts && ProjectDirection == FlowDirection.RightToLeft && (view is TextView || view is Button))
                {
                    SetFont(activity, view);
                }
            }
        }

        public static Android.Graphics.Bitmap GetLargeIconBySize(Context context)
        {
            Android.Graphics.Bitmap icon = Android.Graphics.BitmapFactory.DecodeResource(context.Resources,
                                           Resource.Drawable.Icon);
            var size = ViewsUtility.GetDpiSize(context);
            if (size == Android.Content.Res.ScreenLayout.SizeSmall)
            {
                return Android.Graphics.Bitmap.CreateScaledBitmap(icon, 48, 48, false);
            }
            else if (size == Android.Content.Res.ScreenLayout.SizeNormal)
            {
                return Android.Graphics.Bitmap.CreateScaledBitmap(icon, 64, 64, false);
            }
            else if (size == Android.Content.Res.ScreenLayout.SizeLarge)
            {
                return Android.Graphics.Bitmap.CreateScaledBitmap(icon, 96, 96, false);
            }
            else if (size == Android.Content.Res.ScreenLayout.SizeXlarge)
            {
                return Android.Graphics.Bitmap.CreateScaledBitmap(icon, 128, 128, false);
            }
            return Android.Graphics.Bitmap.CreateScaledBitmap(icon, 192, 192, false);
        }

        public static Android.Content.Res.ScreenLayout GetDpiSize(Context activity)
        {
            var screenSize = activity.Resources.Configuration.ScreenLayout & Android.Content.Res.ScreenLayout.SizeMask;
            return screenSize;
            //switch (screenSize)
            //{
            //    case ScreenLayout.SizeLarge:
            //        Toast.makeText(this, "Large screen", Toast.LENGTH_LONG).show();
            //        break;
            //    case ScreenLayout.SizeNormal:
            //        Toast.makeText(this, "Normal screen", Toast.LENGTH_LONG).show();
            //        break;
            //    case ScreenLayout.SizeSmall:
            //        Toast.makeText(this, "Small screen", Toast.LENGTH_LONG).show();
            //        break;
            //    default:
            //        Toast.makeText(this, "Screen size is neither large, normal or small", Toast.LENGTH_LONG).show();
            //}
        }

        public static void SetFont(Activity activity, View view)
        {
            if ((int)Android.OS.Build.VERSION.SdkInt > 14)
            {
                Typeface type = Typeface.CreateFromAsset(activity.Assets, "fonts/YekanR.ttf");
                ((TextView)view).SetTypeface(type, TypefaceStyle.Normal);
            }
        }

        public static void SetFont(Activity activity, List<View> views)
        {
            foreach (var view in views)
            {
                SetFont(activity, view);
            }
        }

        public static string TimeToString(Context activity, TimeSpan time)
        {
            if (time == null)
                return FindTextLanguage(activity, "Unknown_Language");
            StringBuilder str = new StringBuilder();
            List<string> items = new List<string>();
            items.Add(time.Seconds + " " + FindTextLanguage(activity, "Secound_Language"));
            if (time.Minutes > 0)
            {
                items.Add(time.Minutes + " " + FindTextLanguage(activity, "Minute_Language") + " " + FindTextLanguage(activity, "And_Language") + " ");
                if (time.Hours > 0)
                {
                    items.Add(time.Hours + " " + FindTextLanguage(activity, "Hour_Language") + " " + FindTextLanguage(activity, "And_Language") + " ");
                    if ((int)time.TotalDays > 0)
                        items.Add((int)time.TotalDays + " " + FindTextLanguage(activity, "Day_Language") + " " + FindTextLanguage(activity, "And_Language") + " ");
                }
            }
            items.Reverse();
            foreach (var item in items)
            {
                str.Append(item);
            }
            return str.ToString();
        }

        public static void SetBackground(Activity activity, View view, int backgroundID = Resource.Color.background)
        {
            if (view == null)
                return;

            int sdk = (int)Android.OS.Build.VERSION.SdkInt;
            if (sdk < 16)
            {
                view.SetBackgroundDrawable(GetDrawable(activity, backgroundID));
            }
            else
            {
                view.Background = GetDrawable(activity, backgroundID);
            }
        }

        public static void SetBackgroundDrawable(View view, Android.Graphics.Drawables.Drawable drawable)
        {
            if (view == null)
                return;
            int sdk = (int)Android.OS.Build.VERSION.SdkInt;
            if (sdk < 16)
            {
                view.SetBackgroundDrawable(drawable);
            }
            else
            {
                view.Background = drawable;
            }
        }

        public static int GetApiVersion()
        {
            return (int)Android.OS.Build.VERSION.SdkInt;
        }

        public static Color GetColor(Activity activity, int colorID)
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                return activity.Resources.GetColor(colorID, activity.ApplicationContext.Theme);
            }
            else
            {
                return activity.Resources.GetColor(colorID);
            }
        }

        public static Android.Graphics.Drawables.Drawable GetDrawable(Activity activity, int id)
        {
            if ((int)Build.VERSION.SdkInt >= 21)
            { //>= API 21
                return activity.Resources.GetDrawable(id, activity.ApplicationContext.Theme);
            }
            else
            {
                return activity.Resources.GetDrawable(id);
            }
        }

        public static void SetProgressBarProgressDrawable(Activity activity, ProgressBar view, int backgroundID = Resource.Color.background)
        {
            //int sdk = (int)Android.OS.Build.VERSION.SdkInt;
            //if (sdk < 16)
            //{
            //    view.SetProgressDrawableTiled(activity.Resources.GetDrawable(backgroundID));
            //}
            //else
            //{
            //view.ProgressDrawable = ;
            if (view == null)
                return;
            Rect bounds = view.ProgressDrawable.Bounds;
            view.ProgressDrawable = GetDrawable(activity, backgroundID);

            view.ProgressDrawable.Bounds = bounds;
            var val = view.Progress;
            var max = view.Max;
            view.Progress++;
            view.Progress--;
            view.Max++;
            view.Max--;
            view.Progress = val;
            view.Max = max;

            // }
        }

        public static void SetTextViewTextColor(Activity activity, TextView view, int colorID)
        {
            view.SetTextColor(GetColor(activity, colorID));
        }

        public static void SetAlphaByEnabled(View view, float alphaV = 0.5f, bool clearAnimation = false)
        {
            try
            {
                if ((int)Android.OS.Build.VERSION.SdkInt >= 11)
                {
                    if (view.Enabled)
                    {
                        view.Alpha = 1.0F;
                        if (clearAnimation)
                            view.ClearAnimation();
                    }
                    else
                    {
                        view.Alpha = alphaV;
                    }
                }
                else
                {
                    float from, to;
                    if (view.Enabled)
                    {

                        from = alphaV;
                        to = 1.0F;
                    }
                    else
                    {
                        to = alphaV;
                        from = 1.0F;
                    }

                    AlphaAnimation alpha = new AlphaAnimation(from, to);
                    alpha.Duration = 0;
                    alpha.FillAfter = true;
                    view.StartAnimation(alpha);
                    if (clearAnimation && view.Enabled)
                    {
                        view.ClearAnimation();
                    }
                }
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "SetAlphaByEnabled");
            }
        }
        public static void SetAlpha(View view, bool isEnable, float alphaV = 0.5f)
        {
            try
            {
                if ((int)Android.OS.Build.VERSION.SdkInt >= 11)
                {
                    if (isEnable)
                    {
                        view.Alpha = 1.0F;
                    }
                    else
                    {
                        view.Alpha = alphaV;
                    }
                }
                else
                {
                    float from, to;
                    if (isEnable)
                    {
                        from = alphaV;
                        to = 1.0F;
                    }
                    else
                    {
                        to = alphaV;
                        from = 1.0F;
                    }

                    AlphaAnimation alpha = new AlphaAnimation(from, to);
                    alpha.Duration = 0;
                    alpha.FillAfter = true;
                    view.StartAnimation(alpha);
                }
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "SetAlpha");
            }
        }
        public static void SetBackground(Activity activity, Window view, int backgroundID = Resource.Color.background)
        {
            int sdk = (int)Android.OS.Build.VERSION.SdkInt;
            view.SetBackgroundDrawable(GetDrawable(activity, backgroundID));
        }

        public static ManualDialogParams ShowControlDialog(Activity activity, View viewObj, string title, Action closedAction, Button cancelButton = null)
        {
            ManualDialogParams manualDialogParams = new Helpers.ManualDialogParams();
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            //builder.SetTitle(ViewUtility.FindTextLanguage(activity, "ChangeLinkAddress_Language"));
            //builder.SetTitle(title);
            View customTitle = activity.LayoutInflater.Inflate(Resource.Layout.CustomTitle, null);
            builder.SetCustomTitle(customTitle);
            var titleView = customTitle.FindViewById<TextView>(Resource.CustomTitle.customtitlebar);
            titleView.Text = FindTextLanguage(activity.ApplicationContext, title);

            if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                SetFont(activity, titleView);
                ViewsUtility.SetRightToLeftLayout(customTitle, new List<int>() { Resource.CustomTitle.LinearLayoutRightToLeft });
                ViewsUtility.SetRightToLeftViews(customTitle, new List<int>() { Resource.CustomTitle.customtitlebar });
            }

            LinearLayout layout = new LinearLayout(activity);
            layout.Orientation = Orientation.Vertical;
            layout.SetPadding(10, 10, 10, 10);
            //LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            //layoutParams.SetMargins(5, 5, 5, 5);
            //layout.LayoutParameters = layoutParams;
            ViewsUtility.SetBackground(activity, layout);
            if (viewObj != null)
                layout.AddView(viewObj);

            builder.SetView(layout);
            //builder.SetMessage("sdvld slm ldsm lsdm klsmdkl mskd msklm kds  iuchviu v vc cv cv cv ");
            //builder.SetPositiveButton(ViewUtility.FindTextLanguage(this, "Save_Language"), (EventHandler<DialogClickEventArgs>)null);
            //builder.SetNegativeButton(ViewUtility.FindTextLanguage(this, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);
            AlertDialog dialogW = null;
            builder.SetCancelable(false);
            dialogW = builder.Create();
            dialogW.CancelEvent += (s, ee) =>
            {
                if (closedAction != null)
                    closedAction();
            };

            dialogW.Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
            if (cancelButton != null)
            {
                cancelButton.Click += (s, e) =>
                {
                    dialogW.Cancel();
                };
            }
            //((ViewGroup)((ViewGroup)dialogW.Window.DecorView).GetChildAt(0)).GetChildAt(1).SetBackgroundColor(CurrentActivity.Resources.GetColor(Resource.Color.background));
            //dialogW.RequestWindowFeature((int)WindowFeatures.NoTitle);
            //ViewsUtility.SetBackground(CurrentActivity, dialogW.Window);
            dialogW.Show();

            int dividerId = dialogW.Context.Resources.GetIdentifier("android:id/titleDivider", null, null);
            View divider = dialogW.FindViewById(dividerId);
            ViewsUtility.SetBackground(activity, divider, Resource.Color.foreground);

            //dividerId = dialogW.Context.Resources.GetIdentifier("android:id/buttonPanel", null, null);
            //LinearLayout mainLinearLayout = dialogW.FindViewById(dividerId) as LinearLayout;
            //mainLinearLayout.Visibility = ViewStates.Gone;

            dividerId = dialogW.Context.Resources.GetIdentifier("android:id/custom", null, null);
            FrameLayout padFrame = dialogW.FindViewById(dividerId) as FrameLayout;
            padFrame.SetPadding(0, 0, 0, 0);

            dividerId = dialogW.Context.Resources.GetIdentifier("android:id/topPanel", null, null);
            LinearLayout topPanel = dialogW.FindViewById(dividerId) as LinearLayout;
            topPanel.SetMinimumHeight(0);
            manualDialogParams.ManualClose = () =>
                {
                    dialogW.Cancel();
                };
            manualDialogParams.Layout = layout;
            return manualDialogParams;
        }

        static void CreateFile(string path)
        {
            int i = 0;
            string fileName = i + "test.test";
            string newAddress = System.IO.Path.Combine(path, fileName);
            while (System.IO.File.Exists(newAddress))
            {
                i++;
                fileName = i + "test.test";
                newAddress = System.IO.Path.Combine(path, fileName);
            }
            System.IO.File.Create(newAddress).Dispose();
            System.IO.File.Delete(newAddress);
        }

        public static void ShowFolderBrowseInLayout(Activity currentActivity, LinearLayout mainLayout, string currentPath, Action<string, bool> selectPath, Action cancel = null, bool isBrowse = false)
        {
            var oldView = mainLayout.GetChildAt(0);
            mainLayout.RemoveAllViews();
            var viewObj = currentActivity.LayoutInflater.Inflate(Resource.Layout.FolderBrowserDialog, null);

            if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewsUtility.SetRightToLeftLayout(viewObj, new List<int>() { Resource.FolderBrowserDialog.LinearLayoutReverce1, Resource.FolderBrowserDialog.LinearLayoutReverce2 });
            }

            ViewsUtility.SetTextLanguage(currentActivity, viewObj, new List<int>() { Resource.FolderBrowserDialog.btnBack, Resource.FolderBrowserDialog.btnCancel, Resource.FolderBrowserDialog.btnGo, Resource.FolderBrowserDialog.btnNewFolder, Resource.FolderBrowserDialog.btnSelect, Resource.FolderBrowserDialog.btnSelectDataFolder });

            List<FolderInfo> folders = new List<FolderInfo>();
            int id = 0;
            string currentFolder = "";
            try
            {
                foreach (var item in System.IO.Directory.GetDirectories(currentPath))
                {
                    folders.Add(new FolderInfo() { Name = System.IO.Path.GetFileName(item), Address = item, Id = id });
                    id++;
                }
                currentFolder = currentPath;
            }
            catch (Exception ex)
            {
                currentFolder = "/";
                foreach (var item in System.IO.Directory.GetDirectories("/"))
                {
                    folders.Add(new FolderInfo() { Name = System.IO.Path.GetFileName(item), Address = item, Id = id });
                    id++;
                }
                InitializeApplication.GoException(ex, "ShowFolderBrowseInLayout");

            }

            var vm = new FolderBrowserDialogViewModel(currentActivity, viewObj, Resource.Layout.FolderInfoItem, folders);
            vm.CurrentFolder = currentFolder;
            mainLayout.AddView(viewObj);

            Action finish = () =>
            {
                if (cancel != null)
                    cancel();
                mainLayout.RemoveAllViews();
                if (oldView != null)
                {
                    mainLayout.AddView(oldView);
                }
            };

            vm.SelectFolderAction = (path) =>
            {
                try
                {
                    CreateFile(path);
                    selectPath(path, false);
                    finish();
                }
                catch(Exception ex)
                {
                    Toast.MakeText(currentActivity, ex.Message + "path: "+ path, ToastLength.Long).Show();
                    AutoLogger.LogError(ex, "SelectFolderAction");
                    Agrin.Views.MainActivity.ActionSetPermission = (uri) =>
                    {
                        //try-
                        //{
                        //    var realP = Streams.FileUtils.getPath(currentActivity, uri);
                        //}
                        //catch (Exception ex)
                        //{

                        //}
                        selectPath(uri.ToString(), true);
                        finish();
                    };
                    if (isBrowse)
                        Agrin.Views.BrowseActivity.TriggerStorageAccessFramework();
                    else
                        Agrin.Views.MainActivity.TriggerStorageAccessFramework();
                }
            };
            vm.CancelSelectFolderAction = finish;
        }

        public static void ShowYesCancelMessageBox(Activity currentActivity, string title, string message, Action okAction, Action cancelAction = null)
        {
            TextView txt = new TextView(currentActivity);
            //txt.SetMaxWidth(200);
            ViewsUtility.SetTextViewTextColor(currentActivity, txt, Resource.Color.foreground);
            txt.Text = message;
            txt.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            txt.SetMaxWidth(600);
            ViewsUtility.ShowCustomResultDialog(currentActivity, title, txt, okAction, cancelAction: cancelAction);
        }

        public static void ShowMessageBoxOnlyOkButton(Activity currentActivity, string title, string message, Action okAction)
        {
            TextView txt = new TextView(currentActivity);
            txt.SetSingleLine(false);
            txt.VerticalScrollBarEnabled = true;
            //txt.SetMaxWidth(200);
            ViewsUtility.SetTextViewTextColor(currentActivity, txt, Resource.Color.foreground);
            txt.Text = message;
            txt.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            txt.SetMaxWidth(600);
            ViewsUtility.ShowCustomResultDialog(currentActivity, title, txt, okAction, isCancel: false);
        }

        public static void ShowCustomResultDialog(Activity currentActivity, string title, View content, Action okAction, Action cancelAction = null, Action noAction = null, string okButtonText = null, string noButtonText = null, bool isCancel = true)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(currentActivity);
            //builder.SetTitle(ViewUtility.FindTextLanguage(activity, "ChangeLinkAddress_Language"));
            //builder.SetTitle(title);

            View customTitle = currentActivity.LayoutInflater.Inflate(Resource.Layout.CustomTitle, null);
            builder.SetCustomTitle(customTitle);
            var titleView = customTitle.FindViewById<TextView>(Resource.CustomTitle.customtitlebar);
            titleView.Text = ViewsUtility.FindTextLanguage(currentActivity.ApplicationContext, title);

            if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewsUtility.SetFont(currentActivity, titleView);
                ViewsUtility.SetRightToLeftLayout(customTitle, new List<int>() { Resource.CustomTitle.LinearLayoutRightToLeft });
                ViewsUtility.SetRightToLeftViews(customTitle, new List<int>() { Resource.CustomTitle.customtitlebar });
            }

            View dialogResultButtons = currentActivity.LayoutInflater.Inflate(Resource.Layout.DialogResultButtons, null);

            LinearLayout mainLayout = dialogResultButtons.FindViewById<LinearLayout>(Resource.DialogResultButtons.MainLayout);
            ViewsUtility.SetBackground(currentActivity, mainLayout);
            LinearLayout layout = dialogResultButtons.FindViewById<LinearLayout>(Resource.DialogResultButtons.ContentLayout);

            layout.AddView(content);
            LinearLayout rtlLayout = dialogResultButtons.FindViewById<LinearLayout>(Resource.DialogResultButtons.ContentLayout);

            if (ProjectDirection == FlowDirection.RightToLeft)
            {
                //SetFont(activity, titleView);
                SetRightToLeftLayout(dialogResultButtons, new List<int>() { Resource.DialogResultButtons.linearLayoutReverse });
                SetRightToLeftViews(content);
            }

            ViewsUtility.SetTextLanguage(currentActivity, dialogResultButtons, new List<int>() { Resource.DialogResultButtons.btnOK, Resource.DialogResultButtons.btnCancel });

            builder.SetView(dialogResultButtons);
            AlertDialog dialogW = null;

            Button btnOK = dialogResultButtons.FindViewById<Button>(Resource.DialogResultButtons.btnOK);
            Button btnCancel = dialogResultButtons.FindViewById<Button>(Resource.DialogResultButtons.btnCancel);
            Button btnNo = dialogResultButtons.FindViewById<Button>(Resource.DialogResultButtons.btnNo);
            if (okButtonText != null)
            {
                btnOK.Text = FindTextLanguage(currentActivity, okButtonText);
            }
            if (noButtonText != null)
            {
                btnNo.Visibility = ViewStates.Visible;
                btnNo.Text = FindTextLanguage(currentActivity, noButtonText);
                btnNo.Click += (s, args) =>
                {
                    noAction();
                    dialogW.Dismiss();
                };
            }
            dialogW = builder.Create();
            dialogW.SetCancelable(false);
            dialogW.Show();

            btnOK.Click += (s, args) =>
            {
                if (okAction != null)
                    okAction();
                dialogW.Dismiss();
            };
            if (!isCancel)
            {
                btnCancel.Visibility = ViewStates.Gone;
            }
            else
            {
                btnCancel.Click += (s, args) =>
                {
                    if (cancelAction != null)
                        cancelAction();
                    dialogW.Dismiss();
                };
            }

            int dividerId = dialogW.Context.Resources.GetIdentifier("android:id/titleDivider", null, null);
            View divider = dialogW.FindViewById(dividerId);
            ViewsUtility.SetBackground(currentActivity, divider, Resource.Color.foreground);

            //dividerId = dialogW.Context.Resources.GetIdentifier("android:id/buttonPanel", null, null);
            //LinearLayout mainLinearLayout = dialogW.FindViewById(dividerId) as LinearLayout;
            //mainLinearLayout.Visibility = ViewStates.Gone;

            dividerId = dialogW.Context.Resources.GetIdentifier("android:id/custom", null, null);
            FrameLayout padFrame = dialogW.FindViewById(dividerId) as FrameLayout;
            padFrame.SetPadding(0, 0, 0, 0);

            dividerId = dialogW.Context.Resources.GetIdentifier("android:id/topPanel", null, null);
            LinearLayout topPanel = dialogW.FindViewById(dividerId) as LinearLayout;
            topPanel.SetMinimumHeight(0);
        }

        public static void ShowMenuDialog<T>(Activity currentActivity, string title, List<T> items, Func<T, string> generateMenuName, Action<T> clickItem, Action close = null)
        {
            try
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(currentActivity);
                View customTitle = currentActivity.LayoutInflater.Inflate(Resource.Layout.CustomTitle, null);
                builder.SetCustomTitle(customTitle);
                var titleView = customTitle.FindViewById<TextView>(Resource.CustomTitle.customtitlebar);
                titleView.Text = ViewsUtility.FindTextLanguage(currentActivity.ApplicationContext, title);
                titleView.SetPadding(10, 0, 0, 0);
                //titleView.SetLines(1);
                //titleView.Ellipsize = Android.Text.TextUtils.TruncateAt.Middle;
                if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
                {
                    ViewsUtility.SetFont(currentActivity, titleView);
                    ViewsUtility.SetRightToLeftLayout(customTitle, new List<int>() { Resource.CustomTitle.LinearLayoutRightToLeft });
                    ViewsUtility.SetRightToLeftViews(customTitle, new List<int>() { Resource.CustomTitle.customtitlebar });
                }
                LinearLayout mainLayout = new LinearLayout(currentActivity);
                var view = currentActivity.LayoutInflater.Inflate(Resource.Layout.CustomListView, mainLayout, false);
                var mainLayoutListView = view.FindViewById<LinearLayout>(Resource.CustomListView.mainLayoutListView);
                mainLayout.AddView(view);
                AlertDialog dialogW = null;
                foreach (var item in items)
                {
                    var viewItem = currentActivity.LayoutInflater.Inflate(Resource.Layout.LinkInfoMenu, mainLayoutListView, false);
                    var rtlLayout = viewItem.FindViewById<LinearLayout>(Resource.LinkInfoMenu.mainLayout);
                    var txtHeader = viewItem.FindViewById<TextView>(Resource.LinkInfoMenu.txtHeader);
                    ViewsUtility.SetFont(currentActivity, txtHeader);
                    if (generateMenuName == null)
                        txtHeader.Text = item.ToString();
                    else
                        txtHeader.Text = generateMenuName(item);
                    rtlLayout.Click += (s, e) =>
                    {
                        clickItem(item);
                        dialogW.Dismiss();
                    };
                    mainLayoutListView.AddView(viewItem);
                    ViewsUtility.SetRightToLeftLayout(viewItem, new List<int>() { Resource.LinkInfoMenu.mainLayout });
                    ViewsUtility.SetRightToLeftViews(viewItem, new List<int>() { Resource.LinkInfoMenu.txtHeader });
                }


                builder.SetView(mainLayout);

                dialogW = builder.Create();
                WindowManagerLayoutParams lp = new WindowManagerLayoutParams();
                lp.CopyFrom(dialogW.Window.Attributes);
                lp.Width = LinearLayout.LayoutParams.MatchParent;
                lp.Height = LinearLayout.LayoutParams.WrapContent;
                dialogW.Show();
                dialogW.Window.Attributes = lp;
                dialogW.Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Transparent));
                dialogW.DismissEvent += (s, e) =>
                {
                    if (close != null)
                        close();
                };
                int dividerId = dialogW.Context.Resources.GetIdentifier("android:id/titleDivider", null, null);
                View divider = dialogW.FindViewById(dividerId);
                ViewsUtility.SetBackground(currentActivity, divider, Resource.Color.foreground);

                dividerId = dialogW.Context.Resources.GetIdentifier("android:id/custom", null, null);
                FrameLayout padFrame = dialogW.FindViewById(dividerId) as FrameLayout;
                padFrame.SetPadding(0, 0, 0, 0);

                dividerId = dialogW.Context.Resources.GetIdentifier("android:id/topPanel", null, null);
                LinearLayout topPanel = dialogW.FindViewById(dividerId) as LinearLayout;
                topPanel.SetMinimumHeight(0);
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "ShowMenuDialog<T>");
            }
        }

        public static void SetTextAppearance(Activity currentActivity, TextView txt, int appearance)
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                txt.SetTextAppearance(currentActivity, appearance);
            }
            else
            {
                txt.SetTextAppearance(appearance);
            }
        }

        public static void ShowPageDialog(string url, string title, Action okClick, Activity currentActivity, bool cancelButton = false, string btnOkText = "OK_Language")
        {

            LinearLayout layoutWeb = new LinearLayout(currentActivity);
            layoutWeb.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layoutWeb.LayoutParameters = layoutParams;

            var mWebview = new WebView(currentActivity);

            mWebview.Settings.JavaScriptEnabled = true; // enable javascript
            var wclient = new CustomWebViewClient() { activity = currentActivity };

            mWebview.SetWebViewClient(wclient);
            TextView txt = new TextView(currentActivity);
            ProgressBar prog = new ProgressBar(currentActivity);

            txt.Text = ViewsUtility.FindTextLanguage(currentActivity, "Loading_Language");
            SetTextAppearance(currentActivity, txt, Android.Resource.Attribute.TextAppearanceLarge);
            mWebview.SetMinimumHeight(200);
            mWebview.Visibility = ViewStates.Gone;

            layoutWeb.AddView(mWebview);
            layoutWeb.AddView(txt);
            layoutWeb.AddView(prog);


            AlertDialog.Builder builder = new AlertDialog.Builder(currentActivity);
            //builder.SetTitle(ViewUtility.FindTextLanguage(activity, "ChangeLinkAddress_Language"));
            //builder.SetTitle(title);

            View customTitle = currentActivity.LayoutInflater.Inflate(Resource.Layout.CustomTitle, null);
            builder.SetCustomTitle(customTitle);
            var titleView = customTitle.FindViewById<TextView>(Resource.CustomTitle.customtitlebar);
            titleView.Text = ViewsUtility.FindTextLanguage(currentActivity.ApplicationContext, title);

            if (ViewsUtility.ProjectDirection == FlowDirection.RightToLeft)
            {
                ViewsUtility.SetFont(currentActivity, titleView);
                ViewsUtility.SetRightToLeftLayout(customTitle, new List<int>() { Resource.CustomTitle.LinearLayoutRightToLeft });
                ViewsUtility.SetRightToLeftViews(customTitle, new List<int>() { Resource.CustomTitle.customtitlebar });
            }

            View dialogResultButtons = currentActivity.LayoutInflater.Inflate(Resource.Layout.DialogResultButtons, null);

            LinearLayout mainLayout = dialogResultButtons.FindViewById<LinearLayout>(Resource.DialogResultButtons.MainLayout);
            ViewsUtility.SetBackground(currentActivity, mainLayout);
            LinearLayout layout = dialogResultButtons.FindViewById<LinearLayout>(Resource.DialogResultButtons.ContentLayout);

            layout.AddView(layoutWeb);
            LinearLayout rtlLayout = dialogResultButtons.FindViewById<LinearLayout>(Resource.DialogResultButtons.ContentLayout);

            if (ProjectDirection == FlowDirection.RightToLeft)
            {
                //SetFont(activity, titleView);
                SetRightToLeftLayout(dialogResultButtons, new List<int>() { Resource.DialogResultButtons.linearLayoutReverse });
                SetRightToLeftViews(layoutWeb);
            }

            ViewsUtility.SetTextLanguage(currentActivity, dialogResultButtons, new List<int>() { Resource.DialogResultButtons.btnOK, Resource.DialogResultButtons.btnCancel });

            builder.SetView(dialogResultButtons);
            AlertDialog dialogW = null;

            Button btnOK = dialogResultButtons.FindViewById<Button>(Resource.DialogResultButtons.btnOK);
            Button btnCancel = dialogResultButtons.FindViewById<Button>(Resource.DialogResultButtons.btnCancel);

            dialogW = builder.Create();
            dialogW.SetCancelable(false);
            dialogW.Show();

            btnCancel.Visibility = ViewStates.Gone;

            btnOK.Click += (s, args) =>
            {
                dialogW.Dismiss();
            };
            int dividerId = dialogW.Context.Resources.GetIdentifier("android:id/titleDivider", null, null);
            View divider = dialogW.FindViewById(dividerId);
            ViewsUtility.SetBackground(currentActivity, divider, Resource.Color.foreground);

            //dividerId = dialogW.Context.Resources.GetIdentifier("android:id/buttonPanel", null, null);
            //LinearLayout mainLinearLayout = dialogW.FindViewById(dividerId) as LinearLayout;
            //mainLinearLayout.Visibility = ViewStates.Gone;

            dividerId = dialogW.Context.Resources.GetIdentifier("android:id/custom", null, null);
            FrameLayout padFrame = dialogW.FindViewById(dividerId) as FrameLayout;
            padFrame.SetPadding(0, 0, 0, 0);

            dividerId = dialogW.Context.Resources.GetIdentifier("android:id/topPanel", null, null);
            LinearLayout topPanel = dialogW.FindViewById(dividerId) as LinearLayout;
            topPanel.SetMinimumHeight(0);



            wclient.ErrorLoadingAction = () =>
            {
            };

            wclient.LoadComepleteAction = (error) =>
            {
                mWebview.Visibility = ViewStates.Visible;
                prog.Visibility = txt.Visibility = ViewStates.Gone;
            };

            mWebview.LoadUrl(url);
        }

        public static String GetMimeType(String url)
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
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(type.Trim()))
                return "application/x-msdownload";
            return type;
        }

        public static T GetParent<T>(View view) where T : class
        {
            var parent = view.Parent;
            while (parent != null)
            {
                if (parent is T)
                    return (T)parent;
                parent = parent.Parent;
            }
            return null;
        }
    }

    public class ManualDialogParams
    {
        public LinearLayout Layout { get; set; }
        public Action ManualClose { get; set; }
    }

    public class CustomWebViewClient : WebViewClient
    {
        public Activity activity { get; set; }
        public Action<bool> LoadComepleteAction { get; set; }
        public Action ErrorLoadingAction { get; set; }
        bool isError = false;
        public override void OnLoadResource(WebView view, string url)
        {
            isError = false;
            base.OnLoadResource(view, url);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            if (url.ToLower().Contains("http://openandroidintent.agrin"))
            {
                var links = url.Split(new string[] { "#AgrinTag#" }, StringSplitOptions.None);
                if (links.Length == 3)
                {
                    var data = links[1];
                    var value = links[2];
                    Intent browserIntent = new Intent(data, Android.Net.Uri.Parse(value));
                    Views.MainActivity.This.StartActivity(browserIntent);
                    return base.ShouldOverrideUrlLoading(view, value);
                }
            }
            return base.ShouldOverrideUrlLoading(view, url);
        }

        public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
        {
            isError = true;
            if (ErrorLoadingAction != null)
                ErrorLoadingAction();
            Toast.MakeText(activity, description, ToastLength.Short).Show();
        }

        public override void OnPageFinished(WebView view, string url)
        {
            if (LoadComepleteAction != null)
                LoadComepleteAction(isError);
            base.OnPageFinished(view, url);
        }
    }
}