using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class LocationPage : ContentPage
    {
        public LocationPage()
        {
            InitializeComponent();
        }

        public LocationPage(Item it, Store store)
        {
            InitializeComponent();
            if (store != null)
            {
                this.BindingContext = new LocationVM(it, store)
                    {
                        Navi = StoreFactory.CurrentVM.Navi,
                        Parent = StoreFactory.CurrentVM
                    };
            }
        }
    }
}
