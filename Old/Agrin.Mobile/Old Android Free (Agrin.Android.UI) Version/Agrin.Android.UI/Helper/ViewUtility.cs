using System;
using Android.Widget;
using Android.Views;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using System.Text;
using Android.Webkit;
using Android.Content;

namespace Agrin.MonoAndroid.UI
{
    public enum FlowDirection
    {
        LeftToRight,
        RightToLeft
    }

    public static class ViewUtility
    {
        private static FlowDirection _ProjectDirection = FlowDirection.RightToLeft;
        public static FlowDirection ProjectDirection
        {
            get { return ViewUtility._ProjectDirection; }
            set { ViewUtility._ProjectDirection = value; }
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


        public static void SetRightToLeftLayout(Activity activity, List<int> layouts)
        {
            if (ProjectDirection == FlowDirection.LeftToRight)
                return;
            foreach (var item in layouts)
            {
                var view = activity.FindViewById<LinearLayout>(item);
                view.SetGravity(GravityFlags.Right);
                List<View> views = new List<View>();
                for (int x = 0; x < view.ChildCount; x++)
                {
                    views.Add(view.GetChildAt(x));
                }
                view.RemoveAllViews();
                for (int x = views.Count - 1; x >= 0; x--)
                {
                    view.AddView(views[x]);
                }
            }
        }

        public static void SetRightToLeftViews(Activity activity, List<int> views)
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

        public static void SetLeftToRightCheckBox(Activity activity, int item)
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
                chk.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                chk.SetCompoundDrawablesWithIntrinsicBounds(null, null, Android.Graphics.Drawables.Drawable.CreateFromPath("?android:attr/listChoiceIndicatorMultiple"), null);
                chk.SetButtonDrawable(null);
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
                Agrin.Log.AutoLogger.LogError(e, "Error Get Language FindTextLanguage : " + text == null ? "null" : text + " !", true);
                return text;
            }
        }

        public static void SetTextLanguage(Activity activity, List<int> items)
        {
            activity.Title = FindTextLanguage(activity, activity.Title);
            foreach (var item in items)
            {
                var view = activity.FindViewById(item);
                if (view is TextView)
                {
                    ((TextView)view).Text = FindTextLanguage(activity, ((TextView)view).Text);
                }
                else if (view is Button)
                {
                    ((Button)view).Text = FindTextLanguage(activity, ((Button)view).Text);
                }
            }
        }

        public static string TimeToString(Activity activity, TimeSpan time)
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

        public static void ShowPageDialog(string url, string title, Action okClick, Activity currentActivity, bool cancelButton = false, string btnOkText = "OK_Language")
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(currentActivity);
            builder.SetTitle(FindTextLanguage(currentActivity, title));
            LinearLayout layout = new LinearLayout(currentActivity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            var mWebview = new WebView(currentActivity);

            mWebview.Settings.JavaScriptEnabled = false; // enable javascript
            var wclient = new CustomWebViewClient() { activity = currentActivity };

            mWebview.SetWebViewClient(wclient);
            TextView txt = new TextView(currentActivity);
            ProgressBar prog = new ProgressBar(currentActivity);

            txt.Text = ViewUtility.FindTextLanguage(currentActivity, "Loading_Language");
            txt.SetTextAppearance(currentActivity, Android.Resource.Attribute.TextAppearanceLarge);

            mWebview.SetMinimumHeight(200);
            mWebview.Visibility = ViewStates.Gone;

            layout.AddView(mWebview);
            layout.AddView(txt);
            layout.AddView(prog);

            builder.SetView(layout);
            AlertDialog dialogW = null;

            builder.SetNegativeButton(ViewUtility.FindTextLanguage(currentActivity, btnOkText), (EventHandler<DialogClickEventArgs>)null);
            if (cancelButton)
                builder.SetPositiveButton(ViewUtility.FindTextLanguage(currentActivity, "No_Language"), (EventHandler<DialogClickEventArgs>)null);
            dialogW = builder.Create();
            dialogW.SetCancelable(false);
            dialogW.Show();

            // Get the buttons.
            var yesBtn = dialogW.GetButton((int)DialogButtonType.Negative);
            if (cancelButton)
                yesBtn.Enabled = false;
            wclient.ErrorLoadingAction = () =>
            {
                if (cancelButton)
                    yesBtn.Enabled = false;
            };

            wclient.LoadComepleteAction = (error) =>
            {
                mWebview.Visibility = ViewStates.Visible;
                prog.Visibility = txt.Visibility = ViewStates.Gone;
                if (!error)
                    yesBtn.Enabled = true;
            };

            mWebview.LoadUrl(url);
            yesBtn.Click += (s, args) =>
            {
                if (okClick != null)
                    okClick();
                dialogW.Dismiss();
            };

            if (cancelButton)
            {
                var noBtn = dialogW.GetButton((int)DialogButtonType.Positive);
                noBtn.Click += (s, args) =>
                {
                    dialogW.Dismiss();
                };
            }
        }

        public static void ShowMessageDialog(Activity activity, string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle(ViewUtility.FindTextLanguage(activity, title));
            LinearLayout layout = new LinearLayout(activity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            TextView txtMessage = new TextView(activity);
            txtMessage.SetTextAppearance(activity, global::Android.Resource.Style.TextAppearanceSmall);
            txtMessage.SetSingleLine(false);
            txtMessage.VerticalScrollBarEnabled = true;
            txtMessage.Text = ViewUtility.FindTextLanguage(activity, message);
            layout.AddView(txtMessage);
            ScrollView scroll = new ScrollView(activity);
            scroll.AddView(layout);
            builder.SetView(scroll);
            // Set up the buttons
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(activity, "OK_Language"), (dialog, which) =>
            {
                builder.Dispose();
            });

            builder.Show();
        }

        public static void ShowQuestionMessageDialog(Activity activity, string message, string title, Action okClick)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle(ViewUtility.FindTextLanguage(activity, title));
            LinearLayout layout = new LinearLayout(activity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            TextView txtMessage = new TextView(activity);
            txtMessage.SetTextAppearance(activity, global::Android.Resource.Style.TextAppearanceSmall);
            txtMessage.SetSingleLine(false);
            txtMessage.VerticalScrollBarEnabled = true;
            txtMessage.Text = ViewUtility.FindTextLanguage(activity, message);
            layout.AddView(txtMessage);
            ScrollView scroll = new ScrollView(activity);
            scroll.AddView(layout);
            builder.SetView(scroll);
            // Set up the buttons
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(activity, "OK_Language"), (dialog, which) =>
            {
                okClick();
                builder.Dispose();
            });
            builder.SetNegativeButton(ViewUtility.FindTextLanguage(activity, "Cancel_Language"), (dialog, which) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }
    }
}

