using System;
using Legacy.Core.WorldMap;

namespace Legacy.Core.ActionLogging
{
	public class MapPointAddedEventArgs : LogEntryEventArgs
	{
		private WorldMapPoint m_point;

		public MapPointAddedEventArgs(WorldMapPoint p_point)
		{
			m_point = p_point;
		}

		public WorldMapPoint Point => m_point;
	}
}
