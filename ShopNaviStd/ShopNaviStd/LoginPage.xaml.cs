using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ShopNavi.Data;
using Newtonsoft.Json;
using System.Diagnostics;
using ShopNavi.Resources;

namespace ShopNavi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
//            vm.Login = StoreFactory.HalProxy.GetLogin(null);
        }

        public LoginPage(BaseVM vm)
        {
            InitializeComponent();
            CreateListVM cl = vm as CreateListVM;
            cl.Login = StoreFactory.HalProxy.GetLogin(cl);
            try
            {
                this.BindingContext = vm;
            }
            catch (Exception ex)
            {
                StoreFactory.CurrentVM.Logs.Add(ex.Message);
            }
        }       
    }
}