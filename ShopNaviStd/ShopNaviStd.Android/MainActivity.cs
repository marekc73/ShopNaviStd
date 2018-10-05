/*using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ShopNavi;

namespace ShopNavi.Droid
{
    [Activity(Label = "ShopNavi", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
    }
}*/

using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Speech;
using Android.Content;
using ShopNavi.Data;
using Android.Gms.Common;
using System.Collections.Generic;
using XamarinFormsGestures.Droid.Helpers;
using ShopNavi;
using ClientApp;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
using System.Net.Http;

namespace ShopNavi.Droid
{
    [Activity(Label = "ShopNavi", //MainLauncher = true,
        Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation//,
                                                                                   //Theme = "@style/MyTheme"
        )]
    //[Register("com.ShopNavi.Droid.MainActivity")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, GoogleApiClient.IOnConnectionFailedListener
    {
        private IHal halProxy = null;
        private static int RC_SIGN_IN = 9001;

        protected override void OnCreate(Bundle bundle)
        {
            string[] input = null;
            try
            {
                ViewConfiguration config = ViewConfiguration.Get(this);
                var menuKeyField = config.Class.GetDeclaredField("sHasPermanentMenuKey");

                if (menuKeyField != null)
                {
                    menuKeyField.Accessible = true;
                    menuKeyField.SetBoolean(config, false);
                }
            }
            catch (Exception ex)
            {
                // Ignore
            }

            try
            {
                base.OnCreate(bundle);
                global::Xamarin.Forms.Forms.Init(this, bundle);
                global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);

                this.halProxy = new AndroidHalProxy(this.ApplicationContext, this);
                this.halProxy.WriteLog(LogSeverity.Info, "Hal created and inspecting intent");

                input = this.Intent.Extras.GetStringArray("data");
            }
            catch (Exception ex)
            {
                this.halProxy.WriteLog(LogSeverity.Warning, "Error getting intent data");
                input = new string[0];
            }
            finally
            {
                if (input == null)
                {
                    input = new string[0];
                }
            }

            try
            {
                this.halProxy.WriteLog(LogSeverity.Info, "Check input");
                if (input.Length > 0)
                {
                    this.halProxy.WriteLog(LogSeverity.Info, "Entering input mode");
                }
                else
                {
                    this.halProxy.InitialPageIndex = 0;
                }

                this.halProxy.WriteLog(LogSeverity.Info, "Loading app");

                LoadApplication(new ShopNavi.App(this.halProxy));
            }
            catch (Exception ex0)
            {
                Toast.MakeText(this.ApplicationContext, ex0.ToString(), ToastLength.Long);
                this.halProxy.WriteLog(LogSeverity.Error, ex0.Message);
                this.halProxy.WriteLog(LogSeverity.Error, ex0.StackTrace);
            }

            try
            {
                if (IsPlayServicesAvailable())
                {
                    StoreFactory.CurrentVM.Logs.Add("Starting registration service");
                    this.halProxy.WriteLog(LogSeverity.Info, "Starting registration service");
                    var intent = new Intent(this, typeof(RegistrationIntentService));
                    StartService(intent);
                }
                else
                {
                    this.halProxy.WriteLog(LogSeverity.Info, "Google play not available");
                    StoreFactory.CurrentVM.Logs.Add("Google play not available");
                }

                if (StoreFactory.Settings.FromSMS)
                {
                    this.halProxy.WriteLog(LogSeverity.Info, "Registering SMS receiver");
                    this.ApplicationContext.RegisterReceiver(new SmsReceiver(), new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
                }


                if (input.Length > 0)
                {
                    StoreFactory.CurrentVM.Logs.Add("Starting with intent ");
                    this.halProxy.WriteLog(LogSeverity.Info, "Starting with intent ");

                    StoreFactory.CurrentVM.InitInput(input);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.ApplicationContext, ex.ToString(), ToastLength.Long);
                this.halProxy.WriteLog(LogSeverity.Error, ex.Message);
                this.halProxy.WriteLog(LogSeverity.Error, ex.StackTrace);
                throw;
            }

            //CarouselViewRenderer.Init();

            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn).
                                                                         RequestEmail().
                                                                         Build();
            StoreFactory.HalProxy.GoogleClient = new GoogleApiClient.Builder(this)
                                                  .EnableAutoManage(this, this)
                                                  .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                                                  .Build();
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == AndroidHalProxy.VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        this.halProxy.OnRecognizedText(matches, new ErrorResult());
                    }
                    else
                    {
                        this.halProxy.OnRecognizedText(new string[] { "No speech was recognised" }, new ErrorResult(ErrorCodes.InvalidLanguage, "No speech was recognised"));
                    }
                }
                else if (resultVal == Result.Canceled)
                {
                    this.halProxy.OnRecognizedText(new string[] { "cancelled" }, new ErrorResult(ErrorCodes.Cancelled, "Cancelled"));
                }
            }

            base.OnActivityResult(requestCode, resultVal, data);

            if (requestCode == RC_SIGN_IN)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                var profile = HandleSignInResult(result);
            }

        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            Dictionary<string, IList<string>> data = StoreFactory.CurrentVM.SaveState();
            outState.PutStringArrayList("data", data["data"]);
            outState.PutStringArrayList("output", data["output"]);
        }

        protected override void OnPause()
        {
            StoreFactory.HalProxy.FinalizeData();
            base.OnPause();
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this.ApplicationContext);
            if (resultCode != ConnectionResult.Success)
            {
                if (!GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Finish();
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }





        private GoogleProfile HandleSignInResult(GoogleSignInResult result)
        {
            GoogleProfile googleProfile = null;
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                GoogleSignInAccount acct = result.SignInAccount;
                googleProfile = new GoogleProfile()
                {
                    GivenName = acct.GivenName,
                    FamilyName = acct.FamilyName,
                    Email = acct.Email,
                    Id = acct.Id
                };

                //HttpClient client = new HttpClient();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("The resul was not success");
            }

            return googleProfile;
        }

        public void OnConnected(Bundle connectionHint)
        {
            throw new NotImplementedException();
        }


        public void OnConnectionSuspended(int cause)
        {
            throw new NotImplementedException();
        }
    }
}

