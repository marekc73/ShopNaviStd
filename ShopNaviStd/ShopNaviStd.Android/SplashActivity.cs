using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using ShopNavi.Droid;

namespace ShopNavi.Droid
{
    [Activity(Theme = "@android:style/Theme.NoTitleBar", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);
            Log.Debug(TAG, "SplashActivity.OnCreate");
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();

        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }

        // Simulates background work that happens behind the splash screen
        void SimulateStartup()
        {
            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");            
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}