using System;

namespace Legacy.Core.Spells
{
	[Flags]
	public enum ETargetType
	{
		NONE = 0,
		MONSTER = 1,
		PARTY_MEMBER = 2,
		LINE_OF_SIGHT = 4,
		SINGLE = 8,
		MULTY = 16,
		ADJACENT = 32,
		ALL = 64,
		SUMMON = 128,
		ALL_MONSTERS_ON_TARGET_SLOT = 17,
		SINGLE_MONSTER = 9,
		PARTY = 18,
		SINGLE_PARTY_MEMBER = 10,
		ALL_MONSTERS_IN_LINE_OF_SIGHT = 21,
		ALL_ADJACENT_MONSTERS = 33,
		ALL_MONSTERS = 65
	}
}
