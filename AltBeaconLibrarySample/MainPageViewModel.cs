using System.Collections.Generic;
using AltBeaconLibrary.Sample;
using Xamarin.Forms;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Linq;
using Plugin.Vibrate;
using System.Windows.Input;
using System;

namespace AltBeaconLibrarySample
{
	[ImplementPropertyChanged]
	public class MainPageViewModel
	{
		double _stepperValue { get; set; }
		public double StepperValue { get; set; } = 3;
		public double StepperMinValue { get; set; } = 1;
		public double StepperMaxValue { get; set; } = 10;
		public bool IsEnabled { get; set; } = true;
		bool _isLogic { get; set; } = false;
		DateTime _dt { get; set; } = DateTime.Now;

		public ObservableCollection<SharedBeacon> ReceivedBeacons { get; set; } = new ObservableCollection<SharedBeacon>();

		static int MAX_COUNTER = 5;

		public MainPageViewModel()
		{

			StepperValue = 3;

			this.StartRangingCommand = new Command(() => {
				_isLogic = true;
				startRangingBeacon();

			});

			this.StartRangingNoLogicCommand = new Command(() => {
				_isLogic = false;
				startRangingBeacon();
			});
		}

		private void startRangingBeacon() {
			var beaconService = Xamarin.Forms.DependencyService.Get<IAltBeaconService>();

			//		beaconService.StartMonitoring();
			beaconService.StartRanging((int)StepperValue);
			IsEnabled = false;
		}

		public void OnAppearing()
		{
			MessagingCenter.Subscribe<App, List<SharedBeacon>>(this, "BeaconsReceived", (sender, arg) =>
			{
				if (arg != null && arg is List<SharedBeacon>)
				{
					System.Diagnostics.Debug.WriteLine("Received: " + ((List<SharedBeacon>)arg).Count);
					List<SharedBeacon> temp = arg;

					if (_isLogic)
					{

						for (int ii = 0; ii < ReceivedBeacons.Count(); ii++)
							ReceivedBeacons[ii].Counter++;


						// Received a list of beacons, add this beacons to my Property
						foreach (SharedBeacon beacon in temp)
						{
						//SharedBeacon existsBeacon = ReceivedBeacons.ToList().Find(o => o.Id1 == beacon.Id1 && o.Name == beacon.Name);
						SharedBeacon existsBeacon = ReceivedBeacons.ToList().Find(o => o.Id3 == beacon.Id3); // && o.Name == beacon.Name);
							// If a beacon with id and name is already present in the list
							if (existsBeacon != null)
							{
								int idx = ReceivedBeacons.IndexOf(existsBeacon);
								if (idx >= 0)
									ReceivedBeacons.Remove(existsBeacon);
							}
							else {
								beacon.IsNew = true;
								// When receive a new beacon, vibration
								CrossVibrate.Current.Vibration(300); //.Vibrate(int milliseconds = 500);
							}
							beacon.Counter = 0;
							ReceivedBeacons.Add(beacon);

							if (beacon.Distance <= 2)
							{

								System.Diagnostics.Debug.WriteLine("Very near " + beacon.Name + " " + beacon.Distance + "mt");
								CrossVibrate.Current.Vibration(100);
							}
						}

						//// I remove all beacons that have Counter > MAX_COUNTER
						//ReceivedBeacons.ToList().RemoveAll(o => o.Counter >= MAX_COUNTER);
						//#if DEBUG

						//					foreach (SharedBeacon b in ReceivedBeacons)
						//						System.Diagnostics.Debug.WriteLine(string.Format("BEFORE SORT {0} {1}", b.Name, b.Distance));

						//#endif
						// reorder the list
						temp = ReceivedBeacons.OrderBy(o => o.Distance).ToList(); //.RemoveAll(o => o.Counter >= MAX_COUNTER); //.ToList().Sort((x, y) => x.Distance.CompareTo(y.Distance));
																				  // Remove all beacons with a Counter >= MAX_COUNTER
						temp.RemoveAll(o => o.Counter >= MAX_COUNTER);

						//#if DEBUG

						//					foreach (SharedBeacon b in temp)
						//						System.Diagnostics.Debug.WriteLine(string.Format("AFTER SORT {0} {1}", b.Name, b.Distance));

						//#endif
						//System.Diagnostics.Debug.WriteLine("TOTAL RECEIVED:  " + ReceivedBeacons.Count());

						//ReceivedBeacons.OrderBy().Sort((x, y) => x.Distance.CompareTo(y.Distance));
						// Update UI
						ReceivedBeacons.Clear();
					}
					else {
						// No logic...
						DateTime now = DateTime.Now;
						TimeSpan tp = now.Subtract(_dt);
						System.Diagnostics.Debug.WriteLine("DateTime " +tp.Seconds );
						if (tp.Seconds >= StepperValue)
							ReceivedBeacons.Clear();

						foreach (SharedBeacon b in ReceivedBeacons)
							temp.Add(b);

						// Ordering all
						temp = temp.OrderBy(o => o.Distance).ToList();

						bool doClear = false;
						if (tp.Seconds <= 1)
						{
							// Update UI
							ReceivedBeacons.Clear();
							System.Diagnostics.Debug.WriteLine("DateTime >= " + StepperValue + " sec: CLEAR LIST");
							doClear = true;

						}
						_dt = now;

					}

					if (ReceivedBeacons.Count() == 0)
					{
						foreach (SharedBeacon beacon in temp)
						{

							if (beacon.Id3 == "38203")
								beacon.Name = "EST-1";
							else if (beacon.Id3 == "28100")
								beacon.Name = "EST-2";
							else if (beacon.Id3 == "51822")
								beacon.Name = "EST-3";
							else if (beacon.Id3 == "33612")
								beacon.Name = "EST-4";
							else if (beacon.Id3 == "21478")
								beacon.Name = "EST-5";
							else if (beacon.Id3 == "2719")
								beacon.Name = "EST-6";
							
							System.Diagnostics.Debug.WriteLine(string.Format("ID {0} - NAME {1} {2:0000.00}mt", beacon.Id1, beacon.Name, beacon.Distance));

							ReceivedBeacons.Add(beacon);
						}
					}

				}
			});
		}

		public void OnDisappearing()
		{
			MessagingCenter.Unsubscribe<App, List<SharedBeacon>>(this, "BeaconsReceived");
		}

		public DataTemplate ViewCellBeaconTemplate {
			get {
				return new DataTemplate(typeof(MainPage.ViewCellBeacon));
			}
		}

		public ICommand StartRangingCommand { get; protected set;}
		public ICommand StartRangingNoLogicCommand { get; protected set; }

	}
}
