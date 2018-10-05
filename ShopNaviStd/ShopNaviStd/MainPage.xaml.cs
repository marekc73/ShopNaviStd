using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage(IHal proxy)
        {
            StoreFactory.HalProxy = proxy;
            proxy.WriteLog(LogSeverity.Info, "Creating main window");
            var vm = new MainVM(this);
            proxy.WriteLog(LogSeverity.Info, "Binding context");
            this.BindingContext = vm;
            proxy.WriteLog(LogSeverity.Info, "Initialize component");
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                proxy.WriteLog(LogSeverity.Error, "Init component: " + ex.Message);
            }

            proxy.WriteLog(LogSeverity.Info, "Initializing navigation");            
            StoreFactory.CurrentVM.Navi = this.Navigation;
            vm.Navi = this.Navigation;
            proxy.WriteLog(LogSeverity.Info, "Init pages");
            vm.InitPages();
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as PageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.Type));
               // masterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}
