using System;
using System.Collections.Generic;

namespace Legacy.Core.Map.Notes
{
	public class MapNoteCollection : Dictionary<Position, MapNote>
	{
		public Boolean Contains(Position pos)
		{
			return ContainsKey(pos);
		}

		internal void Cleanup()
		{
			List<Position> list = new List<Position>();
			foreach (KeyValuePair<Position, MapNote> keyValuePair in this)
			{
				if (String.IsNullOrEmpty(keyValuePair.Value.Note))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (Position key in list)
			{
				Remove(key);
			}
		}
	}
}
