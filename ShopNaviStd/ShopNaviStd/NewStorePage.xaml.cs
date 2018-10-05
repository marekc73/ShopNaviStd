using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class NewStorePage : ContentPage
    {
        public NewStorePage()
        {
            InitializeComponent();
        }

        public NewStorePage(Store vm)
        {
            this.BindingContext = vm;

            InitializeComponent();
        }
    }
}
