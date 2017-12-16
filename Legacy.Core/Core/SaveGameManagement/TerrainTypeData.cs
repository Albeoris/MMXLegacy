using System;
using Legacy.Core.Map;

namespace Legacy.Core.SaveGameManagement
{
	public struct TerrainTypeData : ISaveGameObject
	{
		public ETerrainType Type;

		public Boolean Visited;

		public Single Height;

		public TerrainTypeData(ETerrainType p_type, Boolean p_visited, Single p_height)
		{
			Type = p_type;
			Visited = p_visited;
			Height = p_height;
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("TerrainType", (Int32)Type);
			p_data.Set<Boolean>("Visited", Visited);
			p_data.Set<Single>("Height", Height);
		}

		public void Load(SaveGameData p_data)
		{
			Type = (ETerrainType)p_data.Get<Int32>("TerrainType", 0);
			Visited = p_data.Get<Boolean>("Visited", false);
			Height = p_data.Get<Single>("Height", Single.MaxValue);
		}
	}
}
