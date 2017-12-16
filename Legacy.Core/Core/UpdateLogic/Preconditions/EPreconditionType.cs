using System;

namespace Legacy.Core.UpdateLogic.Preconditions
{
	public enum EPreconditionType
	{
		NONE,
		PLAIN,
		TEST,
		CHALLENGE,
		INPUT,
		PARTY_CHECK,
		DISARM_TRAP,
		SECRET_CHALLENGE,
		UNLOCK_KEY,
		SELECT_CHARACTER
	}
}
