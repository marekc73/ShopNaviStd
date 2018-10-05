using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ShopNavi.Data;

namespace ShopNavi.Droid
{
    [Activity(Label = "CustomUrlSchemeInterceptorActivity", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(actions: new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, 
        DataSchemes = new[] {
            "com.googleusercontent.apps."  + Constants.AndroidClientId }, 
        DataPaths = new[] { "/oauth2redirect" }, 
        AutoVerify = true
        )]
    public class CustomUrlSchemeInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Convert Android.Net.Url to Uri
            if (Intent.Data.ToString().Contains(StoreFactory.Settings.GmailClientId))
            {
                var uri = new Uri(Intent.Data.ToString());

                // Load redirectUrl page
                AuthenticationState.Authenticator.OnPageLoading(uri);

                Finish();
            }
        }
    }
}