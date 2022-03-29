using Agrin.Download.Engine.Interfaces;
using Agrin.Download.Web.Tasks;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;

namespace Agrin.Download.Web
{
    [Serializable]
    public class TaskInfo : ANotifyPropertyChanged
    {
        int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        bool _IsMainTask = false;

        public bool IsMainTask
        {
            get { return _IsMainTask; }
            set { _IsMainTask = value; }
        }

        bool _IsActive = false;
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                //set rise event in task manager
                OnPropertyChanged("IsActive");
            }
        }

        /// <summary>
        /// شروع حالا
        /// </summary>
        bool _IsStartNow = false;
        public bool IsStartNow
        {
            get { return _IsStartNow; }
            set { _IsStartNow = value; }
        }

        /// <summary>
        /// یک روز یا چند روز از روز های هفته مثل شنبه، یکشنبه، جمعه
        /// </summary>
        bool _isDayOfWeek = false;
        public bool IsDayOfWeek
        {
            get { return _isDayOfWeek; }
            set { _isDayOfWeek = value; }
        }

        /// <summary>
        /// کاربر میتواند زمانی را بعد از ثبت تسک انتخاب کند.مثلاً ده دقیقه دیگر یا 1 روز و 4 ساعت دیگر...در کادر متن بتواند وارد کند.
        /// </summary>
        bool _IsTimeAndDays = false;
        public bool IsTimeAndDays
        {
            get { return _IsTimeAndDays; }
            set { _IsTimeAndDays = value; }
        }

        int _TryErrorCount = 10;
        public int TryErrorCount
        {
            get { return _TryErrorCount; }
            set { _TryErrorCount = value; }
        }

        bool _IsTryExterme = false;
        public bool IsTryExterme
        {
            get { return _IsTryExterme; }
            set { _IsTryExterme = value; }
        }

        DateTime _TimeAndDays;
        public DateTime TimeAndDays
        {
            get { return _TimeAndDays; }
            set { _TimeAndDays = value; }
        }

        List<TaskItemInfo> _TaskItemInfoes = new List<TaskItemInfo>();
        public List<TaskItemInfo> TaskItemInfoes
        {
            get { return _TaskItemInfoes; }
            set { _TaskItemInfoes = value; }
        }

        //List<int> _TaskIds = new List<int>();//برای ایست یا استارت کردن تسک ها
        //public List<int> TaskIds
        //{
        //    get { return _TaskIds; }
        //    set { _TaskIds = value; }
        //}

        List<DateTime> _DateTimes = new List<DateTime>();
        public List<DateTime> DateTimes
        {
            get { return _DateTimes; }
            set { _DateTimes = value; }
        }

        List<DayOfWeek> _DayOfWeek = new List<DayOfWeek>();
        public List<DayOfWeek> DayOfWeek
        {
            get { return _DayOfWeek; }
            set { _DayOfWeek = value; }
        }

        List<TaskModeEnum> _TaskMode = new List<TaskModeEnum>() { TaskModeEnum.Download };
        public List<TaskModeEnum> TaskMode
        {
            get { return _TaskMode; }
            set { _TaskMode = value; }
        }

        /// <summary>
        /// start mode
        /// </summary>
        List<TaskUtilityModeEnum> _TaskUtilityActions = new List<TaskUtilityModeEnum>();
        public List<TaskUtilityModeEnum> TaskUtilityActions
        {
            get { return _TaskUtilityActions; }
            set { _TaskUtilityActions = value; }
        }

        /// <summary>
        /// if mode is download and completed
        /// </summary>
        List<TaskUtilityModeEnum> _LinkCompleteTaskUtilityActions = new List<TaskUtilityModeEnum>();
        public List<TaskUtilityModeEnum> LinkCompleteTaskUtilityActions
        {
            get { return _LinkCompleteTaskUtilityActions; }
            set { _LinkCompleteTaskUtilityActions = value; }
        }


        public event Action<TaskInfo> StateChanged;

        TaskState _State = TaskState.Stoped;
        public TaskState State
        {
            get { return _State; }
            set
            {
                _State = value;
                OnPropertyChanged("State");
                if (StateChanged != null)
                    StateChanged(this);
            }
        }

        List<LinkInfo> _SkippedItems = new List<LinkInfo>();
        public List<LinkInfo> SkippedItems
        {
            get { return _SkippedItems; }
            set { _SkippedItems = value; }
        }

        //[NonSerialized]
        //object lockObj = new object();
        //public void Stop()
        //{
        //    lock (lockObj)
        //    {

        //    }
        //}

        //public void Start()
        //{
        //    lock (lockObj)
        //    {

        //    }
        //}
    }
}
