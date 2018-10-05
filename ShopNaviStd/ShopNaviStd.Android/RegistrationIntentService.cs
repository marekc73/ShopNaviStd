using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using ShopNavi.Data;
using Android.Gms.Iid;

namespace ClientApp
{
    [Service(Exported = false)]
    class RegistrationIntentService : IntentService
    {
        static object locker = new object();

        public RegistrationIntentService() : base("RegistrationIntentService") { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(
                        MessageSender.SenderID, GoogleCloudMessaging.InstanceIdScope, null);

                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
                    StoreFactory.CurrentVM.Logs.Add("GCM Registration Token: " + token);

                    SendRegistrationToAppServer(token);
                    Subscribe(token);
                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                return;
            }
        }

        void SendRegistrationToAppServer(string token)
        {
            StoreFactory.CurrentVM.Logs.Add("Empty app server");
        }

        void Subscribe(string token)
        {
            string topic = "/topics/shopList_" + StoreFactory.Settings.GetAuthor();
            //string topic = "/topics/global";
            StoreFactory.CurrentVM.Logs.Add("Subscribing to GCM " + topic);
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token,  topic, null);
        }
    }
}