using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class PanLabel : Label
    {

        public PanLabel()
        {
            this.Decorator = new PanDecorator(null,this);
            this.Decorator.AllowedOperation = this.AllowedOperation;
        }

        PanContainer GetParentContainer()
        {
            return this.Decorator.GetParentContainer();

        }
        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Decorator.OnPanUpdated(sender, e);
        }

        public static readonly BindableProperty DecoratorProperty = BindableProperty.Create<PanLabel, PanDecorator>(p => p.Decorator, null, BindingMode.TwoWay);

        public PanDecorator Decorator
        {
            get { return (PanDecorator)GetValue(DecoratorProperty); }
            set { SetValue(DecoratorProperty, value); }
        }

        //public static readonly BindableProperty AllowedOperationProperty = BindableProperty.Create<PanContainer, PanOperation>(p => p.AllowedOperation, PanOperation.Delete, BindingMode.TwoWay);
             //BindableProperty.Create(nameof(AllowedOperation), typeof(PanOperation), typeof(PanContainer), PanOperation.Delete, Xamarin.Forms.BindingMode.OneWay);
        public PanOperation AllowedOperation
        {
            get { return this.Decorator.AllowedOperation; }// (PanOperation)GetValue(AllowedOperationProperty); }
            set
            {
                //SetValue(AllowedOperationProperty, value);
                this.Decorator.AllowedOperation = value;
            }
        }

    }

}
