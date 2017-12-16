using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public enum EInteractionTiming
	{
		NEVER,
		ON_SPAWN,
		ON_EXECUTE,
		ON_SUCCESS,
		ON_FAIL,
		ON_AFTER_SPAWN,
		ON_LEVEL_LOADED
	}
}
