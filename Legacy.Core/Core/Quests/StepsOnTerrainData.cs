using System;
using Legacy.Core.Map;

namespace Legacy.Core.Quests
{
	public struct StepsOnTerrainData
	{
		public readonly Int32 NumberOfSteps;

		public readonly ETerrainType TerrainType;

		public StepsOnTerrainData(Int32 p_numberOfSteps, ETerrainType p_type)
		{
			NumberOfSteps = p_numberOfSteps;
			TerrainType = p_type;
		}

		public static StepsOnTerrainData Empty => new StepsOnTerrainData(0, ETerrainType.PASSABLE);
	}
}
