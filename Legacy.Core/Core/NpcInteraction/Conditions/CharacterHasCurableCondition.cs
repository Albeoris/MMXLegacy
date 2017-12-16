using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class CharacterHasCurableCondition : DialogCondition
	{
		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character selectedCharacter = party.SelectedCharacter;
			if (selectedCharacter.ConditionHandler.HasCondition(ECondition.PARALYZED) || selectedCharacter.ConditionHandler.HasCondition(ECondition.SLEEPING) || selectedCharacter.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS) || selectedCharacter.ConditionHandler.HasCondition(ECondition.CURSED) || selectedCharacter.ConditionHandler.HasCondition(ECondition.CONFUSED) || selectedCharacter.ConditionHandler.HasCondition(ECondition.WEAK))
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
