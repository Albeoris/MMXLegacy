using System;

namespace Legacy.Core.Map.Notes
{
	public class MapNote : IEquatable<MapNote>
	{
		public MapNote(String mapId, Position pos, String note)
		{
			MapID = mapId;
			Position = pos;
			Note = note;
		}

		public String MapID { get; private set; }

		public Position Position { get; private set; }

		public String Note { get; internal set; }

		public Boolean Equals(MapNote other)
		{
			return other != null && MapID == other.MapID && Position == other.Position;
		}

		public override Int32 GetHashCode()
		{
			return ((MapID == null) ? 0 : MapID.GetHashCode()) + Position.GetHashCode();
		}
	}
}
