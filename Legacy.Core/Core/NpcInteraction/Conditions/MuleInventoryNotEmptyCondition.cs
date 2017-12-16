using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class MuleInventoryNotEmptyCondition : DialogCondition
	{
		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.MuleInventory.GetCurrentItemCount() > 0)
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
