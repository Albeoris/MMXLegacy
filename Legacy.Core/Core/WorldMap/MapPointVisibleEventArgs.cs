using System;

namespace Legacy.Core.WorldMap
{
	public class MapPointVisibleEventArgs : EventArgs
	{
		public MapPointVisibleEventArgs(WorldMapPoint point)
		{
			Point = point;
		}

		public WorldMapPoint Point { get; private set; }
	}
}
