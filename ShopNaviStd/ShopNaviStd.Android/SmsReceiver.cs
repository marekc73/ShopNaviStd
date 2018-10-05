using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using ShopNavi.Data;

namespace ShopNavi.Droid
{
    public class SmsReceiver :BroadcastReceiver
    {
        public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED";

        public override void OnReceive(Context context, Intent intent)
        {
            InvokeAbortBroadcast();

            if (intent.HasExtra("pdus"))
            {

                Intent serviceStart = new Intent(context, typeof(MainActivity));
                serviceStart.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(serviceStart);

                var smsArray = (Java.Lang.Object[])intent.Extras.Get("pdus");
                string address = "";

                foreach (var item in smsArray)
                {
                    var sms = SmsMessage.CreateFromPdu((byte[])item);
                    if (address == null) address = sms.OriginatingAddress;

                    if (sms.PseudoSubject.StartsWith(StoreFactory.Settings.SMSKey))
                    {
                        StoreFactory.CurrentVM.InitInput(sms.MessageBody.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                        Toast.MakeText(context, "Received SMS" + sms.PseudoSubject, ToastLength.Short).Show();
                    }
                }
            }
        }
    }
}