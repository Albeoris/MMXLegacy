using System;

namespace Legacy.Core.Map
{
	public struct SpiritBeaconPosition
	{
		public readonly Position Position;

		public readonly String Mapname;

		public readonly String LocalizedMapnameKey;

		public readonly Int32 MapPointID;

		public SpiritBeaconPosition(Position p_position, String p_mapname, String p_locaKey, Int32 p_worldPointID)
		{
			Position = p_position;
			Mapname = p_mapname;
			LocalizedMapnameKey = p_locaKey;
			MapPointID = p_worldPointID;
		}
	}
}
