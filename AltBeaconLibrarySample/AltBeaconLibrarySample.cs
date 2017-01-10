using System;
using AltBeaconLibrary.Sample;
using Xamarin.Forms;

namespace AltBeaconLibrarySample
{
	public class App : Application
	{
		public App()
		{
			var beaconService = DependencyService.Get<IAltBeaconService>();
			beaconService.InitializeService();

			//// The root page of your application
			//var content = new ContentPage
			//{
			//	Title = "AltBeaconLibrarySample",

			//	Content = new StackLayout
			//	{
			//		VerticalOptions = LayoutOptions.Center,
			//		Children = {
			//			new Label {
			//				HorizontalTextAlignment = TextAlignment.Center,
			//				Text = "Welcome to Xamarin Forms!"
			//			}
			//		}
			//	}
			//};

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
