using Android.App;
using Android.Content.PM;
using Android.OS;

namespace SubscriptionsTracker
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // ensure status bar is visible (clear fullscreen flag)
            try
            {
                Window?.ClearFlags(Android.Views.WindowManagerFlags.Fullscreen);
            }
            catch { }
        }
    }
}
