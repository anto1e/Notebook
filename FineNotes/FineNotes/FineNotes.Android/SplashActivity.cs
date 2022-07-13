using Android.App;
using Android.OS;
using FineNotes.Droid;
using Android.Content.PM;
using Android.Widget;
using System.Threading;
using Android.Content;

[Activity(Label = "FineNotes", Theme = "@style/MyTheme.Splash", Icon = "@drawable/pen_logo", MainLauncher = true, NoHistory = true)]
public class SplashActivity : Activity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Drawable.splash_screen);
        System.Threading.Tasks.Task splashscreen = new System.Threading.Tasks.Task(() => {
            SplashScreen();
        });
        splashscreen.Start();

    }
    public async void SplashScreen()
    {
        await System.Threading.Tasks.Task.Delay(2000);
        var intent = new Intent(this, typeof(MainActivity));
        StartActivity(intent);
        Finish();
    }
}