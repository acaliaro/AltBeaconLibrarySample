
using System;
using PropertyChanged;
namespace AltBeaconLibrary.Sample
{
	[ImplementPropertyChanged]
	public class SharedBeacon 
	{
		public string Id1 { get; set; }
		public string Id2 { get; set; }
		public string Id3 { get; set; }
		public double Distance { get; set; }
		public string Name {get;set;}
		public int Counter { get; set; }
		public bool IsNew { get; set; } = false;

	}
}
