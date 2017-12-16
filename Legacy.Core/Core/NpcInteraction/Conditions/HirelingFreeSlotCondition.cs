using System;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class HirelingFreeSlotCondition : DialogCondition
	{
		public override EDialogState CheckCondition(Npc p_npc)
		{
			if (LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HasFreeSlot())
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
