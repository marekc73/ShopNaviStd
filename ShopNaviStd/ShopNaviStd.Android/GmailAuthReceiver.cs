using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ShopNavi.Data;
using Xamarin.Auth;

namespace ShopNavi.Droid
{
    public class GmailAuthReceiver
         : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            InvokeAbortBroadcast();

            // Convert Android.Net.Url to Uri
            if (intent.Data.ToString().Contains(StoreFactory.Settings.GmailClientId))
            {
                var uri = new Uri(intent.Data.ToString());

                // Load redirectUrl page
                ((OAuth2Authenticator)AuthenticationState.Authenticator).OnPageLoading(uri);

            }
        }
    
    }
}