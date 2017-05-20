
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;

namespace Agrin.Views.Controls
{
//OnTouchLinearLayout
    public class DateTimeSelectorLayout : LinearLayout
    {
        LinearLayout _mainL = null;
        TextView _txtValue = null, _txtBack, _txtNext;
        //GestureDetector lis = null;
        protected override void OnFinishInflate()
        {
            LoadControl();
            base.OnFinishInflate();
        }

        public void Initialize()
        {
            _mainL = this.FindViewById<LinearLayout>(Resource.PlayQueueDateTimeSelector.mainLayout);

            //lis = new GestureDetector(new GestureListener()
            //{
            //    OnSwipeTopAction = () => { Value++; },
            //    OnSwipeBottomAction = () => { Value--; }
            //});
            this.Touch -= OnTouchLinearLayout_Touch;
            this.Touch += OnTouchLinearLayout_Touch;
        }

        bool loadedControls = false;
        public void LoadControl()
        {
            if (loadedControls)
                return;
            //var numericUpDown = this.FindViewById<View>(Resource..numericUpDown);
            //if (numericUpDown == null)
            //    return;
            loadedControls = true;
            _txtBack = this.FindViewById<TextView>(Resource.CustomNumericUpDown.txtBack);
            _txtValue = this.FindViewById<TextView>(Resource.CustomNumericUpDown.txtValue);
            _txtNext = this.FindViewById<TextView>(Resource.CustomNumericUpDown.txtNext);
           
            this.Touch -= OnTouchLinearLayout_Touch;
            this.Touch += OnTouchLinearLayout_Touch;
        }

        public DateTimeSelectorLayout(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            Initialize();
        }

        public DateTimeSelectorLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return true;
        }

        int _Value = 0;

        public int Value
        {
            get { return _Value; }
            set
            {
                _Value = GetValue(value);
                _txtValue.Text = _Value.ToString();
                _txtBack.Text = GetValue(_Value - 1).ToString();
                _txtNext.Text = GetValue(_Value + 1).ToString();
            }
        }

        int _Minimum = 0;

        public int Minimum
        {
            get { return _Minimum; }
            set
            {
                _Minimum = value;
                var newValue = GetValue(Value);
                if (newValue != Value)
                    Value = newValue;
            }
        }

        int _Maximum = 24;

        public int Maximum
        {
            get { return _Maximum; }
            set
            {
                _Maximum = value;
                var newValue = GetValue(Value);
                if (newValue != Value)
                    Value = newValue;
            }
        }

        public int GetValue(int value)
        {
            if (value > Maximum)
                return Minimum;
            else if (value < Minimum)
                return Maximum;
            return value;
        }

        //private static int SWIPE_THRESHOLD = 100;
        //private static int SWIPE_VELOCITY_THRESHOLD = 100;

        //private VelocityTracker mVelocityTracker = null;
        int oldX = -1, oldY = -1;
        void OnTouchLinearLayout_Touch(object sender, View.TouchEventArgs e)
        {
            try
            {
                var ev = e.Event;
                int index = ev.ActionIndex;
                var action = ev.ActionMasked;
                int pointerId = ev.GetPointerId(index);
                //switch (action)
                //{
                //    case MotionEventActions.Down:
                //        if (mVelocityTracker == null)
                //        {

                //            // Retrieve a new VelocityTracker object to watch the velocity
                //            // of a motion.
                //            mVelocityTracker = VelocityTracker.Obtain();
                //        }
                //        else
                //        {


                //        }

                //        // Add a user's movement to the tracker.
                //        mVelocityTracker.AddMovement(ev);
                //        break;
                //    case MotionEventActions.Move:
                //        mVelocityTracker.AddMovement(ev);
                //        // When you want to determine the velocity, call
                //        // computeCurrentVelocity(). Then call getXVelocity()
                //        // and getYVelocity() to retrieve the velocity for each pointer ID.
                //        mVelocityTracker.ComputeCurrentVelocity(5);

                //        // Log velocity of pixels per second
                //        // Best practice to use VelocityTrackerCompat where possible.
                //        //_txtValue.Text="X velocity: "
                //        //        + VelocityTrackerCompat.getXVelocity(mVelocityTracker,
                //        //                pointerId)
                //        //        + "\nY velocity: "
                //        //        + VelocityTrackerCompat.getYVelocity(mVelocityTracker,
                //        //                pointerId));
                //        break;
                //    case MotionEventActions.Up:
                //        {
                //            // Reset the velocity tracker back to its initial state.
                //            mVelocityTracker.Clear();
                //        }
                //        break;
                //    case MotionEventActions.Cancel:
                //        // Return a VelocityTracker object back to be re-used by others.
                //        mVelocityTracker.Recycle();
                //        break;
                //}

                if (oldX == -1 || oldY == -1)
                {
                    oldX = (int)ev.GetX();
                    oldY = (int)ev.GetY();
                    return;
                }


                int x = (int)ev.GetX(), y = (int)ev.GetY();
                //float diffY = y - oldY;
                if (action == MotionEventActions.Move)
                {
                    if (y != oldY)
                    {
                        //var speed = (int)mVelocityTracker.GetYVelocity(pointerId);
                        if (y < oldY)
                        {
                            if (oldY - y > 20)
                                Value++;
                            //Value += speed;
                            else
                                return;
                        }
                        else
                        {
                            if (y - oldY > 20)
                                Value--;
                            //Value += speed;
                            else
                                return;
                        }

                    }
                }
            }
            catch (Exception exception)
            {

            }
            oldX = (int)e.Event.GetX();
            oldY = (int)e.Event.GetY();
        }
    }
}