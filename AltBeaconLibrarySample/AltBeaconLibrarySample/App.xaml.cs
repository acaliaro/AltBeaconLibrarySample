using AltBeaconLibrarySample.Interface;
using AltBeaconLibrarySample.Page;
using System;
using Xamarin.Forms;

namespace AltBeaconLibrarySample
{
    public partial class App : Application
    {

        bool _closeTimer = false;
        
        public App()
        {
            InitializeComponent();

            DependencyService.Get<IAltBeaconService>().InitializeService();

            MainPage = new MainPage();

        }

        void startTimer()
        {

            _closeTimer = false;

            Device.StartTimer(TimeSpan.FromSeconds(10), () => {

                if (_closeTimer)
                {

                    System.Diagnostics.Debug.WriteLine("StartTimer: stop repeating");
                    return false;
                }

                Xamarin.Forms.MessagingCenter.Send<App>((App)Xamarin.Forms.Application.Current, "CleanBeacons");

                System.Diagnostics.Debug.WriteLine("StartTimer: end with " + (!_closeTimer ? "repeating" : "stop repeating"));

                return !_closeTimer;
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            DependencyService.Get<IAltBeaconService>().SetBackgroundMode(false);

            startTimer();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            DependencyService.Get<IAltBeaconService>().SetBackgroundMode(true);
            closeTimer();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            DependencyService.Get<IAltBeaconService>().SetBackgroundMode(false);
            startTimer();
        }

        private void closeTimer()
        {
            _closeTimer = true;
        }
    }
}
