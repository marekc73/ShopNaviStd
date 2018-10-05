using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Text;
using Android.Telephony;
using Android.Provider;
using Android.Util;

namespace ShopNavi.Droid
{
    //[BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
    //[IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
    //public class IncomingSms : Android.Support.V4.Content.WakefulBroadcastReceiver
    //{
    //    private const string Tag = "SMSBroadcastReceiver";

    //    public override void OnReceive(Context context, Intent intent)
    //    {
    //        Log.Info(Tag, "Intent received: " + intent.Action);
    //        if (intent.Action != "android.provider.Telephony.SMS_RECEIVED") return;
    //        SmsMessage[] messages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);

    //        var sb = new StringBuilder();
    //        for (var i = 0; i < messages.Length; i++)
    //        {
    //            sb.Append(string.Format("SMS From: {0}{1}Body: {2}{1}", messages[i].OriginatingAddress,
    //                System.Environment.NewLine, messages[i].MessageBody));
    //        }
    //    }
    //}
}