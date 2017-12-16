using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class UnidentifiedItemsCondition : DialogCondition
	{
		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.Inventory.HasUnidentifiedItems() || party.MuleInventory.HasUnidentifiedItems())
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
