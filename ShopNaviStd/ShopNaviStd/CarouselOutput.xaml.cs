using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopNavi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarouselOutput : OutputContentPage
    {
        public CarouselOutput()
        {
            InitializeComponent();
        }
        public CarouselOutput(CommonVM vm)
        {
            this.BindingContext = vm;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.outputCarousel.HeightRequest = 300;
        }

    }
}