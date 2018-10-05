using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class PanContainer : ContentView
    {
        PanDecorator decorator;

        public class MoveEventArgs : EventArgs
        {
            public MoveEventArgs(EventArgs e, int index)
            {
                this.Index = index;                
            }

            public int Index { get; set; }
        }

        public PanContainer()
        {
            this.Decorator = new PanDecorator(this,this);
            this.Decorator.AllowedOperation = this.AllowedOperation;
            this.Decorator.SaveMargins();
        }

        public event EventHandler OnDeleteFinished;
        public event EventHandler OnMoveFinished;

        public static readonly BindableProperty LineProperty = BindableProperty.Create<PanContainer, OrderLineVM>(p => p.Line, null, BindingMode.TwoWay, propertyChanged: OnLineChanged);

        public OrderLineVM Line
        {
            get { return (OrderLineVM)GetValue(LineProperty); }
            set
            {
                SetValue(LineProperty, value);
            }
        }

        //public static readonly BindableProperty AllowedOperationProperty = BindableProperty.Create<PanContainer, PanOperation>(p => p.AllowedOperation, PanOperation.Delete, BindingMode.TwoWay);
        //BindableProperty.Create(nameof(AllowedOperation), typeof(PanOperation), typeof(PanContainer), PanOperation.Delete, Xamarin.Forms.BindingMode.OneWay);
        public PanOperation AllowedOperation
        {
            get { return this.Decorator.AllowedOperation; }
            set
            {
                //SetValue(AllowedOperationProperty, value);
                this.Decorator.AllowedOperation = value;
            }
        }

        private static void OnLineChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var cell = (PanContainer)bindable;
            cell.Line = (OrderLineVM)newValue;
        }

        public void RaisePanFinished(PanOperation oper, EventArgs e)
        {
            if (oper == PanOperation.Delete)
            {
                StoreFactory.HalProxy.RunOnUiThread(() => this.OnDeleteFinished?.Invoke(this.Line, null));
            }
            else
            {
                StoreFactory.HalProxy.RunOnUiThread(() => this.OnMoveFinished?.Invoke(this.Line, e));
            }
        }

        PanContainer GetParentContainer()
        {
            return this.Decorator.GetParentContainer();

        }
        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Decorator.OnPanUpdated(sender, e);

            this.RaiseChild((View)sender);
        }

        public static readonly BindableProperty DecoratorProperty = BindableProperty.Create<PanContainer, PanDecorator>(p => p.Decorator, null, BindingMode.TwoWay);

        public PanDecorator Decorator
        {
            get { return (PanDecorator)GetValue(DecoratorProperty); }
            set { SetValue(DecoratorProperty, value); }
        }


    }
}
