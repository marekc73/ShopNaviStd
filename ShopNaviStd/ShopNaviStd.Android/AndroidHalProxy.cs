using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Net;
using Android.Speech;
using Android.Telephony.Gsm;
using Android.Widget;
using ShopNavi.Data;
using Xamarin.Forms;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using Android.Gms.Auth.Api.SignIn;
using Org.Apache.Http.Authentication;
using Android.Gms.Common.Apis;
using Android.Support.V4.App;
using Xamarin.Auth;
using Newtonsoft.Json;
using Android.Gms.Auth.Api;
using ShopNavi;

namespace ShopNavi.Droid
{
    public class AndroidHalProxy : IHal
    {
        public static readonly int VOICE = 10;
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";
        GoogleApiClient mGoogleApiClient;
        private static int RC_SIGN_IN = 9001;
        public AndroidHalProxy(Context context, MainActivity act)
        {
            this.appContext = context;
            this.activity = act;
            //AndroidHalProxy.swipeRecognizer = new DroidSwipeGestureRecognizer(context);
            try
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                this.logPath = Path.Combine(dir, "log.txt");
            }
            catch
            {
                doLog = false;
            }
        }

        private Context appContext = null;
        private MainActivity activity;
        private Action<IList<string>, ErrorResult> onRecognizedTextDelegate = null;
        private string logPath;
        private bool doLog = true;

        public List<Store> GetStoreList()
        {
            List<Store> ret = new List<Store>();

            string[] storeFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Store.*.xml");
            Stream[] streams = new Stream[storeFiles.Length];
            if (storeFiles.Length > 0)
            {
                using (streams[0] = new FileStream(storeFiles[0], FileMode.Open))
                    try
                    {
                        for (int i = 1; i < storeFiles.Length; i++)
                        {
                            streams[i] = new FileStream(storeFiles[i], FileMode.Open);
                        }

                        ret = StoreFactory.GetAllStores(streams);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        ret = StoreFactory.GetAllStores(null);
                    }
                    finally
                    {
                        for (int i = 1; i < storeFiles.Length; i++)
                        {
                            streams[i].Dispose();
                        }
                    }
            }
            else
            {
                ret = StoreFactory.GetAllStores(null);
            }

            return ret;
        }

        public List<string> GetClipboardList()
        {
            ClipboardManager clipboard = (ClipboardManager)appContext.GetSystemService(Context.ClipboardService);
            return clipboard.HasText ? clipboard.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
        }

        public AllItems GetAllItems()
        {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "allItems.xml");
            try
            {
                return AllItems.Read(new FileStream(fileName, FileMode.Open));
            }
            catch (Exception ex)
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                Console.WriteLine(ex);
                return new AllItems();
            }
        }


        public void Print(List<OutputLine> output)
        {
            throw new NotImplementedException();
        }

        public bool SupportsGcm
        {
            get { return true; }
        }

        public ErrorResult FinalizeData()
        {
            try
            {
                StoreFactory.CurrentVM.Save();

                StoreFactory.Items.Save(new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "allItems.xml"), FileMode.Create));
                StoreFactory.CurrentStore.Save(new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), StoreFactory.CurrentStore.FileName), FileMode.Create));
                SettingsBaseVM baseSettings = new SettingsBaseVM(StoreFactory.Settings);
                baseSettings.Save(new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "settings.xml"), FileMode.Create));

                return new ErrorResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.HResult, ex.Message);
            }
        }

        public Stream GetWriteStream(string fileName)
        {
            return new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fileName), FileMode.Create);
        }

        public ErrorResult RecordLines(Action<IList<string>, ErrorResult> onRecognizedText, CultureInfo language)
        {
            ErrorResult ret = new ErrorResult();
            this.onRecognizedTextDelegate = onRecognizedText;

            // check to see if we can actually record - if we can, assign the event to the button
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert

                ret = new ErrorResult(1, "Error: No microphone present");
            }
            else
            {
                // create the intent and start the activity
                var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

                // put a message on the modal dialog
                voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now!");

                // if there is more then 1.5s of silence, consider the speech over
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 5000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 3000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

                // you can specify other languages recognised here, for example
                // voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.German);
                // if you wish it to recognise the default Locale language and German
                // if you do use another locale, regional dialects may not be recognised very well

                try
                {
                    voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.GetISOLanguages().FirstOrDefault(x => x.StartsWith(language.TwoLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase)));
                }
                catch
                {
                    StoreFactory.CurrentVM.Logs.Add("No language configured using default");
                    Toast.MakeText(this.appContext, "No language configured using default", ToastLength.Short);
                }

                try
                {
                    this.activity.StartActivityForResult(voiceIntent, VOICE);
                }
                catch (Exception ex)
                {
                    ret = new ErrorResult(ex.HResult, "Error: " + ex.Message);
                }
            }

            return ret;
        }


        public void OnRecognizedText(IList<string> lines, ErrorResult err)
        {
            this.onRecognizedTextDelegate(lines, err);
        }

        public void SendNotification(string message, string[] lines)
        {
            var intent = new Intent(this.appContext, typeof(MainActivity));
            intent.AddCategory("shopList");
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("data", lines);
            Android.OS.Bundle bundle = new Android.OS.Bundle();
            bundle.PutStringArrayList("data", lines);

            var pendingIntent = PendingIntent.GetActivity(this.appContext, 0, intent, PendingIntentFlags.UpdateCurrent, bundle);

            var notificationBuilder = new Notification.Builder(this.appContext)
                .SetSmallIcon(ShopNavi.Droid.Resource.Drawable.icon)
                .SetContentTitle("ShopNavi message")
                .SetDeleteIntent(pendingIntent)
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetExtras(bundle);

            var notificationManager = (NotificationManager)appContext.GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }


        public object GetImage(string name)
        {
            var image = new Image { Aspect = Aspect.AspectFit };
            image.Source = ImageSource.FromFile(name);
            return image;
        }


        public SettingsBaseVM ReadSettings()
        {
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "settings.xml");
            try
            {
                return SettingsBaseVM.Read(new FileStream(file, FileMode.Open));
            }
            catch (Exception ex)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                StoreFactory.CurrentVM.Logs.Add(ex.Message);
                return null;
            }
        }

        public bool NetworkConnected
        {
            get
            {
                try
                {
                    ConnectivityManager connectivityManager = (ConnectivityManager)this.appContext.GetSystemService(Context.ConnectivityService);
                    NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
                    return (activeConnection != null) && activeConnection.IsConnected;
                }
                catch(Exception ex)
                {

                    return false;
                }
            }
        }

        //private static DroidSwipeGestureRecognizer swipeRecognizer;

        //public ISwipeGestureRecognizer SwipeRecognizer
        //{
        //    get { return AndroidHalProxy.swipeRecognizer; }
        //}


        double x, y;
        private double initialX;
        private double initialY;
        private int initialPageIndex = 1;

        public void HandlePanUpdated(GestureData data)
        {
            SwipeType swipe = SwipeType.None;
            OutputLine line = null;
            Section sect = null;

            switch (data.status)
            {
                case GestureStatus.Started:
                    initialX = data.totalX;
                    initialY = data.totalY;
                    break;
                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    x = data.totalX;
                    y = data.totalY;

                    swipe = this.GetSwipe();
                    sect = (data.sender as View).Parent.Parent.BindingContext as Section;
                    if (sect != null)
                    {
                        sect.OnDragCmd.Execute(new SwipeAction() { Type = swipe, Finished = false });
                    }
                    break;

                case GestureStatus.Completed:
                    swipe = this.GetSwipe();
                    try
                    {
                        line = (data.sender as View).Parent.Parent.BindingContext as OutputLine;
                        sect = (data.sender as View).Parent.Parent.BindingContext as Section;
                        if (line != null)
                        {
                            line.OnDragCmd.Execute(swipe as object);
                        }
                        else if (sect != null)
                        {
                            sect.OnDragCmd.Execute(new SwipeAction() { Type = swipe, Finished = true });
                        }
                    }
                    catch (Exception ex)
                    {
                        StoreFactory.CurrentVM.Logs.Add("Swipe exception: " + ex.Message);
                    }

                    break;
            }
        }

        private SwipeType GetSwipe()
        {
            SwipeType swipe = SwipeType.None;
            if (initialX < x)
            {
                swipe = SwipeType.Right;
            }
            else if (initialX > x)
            {
                swipe = SwipeType.Left;
            }

            if (initialY < y)
            {
                swipe |= SwipeType.Down;
            }
            else if (initialY > y)
            {
                swipe |= SwipeType.Up;
            }

            return swipe;
        }


        public string UserName
        {
            get
            {
                return Environment.UserName;
            }
        }

        public void RunOnUiThread(Action action)
        {
            this.activity.RunOnUiThread(action);
        }

        public ErrorResult SendSMS(string number, string subject, string[] lines, Action<int> onProgress)
        {
            try
            {

                string text = string.Empty;
                foreach (var line in lines)
                {
                    text += line + Environment.NewLine;
                }

                var smsUri = Android.Net.Uri.Parse("smsto:" + number);
                var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                smsIntent.SetFlags(ActivityFlags.NewTask);
                smsIntent.PutExtra("subject", subject);
                smsIntent.PutExtra("sms_body", text);
                this.appContext.StartActivity(smsIntent);
                return new ErrorResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult((int)ErrorCodes.SmsError, ex.Message);
            }
        }

        public int InitialPageIndex
        {
            get
            {
                return this.initialPageIndex;
            }
            set
            {
                this.initialPageIndex = value;
            }
        }

        public Store ReadStore(Stream file)
        {
            using (TextReader textReader = new StreamReader(file))
            {
                var xmlSerializer = new XmlSerializer(typeof(Store));
                try
                {
                    Store ret = xmlSerializer.Deserialize(textReader) as Store;
                    ret.AssignParent(StoreFactory.CurrentVM);
                    return ret;
                }
                catch (Exception ex)
                {
                    if (file is FileStream)
                    {
                        File.Delete((file as FileStream).Name);
                    }
                    throw ex;
                }
            }
        }

        public void WriteLog(LogSeverity severity, string text)
        {
            if (this.doLog)
            {
                File.AppendAllText(
                    this.logPath,
                    string.Format("{0}:{1}\t\t{2}{3}", DateTime.Now, severity, text, Environment.NewLine),
                    Encoding.ASCII);
            }
        }

        public void ShowMessage(string text)
        {
            Toast.MakeText(this.appContext, text, ToastLength.Short);
        }

        public int LineHeight
        {
            get
            {
                try
                {
                    return (int)Device.GetNamedSize(NamedSize.Large, typeof(Xamarin.Forms.Label));
                }
                catch
                {
                    return 30;
                }
            }
        }
        public IEnumerable<string> ReadGmail()
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/gmail-dotnet-quickstart.json

            UserCredential credential;

            using (var stream =
                new MemoryStream(Encoding.UTF8.GetBytes("")))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true))
                    .Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

            // List labels.
            var labels = request.Execute().Labels;

            return labels.Select(x => x.Name);
            //Console.WriteLine("Labels:");
            //if (labels != null && labels.Count > 0)
            //{
            //    foreach (var labelItem in labels)
            //    {
            //        Console.WriteLine("{0}", labelItem.Name);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No labels found.");
            //}
            //Console.Read();
        }

        public User GmailUser
        {
            get;
            set;
        }

        public IList<String> ReadMailIds(string userJson)
        {
            StoreFactory.CurrentVM.Logs.Add("Gmail json size = " + userJson.Length);
            var messages = JsonConvert.DeserializeObject<MessageResult>(userJson);
            List<string> ret = new List<string>();
            StoreFactory.CurrentVM.Logs.Add("Gmail message id(s) received, count = " + ret.Count);
            foreach (Message msg in messages.Messages)
            {
                ret.Add(msg.Id);
            }

            return ret;
        }

        public Email ReadMailContent(string userJson, ICreateListVM vm)
        {
            var message = JsonConvert.DeserializeObject<Message>(userJson);
            StringBuilder builder = new StringBuilder();
            this.getPlainTextFromMessageParts(message.Payload.Parts, builder);
            try
            {

                string body = builder.ToString();
                return new Email(vm)
                {
                    Subject = message.Payload.Headers.First(x => x.Name == "Subject").Value,
                    Body = body
                };
            }
            catch (Exception ex)
            {
                return new Email(vm)
                {
                    Subject = message.Payload.Headers.First(x => x.Name == "Subject").Value,
                    Body = string.Empty
                };

            }

        }
        private static byte[] FromBase64ForUrlString(string base64ForUrlInput)
        {
            int padChars = (base64ForUrlInput.Length % 4) == 0 ? 0 : (4 - (base64ForUrlInput.Length % 4));
            StringBuilder result = new StringBuilder(base64ForUrlInput, base64ForUrlInput.Length + padChars);
            result.Append(String.Empty.PadRight(padChars, '='));
            result.Replace('-', '+');
            result.Replace('_', '/');
            return Convert.FromBase64String(result.ToString());
        }
        private void getPlainTextFromMessageParts(IList<MessagePart> msgParts, StringBuilder stringBuilder)
        {
            foreach (MessagePart messagePart in msgParts)
            {
                if (messagePart.MimeType == "text/plain" )
                {
                    try
                    {
                        stringBuilder.Append(System.Text.Encoding.UTF8.GetString(FromBase64ForUrlString(messagePart.Body.Data)));
                    }
                    catch(Exception ex)
                    {
                        StoreFactory.CurrentVM.Logs.Add(ex.ToString() + "..." + messagePart.Body.Data.TakeLast(10));
                    }
                }

                if (messagePart.Parts != null)
                {
                    getPlainTextFromMessageParts(messagePart.Parts, stringBuilder);
                }
            }
        }



        public object GoogleClient
        {
            get
            {
                return this.mGoogleApiClient;
            }
            set
            {
                this.mGoogleApiClient = (GoogleApiClient)value;
            }
        }
        public object MainActivity
        {
            get
            {
                return this.activity;
            }
        }

        public string ResourcePrefix
        {
            get
            {
                return String.Empty;
            }
        }

        public void SignIn()
        {
            Intent signInIntent = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInIntent(this.mGoogleApiClient);
            this.activity.StartActivityForResult(signInIntent, RC_SIGN_IN);
        }

        public void MakeToast(string msg)
        {
            Toast.MakeText(this.appContext, msg, ToastLength.Short).Show();

        }

        /// <summary>
        /// commands
        /// </summary>
        /// <param name="it"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public System.Windows.Input.ICommand NewLinkCmd(Item it, BaseVM vm)
        {
            return new Command(async () =>
            {
                if (it != null && it.Parent != null && it.Parent.Navi != null && (it.Link == null || it.Link.Id <= 0))//go to section list
                {
                    await it.Parent.Navi.PushModalAsync(new SectionListPage(it, false));
                }
                else if (it != null && it.Parent != null && it.CommonVM.ItemEditMode)
                {
                    it.LinkId = -1;//0
                }
                else
                {
                    StoreFactory.HalProxy.ShowMessage("Selection failed");
                    StoreFactory.CurrentVM.Logs.Add("Selection failed");
                    StoreFactory.HalProxy.WriteLog(LogSeverity.Warning, "Selection failed");
                }
            });
        }

        public System.Windows.Input.ICommand GetLocationCmd(Item it, Store store)
        {
            return new Command(async () =>
            {
                await it.Parent.Navi.PushModalAsync(new LocationPage(it, store));

            });
        }

        public System.Windows.Input.ICommand GetGoToInputCmd(ICreateListVM vm)
        {
            return new Command(() =>
            {
                (vm.Parent as MainVM).SetPageIndex(2);
            });
        }

        public System.Windows.Input.ICommand AcceptLinkCmd(Item it)
        {
            return new Command((section) =>
            {
                it.Link = section as Section;
                it.Parent.Navi.PopModalAsync();
            });
        }

        public System.Windows.Input.ICommand AcceptChangeCmd()
        {
            return new Command((it) =>
            {
                BaseVM vm = it as BaseVM;
                if (vm != null)
                {
                    vm.Parent.Navi.PopModalAsync();
                }
            });
        }

        
        public System.Windows.Input.ICommand GetDelInputLineCmd(CommonBaseVM vm, Item key)
        {
            return new Command(() =>
            {
                vm.InputList.Remove(key);
            });
        }
        public System.Windows.Input.ICommand GetMoveSectionCmd(CommonBaseVM vm, IOrdered line)
        {
            return new Command((arg) =>
            {
                line.TimeStamp = DateTime.Now;

                Task.Factory.StartNew(() => Task.Delay(3000))
                    .ContinueWith((t, x) =>
                    {
                        if ((DateTime.Now.Subtract((x as IOrdered).TimeStamp).TotalSeconds > 3))
                        {
                            (x as IOrdered).OrderImageName = "empty.png";
                        }
                    },
                        line);


                SwipeAction swipe = (arg as SwipeAction?).Value;
                SwipeType swipeType = SwipeType.None;
                if ((swipe.Type & SwipeType.Up) != 0 || (swipe.Type & SwipeType.Left) != 0)
                {
                    line.OrderImageName = "up.png";
                    swipeType = SwipeType.Up;
                }
                else if ((swipe.Type & SwipeType.Down) != 0 || (swipe.Type & SwipeType.Right) != 0)
                {
                    line.OrderImageName = "down.png";
                    swipeType = SwipeType.Down;
                }
                else
                {
                    line.OrderImageName = "empty.png";
                }

                if (swipe.Finished)
                {
                    vm.Order.MoveOutputLine(line, -1, swipeType);
                    line.OrderImageName = "empty.png";
                }

                vm.RaiseChanges();
            });
        }

        public System.Windows.Input.ICommand SwipeTimeout(IOrdered line)
        {
            return new Command(() =>
            {
            });
        }

        public System.Windows.Input.ICommand GetDelLineCmd(CommonBaseVM vm, OutputLine key)
        {
            return new Command(() =>
            {
                vm.DeleteOutputLine(vm.OutputList.FirstOrDefault(x => x.Name == key.Name));
            });
        }

        public ILoginOAuth GetLogin(ICreateListVM vm)
        {
            return new LoginOAuth(vm);
        }
    }
}

