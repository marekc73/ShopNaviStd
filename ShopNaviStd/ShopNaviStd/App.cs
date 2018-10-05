using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class AppOld : Application
    {
        public AppOld(IHal proxy)
        {

            MainPage = new MainPage(proxy)
            {
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
