using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ShopNavi
{
    public partial class ItemListPage : ContentPage
    {
        public ItemListPage()
        {
            InitializeComponent();
        }

        public ItemListPage(CreateListVM vm)
        {
            vm.PoolItem = vm.AllItems.DefaultIfEmpty(vm.AllItems.First()).FirstOrDefault(x => x.Name.StartsWith(vm.NewItem.ToString(),StringComparison.CurrentCultureIgnoreCase));
            
            this.BindingContext = vm;
            InitializeComponent();
        }
    }
}
