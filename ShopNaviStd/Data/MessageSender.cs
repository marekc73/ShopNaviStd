using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ShopNavi.Data
{
    public class MessageSender
    {
        public const string firePrjKey = "firebase-ShopNavi";
        public const string  fileApiKey = "AIzaSyBaFcpknsR07jEHSNLAkIRmyusflae3C_o";

//        public const string API_KEY = "AAAAkZ-Z0Y4:APA91bE_CdKNQtdG2YGvZrXX5C6WzGEXb78Dbcxa501kI2C8FROcID0LwRoeirSVNiumA85JFDsU3HQDoc-xAYqi-_RCl059PfV1F3lBFLNKB5p0XVblAk6j_OiU9eiKCJHaES8dTKB9cKAdSmgbiyfqo7gDefL90w";
//        public const string SenderID = "625447915918";

        public const string API_KEY = "AAAAi9qhhz0:APA91bGabCpAN07NRBLW8OI3OJtPujnBsvZalIKYcMwgFLTwceLeiSjKaKUIOdSb7oRVbmu8Kk3YWizY_FPsaSQ_fgkvcPIXqU03imv9az7dMFwsA3a_9oAIdnOMNpFFVge1V5BHu3LF";
        public const string SenderID = "600668473149";

        public static async Task<int> SendText(string[] lines, Action<int, ErrorResult> onProgress)
        {
            if(StoreFactory.Settings.FromCloud)
            {
                await SendGcm(lines, onProgress, API_KEY);
            }
            else
            {
                await SendSMS(lines, onProgress);
            }

            return 0;
        }

        private static async Task SendSMS(string[] lines, Action<int, ErrorResult> onProgress)
        {
            await Task.Factory.StartNew(() =>
                {
                    onProgress(100, StoreFactory.HalProxy.SendSMS(StoreFactory.Settings.SMSNumber, StoreFactory.Settings.SMSKey, lines, null));
                });                
        }

        public static async Task SendGcm(string[] lines, Action<int, ErrorResult> onProgress, string authKey)
        {
            var jGcmData = new JObject();
            var jData = new JObject();

            int i = 0;
            jData.Add("label", "nakup: "+ DateTime.Now);
            foreach (string line in lines)
            {
                jData.Add("line" + i++, line);
            }

            jGcmData.Add("to", "/topics/shopList_" + StoreFactory.HalProxy.UserName);
            //jGcmData.Add("to", "/topics/global");
            jGcmData.Add("data", jData);

            var url = new Uri("https://gcm-http.googleapis.com/gcm/send");
            ErrorResult res = new ErrorResult();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization", "key=" + authKey);

                    
                await Task.Factory.StartNew( () =>
                    {
                        try
                        {
                            onProgress(0, new ErrorResult());
                            var task = client.PostAsync(url,
                            new StringContent(jGcmData.ToString(), Encoding.UTF8, "application/json"))
                            .ContinueWith(response =>
                            {
                                if (response.IsCompleted && response.Result.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    onProgress(100, new ErrorResult((int)ErrorCodes.CommError, response.Result.ToString()));
                                }
                                else
                                {
                                    onProgress(100, new ErrorResult((int)ErrorCodes.OK, "OK"));
                                }
                            });

                            int k = 0;
                            for(k=0; k < 100 && !task.Wait(100); k++)
                            {
                                onProgress(k, new ErrorResult((int)ErrorCodes.OK, "Continue"));
                            }

                            if(k >= 100)
                            {
                                onProgress(100, new ErrorResult((int)ErrorCodes.CommError, "Timeout"));
                            }
                        }
                        catch(Exception ex)
                        {
                            onProgress(100, new ErrorResult(ex.HResult, "Unable to send GCM message: " + ex.Message));
                        }
                    });
            }
        }
    }
}