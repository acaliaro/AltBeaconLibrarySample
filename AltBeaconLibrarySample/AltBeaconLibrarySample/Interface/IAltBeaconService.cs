using System;

namespace AltBeaconLibrarySample.Interface
{
	public interface IAltBeaconService
	{
		void InitializeService();	
		void StartMonitoring();
		void StartRanging();
        void SetBackgroundMode(bool isBackground);
        void OnDestroy();
        
        //event EventHandler<ListChangedEventArgs> ListChanged;
		//event EventHandler DataClearing;
	}
}