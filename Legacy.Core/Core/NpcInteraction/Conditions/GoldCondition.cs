using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class GoldCondition : DialogCondition
	{
		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.Gold - p_npc.GetCosts(ConditionTarget) >= 0)
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
