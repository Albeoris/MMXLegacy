using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class UnrestoredCharactersCondition : DialogCondition
	{
		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character[] members = party.Members;
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i] != null && members[i].HealthPoints < members[i].MaximumHealthPoints)
				{
					return EDialogState.NORMAL;
				}
			}
			return FailState;
		}
	}
}
