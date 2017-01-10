using System;

namespace AltBeaconLibrary.Sample
{
	public interface IAltBeaconService
	{
		void InitializeService();	
		void StartMonitoring();
		void StartRanging(int pol);

		//event EventHandler<ListChangedEventArgs> ListChanged;
		//event EventHandler DataClearing;
	}
}