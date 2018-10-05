using ShopNavi.Data;
using ShopNavi.UWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ShopNaviStd.UWP
{
    public sealed partial class MainPage
    {
        private UwpHalProxy halProxy;

        public MainPage()
        {
            this.InitializeComponent();
            this.halProxy = new UwpHalProxy(this.DataContext, this.Frame);

            LoadApplication(new ShopNavi.App(this.halProxy));
        }

        public IHal Proxy
        {
            get
            {
                return this.halProxy;
            }
        }
    }
}
