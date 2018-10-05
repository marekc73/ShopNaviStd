using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            try
            {
                this.BindingContext = new SettingsVM(StoreFactory.HalProxy.ReadSettings());
            }
            catch(Exception ex)
            {
                StoreFactory.CurrentVM.Logs.Add(ex.Message);
            }
        }

        public SettingsPage(SettingsBaseVM vm)
        {
            InitializeComponent();
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
