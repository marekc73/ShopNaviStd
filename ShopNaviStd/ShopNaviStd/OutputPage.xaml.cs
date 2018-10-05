using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class OutputPage : OutputContentPage
    {
        public OutputPage()
        {
            InitializeComponent();
        }

        public OutputPage(CommonVM vm)
        {
            this.BindingContext = vm;
            PanDecorator.ResetLastSelected();

            InitializeComponent();
        }

        private void OnOffTapped(object sender, EventArgs e)
        {
        }

        private void OnSwipeRight(object sender, EventArgs e)
        {
        }

        private void OnSwipeLeft(object sender, EventArgs e)
        {
        }

        //void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        //{
        //    StoreFactory.CurrentVM.Logs.Add(string.Format("Gesture {0}; status {1}", e.GestureId, e.StatusType.ToString()));
        //    StoreFactory.HalProxy.HandlePanUpdated(new GestureData()
        //    {
        //        status = e.StatusType,
        //        totalX = e.TotalX,
        //        totalY = e.TotalY,
        //        sender = sender
        //    });
        //}

        public override void OnMoveStart(object sender, EventArgs e)
        {
            base.OnMoveStart(sender, e);
            this.InitializeComponent();
        }

        public void OnDeleteFinished(object sender, EventArgs e)
        {
            OutputLine line = sender as OutputLine;
            StoreFactory.HalProxy.MakeToast(ShopNavi.Resources.TextResource.DeletingLine);
            line.DeleteCmd.Execute(sender);
        }

    }
}
