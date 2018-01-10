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
using Agrin.Helpers;
using Agrin.Download.Web;
using Agrin.Download.Manager;
using System.Threading.Tasks;
using System.Threading;
using Agrin.Models;

namespace Agrin.Views.List
{
    public class TaskInfoesManager : IDisposable
    {
        static TaskInfoesManager currentTaskInfoesManager = null;
        Activity CurrentActivity { get; set; }

        LinearLayout _mainLayoutListView = null;
        LinearLayout _mainLayout;
        LinearLayout _actionToolBoxLayout;
        LinearLayout _actionTopToolBoxLayout;
        int _templateResourceId;
        View _CustomListView;
        public TaskInfoesManager(Activity context, int templateResourceId, LinearLayout mainLayout, LinearLayout actionToolBoxLayout, LinearLayout actionTopToolBoxLayout)
        {
            currentTaskInfoesManager = this;
            CurrentActivity = context;
            _mainLayout = mainLayout;
            _templateResourceId = templateResourceId;
            _CustomListView = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.CustomListView, mainLayout, false);
            _mainLayoutListView = _CustomListView.FindViewById<LinearLayout>(Resource.CustomListView.mainLayoutListView);
            _actionToolBoxLayout = actionToolBoxLayout;
            _actionTopToolBoxLayout = actionTopToolBoxLayout;
            InitializeView();


            InitializeAllItems();
            AutoDraw();
            ApplicationTaskManager.Current.TaskInfoes.CollectionChanged -= TaskInfoes_CollectionChanged;
            ApplicationTaskManager.Current.TaskInfoes.CollectionChanged += TaskInfoes_CollectionChanged;
        }

        void TaskInfoes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                NotifyChanged();
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "TaskInfoesManager notifyChanged");
            }
        }

        static bool startedAutoDraw = false;
        public static bool IsPauseAutoDraw = false;
        static void AutoDraw()
        {
            if (startedAutoDraw)
                return;
            startedAutoDraw = true;
            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!IsPauseAutoDraw && !currentTaskInfoesManager._isDispose)
                        {
                            currentTaskInfoesManager.CurrentActivity.RunOnUI(() =>
                            {
                                try
                                {
                                    lock (lockOBJ)
                                    {
                                        foreach (var item in ApplicationTaskManager.Current.TaskInfoes.ToArray())
                                        {
                                            if (item.IsActive && (item.State == TaskState.Started || item.State == TaskState.WaitingForWork || item.State == TaskState.Working))
                                            {
                                                if (currentTaskInfoesManager.getViewByTaskInfo.ContainsKey(item))
                                                    currentTaskInfoesManager.DrawOneTaskInfo(item, currentTaskInfoesManager.getViewByTaskInfo[item]);
                                            }
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    InitializeApplication.GoException(ex, "TaskInfoesManager AutoDraw 2");
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        InitializeApplication.GoException(ex, "TaskInfoesManager AutoDraw");
                    }
                    Thread.Sleep(1000);
                }
            });
            task.Start();
        }

        public void InitializeView()
        {
            if (_actionToolBoxLayout.ChildCount > 0)
                _actionToolBoxLayout.RemoveAllViews();
            _actionToolBoxLayout.Visibility = ViewStates.Gone;

            if (_actionTopToolBoxLayout.ChildCount > 0)
                _actionTopToolBoxLayout.RemoveAllViews();
            _actionTopToolBoxLayout.Visibility = ViewStates.Gone;

            if (_mainLayout.ChildCount > 0)
                _mainLayout.RemoveAllViews();
            _mainLayout.AddView(_CustomListView);
        }

        Dictionary<TaskInfo, View> getViewByTaskInfo = new Dictionary<TaskInfo, View>();
        static object lockOBJ = new object();
        List<TaskInfo> lastChnages = null;

        public void InitializeAllItems()
        {
            lock (lockOBJ)
            {
                getViewByTaskInfo.Clear();
                var list = ApplicationTaskManager.Current.TaskInfoes.ToList();
                lastChnages = ApplicationTaskManager.Current.TaskInfoes.ToList();
                foreach (var item in list)
                {
                    if (_isDispose)
                        return;
                    if (getViewByTaskInfo.ContainsKey(item))
                    {
                        _mainLayoutListView.RemoveView(getViewByTaskInfo[item]);
                        getViewByTaskInfo[item].Dispose();
                        getViewByTaskInfo.Remove(item);
                        BindingHelper.DisposeObject(item);
                    }
                    var view = DrawOneView(item);
                    _mainLayoutListView.AddView(view, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    getViewByTaskInfo.Add(item, view);
                }
            }
        }

        public void NotifyChanged()
        {
            int index = 10;
            //اضافه شده ها
            var added = ApplicationTaskManager.Current.TaskInfoes.Except(lastChnages).ToList();
            var removed = lastChnages.Except(ApplicationTaskManager.Current.TaskInfoes).ToList();
            if (added.Count == 0 && removed.Count == 0)
                return;

            foreach (var item in added)
            {
                if (_isDispose)
                    return;
                if (getViewByTaskInfo.ContainsKey(item))
                {
                    _mainLayoutListView.RemoveView(getViewByTaskInfo[item]);
                    getViewByTaskInfo[item].Dispose();
                    getViewByTaskInfo.Remove(item);
                    BindingHelper.DisposeObject(item);
                }
                index = ApplicationTaskManager.Current.TaskInfoes.IndexOf(item);
                var view = DrawOneView(item);
                _mainLayoutListView.AddView(view, index, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
                getViewByTaskInfo.Add(item, view);
            }

            foreach (var item in removed)
            {
                if (_isDispose)
                    return;
                BindingHelper.DisposeObject(item);
                _mainLayoutListView.RemoveView(getViewByTaskInfo[item]);
                getViewByTaskInfo.Remove(item);
            }
            lastChnages = ApplicationTaskManager.Current.TaskInfoes.ToList();
        }

        bool _isDispose = false;

        public View DrawOneView(TaskInfo item)
        {
            if (_isDispose)
                return null;

            var view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, _mainLayoutListView, false);

            var txtTaskName = view.FindViewById<TextView>(Resource.TaskInfoItem.txtTaskName);
            txtTaskName.Text = item.Name;


            //layouts :

            ViewsUtility.SetRightToLeftLayout(view, new List<int>()
            {
                Resource.TaskInfoItem.rootLayout, Resource.TaskInfoItem.nameLayout, Resource.TaskInfoItem.startTimeLayout,
                Resource.TaskInfoItem.startTimeTextLayout,Resource.TaskInfoItem.tasksForStartLayout,Resource.TaskInfoItem.tasksForStartTitleLayout,
                Resource.TaskInfoItem.tasksForStartTextLayout,
                Resource.TaskInfoItem.remaningTimeLayout,Resource.TaskInfoItem.statusLayout,
                
                Resource.TaskInfoItem.remaningTimeTitleLayout,Resource.TaskInfoItem.remaningTimeTextLayout,Resource.TaskInfoItem.stateLayout,
                Resource.TaskInfoItem.stateTitleLayout,Resource.TaskInfoItem.stateTextLayout,Resource.TaskInfoItem.OnOffLayout,
                Resource.TaskInfoItem.OnOffTitleLayout,Resource.TaskInfoItem.OnOffTextLayout,Resource.TaskInfoItem.tasksForEndLayout,
                Resource.TaskInfoItem.tasksForEndTitleLayout,Resource.TaskInfoItem.tasksForEndTextLayout
            });

            ViewsUtility.SetTextLanguage(CurrentActivity, view, new List<int>() 
            {
                Resource.TaskInfoItem.txtStartTimeTitle,Resource.TaskInfoItem.txtTimeRemaningTitle,Resource.TaskInfoItem.txtstateTitle,
                Resource.TaskInfoItem.txtOnOffTitle,Resource.TaskInfoItem.txtTasksForStartTitle,Resource.TaskInfoItem.txttasksForEndTitle
            }, false);

            DrawOneTaskInfo(item, view);

            return view;
        }

        public void DrawOneTaskInfo(TaskInfo item, View view)
        {
            var txtstate = view.FindViewById<TextView>(Resource.TaskInfoItem.txtstate);
            if (item.State == TaskState.Started)
                txtstate.Text = ViewsUtility.FindTextLanguage(CurrentActivity, "TaskStateStarted_Language");
            else if (item.State == TaskState.Stoped)
                txtstate.Text = ViewsUtility.FindTextLanguage(CurrentActivity, "TaskStateStoped_Language");
            else if (item.State == TaskState.WaitingForWork)
                txtstate.Text = ViewsUtility.FindTextLanguage(CurrentActivity, "TaskStateWaitingForWork_Language");
            else if (item.State == TaskState.Working)
                txtstate.Text = ViewsUtility.FindTextLanguage(CurrentActivity, "TaskStateWorking_Language");
            var mainLayout = view.FindViewById<View>(Resource.TaskInfoItem.mainLayout);

            var startTimeTitleLayout = view.FindViewById<View>(Resource.TaskInfoItem.startTimeTitleLayout);
            var startTimeTextLayout = view.FindViewById<View>(Resource.TaskInfoItem.startTimeTextLayout);
            var tasksForStartTitleLayout = view.FindViewById<View>(Resource.TaskInfoItem.tasksForStartTitleLayout);
            var tasksForStartTextLayout = view.FindViewById<View>(Resource.TaskInfoItem.tasksForStartTextLayout);
            var tasksForEndTitleLayout = view.FindViewById<View>(Resource.TaskInfoItem.tasksForEndTitleLayout);
            var tasksForEndTextLayout = view.FindViewById<View>(Resource.TaskInfoItem.tasksForEndTextLayout);
            var stateTitleLayout = view.FindViewById<View>(Resource.TaskInfoItem.stateTitleLayout);
            var stateTextLayout = view.FindViewById<View>(Resource.TaskInfoItem.stateTextLayout);
            var OnOffTitleLayout = view.FindViewById<View>(Resource.TaskInfoItem.OnOffTitleLayout);
            var OnOffTextLayout = view.FindViewById<View>(Resource.TaskInfoItem.OnOffTextLayout);
            var remaningTimeTitleLayout = view.FindViewById<View>(Resource.TaskInfoItem.remaningTimeTitleLayout);
            var remaningTimeTextLayout = view.FindViewById<View>(Resource.TaskInfoItem.remaningTimeTitleLayout);
            var nameLayout = view.FindViewById<View>(Resource.TaskInfoItem.nameLayout);
            var txtOnOff = view.FindViewById<TextView>(Resource.TaskInfoItem.txtOnOff);
            var txtTasksForStart = view.FindViewById<TextView>(Resource.TaskInfoItem.txtTasksForStart);
            var txttasksForEnd = view.FindViewById<TextView>(Resource.TaskInfoItem.txttasksForEnd);
            var txtTimeRemaning = view.FindViewById<TextView>(Resource.TaskInfoItem.txtTimeRemaning);
            var txtStartTime = view.FindViewById<TextView>(Resource.TaskInfoItem.txtStartTime);

            var remaningTimeLayout = view.FindViewById<View>(Resource.TaskInfoItem.remaningTimeLayout);

            nameLayout.Enabled = remaningTimeTextLayout.Enabled = remaningTimeTitleLayout.Enabled = OnOffTextLayout.Enabled = OnOffTitleLayout.Enabled = stateTextLayout.Enabled = stateTitleLayout.Enabled = tasksForEndTextLayout.Enabled = tasksForEndTitleLayout.Enabled = tasksForStartTextLayout.Enabled = tasksForStartTitleLayout.Enabled = startTimeTextLayout.Enabled = startTimeTitleLayout.Enabled = item.IsActive;

            ViewsUtility.SetAlphaByEnabled(startTimeTitleLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(startTimeTextLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(tasksForStartTitleLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(tasksForStartTextLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(tasksForEndTitleLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(tasksForEndTextLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(stateTitleLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(stateTextLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(OnOffTitleLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(OnOffTextLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(remaningTimeTitleLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(remaningTimeTextLayout, clearAnimation: true);
            ViewsUtility.SetAlphaByEnabled(nameLayout, clearAnimation: true);

            if (!item.IsActive || (item.DateTimes.Count > 0 && item.DateTimes.Max() < DateTime.Now) || (item.State != TaskState.WaitingForWork && item.State != TaskState.Working))
            {
                remaningTimeLayout.Visibility = ViewStates.Invisible;
            }
            else
            {
                remaningTimeLayout.Visibility = ViewStates.Visible;
            }

            if (item.IsActive)
            {
                txtOnOff.Text = ViewsUtility.FindTextLanguage(CurrentActivity, "TaskActive_Language");
            }
            else
            {
                txtOnOff.Text = ViewsUtility.FindTextLanguage(CurrentActivity, "TaskDeActive_Language");
            }

            txtTasksForStart.Text = GetTextFromTaskInfoTask(item.TaskUtilityActions);
            txttasksForEnd.Text = GetTextFromTaskInfoTask(item.LinkCompleteTaskUtilityActions);

            txtTimeRemaning.Text = GetRemainingTimeString(item);
            txtStartTime.Text = GetStartTimeString(item);
            mainLayout.Tag = item.Cast();
            mainLayout.Click -= mainLayout_Click;
            mainLayout.Click += mainLayout_Click;
        }

        void mainLayout_Click(object sender, EventArgs e)
        {
            View view = sender as View;
            MainActivity.SelectedTaskInfoFromMenu = view.Tag.Cast<TaskInfo>();
            CurrentActivity.RegisterForContextMenu(view);
            CurrentActivity.OpenContextMenu(view);
            CurrentActivity.UnregisterForContextMenu(view);
        }

        public string GetTextFromTaskInfoTask(List<TaskUtilityModeEnum> utilities)
        {
            StringBuilder text = new StringBuilder();
            foreach (var item in utilities)
            {
                switch (item)
                {
                    case TaskUtilityModeEnum.ActiveTasks:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityActiveTasks_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.CloseApplication:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityCloseApplication_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.DeactiveTasks:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityDeactiveTasks_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.InternetOff:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityInternetOff_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.InternetOn:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityInternetOn_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.Sleep:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilitySleep_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.StartLink:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityStartLink_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.StopLink:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityStopLink_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.StopTasks:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityStopTasks_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.TurrnOff:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityTurrnOff_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.WiFiOff:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityWiFiOff_Language"));
                            text.Append(" , ");
                            break;
                        }
                    case TaskUtilityModeEnum.WiFiOn:
                        {
                            text.Append(ViewsUtility.FindTextLanguage(CurrentActivity, "TaskUtilityWiFiOn_Language"));
                            text.Append(" , ");
                            break;
                        }
                }
            }

            return text.ToString().Trim().Trim(new char[] { ',' }).Trim();
        }

        public string GetRemainingTimeString(TaskInfo taskInfo)
        {
            StringBuilder text = new StringBuilder();
            if (taskInfo.DateTimes.Count == 0)
            {
                if (taskInfo.IsStartNow)
                {
                    return "شروع در لحظه";
                }
                else
                    return "زمانی درج نشده";
            }
            TimeSpan time = taskInfo.DateTimes.Min() - DateTime.Now;
            if (time.Ticks < 0)
                return "از موعد گذشته";
            if (time.Days > 0)
            {
                text.Append(time.Days + " روز و ");
            }
            if (time.Hours > 0)
            {
                text.Append(time.Hours + " ساعت و ");
            }
            if (time.Minutes > 0)
            {
                text.Append(time.Minutes + " دقیقه و ");
            }
            if (time.Seconds >= 0)
            {
                text.Append(time.Seconds + " ثانیه ");
            }
            return text.ToString();
        }

        System.Globalization.Calendar cal = new System.Globalization.PersianCalendar();
        string DateTimeToText(DateTime dt)
        {
            return cal.GetYear(dt).ToString().Substring(2, 2) + "/" + cal.GetMonth(dt) + "/" + cal.GetDayOfMonth(dt) + " زمان: " + dt.Hour + ":" + dt.Minute;
        }

        public string GetStartTimeString(TaskInfo taskInfo)
        {
            StringBuilder text = new StringBuilder();
            if (taskInfo.DateTimes.Count == 0)
            {
                if (taskInfo.IsStartNow)
                {
                    return "شروع در لحظه";
                }
                else
                    return "زمانی درج نشده";
            }
            var dt = taskInfo.DateTimes.Min();
            TimeSpan time = taskInfo.DateTimes.Min() - DateTime.Now;
            var tomorrow = DateTime.Today.AddDays(1);

            if (time.Ticks < 0)
                return "از موعد گذشته";
            if (dt.Year == DateTime.Now.Year && dt.Month == DateTime.Now.Month && dt.Day == DateTime.Now.Day)
                text.Append("امروز ");
            else if (dt.Year == tomorrow.Year && dt.Month == tomorrow.Month && dt.Day == tomorrow.Day)
                text.Append("فردا ");
            else
                text.Append(time.Days + " روز دیگه ");

            text.Append(DateTimeToText(dt));

            return text.ToString();
        }

        public static List<MenuItem> GenerateMenus(Activity activity)
        {
            Func<string, string> getText = (text) =>
            {
                return ViewsUtility.FindTextLanguage(activity, text);
            };

            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem() { Content = getText("SetTaskActive_Language"), Mode = MenuItemModeEnum.ActiveTask });
            items.Add(new MenuItem() { Content = getText("SetTaskDeActive_Language"), Mode = MenuItemModeEnum.DeActiveTask });
            items.Add(new MenuItem() { Content = getText("Delete_Language"), Mode = MenuItemModeEnum.Delete });

            return items;
        }

        public static void TaskInfoClickMenuItem(MenuItem menuItem, TaskInfo info, Activity activity)
        {
            try
            {
                if (menuItem.Mode == MenuItemModeEnum.ActiveTask)
                {
                    ApplicationTaskManager.Current.ActiveTask(info);
                    if (currentTaskInfoesManager.getViewByTaskInfo.ContainsKey(info))
                        currentTaskInfoesManager.DrawOneTaskInfo(info, currentTaskInfoesManager.getViewByTaskInfo[info]);
                }
                else if (menuItem.Mode == MenuItemModeEnum.DeActiveTask)
                {
                    ApplicationTaskManager.Current.DeActiveTask(info);
                    if (currentTaskInfoesManager.getViewByTaskInfo.ContainsKey(info))
                        currentTaskInfoesManager.DrawOneTaskInfo(info, currentTaskInfoesManager.getViewByTaskInfo[info]);
                }
                else if (menuItem.Mode == MenuItemModeEnum.Delete)
                {
                    ApplicationTaskManager.Current.RemoveTask(info);
                }
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "TaskInfoClickMenuItem " + (menuItem == null ? "null" : menuItem.Mode.ToString()));
            }
        }

        public void Dispose()
        {
            try
            {
                _isDispose = true;
                ApplicationTaskManager.Current.TaskInfoes.CollectionChanged -= TaskInfoes_CollectionChanged;
                foreach (var item in getViewByTaskInfo)
                {
                    item.Value.Dispose();
                    BindingHelper.DisposeObject(item.Key);
                }
                getViewByTaskInfo.Clear();
                _mainLayoutListView.RemoveAllViews();
                lastChnages = null;
                CurrentActivity = null;
                _mainLayoutListView.Dispose();
                _mainLayoutListView = null;

                _mainLayout.Dispose();
                _CustomListView.Dispose();
                _mainLayout = null;
                _CustomListView = null;

                cal = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "TaskInfoesManager Dispose");
            }
        }
    }
}