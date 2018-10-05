using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class SectionListPage : ContentPage
    {
        public SectionListPage()
        {
            InitializeComponent();
        }

        public SectionListPage(BaseVM vm, bool edit)
        {
            this.BindingContext = vm;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (StoreFactory.CurrentStore.LastSelectedSection != null)
            {
                sectionList.ScrollTo(StoreFactory.CurrentStore.LastSelectedSection, ScrollToPosition.Start, true);
            }
        }
        public void OnMoveConfirm(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
        }

        private void sectionList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Item it = BindingContext as Item;
            if(it != null && it.IsAutoOk)
            {
                StoreFactory.CurrentStore.LastSelectedSection = it.Link;
                it.OkCmd.Execute(it.Link);
            }
        }

        //void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        //{
        //    StoreFactory.HalProxy.HandlePanUpdated(new GestureData()
        //    {
        //        status = e.StatusType,
        //        totalX = e.TotalX,
        //        totalY = e.TotalY,
        //        sender = sender
        //    });
        //}

    }
}
