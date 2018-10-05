using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Iid;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ClientApp;
using ShopNavi.Data;

namespace ShopNavi.Droid
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    public class ShopNaviInstanceIdListenerService : InstanceIDListenerService
    {
        public override void OnTokenRefresh()
        {
            StoreFactory.CurrentVM.Logs.Add("OnTokenRefresh");
            var intent = new Intent(this, typeof(RegistrationIntentService));
            StartService(intent);
        }
    }


    [Service(Exported = true, Enabled=true), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class ShopNaviGcmListenerService : GcmListenerService
    {
        public override void OnMessageReceived(string from, Bundle data)
        {
            StoreFactory.CurrentVM.Logs.Add("OnMessageReceived");
            if (from.StartsWith("/topics/shopList_" + StoreFactory.Settings.GetAuthor()))
            {
                List<string> exprList = new List<string>();
                string label = data.GetString("label"); 

                for (int i = 0; i < 1000 && !string.IsNullOrEmpty(data.GetString("line" + i)); i++)
                {
                    var message = data.GetString("line" + i);
                    StoreFactory.CurrentVM.Logs.Add("OnMessageReceived message = " + message);
                    exprList.Add(message);
                }

                StoreFactory.CurrentVM.InitInput(exprList);
                StoreFactory.HalProxy.SendNotification(label, exprList.ToArray());
            }
            else
            {
                StoreFactory.CurrentVM.Logs.Add("Ignoring notification from " + from);
                StoreFactory.HalProxy.SendNotification("Ignoring notification from " + from, new string[]{});
            }
        }
    }
}