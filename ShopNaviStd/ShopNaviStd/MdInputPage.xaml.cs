using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ShopNavi
{
    public partial class MdInputPage : MasterDetailPage
    {
        public MdInputPage()
        {
            InitializeComponent();
        }

        public void OnStoreClick(object sender, EventArgs e)
        {
            ChangeDetail(new ShopListPage() { BindingContext = (this.BindingContext as CommonVM).StoreList });
        }

        private void ChangeDetail(Page page)
        {
            var navigationPage = Detail as NavigationPage;
            if (navigationPage != null)
            {
                navigationPage.PushAsync(page);
                return;
            }
            Detail = new NavigationPage(page);
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //var item = e.SelectedItem as MasterPageItem;
            //if (item != null)
            //{
            //    Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
            //    masterPage.ListView.SelectedItem = null;
            //    IsPresented = false;
            //}
        }

    }
}
