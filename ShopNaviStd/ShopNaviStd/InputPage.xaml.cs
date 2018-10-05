using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class InputPage : InputContentPage
    {
        public InputPage()
        {
            InitializeComponent();
            var vm = new CommonVM();
            StoreFactory.CurrentVM = vm;
            StoreFactory.CurrentVM.Navi = this.Navigation;
        }

        public InputPage(CommonVM vm)
        {
            this.BindingContext = vm;
            InitializeComponent();
            
            vm.Navi = this.Navigation;
            vm.ShowAlert = this.ShowAlert;
            PanDecorator.ResetLastSelected();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            CommonVM vm = this.BindingContext as CommonVM;
        }

        async public Task<bool> AskAlertAsync(string message)
        {
            return await DisplayAlert("alert", message, "ok", "cancelled");
        }

        public void OnDeleteFinished(object sender, EventArgs e)
        {
            Item line = sender as Item;
            StoreFactory.HalProxy.MakeToast(ShopNavi.Resources.TextResource.DeletingLine);
            line.DeleteCmd.Execute(sender);
        }
        public bool AskAlert(string message)
        {
            return AskAlertAsync(message).Result;
        }

        public void ShowAlert(string message)
        {
            DisplayAlert("alert", message, "ok");
        }
    }
}
