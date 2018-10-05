using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class ShopListPage : ContentPage
    {
        public ShopListPage()
        {
            InitializeComponent();
        }

        public ShopListPage(CommonVM vm)
        {
            this.BindingContext = vm;
            foreach (var store in vm.StoreList)
            {
                store.AssignParent(vm);
            }
            vm.PendingStore = vm.CurrentStore;

            InitializeComponent();
            this.storeList.SelectedItem = vm.PendingStore;
        }
    }
}
