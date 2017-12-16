using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class RestedCondition : DialogCondition
	{
		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (p_npc.GetRestCount(ConditionTarget) < party.RestCount)
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
