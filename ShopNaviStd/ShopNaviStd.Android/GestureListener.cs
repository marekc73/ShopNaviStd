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

namespace XamarinFormsGestures.Droid.Helpers
{
    public class DroidGestureListener : GestureDetector.SimpleOnGestureListener
    {
        public DroidGestureListener():base()
        {
        }

        private static int SWIPE_THRESHOLD = 100;
        private static int SWIPE_VELOCITY_THRESHOLD = 100;

        public event EventHandler SwipeDown;
        public event EventHandler SwipeUp;
        public event EventHandler SwipeLeft;
        public event EventHandler SwipeRight;

        public override void OnLongPress(MotionEvent e)
        {
            base.OnLongPress(e);
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            float diffY = e2.GetY() - e1.GetY();
            float diffX = e2.GetX() - e1.GetX();

            if (Math.Abs(diffX) > Math.Abs(diffY))
            {
                if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                {
                    if (diffX > 0)
                    {
                        if (SwipeRight != null)
                            SwipeRight(this, null);
                    }
                    else
                    {
                        if (SwipeLeft != null)
                            SwipeLeft(this, null);
                    }
                }
            }
            else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
            {
                if (diffY > 0)
                {
                    if (SwipeDown != null)
                        SwipeDown(this, null);
                }
                else
                {
                    if (SwipeUp != null)
                        SwipeUp(this, null);
                }
            }
            return true;
        }
    }
}