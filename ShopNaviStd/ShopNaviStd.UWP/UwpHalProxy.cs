using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
//using Org.Apache.Http.Authentication;
using Xamarin.Auth;
using Newtonsoft.Json;
using ShopNavi;
using ShopNavi.Data;
using System.IO;
using System.Xml.Serialization;
using Xamarin.Forms;
using Windows.Storage.Pickers;
using System.Globalization;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ShopNavi.UWP
{
    public class UwpHalProxy : IHal
    {
        public static readonly int VOICE = 10;
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";
        //GoogleApiClient mGoogleApiClient;
        Windows.Storage.StorageFolder appData = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.StorageFolder personalData = Windows.Storage.ApplicationData.Current.LocalFolder;

        private static int RC_SIGN_IN = 9001;
        public UwpHalProxy(object context, Windows.UI.Xaml.Controls.Frame act)
        {
            this.appContext = context;
            this.activity = act;
            //AndroidHalProxy.swipeRecognizer = new DroidSwipeGestureRecognizer(context);
            try
            {

                this.logPath = Path.Combine(appData.Path, "log.txt");
            }
            catch
            {
                doLog = false;
            }
        }

        private object appContext = null;
        private Windows.UI.Xaml.Controls.Frame activity;
        private Action<IList<string>, ErrorResult> onRecognizedTextDelegate = null;
        private string logPath;
        private bool doLog = true;

        public List<Store> GetStoreList()
        {
            List<Store> ret = new List<Store>();

            string[] storeFiles = Directory.GetFiles(appData.Path, "Store.*.xml");
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
            return new List<string>();
            //ClipboardManager clipboard = (ClipboardManager)appContext.GetSystemService(Context.ClipboardService);
            //return clipboard.HasText ? clipboard.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
        }

        public AllItems GetAllItems()
        {
            var fileName = Path.Combine(appData.Path, "allItems.xml");
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

                StoreFactory.Items.Save(new FileStream(Path.Combine(appData.Path, "allItems.xml"), FileMode.Create));
                StoreFactory.CurrentStore.Save(new FileStream(Path.Combine(appData.Path, StoreFactory.CurrentStore.FileName), FileMode.Create));
                SettingsBaseVM baseSettings = new SettingsBaseVM(StoreFactory.Settings);
                baseSettings.Save(new FileStream(Path.Combine(appData.Path, "settings.xml"), FileMode.Create));

                return new ErrorResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.HResult, ex.Message);
            }
        }

        public Stream GetWriteStream(string fileName)
        {
            return new FileStream(Path.Combine(appData.Path, fileName), FileMode.Create);
        }

        public ErrorResult RecordLines(Action<IList<string>, ErrorResult> onRecognizedText, CultureInfo language)
        {
            return new ErrorResult();
        }


        public void OnRecognizedText(IList<string> lines, ErrorResult err)
        {
            this.onRecognizedTextDelegate(lines, err);
        }

        public void SendNotification(string message, string[] lines)
        {
        }


        public object GetImage(string name)
        {
            var image = new Image { Aspect = Aspect.AspectFit };
            image.Source = ImageSource.FromFile(name);
            return image;
        }


        public SettingsBaseVM ReadSettings()
        {
            string file = Path.Combine(appData.Path, "settings.xml");
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
                return false;
                //try
                //{
                //    ConnectivityManager connectivityManager = (ConnectivityManager)this.appContext.GetSystemService(Context.ConnectivityService);
                //    NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
                //    return (activeConnection != null) && activeConnection.IsConnected;
                //}
                //catch (Exception ex)
                //{

                //    return false;
                //}
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
                return "test";
            }
        }

        public void RunOnUiThread(Action action)
        {
            action();
        }

        public ErrorResult SendSMS(string number, string subject, string[] lines, Action<int> onProgress)
        {
             return new ErrorResult((int)ErrorCodes.SmsError, String.Empty);
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
            MakeToast(text);
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
                string credPath = Path.Combine(personalData.Path, ".credentials/gmail-dotnet-quickstart.json");

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
                if (messagePart.MimeType == "text/plain")
                {
                    try
                    {
                        stringBuilder.Append(System.Text.Encoding.UTF8.GetString(FromBase64ForUrlString(messagePart.Body.Data)));
                    }
                    catch (Exception ex)
                    {
                        StoreFactory.CurrentVM.Logs.Add(ex.ToString() + "..." + messagePart.Body.Data.Substring(0, 10));
                    }
                }

                if (messagePart.Parts != null)
                {
                    getPlainTextFromMessageParts(messagePart.Parts, stringBuilder);
                }
            }
        }



        //public object GoogleClient
        //{
            //get
            //{
            //    return this.mGoogleApiClient;
            //}
            //set
            //{
            //    this.mGoogleApiClient = (GoogleApiClient)value;
            //}
        //}
        public object MainActivity
        {
            get
            {
                return this.activity;
            }
        }

        object IHal.GoogleClient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void SignIn()
        {
        }

        public void MakeToast(string msg)
        {
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = msg
                        },

                    },

                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = "ShopNavi",
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                }
            };
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
            return null;// return new LoginOAuth(vm);
        }

        public string ResourcePrefix
        {
            get
            {
                return "Assets/";
            }
        }

    }
}
