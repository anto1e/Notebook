using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
[assembly: ExportFont("noto-sans-display-bold-italic.otf", Alias = "noto-header")]
[assembly: ExportFont("noto-sans-italic.otf", Alias = "noto-italic")]
[assembly: ExportFont("noto-sans-reg.otf", Alias = "noto-reg")]
namespace FineNotes
{
    public partial class App : Application
    {
        public App ()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());
            //MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

