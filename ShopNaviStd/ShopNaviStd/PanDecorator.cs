using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class PanDecorator : IPanDecorator
    {
        DateTime start;
        Layout parentLayout;
        double endX, endY;
        Color background;
        const int SwipeLength = 60;
        private double marginLeft = 0;
        private double marginRight = 0;
        private double marginTop = 0;
        private double marginBottom = 0;
        ManualResetEvent finished = new ManualResetEvent(false);
        private View view;
        PanOperation operation = PanOperation.None;
        static View lastSelected = null;
        static Color lastSelectedColor;

        public static void ResetLastSelected()
        {
            lastSelected = null;
        }
        public PanOperation AllowedOperation
        {
            get; set;
        }

        public double MarginLeft
        {
            get
            {
                return this.marginLeft;
            }

            set
            {
                this.marginLeft = value;
            }
        }

        public double MarginRight
        {
            get
            {
                return this.marginRight;
            }

            set
            {
                this.marginRight = value;
            }
        }
        public double MarginTop
        {
            get
            {
                return this.marginTop;
            }

            set
            {
                this.marginTop = value;
            }
        }

        public double MarginBottom
        {
            get
            {
                return this.marginBottom;
            }

            set
            {
                this.marginBottom = value;
            }
        }


        public PanDecorator(Layout layout, View view)
        {
            this.view = view;
            this.parentLayout = layout;
            this.SetGestureRecognizers(view);
        }

        private void SetGestureRecognizers(View view)
        {
            this.view = view;
            //this.view.GestureRecognizers.Clear();
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            this.view.GestureRecognizers.Add(panGesture);
            this.view.GestureRecognizers.Add(new TapGestureRecognizer(this.OnTapped));
        }

        public void OnTapped(View arg1, object arg2)
        {
            this.HandleTap(true);
        }

        private void HandleTap(bool setColor)
        {

            bool isBaseContainer = this.view is PanContainer;


            if (!isBaseContainer)
            {
                this.GetParentContainer()?.Decorator?.HandleTap(setColor);
            }
            else
            {
                if (lastSelected != null)
                {
                    lastSelected.BackgroundColor = lastSelectedColor;
                }

                lastSelected = this.view;
                lastSelectedColor = this.view.BackgroundColor;
                if (setColor)
                {
                    this.view.BackgroundColor = Color.DarkOrange;
                }
            }
        }

        public PanContainer GetParentContainer()
        {
            Element parent = this.view;
            while (parent != null && !(parent is PanContainer))
            {
                parent = parent.Parent;
            }

            return parent as PanContainer;

        }
        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    this.GetParentContainer()?.Decorator?.StartSwipe(e);
                    break;
                case GestureStatus.Running:
                    this.GetParentContainer()?.Decorator?.RunSwipe(e);
                    break;
                case GestureStatus.Completed:
                    this.GetParentContainer()?.Decorator?.FinishSwipe();
                    break;
                case GestureStatus.Canceled:
                    this.GetParentContainer()?.Decorator?.CancelDelete(e);
                    this.GetParentContainer()?.Decorator?.CancelMove(e);
                    break;
            }

        }

        private void StartSwipe(PanUpdatedEventArgs e)
        {
            this.HandleTap(false);
            this.start = DateTime.Now;
            this.operation = PanOperation.None;
            this.finished.Reset();
            Task.Run(new Action(() => CompleteSwipeAsync(this, 1000, e)));

            this.background = this.view.BackgroundColor;
            this.view.BackgroundColor = Color.Orange;


            Debug.WriteLine("Starting swipe from container");
        }

        public void SaveMargins()
        {
            this.MarginLeft = this.parentLayout.Padding.Left;
            this.MarginRight = this.parentLayout.Padding.Right;
            this.MarginTop = this.parentLayout.Padding.Top;
            this.MarginBottom = this.parentLayout.Padding.Bottom;
        }

        private void RunSwipe(PanUpdatedEventArgs e)
        {
            if (this.parentLayout.Padding.Top == this.MarginTop && DateTime.Now.Subtract(this.start).TotalSeconds > 1)
            {
                this.operation = PanOperation.Move;
            }

            if (this.AllowedOperation == PanOperation.Delete && this.parentLayout.Padding.Left <= this.MarginLeft + e.TotalX)
            {
                StoreFactory.HalProxy.RunOnUiThread(() => SetPadding(this, e.TotalX, 0));

                Debug.WriteLine("new padding1 " + this.parentLayout.Padding.Left);
            }
            else if (this.AllowedOperation == PanOperation.Move && this.parentLayout.Padding.Top <= this.MarginTop + e.TotalY)
            {
                Debug.WriteLine("new padding2 " + this.parentLayout.Padding.ToString());
                StoreFactory.HalProxy.RunOnUiThread(() => SetPadding(this, 0, e.TotalY));
            }
            else if (this.AllowedOperation == PanOperation.All && (this.parentLayout.Padding.Left <= this.MarginLeft + e.TotalX || this.parentLayout.Padding.Top <= this.MarginTop + e.TotalY))
            {
                Debug.WriteLine("new padding3 " + this.parentLayout.Padding.ToString());
                StoreFactory.HalProxy.RunOnUiThread(() => SetPadding(this, e.TotalX, e.TotalY));
            }

            Debug.WriteLine("Left margin = " + this.parentLayout.Padding.Left);
        }

        private void FinishSwipe()
        {
            this.finished.Set();

        }
        private void CancelMove(PanUpdatedEventArgs e)
        {
        }

        public static async void CompleteSwipeAsync(PanDecorator decorator, int timeout, PanUpdatedEventArgs e)
        {
            if (decorator.finished.WaitOne(timeout))
            {
                Debug.WriteLine("Got status completed");
            }
            else
            {
                Debug.WriteLine("Swipe timeout");
            }

            StoreFactory.HalProxy.RunOnUiThread(() => decorator.CompleteSwipe(e));

        }
        public void CompleteSwipe(PanUpdatedEventArgs e)
        {
            this.view.BackgroundColor = background;
            bool isBaseContainer = this.view is PanContainer;

            if (this.parentLayout.Padding.Left - this.MarginLeft > SwipeLength)
            {
                Debug.WriteLine("Going to delete");

                this.GetParentContainer()?.RaisePanFinished(PanOperation.Delete, e);
            }
            else if (this.parentLayout.Padding.Top - this.MarginTop > this.view.Height && this.operation == PanOperation.Move)
            {
                Debug.WriteLine("Going to move");

                this.GetParentContainer()?.RaisePanFinished(PanOperation.Move, new PanContainer.MoveEventArgs(e, (int)((this.parentLayout.Padding.Top - this.MarginTop) / this.view.Height)));
            }
            else 
            {
                this.CancelDelete(e);
                this.operation = PanOperation.None;

            }
        }

        private void CancelDelete(PanUpdatedEventArgs e)
        {
            Debug.WriteLine("Cancelling delete");

            Debug.WriteLine("Restore color and margin to " + this.MarginLeft + " component : " + this.view.GetType().ToString());
            this.RestoreUi();
            this.operation = PanOperation.None;

        }

        private static void SetPadding(PanDecorator decorator, double x, double y)
        {
            decorator.parentLayout.Padding = new Thickness(decorator.MarginLeft + x, decorator.MarginTop + y, decorator.MarginRight - x, decorator.MarginBottom - y);
        }

        private void RestoreUi()
        {
            this.parentLayout.Padding = new Thickness(this.MarginLeft, this.MarginTop, this.MarginRight, this.MarginBottom);
            this.view.BackgroundColor = this.background;
            StoreFactory.HalProxy.MakeToast(ShopNavi.Resources.TextResource.SwipeDelete);
        }
    }
}
