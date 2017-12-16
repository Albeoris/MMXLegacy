using System;
using Legacy.Core.Map;

namespace Legacy.Core.EventManagement
{
	internal class MapAreaEventArgs : EventArgs
	{
		public MapAreaEventArgs(EMapArea p_currentArea) : this(EMapArea.NONE, p_currentArea)
		{
		}

		public MapAreaEventArgs(EMapArea p_lastArea, EMapArea p_currentArea)
		{
			LastArea = p_lastArea;
			CurrentArea = p_currentArea;
		}

		public EMapArea LastArea { get; private set; }

		public EMapArea CurrentArea { get; private set; }
	}
}
