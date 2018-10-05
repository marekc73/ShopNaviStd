using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class StoreEditPage : StoreContentPage
    {
        public StoreEditPage()
        {
            InitializeComponent();
        }

        public StoreEditPage(BaseVM vm)
        {
            this.BindingContext = vm;
            vm.Swipe = PanOperation.All;
            PanDecorator.ResetLastSelected();

            InitializeComponent();
        }

        public override void OnMoveStart(object sender, EventArgs e)
        {
            base.OnMoveStart(sender, e);
            this.InitializeComponent();
        }

        public void OnMoveFinished(object sender, EventArgs e)
        {
            PanContainer.MoveEventArgs ea = e as PanContainer.MoveEventArgs;
            Section line = sender as Section;
            StoreFactory.HalProxy.MakeToast(ShopNavi.Resources.TextResource.DeletingLine);
            this.OrderVM.Order.MoveOutputLine(line, line.Index + ea.Index, SwipeType.Down);
        }

        public void OnDeleteFinished(object sender, EventArgs e)
        {
            Section line = sender as Section;
            StoreFactory.HalProxy.MakeToast(ShopNavi.Resources.TextResource.DeletingLine);
            line.DeleteCmd.Execute(sender);
        }


        //void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        //{
        //    //StoreFactory.HalProxy.HandlePanUpdated(new GestureData()
        //    //{
        //    //    status = e.StatusType,
        //    //    totalX = e.TotalX,
        //    //    totalY = e.TotalY,
        //    //    sender = sender
        //    //});
        //}

    }
}
