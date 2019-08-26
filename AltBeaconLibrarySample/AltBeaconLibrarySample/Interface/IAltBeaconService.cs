using System;

namespace AltBeaconLibrarySample.Interface
{
	public interface IAltBeaconService
	{
		void InitializeService();	
		void StartMonitoring();
		void StartRanging();
        void StopRanging();
        void SetBackgroundMode(bool isBackground);
        void OnDestroy();
        
	}
}