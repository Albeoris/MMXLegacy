using System;

namespace Legacy.Core.Map
{
	[Flags]
	public enum ETerrainType
	{
		NONE = 0,
		PASSABLE = 1,
		WATER = 2,
		BLOCKED = 4,
		ROUGH = 8,
		FOREST = 16,
		HAZARDOUS = 32,
		LAVA = 64,
		SHOOT_THROUGH = 128,
		SEE_THROUGH = 256,
		FLY_THROUGH = 512,
		NO_PARTY_BARK = 1024
	}
}
