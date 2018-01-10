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

namespace Agrin.Views.Controls
{
    public class GestureListener : GestureDetector.SimpleOnGestureListener
    {
        public Action OnSwipeRightAction { get; set; }
        public Action OnSwipeLeftAction { get; set; }
        public Action OnSwipeTopAction { get; set; }
        public Action OnSwipeBottomAction { get; set; }

        private static int SWIPE_THRESHOLD = 100;
        private static int SWIPE_VELOCITY_THRESHOLD = 100;

        public override bool OnDown(MotionEvent e)
        {
            return true;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            bool result = false;
            try
            {
                float diffY = e2.GetY() - e1.GetY();
                float diffX = e2.GetX() - e1.GetX();
                if (Math.Abs(diffX) > Math.Abs(diffY))
                {
                    if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                    {
                        if (diffX > 0)
                        {
                            OnSwipeRight();
                        }
                        else
                        {
                            OnSwipeLeft();
                        }
                    }
                    result = false;
                }
                else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
                {
                    if (diffY > 0)
                    {
                        OnSwipeBottom();
                    }
                    else
                    {
                        OnSwipeTop();
                    }
                }
                result = false;
            }
            catch (Exception exception)
            {

            }
            return result;
        }

        public void OnSwipeRight()
        {
            if (OnSwipeRightAction != null)
                OnSwipeRightAction();
        }

        public void OnSwipeLeft()
        {
            if (OnSwipeLeftAction != null)
                OnSwipeLeftAction();
        }

        public void OnSwipeTop()
        {
            if (OnSwipeTopAction != null)
                OnSwipeTopAction();
        }

        public void OnSwipeBottom()
        {
            if (OnSwipeBottomAction != null)
                OnSwipeBottomAction();
        }
    }
}