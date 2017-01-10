using System;
using AltBeaconOrg.BoundBeacon;
using AltBeaconLibrary.Sample.Droid.Services;
using Android.Widget;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Android.App;
using AltBeaconLibrarySample;

[assembly: Xamarin.Forms.Dependency(typeof(AltBeaconService))]

namespace AltBeaconLibrary.Sample.Droid.Services
{
	public class AltBeaconService : Java.Lang.Object, IAltBeaconService
	{
		private readonly MonitorNotifier _monitorNotifier;
		private readonly RangeNotifier _rangeNotifier;
		private BeaconManager _beaconManager;

		Region _tagRegion;

		Region _emptyRegion;
		private ListView _list;
		private readonly List<Beacon> _data;

		public AltBeaconService()
		{
			_monitorNotifier = new MonitorNotifier();
			_rangeNotifier = new RangeNotifier();
			_data = new List<Beacon>();
		}

		//public event EventHandler<ListChangedEventArgs> ListChanged;
		//public event EventHandler DataClearing;

		public BeaconManager BeaconManagerImpl
		{
			get {
				if (_beaconManager == null)
				{
					_beaconManager = InitializeBeaconManager();
				}
				return _beaconManager;
			}
		}

		public void InitializeService()
		{
			_beaconManager = InitializeBeaconManager();
		}

		private BeaconManager InitializeBeaconManager()
		{
			// Enable the BeaconManager 
			BeaconManager bm = BeaconManager.GetInstanceForApplication(Xamarin.Forms.Forms.Context);

			#region Set up Beacon Simulator if testing without a BLE device
//			var beaconSimulator = new BeaconSimulator();
//			beaconSimulator.CreateBasicSimulatedBeacons();
//
//			BeaconManager.BeaconSimulator = beaconSimulator;
			#endregion

			var iBeaconParser = new BeaconParser();
			//	Estimote > 2013
			iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
			bm.BeaconParsers.Add(iBeaconParser);

			_monitorNotifier.EnterRegionComplete += EnteredRegion;
			_monitorNotifier.ExitRegionComplete += ExitedRegion;
			_monitorNotifier.DetermineStateForRegionComplete += DeterminedStateForRegionComplete;
			_rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;

			_tagRegion = new AltBeaconOrg.BoundBeacon.Region("Beacon 2", Identifier.Parse("8E6DBFBB-489D-418A-9560-1BA1CE6301AA"), null, null);
			_tagRegion = new AltBeaconOrg.BoundBeacon.Region("myUniqueBeaconId", Identifier.Parse("B9407F30-F5F8-466E-AFF9-25556B57FE6D"), null, null);
			_emptyRegion = new AltBeaconOrg.BoundBeacon.Region("myEmptyBeaconId", null, null, null);

			bm.SetBackgroundMode(false);
			bm.Bind((IBeaconConsumer)Xamarin.Forms.Forms.Context);

			return bm;
		}

		public void StartMonitoring()
		{
			BeaconManagerImpl.SetForegroundBetweenScanPeriod(5000); // 5000 milliseconds
			BeaconManagerImpl.SetBackgroundBetweenScanPeriod(5000);

			BeaconManagerImpl.SetMonitorNotifier(_monitorNotifier); 
			_beaconManager.StartMonitoringBeaconsInRegion(_tagRegion);
			_beaconManager.StartMonitoringBeaconsInRegion(_emptyRegion);
		}

		public void StartRanging(int pol)
		{
			int val = pol * 1000;
			//BeaconManagerImpl.SetForegroundBetweenScanPeriod(3000); // 5000 milliseconds
			//BeaconManagerImpl.SetBackgroundScanPeriod(3000);
			//BeaconManagerImpl.SetBackgroundBetweenScanPeriod (3000);
			//BeaconManagerImpl.SetForegroundScanPeriod (3000);

			BeaconManagerImpl.SetForegroundBetweenScanPeriod(val); // 5000 milliseconds
			BeaconManagerImpl.SetBackgroundScanPeriod(val);
			BeaconManagerImpl.SetBackgroundBetweenScanPeriod(val);
			BeaconManagerImpl.SetForegroundScanPeriod(val);

			BeaconManagerImpl.SetRangeNotifier(_rangeNotifier);

			try
			{
				_beaconManager.StartRangingBeaconsInRegion(_tagRegion);
				_beaconManager.StartRangingBeaconsInRegion(_emptyRegion);
			}
			catch { }

		}

		//public void SetPolling(int pol) {
		//	int val = pol * 1000;
		//	try
		//	{
		//		_beaconManager.StopRangingBeaconsInRegion(_tagRegion);
		//		_beaconManager.StopRangingBeaconsInRegion(_emptyRegion);
		//	}
		//	catch (Exception ex) { }


		//	BeaconManagerImpl.SetForegroundBetweenScanPeriod(val); // 5000 milliseconds
		//	BeaconManagerImpl.SetBackgroundScanPeriod(val);
		//	BeaconManagerImpl.SetBackgroundBetweenScanPeriod(val);
		//	BeaconManagerImpl.SetForegroundScanPeriod(val);

		//	BeaconManagerImpl.SetRangeNotifier(_rangeNotifier);

		//	try
		//	{
		//		_beaconManager.StartRangingBeaconsInRegion(_tagRegion);
		//		_beaconManager.StartRangingBeaconsInRegion(_emptyRegion);
		//	}
		//	catch { }

		//}

		private void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e)
		{
			Console.WriteLine("DeterminedStateForRegionComplete");
		}

		private void ExitedRegion(object sender, MonitorEventArgs e)
		{
			string region = "???";
			if (e.Region != null) {
				if (e.Region.Id1 == null)
					region = "null region";
				else
					region = e.Region.Id1.ToString ().ToUpper ();
			}

			Xamarin.Forms.MessagingCenter.Send<App, string> ((App)Xamarin.Forms.Application.Current, "ExitedRegion", region);
			Console.WriteLine("ExitedRegion");
		}

		private void EnteredRegion(object sender, MonitorEventArgs e)
		{
			string region = "???";
			if (e.Region != null) {
				if (e.Region.Id1 == null)
					region = "null region";
				else
					region = e.Region.Id1.ToString ().ToUpper ();
			}

			Xamarin.Forms.MessagingCenter.Send<App, string> ((App)Xamarin.Forms.Application.Current, "EnteredRegion", region);

			Console.WriteLine("EnteredRegion");
		}

		async void RangingBeaconsInRegion(object sender, RangeEventArgs e)
		{
			await ClearData();

			var allBeacons = new List<Beacon>();
			//allBeacons.Clear();
			var data = new List<SharedBeacon>();
			/*
			foreach (var beacon in e.Beacons)
			{
				allBeacons.Add(beacon);
			}


			allBeacons.Sort((x, y) => x.Distance.CompareTo(y.Distance));
			*/
			//allBeacons.ForEach(b =>
			foreach(Beacon beacon in e.Beacons)                 
			{
				System.Diagnostics.Debug.WriteLine(string.Format("NAME {0} {1}", beacon.BluetoothName, beacon.Distance));
				data.Add(new SharedBeacon
				{
					Id1 = beacon.Id1.ToString(),
					Id2 = beacon.Id2.ToString(),
					Id3 = beacon.Id3.ToString(),
					Distance = beacon.Distance, // string.Format("{0:N2}m", b.Distance),
					Name = beacon.BluetoothName
				});
			};

			((Activity)Xamarin.Forms.Forms.Context).RunOnUiThread(() =>
			{
				if (data.Count > 0)
				{
					System.Diagnostics.Debug.WriteLine("I SEND TO XF " + data.Count + " BEACONS");
					Xamarin.Forms.MessagingCenter.Send<App, List<SharedBeacon>>((App)Xamarin.Forms.Application.Current, "BeaconsReceived", data);
				}
			});	

			return;

			if(e.Beacons.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine ("-----START-----");
				foreach(var b in e.Beacons)
				{
					System.Diagnostics.Debug.WriteLine (b.Id1.ToString() + " -- " + b.Distance.ToString());

					allBeacons.Add(b);
				}
				System.Diagnostics.Debug.WriteLine ("----- END -----");

				var orderedBeacons = allBeacons.OrderBy(b => b.Distance).ToList();
				await UpdateData(orderedBeacons);
			}
			else
			{
				// unknown
				await ClearData();
			}
		}

		private async Task UpdateData(List<Beacon> beacons)
		{
			await Task.Run(() =>
			{	
				var newBeacons = new List<Beacon>();
				System.Diagnostics.Debug.WriteLine("BEACONS N." + beacons.Count);

				foreach(var beacon in beacons)
				{
					if(_data.All(b => b.Id1.ToString() == beacon.Id1.ToString()))
					{
						newBeacons.Add(beacon);
					}
				}

				((Activity)Xamarin.Forms.Forms.Context).RunOnUiThread(() =>
				{
					foreach(var beacon in newBeacons)
					{
						_data.Add(beacon);
					}

					_data.Sort((x, y) => x.Distance.CompareTo(y.Distance));
					var data = new List<SharedBeacon>();
					_data.ForEach(b =>
					{
						data.Add(new SharedBeacon
						{
							Id1 = b.Id1.ToString(),
							Id2 = b.Id2.ToString(),
							Id3 = b.Id3.ToString(),
							Distance = b.Distance, // string.Format("{0:N2}m", b.Distance),
							Name = b.BluetoothName
						});
					});
					Xamarin.Forms.MessagingCenter.Send<App, List<SharedBeacon>>((App)Xamarin.Forms.Application.Current, "BeaconsReceived", data);

					/*
					if (newBeacons.Count > 0)
					{
						_data.Sort((x,y) => x.Distance.CompareTo(y.Distance));
						var data = new List<SharedBeacon>();
						_data.ForEach(b =>
						{
							data.Add(new SharedBeacon {
									Id = b.Id1.ToString(), 
									Distance = string.Format("{0:N2}m", b.Distance),
									Name = b.BluetoothName});
						});
						Xamarin.Forms.MessagingCenter.Send<App, List<SharedBeacon>>((App)Xamarin.Forms.Application.Current, "BeaconsReceived", data);

						//if(_data.Count > 3)
						//	_data.RemoveRange(3, _data.Count - 3);
						//UpdateList();
					}
					*/
				});
			});
		}

		private async Task ClearData()
		{
			//((Activity)Xamarin.Forms.Forms.Context).RunOnUiThread(() =>
			//{
			//	_data.Clear();
			//	OnDataClearing();
			//});
		}


		//private void OnDataClearing()
		//{
		//	var handler = DataClearing;
		//	if(handler != null)
		//	{
		//		handler(this, EventArgs.Empty);
		//	}
		//}

		//private void UpdateList()
		//{
		//	((Activity)Xamarin.Forms.Forms.Context).RunOnUiThread(() => 
		//	{
		//		OnListChanged();
		//	});
		//}

		//private void OnListChanged()
		//{
		//	var handler = ListChanged;
		//	if(handler != null)
		//	{
		//		var data = new List<SharedBeacon>();
		//		_data.ForEach(b =>
		//		{
		//			data.Add(new SharedBeacon {
		//					Id = b.Id1.ToString(), 
		//					Distance = string.Format("{0:N2}m", b.Distance),
		//					Name = b.BluetoothName});
		//		});
		//		handler(this, new ListChangedEventArgs(data));
		//	}
		//}
	}
}

