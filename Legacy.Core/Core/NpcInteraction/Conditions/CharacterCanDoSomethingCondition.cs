using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class CharacterCanDoSomethingCondition : DialogCondition
	{
		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character selectedCharacter = party.SelectedCharacter;
			if (selectedCharacter != null && (selectedCharacter.ConditionHandler.HasCondition(ECondition.DEAD) || selectedCharacter.ConditionHandler.HasCondition(ECondition.PARALYZED) || selectedCharacter.ConditionHandler.HasCondition(ECondition.SLEEPING) || selectedCharacter.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS)))
			{
				return FailState;
			}
			return EDialogState.NORMAL;
		}
	}
}
