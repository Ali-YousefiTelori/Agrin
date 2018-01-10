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
using Android.Util;

namespace Agrin.Views.Controls
{
    public class TochDetectorLayout : LinearLayout
    {
        public Func<bool> OnSwipeRightFunction { get; set; }
        public Func<bool> OnSwipeLeftFunction { get; set; }
        public Func<bool> OnSwipeTopFunction { get; set; }
        public Func<bool> OnSwipeBottomFunction { get; set; }

        protected override void OnFinishInflate()
        {
            LoadControl();
            base.OnFinishInflate();
        }

        public void Initialize()
        {
            if (loadedControls)
                return;
            loadedControls = true;
            this.Touch += OnTouchLinearLayout_Touch;
        }

        bool loadedControls = false;
        public void LoadControl()
        {
            if (loadedControls)
                return;
            loadedControls = true;
            this.Touch += OnTouchLinearLayout_Touch;
        }

        public TochDetectorLayout(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            Initialize();
        }

        public TochDetectorLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
        }

        public TochDetectorLayout(Context context)
            : base(context)
        {
            Initialize();
        }
        public bool IsAnimating = false;
        bool isLock = false;
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (isLock || IsAnimating)
                return true;
            else
            {
                int index = ev.ActionIndex;
                var action = ev.ActionMasked;
                int pointerId = ev.GetPointerId(index);

                float diffY = ev.GetY() - oldY;
                float diffX = ev.GetX() - oldX;
                switch (action)
                {
                    case MotionEventActions.Down:
                        if (mVelocityTracker == null)
                        {
                            OneSwipe = false;
                            // Retrieve a new VelocityTracker object to watch the velocity
                            // of a motion.
                            mVelocityTracker = VelocityTracker.Obtain();
                        }
                        else
                        {


                        }
                        oldX = 0;
                        oldY = 0;
                        downX = (int)ev.GetX();
                        downY = (int)ev.GetY();
                        downLeftPadding = this.PaddingLeft;
                        downRightPadding = this.PaddingRight;
                        // Add a user's movement to the tracker.
                        mVelocityTracker.AddMovement(ev);
                        break;
                    case MotionEventActions.Move:
                        {
                            int x = (int)ev.GetX() - downX;
                            if (Math.Abs(x) > 10)
                            {
                                isLock = true;

                                return true;
                            }
                            break;
                        }
                }
            }
            return false;
        }

        private static int SWIPE_THRESHOLD = 20;
        private static int SWIPE_VELOCITY_THRESHOLD = 100;

        private VelocityTracker mVelocityTracker = null;

        int oldX = 0, oldY = 0;
        int downX = 0, downY = 0;
        int downLeftPadding = 0, downRightPadding = 0;
        bool _oneSwipe = false;

        public bool OneSwipe
        {
            get { return _oneSwipe; }
            set { _oneSwipe = value; }
        }

        void OnTouchLinearLayout_Touch(object sender, View.TouchEventArgs e)
        {
            try
            {
                var ev = e.Event;
                int index = ev.ActionIndex;
                var action = ev.ActionMasked;
                int pointerId = ev.GetPointerId(index);
                if (IsAnimating)
                {
                    if (action == MotionEventActions.Up)
                    {
                        isLock = false;
                        OneSwipe = false;
                        canSetPadding = true;
                    }
                    return;
                }



                int diffY = (int)ev.GetY() - oldY;
                int diffX = (int)ev.GetX() - oldX;
                switch (action)
                {
                    case MotionEventActions.Down:
                        if (mVelocityTracker == null)
                        {
                            OneSwipe = false;
                            // Retrieve a new VelocityTracker object to watch the velocity
                            // of a motion.
                            mVelocityTracker = VelocityTracker.Obtain();
                        }
                        else
                        {


                        }
                        oldX = 0;
                        oldY = 0;
                        downX = (int)ev.GetX();
                        downY = (int)ev.GetY();
                        downLeftPadding = this.PaddingLeft;
                        downRightPadding = this.PaddingRight;
                        // Add a user's movement to the tracker.
                        mVelocityTracker.AddMovement(ev);
                        break;
                    case MotionEventActions.Move:
                        mVelocityTracker.AddMovement(ev);
                        // When you want to determine the velocity, call
                        // computeCurrentVelocity(). Then call getXVelocity()
                        // and getYVelocity() to retrieve the velocity for each pointer ID.
                        mVelocityTracker.ComputeCurrentVelocity(200);

                        // Log velocity of pixels per second
                        // Best practice to use VelocityTrackerCompat where possible.
                        //_txtValue.Text="X velocity: "
                        //        + VelocityTrackerCompat.getXVelocity(mVelocityTracker,
                        //                pointerId)
                        //        + "\nY velocity: "
                        //        + VelocityTrackerCompat.getYVelocity(mVelocityTracker,
                        //                pointerId));
                        break;
                    case MotionEventActions.Up:
                        {
                            mVelocityTracker.AddMovement(ev);
                            mVelocityTracker.ComputeCurrentVelocity(200);
                            // Reset the velocity tracker back to its initial state.
                            
                            isLock = false;
                            OneSwipe = false;
                        }
                        break;
                    case MotionEventActions.Cancel:
                        // Return a VelocityTracker object back to be re-used by others.
                        mVelocityTracker.Recycle();
                        break;
                }
                if (!OneSwipe)
                {
                    //if (Math.Abs(diffX) > Math.Abs(diffY))
                    //{
                    //Math.Abs(diffX) > SWIPE_THRESHOLD &&
                    if (Math.Abs(mVelocityTracker.XVelocity) > SWIPE_VELOCITY_THRESHOLD)
                    {
                        if (oldX != 0 && diffX != 0)
                        {
                            if (Math.Abs(diffX) > Math.Abs(diffY))
                            {
                                if (diffX > 0)
                                {
                                    OneSwipe = true;
                                    OnSwipeRight();
                                }
                                else
                                {
                                    OneSwipe = true;
                                    OnSwipeLeft();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (action !=  MotionEventActions.Up)
                        {

                        }
                        else if (action == MotionEventActions.Move)
                        {

                        }
                    }
                    //}
                    //Math.Abs(diffY) > SWIPE_THRESHOLD &&
                    if (Math.Abs(mVelocityTracker.YVelocity) > SWIPE_VELOCITY_THRESHOLD)
                    {
                        if (Math.Abs(diffX) > Math.Abs(diffY))
                        {
                            if (diffY > 0)
                            {
                                OneSwipe = true;
                                OnSwipeBottom();
                            }
                            else
                            {
                                OneSwipe = true;
                                OnSwipeTop();
                            }
                        }
                    }
                    else
                    {

                    }
                    var xxx = ev.GetX();
                    if (action != MotionEventActions.Up && canSetPadding && !OneSwipe)
                    {
                        int x = (int)ev.GetX() - downX;
                        this.SetPadding(x, this.PaddingTop, -x, this.PaddingBottom);
                    }
                }
                else
                {

                }
                if (action == MotionEventActions.Up)
                {
                    if (canSetPadding && !OneSwipe)
                        this.SetPadding(downLeftPadding, this.PaddingTop, downRightPadding, this.PaddingBottom);
                    else
                        canSetPadding = true;
                    mVelocityTracker.Clear();
                }
            }
            catch (Exception exception)
            {

            }
            oldX = (int)e.Event.GetX();
            oldY = (int)e.Event.GetY();
        }

        public void OnSwipeRight()
        {
            if (OnSwipeRightFunction != null)
            {
                canSetPadding = OnSwipeRightFunction();
                if (canSetPadding)
                {
                    OneSwipe = false;
                }
            }
            else
                OneSwipe = false;
        }

        bool canSetPadding = true;

        public void OnSwipeLeft()
        {
            if (OnSwipeLeftFunction != null)
            {
                canSetPadding = OnSwipeLeftFunction();
                if (canSetPadding)
                {
                    OneSwipe = false;
                }
            }
            else
                OneSwipe = false;
        }

        public void OnSwipeTop()
        {
            if (OnSwipeTopFunction != null)
            {
                canSetPadding = OnSwipeTopFunction();
                if (canSetPadding)
                {
                    OneSwipe = false;
                }
            }
            else
                OneSwipe = false;
        }

        public void OnSwipeBottom()
        {
            if (OnSwipeBottomFunction != null)
            {
                canSetPadding = OnSwipeBottomFunction();
                if (canSetPadding)
                {
                    OneSwipe = false;
                } 
            }
            else
                OneSwipe = false;
        }
    }
}