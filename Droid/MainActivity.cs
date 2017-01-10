using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using AltBeaconOrg.BoundBeacon;
using AltBeaconLibrary.Sample;

namespace AltBeaconLibrarySample.Droid
{
	[Activity(Label = "AltBeaconLibrarySample.Droid", 
	          Icon = "@drawable/icon", 
	          Theme = "@style/MyTheme",
	          MainLauncher = true, 
	          LaunchMode = Android.Content.PM.LaunchMode.SingleInstance,
	          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IBeaconConsumer
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			LoadApplication(new App());
		}

		#region IBeaconConsumer Implementation
		public void OnBeaconServiceConnect()
		{
			//var beaconService = Xamarin.Forms.DependencyService.Get<IAltBeaconService>();

			////		beaconService.StartMonitoring();
			//beaconService.StartRanging();
		}
		#endregion
	}
}
