using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;

namespace CulturalCitiesApp
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof (SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup ()
        {
            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
            await Task.Delay(1000); // Simulate a bit of startup work.
            Log.Debug(TAG, "Startup work is finished - starting MainActivity.");


            Context mContext = Android.App.Application.Context;
            AppPreferences ap = new AppPreferences(mContext);
            string key = ap.getAccessKey();

            if (string.IsNullOrEmpty(key))
            {
                StartActivity(new Intent(Application.Context, typeof(LoginIntent)));
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
        }
    }
}