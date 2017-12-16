using System;

namespace Legacy.Core.PartyManagement
{
	[Flags]
	public enum ECondition
	{
		NONE = 0,
		DEAD = 1,
		UNCONSCIOUS = 2,
		PARALYZED = 4,
		STUNNED = 8,
		SLEEPING = 16,
		POISONED = 32,
		CONFUSED = 64,
		WEAK = 128,
		CURSED = 256
	}
}
