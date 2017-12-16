using System;

namespace Legacy.Map
{
	[Serializable]
	public class RegionEntry
	{
		public WorldRegionTrigger RegionTrigger;

		public RegionState CurrentState = RegionState.Passive;

		public Int32 RegionCounter;
	}
}
