using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ShopNavi
{
    public partial class LogsPage : ContentPage
    {
        public LogsPage()
        {
            InitializeComponent();
        }

        public LogsPage(CommonVM vm)
        {
            this.BindingContext = vm;
            InitializeComponent();
        }
    }
}
