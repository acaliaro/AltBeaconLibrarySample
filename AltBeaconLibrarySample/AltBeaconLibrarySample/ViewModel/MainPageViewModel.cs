using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System;
using System.ComponentModel;
using AltBeaconLibrarySample.Model;
using AltBeaconLibrarySample.Interface;
using AltBeaconLibrarySample.Page;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

namespace AltBeaconLibrarySample.ViewModel
{
	public class MainPageViewModel : INotifyPropertyChanged
	{
		public bool IsEnabled { get; set; } = true;

		public ObservableCollection<SharedBeacon> ReceivedBeacons { get; set; } = new ObservableCollection<SharedBeacon>();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
		{

			this.StartRangingCommand = new Command(() => {
				startRangingBeacon();
			});

            this.OnAppearingCommand = new Command(() =>
            {

                Task.Run(async () =>
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                    if (status != PermissionStatus.Granted)
                        status = await Util.Permissions.CheckPermissions(Permission.Location);
                });

                MessagingCenter.Subscribe<App>(this, "CleanBeacons", (sender) =>
                {
                    updateBeaconCurrentDateTime(DateTime.Now);
                    deleteOldBeacons();
                });

                MessagingCenter.Subscribe<App, List<SharedBeacon>>(this, "BeaconsReceived", (sender, arg) =>
                {
                    if (arg != null && arg is List<SharedBeacon>)
                    {
                        System.Diagnostics.Debug.WriteLine("Received: " + ((List<SharedBeacon>)arg).Count);
                        List<SharedBeacon> temp = arg;

                        if (arg != null && arg.Count > 0)
                        {

                            DateTime now = DateTime.Now;

                            updateBeaconCurrentDateTime(now);

                            foreach (SharedBeacon sharedBeacon in arg)
                            {

                                // Is the beacon already in list?
                                var ret = ReceivedBeacons.Where(o => o.BluetoothAddress == sharedBeacon.BluetoothAddress).FirstOrDefault();
                                if (ret != null) // Is present
                                {
                                    var index = ReceivedBeacons.IndexOf(ret);
                                    ReceivedBeacons[index].Update(now, sharedBeacon.Distance, sharedBeacon.Rssi); // Update last received date time
                                }
                                else
                                {
                                    ReceivedBeacons.Insert(0, sharedBeacon);
                                }
                            }

                            deleteOldBeacons();

                            ReceivedBeacons = new ObservableCollection<SharedBeacon>(ReceivedBeacons.OrderByDescending(o => o.Rssi).ToList());

                        }

                    }
                });

            });

            this.OnDisappearingCommand = new Command(() =>
            {
                MessagingCenter.Unsubscribe<App, List<SharedBeacon>>(this, "BeaconsReceived");
                MessagingCenter.Unsubscribe<App>(this, "CleanBeacons");
            });
		}

		private void startRangingBeacon() {
			var beaconService = Xamarin.Forms.DependencyService.Get<IAltBeaconService>();

			beaconService.StartRanging();
			IsEnabled = false;
		}

        private void updateBeaconCurrentDateTime(DateTime now)
        {
            // Update current received date time
            foreach (SharedBeacon shared in ReceivedBeacons)
                shared.CurrentDateTime = now;
        }

        private void deleteOldBeacons()
        {
            int count = ReceivedBeacons.Count;

            // Delete old beacons
            for (int ii = count - 1; ii >= 0; ii--)
            {
                try
                {
                    if (ReceivedBeacons[ii].ForceDelete)
                        ReceivedBeacons.RemoveAt(ii);
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        public DataTemplate ViewCellBeaconTemplate {
			get {
				return new DataTemplate(typeof(MainPage.ViewCellBeacon));
			}
		}

		public ICommand StartRangingCommand { get; protected set; }
        public ICommand OnAppearingCommand { get; protected set; }
        public ICommand OnDisappearingCommand { get; protected set; }

	}
}
